using System;
using UnityEngine;
using System.Collections;

public class MovingObjectScript : MonoBehaviour {
    public float SpeedModifier = 1.0f;
    private float _moveSpeed = 100.0f;

    public Vector3 RotationSpeed = new Vector3(0f, 0f, 0f);

    public bool IsWide;
    public DangerousObjectManagerScript.StartPosition StartPositions;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
    void Update() {
        
        transform.Rotate(RotationSpeed.x, RotationSpeed.y, RotationSpeed.z);
    }

    public void StartMoving(float moveSpeed) {
        _moveSpeed = moveSpeed;
        rigidbody.velocity = Vector3.zero;
        //rigidbody.angularVelocity = Vector3.zero;
        rigidbody.AddForce(0f, 0f, -_moveSpeed, ForceMode.Acceleration);
    }
}
