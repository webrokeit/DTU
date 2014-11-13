using System;
using Assets.Managers;
using UnityEngine;
using System.Collections;

public class PersonaControl : MonoBehaviour {
    private bool _canJump;
    private readonly Vector3 _playerResetPosition = new Vector3(0f, 1.235f, 0f);

	// Use this for initialization
	void Start () {
        /*_canJump = true;*/
	    _canJump = false;
        rigidbody.transform.position = _playerResetPosition;
	    renderer.enabled = false;
	    rigidbody.isKinematic = true;
	    enabled = false;
	    GameEventManager.GameStart += GameStart;
	    GameEventManager.GameOver += GameOver;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(KeyCode.Space) && _canJump) {
	        _canJump = false;
            rigidbody.AddForce(Vector3.up * 250f);
	    }
	}

    void OnCollisionEnter() {
        Debug.Log("OnCollisionEnter");
        _canJump = true;
    }

    void OnCollisionExit() {
        Debug.Log("OnCollisionExit");
        _canJump = false;
    }

    private void GameStart() {
        rigidbody.transform.position = _playerResetPosition;
        rigidbody.transform.eulerAngles = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;
        renderer.enabled = true;
        rigidbody.isKinematic = false;
        this.enabled = true;
        _canJump = true;
    }

    private void GameOver() {
        renderer.enabled = false;
        rigidbody.isKinematic = true;
        this.enabled = false;
        _canJump = false;
    }
}
