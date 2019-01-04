using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static readonly string HIGHSCORE_TEXT = "HighScore";
    private static readonly int INITIAL_SCORE = 0;
    private static readonly bool GAMEOVER_STATE = true;

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver
    }

    int score = INITIAL_SCORE;
    bool gameOver = GAMEOVER_STATE;

    public bool GameOver { get { return gameOver; } }

    public void ConfirmGameOver()
    {
        SetPageState(PageState.Start);
        scoreText.text = INITIAL_SCORE.ToString();
        OnGameOverConfirmed();
    }

    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
    }

    void OnDisable()
    {
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted();
        score = INITIAL_SCORE;
        gameOver = !GAMEOVER_STATE;
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void OnPlayerDied()
    {
        gameOver = GAMEOVER_STATE;
        int savedScore = PlayerPrefs.GetInt(HIGHSCORE_TEXT);
        if (score > savedScore)
        {
            PlayerPrefs.SetInt(HIGHSCORE_TEXT, score);
        }
        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
        }
    }
}