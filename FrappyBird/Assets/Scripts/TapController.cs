using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController: MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource deadAudio;

    Rigidbody2D rigidbody;
    //Rotation
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

	// Use this for initialization
	void Start () {
        //Get the component
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -60);
        forwardRotation = Quaternion.Euler(0, 0, 25);
        game = GameManager.Instance;
        rigidbody.simulated = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update () {
        if (game.GameOver) return;
        //If "tap"
        if (Input.GetMouseButtonDown(0))
        {
            tapAudio.Play();
            // Bird should fly
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector3.up * tapForce, ForceMode2D.Force);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth*Time.deltaTime);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "ScoreZone")
        {
            // register a score event
            OnPlayerScored(); //event sent to GameManager
            // play a sound
            scoreAudio.Play();
        }

        if(collision.gameObject.tag == "DeadZone")
        {
            rigidbody.simulated = false;
            // register a dead event
            OnPlayerDied(); //event sent to GameManager
            // play a sound
            deadAudio.Play();
        }
    }
}
