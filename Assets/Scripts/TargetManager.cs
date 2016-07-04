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
        int _targetHitsRequired;
        int _currentHits;
        int _prevIdx, _currentIdx;

        void Awake()
        {
            if(Instance == null)
                Instance = this;

            Core.SubscribeEvent("OnTargetHitUpdate", OnTargetHitUpdate);
        }

        void OnDestroy()
        {
            Instance = null;

            Core.UnsubscribeEvent("OnTargetHitUpdate", OnTargetHitUpdate);
        }

        object OnTargetHitUpdate(object sender, object args)
        {
            _currentHits++;
            _currentHits = Mathf.Clamp(_currentHits, 0, _targetHitsRequired);
            Core.BroadcastEvent("OnTargetScoreUpdate", this, _currentHits);
            return null;
        }

	    void Start ()
        {
            _targetsMax = targets.Length;
            _targetHitsRequired = targetHitsRequired;

            Reset();
	    }

        IEnumerator ShowTarget(Target target)
        {
            target.Activate();

            while (target.HasInteracted == false)
            {
                yield return null;
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
            _prevIdx = _currentIdx = 0;
            _currentHits = 0;

            foreach (var target in targets)
            {
                target.Deactivate();
            }

            Core.BroadcastEvent("OnTargetScoreUpdate", this, 0);
        }

        public void StartRound()
        {
            StartCoroutine(ShowTarget(targets[0]));
        }
    }
}
