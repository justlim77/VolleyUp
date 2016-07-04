using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Volley
{
    public class HitLabel : MonoBehaviour
    {
        Text _label;
        int _targetedHits;

        // Use this for initialization
        void Start ()
        {
            _label = GetComponent<Text>();

            TargetManager manager = TargetManager.Instance;
            _targetedHits = manager.targetHitsRequired;

            Core.SubscribeEvent("OnTargetScoreUpdate", OnTargetScoreUpdate);
	    }

        void OnDestroy()
        {
            Core.UnsubscribeEvent("OnTargetScoreUpdate", OnTargetScoreUpdate);
        }

        object OnTargetScoreUpdate(object sender, object args)
        {
            if (args is int)
            {
                int currentHits = (int)args;
                string msg = string.Format("{0}/{1}", currentHits, _targetedHits);

                _label.text = msg;
            }

            return null;
        }
    }
}
