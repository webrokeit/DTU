using UnityEngine;
using System.Collections;

public class KinectOverlayer : MonoBehaviour 
{
	public GUITexture backgroundImage;

	public bool DebugOutput = false;
	public GUIText debugText;

	void Start() {}
	
	void Update() {
		var manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized()) {
			if(DebugOutput) Debug.Log("Camera is ready!");
			if(backgroundImage && (backgroundImage.texture == null)) {
				backgroundImage.texture = manager.GetUsersClrTex();
			}
		}
	}
}
