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

        public HitRating perfectRating;
        public HitRating greatRating;
        public HitRating goodRating;

        public Animator playerAnimator;

        void Awake()
        {
            _Instance = this;

            Core.SubscribeEvent("OnBallHit", OnBallHit);
            Core.SubscribeEvent("OnTargetHit", OnTargetHit);
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
                int points = (int)args;
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
