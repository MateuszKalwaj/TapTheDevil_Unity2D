using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] //kiedy wrzucę TapController do obiektu, ten z miejsca dostanie Rigidbody (1)
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10; //tworzy i definiuje pole siły podskoku diabełka(1)
    public float tiltSmooth = 5; //rotacja podczas spadania(1)
    public Vector3 startPos; // miejsce, w którym zacznie diabełek po restarcie gry(1)

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D myRigidbody; //będziemy wykorzystywać rigidbody, tworzymy nowe pole(1)
    Quaternion downRotation; //odpowiada za rotacje, ma komponenty (x, y, z, w)(1)
    Quaternion forwardRotation; //odpowiada za rotacje, ma komponenty (x, y, z, w)(1)

    GameManager game;

//co się dzieje na starcie (1)
    void Start() {  
        myRigidbody = GetComponent<Rigidbody2D>(); //wywołujemy rigidbody (1)
        downRotation = Quaternion.Euler(0, 0, -90); //w tym miejscu korzystamy z vectora 3 i zmieniamy na quaternion z liczbą eulera (1)
        forwardRotation = Quaternion.Euler(0, 0, 35);//w tym miejscu korzystamy z vectora 3 i zmieniamy na quaternion z liczbą eulera (1)
        game = GameManager.Instance;
        myRigidbody.simulated = false; //kiedy odpalimy grę, diabełek nie spadnie(1)
    }

    private void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted() {
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.simulated = true;
    }

    void OnGameOverConfirmed() {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame. Czyli odświeżanie widoku, co powoduje wrażenie działającej gry (1)
    void Update() {
        if (game.GameOver) return;
        if (Input.GetMouseButtonDown(0)) {      //kiedy naciśniemy przycisk w telefonie, to: (1)
            tapAudio.Play();
            transform.rotation = forwardRotation; //wywołujemy naszą rotację (1)
            myRigidbody.velocity = Vector3.zero; // potrzebne do tego, żeby grawitacja nie w każdym momencie działała na diabełka. Kiedy tego nie ma, gra działa, ale jest nieprzyjemna w obsłudze (1)
            myRigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force); //operujemy na vectorach, czyli diabełek leci w górę dzięki naciśnięciu (1)
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime); //co ważne, "transform" odwołuje się do quaternion. W tym miejscu opóźniamy lot i dodajemy lepszy efekt działania (1)
    }

    //ta metoda będzie szukała scoreZone i deadZone, czyli sprawdza, czy dostajemy punkt, czy giniemy (1)
    private void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.tag == "ScoreZone") { //korzystając z Collider'a, możemy zdefiniować co się wydarzy gdy diabełek uderzy w zdefiniowany obiekt (1)
            //register a score event

            OnPlayerScored();  //event sent to GameManager
            //play a sound
            scoreAudio.Play();
        }

        if (col.gameObject.tag == "DeadZone") {  //korzystając z Collider'a, możemy zdefiniować co się wydarzy gdy diabełek uderzy w zdefiniowany obiekt (1)
            myRigidbody.simulated = false;  //przestajemy symulować rigidbody, diabełek spada i ginie (1)
            OnPlayerDied(); //event sent to GameManager
            //register a dead event
            //play a sound
            dieAudio.Play();
        }
    }
}
