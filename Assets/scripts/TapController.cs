using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    Rigidbody2D myRigidbody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;


    void Start() {
        myRigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        //myRigidbody.simulated = false;
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

    // Update is called once per frame
    void Update() {
        if (game.GameOver) return;
        if (Input.GetMouseButtonDown(0))
        {
            transform.rotation = forwardRotation;
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ScoreZone") {
            //register a score event
            OnPlayerScored(); //event sent to GameManager
            //play a sound
        }

        if (col.gameObject.tag == "DeadZone") {
            myRigidbody.simulated = false;
            OnPlayerDied(); //event sent to GameManager
            //register a score event
            //play a sound
        }
    }
}
