using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Highscore : MonoBehaviour
{
    public static int TopScore;

    public KeyCode resetStatsKey = KeyCode.Delete;
    public int addSpeed = 5;

    Text _label;
    int _targetScore;
    float _currentScore;

	// Use this for initialization
	void Start ()
    {
        _label = GetComponent<Text>();

        Core.SubscribeEvent("OnHighscoreUpdate", OnHighscoreUpdate);

        _currentScore = _targetScore = TopScore = PlayerPrefs.GetInt("highscore", 0);
        _label.text = TopScore.ToString();
    }

    void OnDestroy()
    {
        Core.UnsubscribeEvent("OnHighscoreUpdate", OnHighscoreUpdate);
    }

    void Update()
    {
        if (Input.GetKeyDown(resetStatsKey))
        {
            ResetHighScore();
        }
        
        if (_currentScore < _targetScore)
        {
            _currentScore += addSpeed * Time.deltaTime;
            _currentScore = Mathf.Clamp(_currentScore, 0, _targetScore);
            _label.text = _currentScore.ToString("F0");
        }
    }

    object OnHighscoreUpdate(object sender, object args)
    {
        if (args is int)
        {
            int highscore = (int)args;
            if (highscore > TopScore || highscore == 0) // Higher or reset
            {
                TopScore = _targetScore = highscore;
                PlayerPrefs.SetInt("highscore", highscore);
            }
        }

        return null;
    }

    void ResetHighScore()
    {
        PlayerPrefs.SetInt("highscore", 0); // Reset prefs
        _currentScore = _targetScore = 0;   // Reset current/target scores
        _label.text = "0";                  // Reset score label text
    }
}
