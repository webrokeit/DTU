using UnityEngine;
using System.Collections;

public class KinectOverlayer : MonoBehaviour 
{
	public KUInterface KinectInterface;
	public GUITexture backgroundImage;

	public bool DebugOutput = false;
	public GUIText debugText;

	void Start() {}
	
	void Update() {
		var manager = KinectInterface;
		
		if(manager && manager.IsCameryReady) {
			if(DebugOutput) Debug.Log("Camera is ready!");
			if(backgroundImage && (backgroundImage.texture == null)) {
				backgroundImage.texture = manager.GetTextureImage();
			}
		}
	}
}
