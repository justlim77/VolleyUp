using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Volley
{
    [RequireComponent(typeof(Text))]
    public class ScoreLabel : MonoBehaviour
    {
        Text _label;

	    void Awake ()
        {
            _label = GetComponent<Text>();
            if (_label != null)
            {
                _label.text = "0";
            }

            Core.SubscribeEvent("OnScoreUpdate", OnScoreUpdate);
	    }

        void Start() { }

        object OnScoreUpdate(object sender, object args)
        {
            if (args is int)
            {
                int score = (int)args;
                _label.text = score.ToString();
            } 

            return null;
        }
    }
}
