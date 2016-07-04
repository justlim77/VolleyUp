using UnityEngine;
using System;
using System.Collections.Generic;

namespace Volley
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

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

        public float ScoreTimer { get; private set; }
        public float ComboTimer { get; private set; }

        public bool RoundStarted { get; set; }
        public bool FirstHit { get; set; }

        void Awake()
        {
            if(Instance == null)
                Instance = this;

            Core.SubscribeEvent("OnBallHit", OnBallHit);
            Core.SubscribeEvent("OnTargetHit", OnTargetHit);
        }

        void Start()
        {
            Reset();
        }

        void Update()
        {
            if (!RoundStarted && !FirstHit)
                return;

            // Game timer
            ScoreTimer += Time.deltaTime;

            // Combo timer
            ComboTimer += Time.deltaTime;

            if (ComboTimer > comboInterval)
            {
                Combo = 1;
                ComboTimer = 0;
            }

        }

        void OnDestroy()
        {
            Instance = null;
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
                ComboTimer = 0; // Reset combo timer
                Combo++;
                Combo = Mathf.Clamp(Combo, 1, comboMax);
                Core.BroadcastEvent("OnComboUpdate", this, Combo);

                int points = (int)args * Combo;
                Score += points;
                Core.BroadcastEvent("OnScoreUpdate", this, Score);

                // Update highscore
                Core.BroadcastEvent("OnHighscoreUpdate", this, Score);
            }
            return null;
        }

        public void Reset()
        {
            Score = 0;
            ScoreTimer = 0;
            Core.BroadcastEvent("OnScoreUpdate", this, Score);

            Combo = 1;
            ComboTimer = 0;
            Core.BroadcastEvent("OnComboUpdate", this, Combo);

            RoundStarted = false;
            Core.BroadcastEvent("OnTargetScoreUpdate", this, 0);
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
