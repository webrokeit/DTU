using UnityEngine;
using System.Collections;

public class KinectOverlayer : MonoBehaviour 
{
	public GUITexture BackgroundImage;

	public bool DebugOutput = false;

	void Start() {}
	
	void Update() {
		var manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized()) {
			if(BackgroundImage && (BackgroundImage.texture == null)) {
                Debug.Log("Camera is ready!");
                BackgroundImage.texture = manager.GetUsersClrTex();
			}
		}
	}
}
