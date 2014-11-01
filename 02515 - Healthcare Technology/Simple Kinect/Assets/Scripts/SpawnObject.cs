using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour {
	
	//Public variables
	public GameObject ObjectToSpawn;
	public float SecPerObject = 1;
	public float SpawnRadius = 2;
	
	//Private variables
	private float elapsed = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		//Increment time
		elapsed += Time.deltaTime;
		
		if(elapsed > SecPerObject)
		{
			//Reset time
			elapsed = 0;
			
			//Choose a random position to place object
			Vector3 spawnPos = new Vector3(this.transform.position.x + Random.Range(-SpawnRadius,SpawnRadius), this.transform.position.y, this.transform.position.z + Random.Range(-SpawnRadius,SpawnRadius));
			
			//Create a new object
			GameObject newObject = Instantiate(ObjectToSpawn,spawnPos,this.transform.rotation) as GameObject;
			//Make it a child of this GameObject
			newObject.transform.parent = this.transform;
			
			
		}
	}
}
