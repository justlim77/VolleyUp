using UnityEngine;
using System;
using System.Collections;
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

        public float roundEndDelay = 5.0f;

        public HitRating perfectRating;
        public HitRating greatRating;
        public HitRating goodRating;

        public Animator playerAnimator;

        public float GameTimer { get; private set; }
        public float ComboTimer { get; private set; }

        public bool RoundStarted { get; set; }

        public GameState GameState { get; set; }

        bool _pendingReset;

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
            if (_pendingReset)
            {
                _pendingReset = false;
                KinectManager.Instance.ClearKinectUsers();
            }

            if (RoundStarted)
            {            
                // Game timer
                GameTimer += Time.deltaTime;

                // Combo timer
                ComboTimer += Time.deltaTime;

                if (ComboTimer > comboInterval)
                {
                    Combo = 1;
                    ComboTimer = 0;
                }
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

                // Add points
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
            _pendingReset = true;

            RoundStarted = false;

            GameTimer = 0;
            ComboTimer = 0;

            Score = 0;
            Core.BroadcastEvent("OnScoreUpdate", this, Score);

            Combo = 1;
            Core.BroadcastEvent("OnComboUpdate", this, Combo);

            TargetManager.Instance.Reset();
        }

        public void SetState(GameState state)
        {
            switch (state)
            {
                case GameState.Waiting:
                    Reset();
                    break;
                case GameState.Pregame:
                    TargetManager.Instance.ShowTargets();
                    break;
                case GameState.Playing:
                    RoundStarted = true;
                    Core.BroadcastEvent("OnNotificationUpdate", this, "");
                    break;
                case GameState.End:
                    RoundStarted = false;
                    StartCoroutine(GameEndSequence());
                    break;
            }
        }

        public IEnumerator GameEndSequence()
        {
            Core.BroadcastEvent("OnNotificationUpdate", this, "You've hit all targets!");
            TargetManager.Instance.DeactivateAllTargets();
            AudioManager.Instance.PlayOneShot(SoundType.Positive);
            yield return new WaitForSeconds(roundEndDelay);

            SetState(GameState.Waiting);
        }
    }

    
    [Serializable]
    public struct HitRating
    {
        public float RangeMin;
        public float RangeMax;
        public Color Color;
    }

    public enum GameState
    {
        Waiting,
        Pregame,
        Playing,
        End
    }
}
