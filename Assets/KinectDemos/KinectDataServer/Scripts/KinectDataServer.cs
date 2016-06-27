using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Networking.Types;


public class KinectDataServer : MonoBehaviour 
{
	[Tooltip("Port to be used for incoming connections.")]
	public int listenOnPort = 8888;

	[Tooltip("Port used for server broadcast discovery.")]
	public int broadcastPort = 8889;

	[Tooltip("Maximum number of allowed connections.")]
	public int maxConnections = 5;

	[Tooltip("Transform representing this sensor's position and rotation in world space. If missing, the sensor height and angle settings from KinectManager-component are used.")]
	public Transform sensorTransform;

	[Tooltip("GUI-texture used to display the tracked users on scene background.")]
	public GUITexture backgroundImage;

	[Tooltip("GUI-Text to display connection status messages.")]
	public GUIText connStatusText;

	[Tooltip("GUI-Text to display server status messages.")]
	public GUIText serverStatusText;


	private ConnectionConfig serverConfig;
	private int serverChannelId;

	private HostTopology serverTopology;
	private int serverHostId = -1;
	private int broadcastHostId = -1;

	private const int bufferSize = 32768;
	private byte[] recBuffer = new byte[bufferSize];

	private byte[] broadcastOutBuffer = null;

	private KinectManager manager;
	private FacetrackingManager faceManager;
	private long liRelTime = 0;
	private float fCurrentTime = 0f;

	private Dictionary<int, HostConnection> dictConnection = new Dictionary<int, HostConnection>();
	private List<int> alConnectionId = new List<int>();


	private struct HostConnection
	{
		public int hostId; 
		public int connectionId; 
		public int channelId; 

		public bool keepAlive;
		public string reqDataType;
		//public bool matrixSent;
		//public int errorCount;
	}


	void Awake () 
	{
		try 
		{
			NetworkTransport.Init();

			serverConfig = new ConnectionConfig();
			serverChannelId = serverConfig.AddChannel(QosType.StateUpdate);  // QosType.UnreliableFragmented
			serverConfig.MaxSentMessageQueueSize = 2048;  // 128 by default

			// start data server
			serverTopology = new HostTopology(serverConfig, maxConnections);
			serverHostId = NetworkTransport.AddHost(serverTopology, listenOnPort);

			if(serverHostId < 0)
			{
				throw new UnityException("AddHost failed for port " + listenOnPort);
			}

			// add broadcast host
			if(broadcastPort > 0)
			{
				broadcastHostId = NetworkTransport.AddHost(serverTopology, 0);

				if(broadcastHostId < 0)
				{
					throw new UnityException("AddHost failed for broadcast discovery");
				}
			}

			// set broadcast data
			string sBroadcastData = string.Empty;

			try 
			{
				string strHostName = System.Net.Dns.GetHostName();
				IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
				IPAddress[] addr = ipEntry.AddressList;

				string sHostInfo = "Host: " + strHostName;
				for (int i = 0; i < addr.Length; i++)
				{
					if (addr[i].AddressFamily == AddressFamily.InterNetwork)
					{
						sHostInfo += ", IP: " + addr[i].ToString();
						sBroadcastData = "KinectDataServer:" + addr[i].ToString() + ":" + listenOnPort;
						break;
					}
				}

				sHostInfo += ", Port: " + listenOnPort;
				Debug.Log(sHostInfo);

				if(serverStatusText)
				{
					serverStatusText.text = sHostInfo;
				}
			} 
			catch (System.Exception ex) 
			{
				Debug.LogError(ex.Message + "\n\n" + ex.StackTrace);

				if(serverStatusText)
				{
					serverStatusText.text = "Use 'ipconfig' to see the host IP; Port: " + listenOnPort;
				}
			}

			// start broadcast discovery
			if(broadcastPort > 0)
			{
				broadcastOutBuffer = System.Text.Encoding.UTF8.GetBytes(sBroadcastData);
				byte error = 0;

				if (!NetworkTransport.StartBroadcastDiscovery(broadcastHostId, broadcastPort, 8888, 1, 0, broadcastOutBuffer, broadcastOutBuffer.Length, 2000, out error))
				{
					throw new UnityException("Start broadcast discovery failed: " + (NetworkError)error);
				}
			}

			liRelTime = 0;
			fCurrentTime = Time.time;

			System.DateTime dtNow = System.DateTime.UtcNow;
			Debug.Log("Kinect data server started at " + dtNow.ToString() + " - " + dtNow.Ticks);

			if(connStatusText)
			{
				connStatusText.text = "Server running: 0 connection(s)";
			}
		} 
		catch (System.Exception ex) 
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);

