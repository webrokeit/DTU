using UnityEngine;
using System.Collections;

public class KinectJointTracker : MonoBehaviour 
{
//	public Vector3 TopLeft;
//	public Vector3 TopRight;
//	public Vector3 BottomRight;
//	public Vector3 BottomLeft;

	public KUInterface KinectInterface;
	public KinectWrapper.Joints TrackedJoint = KinectWrapper.Joints.KNEE_RIGHT;
	public GameObject OverlayObject;
	public float smoothFactor = 5f;

	public bool DebugOutput = false;
	private float distanceToCamera = 10f;

	void Start() {
		if(OverlayObject) {
			distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;
		}
	}
	
	void Update() 
	{
		var manager = KinectInterface;
		
		if(manager && manager.IsCameryReady) {	
			
			if(manager.IsUserDetected()) {
				int userId = 1;
				
				if(manager.IsJointTracked(userId, TrackedJoint)) {
					var posJoint = manager.GetJointPos(userId, TrackedJoint);

					if(posJoint != Vector3.zero)
					{
						// 3d position to depth
						Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
						
						// depth pos to color pos
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
						
						float scaleX = (float)posColor.x / manager.ColorImageWidth;
						float scaleY = 1.0f - (float)posColor.y / manager.ColorImageHeight;
						
//						Vector3 localPos = new Vector3(scaleX * 10f - 5f, 0f, scaleY * 10f - 5f); // 5f is 1/2 of 10f - size of the plane
//						Vector3 vPosOverlay = backgroundImage.transform.TransformPoint(localPos);
						//Vector3 vPosOverlay = BottomLeft + ((vRight * scaleX) + (vUp * scaleY));
						
						if(OverlayObject)
						{
							Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);
						}
					}
				}
				
			}
			
		}
	}
}
