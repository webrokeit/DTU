using UnityEngine;
using System.Collections;

public class KinectJointTracker : MonoBehaviour  {
	public KinectWrapper.SkeletonJoint TrackedJoint = KinectWrapper.SkeletonJoint.RIGHT_KNEE;
	public GameObject OverlayObject;
	public float SmoothFactor = 5f;

	public bool DebugOutput = false;
	private float _distanceToCamera = 10f;

	void Start() {
		if(OverlayObject) {
			_distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;
		}
	}
	
	void Update() {
		var manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized()) {	
			var jointIndex = (int)TrackedJoint;
			
			if(manager.IsUserDetected()) {
				var userId = manager.GetPlayer1ID();
				
				if(manager.IsJointTracked(userId, jointIndex)) {
					var posJoint = manager.GetRawSkeletonJointPos(userId, jointIndex);
					
					if(posJoint != Vector3.zero) {
						// 3d position to depth
						var posDepth = manager.GetDepthMapPosForJointPos(posJoint);
						
						// depth pos to color pos
						var posColor = manager.GetColorMapPosForDepthPos(posDepth);
						
						var scaleX = posColor.x / KinectWrapper.Constants.ColorImageWidth;
						var scaleY = 1.0f - posColor.y / KinectWrapper.Constants.ColorImageHeight;
						
						if(OverlayObject){ 
							var vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, _distanceToCamera));
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, SmoothFactor * Time.deltaTime);
						}
					}
				}			
			}
		}
	}
}
