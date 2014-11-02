using System;
using UnityEngine;
using System.Collections;

public class PersonaControl : MonoBehaviour {
    private bool _canJump;

	// Use this for initialization
	void Start () {
        _canJump = true;
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
}
