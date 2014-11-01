using UnityEngine;
using System.Collections;

public class BrainScriptAdvanced : MonoBehaviour {
	
	
	public GameObject Ball;
	public GameObject RotationObject;
	public Camera ViewCamera;
	public GameObject StartZone;
	public float RotationGain = 0.3f;
	public float CameraGain = 0.3f;
	public float MaxRotation = 10f;
	
	private Vector3 CamOffset;

	// Use this for initialization
	void Start () {
		//Measure offset to camera
		CamOffset = ViewCamera.transform.position - Ball.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		//LEFT AND RIGHT
		if(Input.GetKey(KeyCode.RightArrow))
		{
			//Rotate around Z-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.z, 360-MaxRotation);	//How many degrees left?
			RotationObject.transform.Rotate(Vector3.forward, RotationGain * angleLeft * Time.deltaTime, Space.Self);
		}
		else if(Input.GetKey(KeyCode.LeftArrow))
		{
			//Rotate around Z-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.z, MaxRotation);	//How many degrees left?
			RotationObject.transform.Rotate(Vector3.forward, RotationGain * angleLeft * Time.deltaTime, Space.Self);		
		}
		
		//UP AND DOWN
		if(Input.GetKey(KeyCode.UpArrow))
		{
			//Rotate around X-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.x, MaxRotation);	//How many degrees left?
			RotationObject.transform.Rotate(Vector3.right, RotationGain * angleLeft * Time.deltaTime, Space.World);
		}
		else if(Input.GetKey(KeyCode.DownArrow))
		{
			//Rotate around X-axis
			float angleLeft = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.x, 360-MaxRotation);	//How many degrees left?
			RotationObject.transform.Rotate(Vector3.right, RotationGain * angleLeft * Time.deltaTime, Space.World);
		}
	
		//Check if space is pressed for reset
		if(Input.GetKey(KeyCode.Space))
			Reset();
		
		
		//Reset rotation when no key is pressed
		if(!Input.anyKey)
		{
			//Rotate around Z-axis
			float angleLeftZ = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.z, 0);	//How many degrees left?
			RotationObject.transform.Rotate(Vector3.forward, RotationGain * angleLeftZ * Time.deltaTime, Space.Self);
			//Rotate around X-axis
			float angleLeftX = Mathf.DeltaAngle(RotationObject.transform.eulerAngles.x, 0);	//How many degrees left?
			RotationObject.transform.Rotate(Vector3.right, RotationGain * angleLeftX * Time.deltaTime, Space.World);
		}
		
		
		//Move camera
		ViewCamera.transform.position = Vector3.Lerp(ViewCamera.transform.position, Ball.transform.position + CamOffset, CameraGain);
		
		//Debugging
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
