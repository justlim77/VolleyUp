using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Highscore : MonoBehaviour
{
    public static int TopScore;

    public KeyCode resetStatsKey = KeyCode.Delete;
    public int addSpeed = 5;

    Text _label;
    int _currentScore;
    int _targetScore;

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
            PlayerPrefs.SetInt("highscore", 0);
            OnHighscoreUpdate(this, 0);
            _currentScore = 0;
        }
        
        if (_currentScore != _targetScore)
        {
            _currentScore += (int)(addSpeed * Time.deltaTime);
            _label.text = TopScore.ToString();
        }
    }

    object OnHighscoreUpdate(object sender, object args)
    {
        if (args is int)
        {
            int highscore = (int)args;
            if (highscore > TopScore)
            {
                TopScore = _targetScore = highscore;
                PlayerPrefs.SetInt("highscore", highscore);
            }
        }

        return null;
    }
}
