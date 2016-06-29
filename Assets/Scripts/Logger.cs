﻿using UnityEngine;
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
        string msg = string.Format("[{0:F2}] > {1}\n", Time.time, args.ToString());
        _Label.text = "";
        _Lines.Add(msg);
        if (_Lines.Count > 100)
        {
            _Lines.RemoveAt(0);
        }
        foreach (var line in _Lines)
        {
            _Label.text += line;
        }
        _Scrollbar.value = 1;
    }
}
