using UnityEngine;
using System.Collections;

public class KinectOverlayer : MonoBehaviour 
{
	public GUITexture BackgroundImage;
	
	void Update() {
		var manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized()) {
			if(BackgroundImage && (BackgroundImage.texture == null)) {
                BackgroundImage.texture = manager.GetUsersClrTex();
			}
		}
	}
}
