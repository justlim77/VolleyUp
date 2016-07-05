using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Volley
{
    public class HitLabel : MonoBehaviour
    {
        Text _label;

        // Use this for initialization
        void Awake()
        {
            _label = GetComponent<Text>();

            Core.SubscribeEvent("OnHitCountUpdate", OnHitCountUpdate);
        }

        void Start ()
        {

	    }

        void OnDestroy()
        {
            Core.UnsubscribeEvent("OnHitCountUpdate", OnHitCountUpdate);
        }

        object OnHitCountUpdate(object sender, object args)
        {
            if (args is int)
            {
                int currentHits = (int)args;
                TargetManager manager = TargetManager.Instance;
                int targetedHits = manager.targetHitsRequired;
                string msg = string.Format("{0}/{1}", currentHits, targetedHits);

                _label.text = msg;
            }

            return null;
        }
    }
}
