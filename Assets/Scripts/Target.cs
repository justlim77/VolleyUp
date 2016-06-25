using UnityEngine;
using System.Collections;
using System;

namespace Volley
{
    public class Target : MonoBehaviour, IInteractable
    {
        public int Multiplier = 1;
        public float TimeToRespawn = 2f;

        int _points;
        public int Points
        {
            get
            {
                return _points;
            }

            set
            {
                _points = value;
            }
        }

        public void Interact(object args)
        {
            Core.BroadcastEvent("OnTargetHit", this, Points * Multiplier);
            gameObject.SetActive(false);
            Invoke("Activate", TimeToRespawn);
        }

        // Use this for initialization
        void Start ()
        {
            Points = 100;
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }

        void Activate()
        {
            gameObject.SetActive(true);
        }
    }

}