			if(connStatusText)
			{
				connStatusText.text = ex.Message;
			}
		}
	}

	void Start()
	{
		if(manager == null)
		{
			manager = KinectManager.Instance;
		}

		if (manager && manager.IsInitialized ()) 
		{
			if (sensorTransform != null) 
			{
				manager.SetKinectToWorldMatrix (sensorTransform.position, sensorTransform.rotation);
			}

			if(backgroundImage)
			{
				Vector3 localScale = backgroundImage.transform.localScale;
				localScale.x = (float)manager.GetDepthImageWidth() * (float)Screen.height / ((float)manager.GetDepthImageHeight() * (float)Screen.width);
				localScale.y = -1f;

				backgroundImage.transform.localScale = localScale;
			}
		}
	}

	void OnDestroy()
	{
		// clear connections
		dictConnection.Clear();

		// stop broadcast
		if (broadcastHostId >= 0) 
		{
			NetworkTransport.StopBroadcastDiscovery();
			NetworkTransport.RemoveHost(broadcastHostId);
			broadcastHostId = -1;
		}

		// close the server port
		if (serverHostId >= 0) 
		{
			NetworkTransport.RemoveHost(serverHostId);
			serverHostId = -1;
		}

		// shitdown the transport layer
		NetworkTransport.Shutdown();
	}
	
	void Update () 
	{
		int recHostId; 
		int connectionId; 
		int recChannelId; 
		int dataSize;

		bool connListUpdated = false;

		if(backgroundImage && backgroundImage.texture == null)
		{
			backgroundImage.texture = manager ? manager.GetUsersLblTex() : null;
		}

		if(faceManager == null)
		{
			faceManager = FacetrackingManager.Instance;
		}

		try 
		{
			byte error = 0;
			NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

			switch (recData)
			{
			case NetworkEventType.Nothing:         //1
				break;
			case NetworkEventType.ConnectEvent:    //2
				if(recHostId == serverHostId && recChannelId == serverChannelId &&
					!dictConnection.ContainsKey(connectionId))
				{
					HostConnection conn = new HostConnection();
					conn.hostId = recHostId;
					conn.connectionId = connectionId;
					conn.channelId = recChannelId;
					conn.keepAlive = true;
					conn.reqDataType = "ka,kb,km,kh";
					//conn.matrixSent = false;

					dictConnection[connectionId] = conn;
					connListUpdated = true;

					//Debug.Log(connectionId + "-conn: " + conn.reqDataType);
				}
				break;
			case NetworkEventType.DataEvent:       //3
				if(recHostId == serverHostId && recChannelId == serverChannelId &&
					dictConnection.ContainsKey(connectionId))
				{
					HostConnection conn = dictConnection[connectionId];
					string sRecvMessage = System.Text.Encoding.UTF8.GetString(recBuffer, 0, dataSize);

					if(sRecvMessage.StartsWith("ka"))
					{
						if(sRecvMessage == "ka")  // vr-examples v1.0 keep-alive message
							sRecvMessage = "ka,kb,km,kh";
						
						conn.keepAlive = true;
						conn.reqDataType = sRecvMessage;
						dictConnection[connectionId] = conn;

						//Debug.Log(connectionId + "-keep: " + conn.reqDataType);
					}
				}
				break;
			case NetworkEventType.DisconnectEvent: //4
				if(dictConnection.ContainsKey(connectionId))
				{
					dictConnection.Remove(connectionId);
					connListUpdated = true;
				}
				break;
			}

			if(connListUpdated)
			{
				// get all connection IDs
				alConnectionId.Clear();
				alConnectionId.AddRange(dictConnection.Keys);

				// display the number of connections
				StringBuilder sbConnStatus = new StringBuilder();
				sbConnStatus.AppendFormat("Server running: {0} connection(s)", dictConnection.Count);

				foreach(int connId in dictConnection.Keys)
				{
					HostConnection conn = dictConnection[connId];
					int iPort = 0; string sAddress = string.Empty; NetworkID network; NodeID destNode;

					NetworkTransport.GetConnectionInfo(conn.hostId, conn.connectionId, out sAddress, out iPort, out network, out destNode, out error);
					if(error == (int)NetworkError.Ok)
					{
						sbConnStatus.AppendLine().Append("    ").Append(sAddress).Append(":").Append(iPort);
					}
				}

				Debug.Log(sbConnStatus);

				if(connStatusText)
				{
					connStatusText.text = sbConnStatus.ToString();
				}
			}

			// send body frame to available connections
			string sBodyFrame = manager ? manager.GetBodyFrameData(ref liRelTime, ref fCurrentTime) : string.Empty;

			if(sBodyFrame.Length > 0 && dictConnection.Count > 0)
			{
				StringBuilder sbSendMessage = new StringBuilder();

				sbSendMessage.Append(manager.GetWorldMatrixData()).Append('|');
				sbSendMessage.Append(sBodyFrame).Append('|');
				sbSendMessage.Append(manager.GetBodyHandData(ref liRelTime)).Append('|');

				if(sbSendMessage.Length > 0 && sbSendMessage[sbSendMessage.Length - 1] == '|')
				{
					sbSendMessage.Remove(sbSendMessage.Length - 1, 1);
				}

				byte[] btSendMessage = System.Text.Encoding.UTF8.GetBytes(sbSendMessage.ToString());

				// check face-tracking requests
				bool bFaceParams = false, bFaceVertices = false, bFaceTriangles = false;
				if(faceManager && faceManager.IsFaceTrackingInitialized())
					CheckFacetrackRequests(out bFaceParams, out bFaceVertices, out bFaceTriangles);

				byte[] btFaceParams = null; //, btFaceVertices = null, btFaceTriangles = null;
				if(bFaceParams)
				{
					string sFaceParams = faceManager.GetFaceParamsAsCsv();
					if(!string.IsNullOrEmpty(sFaceParams))
						btFaceParams = System.Text.Encoding.UTF8.GetBytes(sFaceParams);
				}

				foreach(int connId in alConnectionId)
				{
					HostConnection conn = dictConnection[connId];

					if(conn.keepAlive)
					{
						conn.keepAlive = false;
						dictConnection[connId] = conn;

						if(conn.reqDataType != null && conn.reqDataType.Contains("kb,"))
						{
							error = 0;
							if(!NetworkTransport.Send(conn.hostId, conn.connectionId, conn.channelId, btSendMessage, btSendMessage.Length, out error))
							{
								string sMessage = "Error sending body data via conn " + conn.connectionId + ": " + (NetworkError)error;
								Debug.LogError(sMessage);

								if(serverStatusText)
								{
									serverStatusText.text = sMessage;
								}
							}
						}

						if(bFaceParams && btFaceParams != null &&
							conn.reqDataType != null && conn.reqDataType.Contains("fp,"))
						{
							error = 0;
							if(!NetworkTransport.Send(conn.hostId, conn.connectionId, conn.channelId, btFaceParams, btFaceParams.Length, out error))
							{
								string sMessage = "Error sending face params via conn " + conn.connectionId + ": " + (NetworkError)error;
								Debug.LogError(sMessage);

								if(serverStatusText)
								{
									serverStatusText.text = sMessage;
								}
							}
						}

					}
				}
			}

		} 
		catch (System.Exception ex) 
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);

			if(serverStatusText)
			{
				serverStatusText.text = ex.Message;
			}
		}
	}


	// checks whether facetracking data was requested by any connection
	private void CheckFacetrackRequests(out bool bFaceParams, out bool bFaceVertices, out bool bFaceTriangles)
	{
		bFaceParams = bFaceVertices = bFaceTriangles = false;

		foreach (int connId in alConnectionId) 
		{
			HostConnection conn = dictConnection [connId];

			if (conn.keepAlive && conn.reqDataType != null) 
			{
				if (conn.reqDataType.Contains ("fp,"))
					bFaceParams = true;
				if (conn.reqDataType.Contains ("fv,"))
					bFaceVertices = true;
				if (conn.reqDataType.Contains ("ft,"))
					bFaceTriangles = true;
			}
		}
	}

}
