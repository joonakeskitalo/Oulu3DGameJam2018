using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public GameObject player;
	public float offsetY;
	public float offsetZ;

	// Use this for initialization
	void Start () {
		transform.position = player.transform.position;
		offsetY = 2.5f;
		offsetZ = -2;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = player.transform.position;
		//Debug.Log (pos.y);
		Debug.Log (offsetY);
		pos.y += offsetY;
		pos.z += offsetZ;
		Debug.Log (pos.y);
		transform.position = pos;
	}
}
