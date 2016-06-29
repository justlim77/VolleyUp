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
        public int Combo { get; set; }
        public int comboMax = 5;
        public float comboInterval = 5.0f;

        static bool _Missed;
        public static bool Missed { get; set; }

        public HitRating perfectRating;
        public HitRating greatRating;
        public HitRating goodRating;

        public Animator playerAnimator;

        public float Timer { get; private set; }

        void Awake()
        {
            _Instance = this;

            Core.SubscribeEvent("OnBallHit", OnBallHit);
            Core.SubscribeEvent("OnTargetHit", OnTargetHit);
        }

        void Start()
        {
            Combo = 1;
            Score = 0;
        }

        void Update()
        {
            Timer += Time.deltaTime;

            if (Timer > comboInterval)
            {
                Combo = 1;
                Timer = 0;
            }
        }

        void OnDestroy()
        {
            _Instance = null;
        }

        object OnBallHit(object sender, object args)
        {
            if (args is float)
            {
                float val = (float)args;
                string msg = "";
                if (val > perfectRating.RangeMin && val < perfectRating.RangeMax)
                    msg = string.Format("<color={0}>Perfect</color>", ColorTypeConverter.ToRGBHex(perfectRating.Color));
                else if (val > greatRating.RangeMin && val < greatRating.RangeMax)
                    msg = string.Format("<color={0}>Great</color>", ColorTypeConverter.ToRGBHex(greatRating.Color));
                else
                    msg = string.Format("<color={0}>Good</color>", ColorTypeConverter.ToRGBHex(goodRating.Color));
                Core.BroadcastEvent("OnStatusUpdate", this, msg);
            }
            return null;
        }

        object OnTargetHit(object sender, object args)
        {
            if (args is int)
            {
                // Reset combo timer
                Timer = 0; // Reset combo timer
                Combo++;
                Combo = Mathf.Clamp(Combo, 1, comboMax);
                Core.BroadcastEvent("OnComboUpdate", this, Combo);

                int points = (int)args * Combo;
                Score += points;
                Core.BroadcastEvent("OnScoreUpdate", this, Score);
            }
            return null;
        }
    }

    [Serializable]
    public struct HitRating
    {
        public float RangeMin;
        public float RangeMax;
        public Color Color;
    }
}
