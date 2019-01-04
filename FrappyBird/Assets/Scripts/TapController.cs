using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController: MonoBehaviour {

    private static readonly string SCOREZONE_TAG = "ScoreZone";
    private static readonly string DEADZONE_TAG = "DeadZone";
    private static readonly Quaternion DOWN_ROTATION = Quaternion.Euler(0, 0, -60);
    private static readonly Quaternion FORWARD_ROTATION = Quaternion.Euler(0, 0, 25);

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

	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = DOWN_ROTATION;
        forwardRotation = FORWARD_ROTATION;
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

    void Update () {
        if (game.GameOver) return;
        if (Input.GetMouseButtonDown(0))
        {
            tapAudio.Play();
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector3.up * tapForce, ForceMode2D.Force);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth*Time.deltaTime);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == SCOREZONE_TAG)
        {
            //register a score event
            //event sent to GameManager
            OnPlayerScored();
            scoreAudio.Play();
        }

        if(collision.gameObject.tag == DEADZONE_TAG)
        {
            rigidbody.simulated = false;
            //register a dead event
            //event sent to GameManager
            OnPlayerDied();
            deadAudio.Play();
        }
    }
}
