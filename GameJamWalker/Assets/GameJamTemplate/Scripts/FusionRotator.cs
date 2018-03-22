using UnityEngine;
using System.Collections;
using System;


public class FusionRotator : MonoBehaviour {
    private Rigidbody rb;
    public Quaternion myq;    
    private GameObject root;
    private Fusion myfusion;
    private ConnectionScript conn;
    public bool lockPosition;
    string parentGameObjectName = "";
    private float rotationSpeedMax = 0.5F;   // Maximum rotation speed

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        
        root = GameObject.Find("sensorfusion");
        conn = (ConnectionScript)root.GetComponent<ConnectionScript>();

        // attach rotator to correct sensor fusion instance
        parentGameObjectName = gameObject.name;

        if (parentGameObjectName == "glass_mug")
        {
            myfusion = conn.myfusion;
        }
        
        //myfusion = root.GetComponent<Fusion>();

        myq = new Quaternion();
        lockPosition = false;
    }
	
    // Update is called once per frame
	void Update () {        
        // get quaternion from sensor fusion
        myq = myfusion.getQuaternion();

        try
        {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, myq, rotationSpeedMax);
        }
        catch (Exception err)
        {
            print("error in q: " + myq.ToString() + "err: " + err.ToString());
        }
    }
}
