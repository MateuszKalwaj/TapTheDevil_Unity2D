using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

  /*delegate jest podobny do pointerow z C, pozwala by metody były przekazywane jako parametry. 
    jest typem zmiennej referencyjnej, która przechowuje referencję do obiektu. 
    Służy do implemencji metod dotyczacych wydarzen (event) i odpowiedzi (call-back) do nich
  */
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance; //działa jak singleton. Bedzie odnośnikiem jako "this". Jest potrzebny żeby mieć dostęp do instancji GameManager

    //potrzebujemy poniższych referencji do stworzenia naszych "canvas" w Unity. Dzięki temu okreslamy stany gry (2)
    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    //enum jest już dokładnie po to aby odczytywać stany gry (2)
    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver
    }

    int score = 0; //wartość punktów (2)
    bool gameOver = true; //początek gry i stan gameOver (2)

    public bool GameOver { get { return gameOver; } } //taki zapis powoduje, że "gameOver" jest dostępny i widoczny, ale niemodyfikolwany (2)
    public int Score { get { return score; } } //taka sama sytuacja jak wyżej

    void Awake() {
        Instance = this; //tutaj pojawia się pierwsze wywołanie instancji GameManager
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
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }
    }

    public void ConfirmGameOver() {
        OnGameOverConfirmed(); //event sent to TapController
        scoreText.text = "0";
        SetPageState(PageState.Start);

        //activated on replay button
        
    }

    public void StartGame() {
        //activated on play button
        SetPageState(PageState.Countdown);
    }
}


