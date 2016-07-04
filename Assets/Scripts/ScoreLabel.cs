using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Volley
{
    [RequireComponent(typeof(Text))]
    public class ScoreLabel : MonoBehaviour
    {
        public int addSpeed = 5;

        Text _label;
        int _targetScore;
        float _currentScore;

	    void Awake ()
        {
            _label = GetComponent<Text>();
            if (_label != null)
            {
                _label.text = "0";
            }

            _currentScore = _targetScore = 0;

            Core.SubscribeEvent("OnScoreUpdate", OnScoreUpdate);
	    }

        void Start() { }

        void Update()
        {
            if (_currentScore < _targetScore)
            {
                _currentScore += addSpeed * Time.deltaTime;
                _currentScore = Mathf.Clamp(_currentScore, 0, _targetScore);
                _label.text = _currentScore.ToString("F0");
            }
        }

        object OnScoreUpdate(object sender, object args)
        {
            if (args is int)
            {
                _targetScore = (int)args;

                if (_targetScore == 0)
                {
                    _currentScore = 0;
                    _label.text = _currentScore.ToString("F0");                }
            } 

            return null;
        }
    }
}
