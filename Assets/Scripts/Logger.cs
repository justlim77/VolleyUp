using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Logger : MonoBehaviour
{
    static Logger _Instance;
    public static Logger Instance
    {
        get { return _Instance; }
    }

    public Text Label;

    static List<string> _Lines = new List<string>();
    static Text _Label;
    static Scrollbar _Scrollbar;

    void Awake()
    {
        _Instance = this;

        _Label = Label;
        _Scrollbar = GetComponent<ScrollRect>().verticalScrollbar;
    }

    void OnDestroy()
    {
        _Instance = null;
    }

	// Use this for initialization
	void Start () {
	
	}

    public static void Log(object args)
    {
        string msg = (string)args.ToString() + "\n";
        _Label.text += msg;
        _Lines.Add(msg);
        _Scrollbar.value = 1;
    }
}
