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

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        
        root = GameObject.Find("sensorfusion");
        conn = (ConnectionScript)root.GetComponent<ConnectionScript>();

        // attach rotator to correct sensor fusion instance
        parentGameObjectName = gameObject.name;

        if (parentGameObjectName == "kionix_iot")
        {
            myfusion = conn.myfusion;
        }
        else if (parentGameObjectName == "kionix_iot_2")
        {
            myfusion = conn.myfusion2;
        }
        else if (parentGameObjectName == "kionix_iot_3")
        {
            myfusion = conn.myfusion3;
        }
        else if (parentGameObjectName == "kionix_iot_4")
        {
            myfusion = conn.myfusion4;
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
            rb.rotation = myq;
        }
        catch (Exception err)
        {
            print("error in q: " + myq.ToString() + "err: " + err.ToString());
        }
    }
}
