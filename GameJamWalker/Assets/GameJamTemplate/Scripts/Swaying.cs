using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swaying : MonoBehaviour {

	public float rotationScale = 1.0f;
	public float zScale = 1.0f;
	public float zScale2 = 2.0f;
	public float time;
	public float noiseValue;
	public float zRot;
	public float zRotSecondary;
	public float xSway;
	public float zRotTemp = 0;
	public float seed;

	// Use this for initialization
	void Start () {
		seed = Random.value * 100;
	}
	
	// Update is called once per frame
	void Update () {
		float noiseValue = rotationScale * Mathf.PerlinNoise((seed + Time.time * zScale), 0.0f);
		float noiseValue2 = (rotationScale * Mathf.PerlinNoise((seed + Time.time * zScale2 + 10000), 0.0f));

		time = Time.time;
		noiseValue -= 0.5f;
		noiseValue2 -= 0.5f;

		xSway = noiseValue * (time/50);

		//Debug.Log(xSway);    

		zRotTemp = zRot/1.5f;

		zRot = noiseValue * time * 1.5f;
		zRotSecondary = noiseValue2 * 2;

		//zRot += zRotTemp*1.5f;

		//xSway *= Mathf.Sign(zRot);

		Vector3 pos = transform.position;
		pos.x = xSway;
		pos.z += 0.05f;
		transform.position = pos;


		Quaternion target = Quaternion.Euler(0,0,zRot);
		//transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime);

		Quaternion target2 = Quaternion.Euler(0,0,zRotSecondary);
		Quaternion targetWhole = target2 * target;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetWhole, Time.deltaTime);


		//Debug.Log(zRot);
		//Debug.Log(target);
		//Debug.Log(seed);
	}
}
