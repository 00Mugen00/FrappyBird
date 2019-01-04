using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HighscoreText : MonoBehaviour {

    private static readonly string HIGHSCORE_TEXT = "HighScore";

    Text highscore;

    void OnEnable()
    {
        highscore = GetComponent<Text>();
        highscore.text = HIGHSCORE_TEXT + ": " +PlayerPrefs.GetInt(HIGHSCORE_TEXT).ToString();
    }
}
