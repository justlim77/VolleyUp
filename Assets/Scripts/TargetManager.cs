using UnityEngine;
using System.Collections;

namespace Volley
{
    public class TargetManager : MonoBehaviour
    {
        public Target[] targets;
        public int targetHitsRequired;

        public static TargetManager Instance { get; private set; }

        int _targetsMax;
        public int _currentHits;
        int _prevIdx, _currentIdx;
        bool _firstHit;

        bool HitAllTargets
        {
            get
            {
                return _currentHits >= targetHitsRequired;
            }
        }

        void Awake()
        {
            if(Instance == null)
                Instance = this;

            Core.SubscribeEvent("OnTargetHitUpdate", OnTargetHitUpdate);
            Core.SubscribeEvent("OnHitReqChange", OnHitReqChange);
        }

        void OnDestroy()
        {
            Instance = null;

            Core.UnsubscribeEvent("OnTargetHitUpdate", OnTargetHitUpdate);
            Core.UnsubscribeEvent("OnHitReqChange", OnHitReqChange);
        }

        object OnTargetHitUpdate(object sender, object args)
        {
            _currentHits++;
            _currentHits = Mathf.Clamp(_currentHits, 0, targetHitsRequired);

            Core.BroadcastEvent("OnHitCountUpdate", this, _currentHits);

            // If required amount of targets are hit, change game state to end
            if (HitAllTargets == true)
            {
                Core.BroadcastEvent("OnNotificationUpdate", this, "All targets hit!");
                DeactivateAllTargets();
                GameManager.Instance.SetState(GameState.End);
            }

            return null;
        }

        object OnHitReqChange(object sender, object args)
        {
            if (args is int)
            {
                int interval = (int)args;
                targetHitsRequired += interval;
                targetHitsRequired = Mathf.Clamp(targetHitsRequired, 1, 100);

                Core.BroadcastEvent("OnHitCountUpdate", this, _currentHits);
            }
            return null;
        }


        void Start ()
        {
            _targetsMax = targets.Length;

            Reset();
	    }

        IEnumerator ShowTarget(Target target)
        {
            target.Activate();

            while (target.HasInteracted == false)
            {
                yield return null;
            }

            // Check for first hit to start round timer
            if (_firstHit)
            {
                _firstHit = false;
                GameManager.Instance.SetState(GameState.Playing);
            }

            do
            {
                _currentIdx = Random.Range(0, _targetsMax);
                yield return null;
            } while (_currentIdx == _prevIdx);

            Target nextTarget = targets[_currentIdx];
            _prevIdx = _currentIdx;

            StartCoroutine(ShowTarget(nextTarget));
        }

        public void Reset()
        {   
            // Reset parameters
            _prevIdx = _currentIdx = 0;
            _currentHits = 0;
            _firstHit = true;

            DeactivateAllTargets(); // Deactivate any remaining targets

            Core.BroadcastEvent("OnHitCountUpdate", this, 0);
        }

        public void ShowTargets()
        {
            StartCoroutine(ShowTarget(targets[0]));     // Show the first target
        }

        public void DeactivateAllTargets()
        {
            StopAllCoroutines();    // Ensure all coroutines are stopped

            foreach (var target in targets)
            {
                target.Deactivate();
            }
        }
    }
}
