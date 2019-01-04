using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour {

    private static readonly string COUNTDOWN_TEXT = "Countdown";
    private static readonly int COUNTDOWN = 3;

    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    Text countdown;

    void OnEnable()
    {
        countdown = GetComponent<Text>();
        countdown.text = COUNTDOWN.ToString();
        StartCoroutine(COUNTDOWN_TEXT);
    }

    IEnumerator Countdown()
    {
        int count = COUNTDOWN;
        for(int i=0; i<count; i++)
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }
        OnCountdownFinished();
    }
}
