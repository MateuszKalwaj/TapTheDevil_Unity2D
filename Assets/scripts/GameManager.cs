using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance; //działa jak singleton. Bedzie odnośnikiem jako "this"

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countDownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        GameOver,
        CountDown
    }

    int score = 0;
    bool gameOver = false;

    public bool GameOver { get { return gameOver; } }

    void Awake() {
        Instance = this;
    }

    void OnEnable() {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    void OnDisable() {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountdownFinished() {
        SetPageState(PageState.None);
        OnGameStarted(); //event sent to TapController
        score = 0;
        gameOver = false;
    }

    void OnPlayerDied() {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore) {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    void OnPlayerScored() {
        score++;
        scoreText.text = score.ToString();
    }

    void SetPageState(PageState state) {

        switch (state) {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countDownPage.SetActive(false);
                break;
            case PageState.CountDown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(true);
                break;

        }
    }

    public void ConfirmGameOver() {
        //activated on replay button
        OnGameOverConfirmed(); //event sent to TapController
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame() {
        //activated on play button
        SetPageState(PageState.CountDown);
    }
}


