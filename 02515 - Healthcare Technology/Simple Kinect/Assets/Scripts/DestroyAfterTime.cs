using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {
	
	public float timeToDestroy;
	
	void Start()
    {
        Destroy (gameObject, timeToDestroy);
    }
	
}
