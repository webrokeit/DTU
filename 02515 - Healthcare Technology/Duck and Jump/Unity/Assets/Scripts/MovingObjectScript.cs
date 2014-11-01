using UnityEngine;
using System.Collections;

public class MovingObjectScript : MonoBehaviour {
    public float MoveForce = 10f;

    private bool done;
	// Use this for initialization
	void Start () {
	    done = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if (done) return;
        rigidbody.AddForce(0f, 0f, -MoveForce, ForceMode.Acceleration);
	    done = true;
	}
}
