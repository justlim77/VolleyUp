using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;


public class KinectDataServer : MonoBehaviour 
{
	[Tooltip("Port to be used for incoming connections.")]
	public int listenOnPort = 8888;

	[Tooltip("Maximum number of allowed connections.")]
	public int maxConnections = 10;

	[Tooltip("Transform representing this sensor's position and rotation in world space. If missing, the sensor height and angle settings from KinectManager-component are used.")]
	public Transform sensorTransform;

	[Tooltip("GUI-texture used to display the tracked users on scene background.")]
	public GUITexture backgroundImage;

	[Tooltip("GUI-Text to display status messages.")]
	public GUIText statusText;


	private ConnectionConfig serverConfig;
	private int serverChannelId;

	private HostTopology serverTopology;
	private int serverHostId;

	private const int bufferSize = 32768;
	private byte[] recBuffer = new byte[bufferSize]; 

	private KinectManager manager;
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

			serverTopology = new HostTopology(serverConfig, maxConnections);
			serverHostId = NetworkTransport.AddHost(serverTopology, listenOnPort);

			liRelTime = 0;
			fCurrentTime = Time.time;

			System.DateTime dtNow = System.DateTime.UtcNow;
			Debug.Log("Kinect data server started at " + dtNow.ToString() + " - " + dtNow.Ticks);

			if(statusText)
			{
				statusText.text = "Server running: 0 connection(s)";
			}
		} 
		catch (System.Exception ex) 
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);

			if(statusText)
			{
				statusText.text = ex.Message;
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

//		if(manager == null)
//		{
//			manager = KinectManager.Instance;
//		}

		if(backgroundImage && backgroundImage.texture == null)
		{
			backgroundImage.texture = manager ? manager.GetUsersLblTex() : null;
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
					//conn.matrixSent = false;

					dictConnection[connectionId] = conn;
					connListUpdated = true;
				}
				break;
			case NetworkEventType.DataEvent:       //3
				if(recHostId == serverHostId && recChannelId == serverChannelId &&
					dictConnection.ContainsKey(connectionId))
				{
					HostConnection conn = dictConnection[connectionId];
					string sRecvMessage = System.Text.Encoding.UTF8.GetString(recBuffer, 0, dataSize);

					if(sRecvMessage == "ka")
					{
						conn.keepAlive = true;
						dictConnection[connectionId] = conn;
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

				Debug.Log(sbConnStatus);

				if(statusText)
				{
					statusText.text = sbConnStatus.ToString();
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

				foreach(int connId in alConnectionId)
				{
					HostConnection conn = dictConnection[connId];

					if(conn.keepAlive)
					{
						conn.keepAlive = false;
						dictConnection[connId] = conn;

						error = 0;
						if(!NetworkTransport.Send(conn.hostId, conn.connectionId, conn.channelId, btSendMessage, btSendMessage.Length, out error))
						{
							Debug.LogError("Error sending packet via connection " + conn.connectionId + ": " + (NetworkError)error);
						}
					}
				}
			}
		} 
		catch (System.Exception ex) 
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);

			if(statusText)
			{
				statusText.text = ex.Message;
			}
		}
	}

}
