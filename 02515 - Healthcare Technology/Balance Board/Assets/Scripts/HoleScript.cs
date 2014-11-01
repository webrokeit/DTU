using UnityEngine;
using System.Collections;

public class HoleScript : MonoBehaviour {
	
	public BrainScript Brain;
	
	void OnTriggerEnter(Collider other)
	{
		Brain.Reset();
	}
}
