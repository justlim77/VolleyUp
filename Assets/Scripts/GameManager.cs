using UnityEngine;
using System;
using System.Collections.Generic;

namespace Volley
{
    public class GameManager : MonoBehaviour
    {
        static GameManager _Instance;
        public static GameManager Instance
        {
            get
            {
                return _Instance;
            }
        }
        public int Score { get; set; }

        void Awake()
        {
            _Instance = this;

            Core.SubscribeEvent("OnBallHit", OnBallHit);
        }

        void OnDestroy()
        {
            _Instance = null;
        }

        object OnBallHit(object sender, object args)
        {
            if (args is int)
            {
                int points = (int)args;
                Score += points;
                Core.BroadcastEvent("OnScoreUpdate", this, Score);
            }
            return null;
        }
    }
}
