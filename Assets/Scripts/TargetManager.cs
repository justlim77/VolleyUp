using UnityEngine;
using System.Collections;

namespace Volley
{
    public class TargetManager : MonoBehaviour
    {
        public Target[] targets;

        static TargetManager _Instance;
        public static TargetManager Instance
        {
            get
            {
                return _Instance;
            }
        }

        int targetsMax;
        int prevIdx, currentIdx;

        void Awake()
        {
            _Instance = this;
        }

        void OnDestroy()
        {
            _Instance = null;
        }

	    void Start ()
        {
            targetsMax = targets.Length;
            prevIdx = currentIdx = 0;

            StartCoroutine(ShowTarget(targets[0]));
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
                currentIdx = Random.Range(0, targetsMax);
                yield return null;
            } while (currentIdx == prevIdx);

            Target nextTarget = targets[currentIdx];
            prevIdx = currentIdx;

            StartCoroutine(ShowTarget(nextTarget));
        }
    }
}
