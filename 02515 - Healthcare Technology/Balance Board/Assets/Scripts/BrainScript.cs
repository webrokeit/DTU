using UnityEngine;
using System.Collections;

public class BrainScript : MonoBehaviour {
	
	
	public GameObject Ball;
	public GameObject RotationObject;
	public GameObject StartZone;
	public float RotationSpeed = 10f;
	public float MaxRotation = 10f;
		

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {

		//LEFT AND RIGHT
		if(Input.GetKey(KeyCode.RightArrow))
		{
			//Rotate around Z-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.z, 360-MaxRotation);	//How many degrees left?
			if(angleLeft < 0)
				RotationObject.transform.Rotate(Vector3.forward, -RotationSpeed * Time.deltaTime, Space.Self);
		}
		else if(Input.GetKey(KeyCode.LeftArrow))
		{
			//Rotate around Z-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.z, MaxRotation);	//How many degrees left?
			if(angleLeft > 0)
				RotationObject.transform.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime, Space.Self);		
		}
		
		//UP AND DOWN
		if(Input.GetKey(KeyCode.UpArrow))
		{
			//Rotate around X-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.x, MaxRotation);	//How many degrees left?
			if(angleLeft > 0)
				RotationObject.transform.Rotate(Vector3.right, RotationSpeed * Time.deltaTime, Space.World);
		}
		else if(Input.GetKey(KeyCode.DownArrow))
		{
			//Rotate around X-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.x, 360-MaxRotation);	//How many degrees left?
			if(angleLeft < 0)
				RotationObject.transform.Rotate(Vector3.right, -RotationSpeed * Time.deltaTime, Space.World);
		}
	
		if(Input.GetKey(KeyCode.Space))
			Reset();
		
		if(Input.anyKey)
		{
			/*Debug.Log("Distance to 30: "+Mathf.DeltaAngle(currentAngles.x, 30));
			Debug.Log("Distance to 330: "+Mathf.DeltaAngle(currentAngles.x, 330));
			Debug.Log("Local rot: "+RotationObject.transform.localEulerAngles.ToString());
			Debug.Log("Global rot: "+RotationObject.transform.eulerAngles.ToString());
			Debug.Log("Rotation: "+(RotationSpeed * Time.deltaTime));	*/

		}

	
	}
	
	
	public void Reset()
	{
		//Reset physics on ball
		Ball.rigidbody.velocity = Vector3.zero;
		Ball.rigidbody.angularVelocity = Vector3.zero;
		
		//Reset position to 4 units above the startzone
		Ball.transform.position = StartZone.transform.position + 4 * Vector3.up;
		
	}
}
