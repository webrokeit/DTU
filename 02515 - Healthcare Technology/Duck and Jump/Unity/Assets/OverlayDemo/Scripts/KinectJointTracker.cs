using UnityEngine;
using System.Collections;

public class KinectJointTracker : MonoBehaviour  {
	public KinectWrapper.SkeletonJoint TrackedJoint = KinectWrapper.SkeletonJoint.RIGHT_KNEE;
	public GameObject OverlayObject;
	public float SmoothFactor = 5f;

	public bool DebugOutput = false;
	private float distanceToCamera = 10f;

	void Start() {
		if(OverlayObject) {
			distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;
		}
	}
	
	void Update() {
		var manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized()) {	
			int jointIndex = (int)TrackedJoint;
			
			if(manager.IsUserDetected()) {
				uint userId = manager.GetPlayer1ID();
				
				if(manager.IsJointTracked(userId, jointIndex)) {
					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, jointIndex);
					
					if(posJoint != Vector3.zero) {
						// 3d position to depth
						Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
						
						// depth pos to color pos
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
						
						float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;
						float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;
						
						if(OverlayObject){ 
							Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, SmoothFactor * Time.deltaTime);
						}
					}
				}			
			}
		}
	}
}
