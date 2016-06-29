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

        bool _hasInteracted = false;
        public void Interact(object args)
        {
            if (_hasInteracted)
                return;

            _hasInteracted = true;
            Core.BroadcastEvent("OnTargetHit", this, Points * Multiplier);
            gameObject.SetActive(false);
            Invoke("Activate", TimeToRespawn);
        }

        // Use this for initialization
        void Start ()
        {
            Points = 10;
	    }

        void Activate()
        {
            gameObject.SetActive(true);
            _hasInteracted = false;
        }
    }

}
