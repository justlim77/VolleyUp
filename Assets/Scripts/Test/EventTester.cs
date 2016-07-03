using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventTester : MonoBehaviour
{

    public List<EventKeyPair> eventKeyPairList = new List<EventKeyPair>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        foreach (var pair in eventKeyPairList)
        {
            if (Input.GetKeyDown(pair.KeyCode))
            {
                switch (pair.ArgType)
                {
                    case ArgType.Float:
                        Core.BroadcastEvent(pair.EventName, this, (float)pair.Arg);
                        break;
                    case ArgType.Int:
                        Core.BroadcastEvent(pair.EventName, this, (int)pair.Arg);
                        break;
                    case ArgType.String:
                        Core.BroadcastEvent(pair.EventName, this, pair.StringArg);
                        break;
                }
            }
        }
	}
}

[System.Serializable]
public struct EventKeyPair
{
    public KeyCode KeyCode;
    public string EventName;
    public ArgType ArgType;
    public int Arg;
    public string StringArg;
}

public enum ArgType
{
    Int,
    Float,
    String
}
