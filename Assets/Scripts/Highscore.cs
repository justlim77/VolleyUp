using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Highscore : MonoBehaviour
{
    public KeyCode resetStatsKey = KeyCode.Delete;

    Text _label;
    public static int TopScore;

	// Use this for initialization
	void Start ()
    {
        _label = GetComponent<Text>();

        Core.SubscribeEvent("OnHighscoreUpdate", OnHighscoreUpdate);

        TopScore = PlayerPrefs.GetInt("highscore", 0);
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
        }
    }

    object OnHighscoreUpdate(object sender, object args)
    {
        if (args is int)
        {
            int highscore = (int)args;
            if (highscore > TopScore)
            {
                TopScore = highscore;
                PlayerPrefs.SetInt("highscore", highscore);
                _label.text = TopScore.ToString();
            }
        }

        return null;
    }
}
