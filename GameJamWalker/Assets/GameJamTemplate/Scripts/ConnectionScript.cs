using UnityEngine;
using System.Threading;
using System;


public class ConnectionScript : MonoBehaviour
{
    private TCPConnection myTCP;
    private TCPConnection myTCP2;
    private TCPConnection myTCP3;
    private TCPConnection myTCP4;
    
    private Rect rect;
    private GUIStyle style;
    private float timemultiplier = 1.0f;
    public static float timer;
    public static bool timeStarted = true;
    private string text;
    private int sampleCount = 0;
    private int odr = 100;
    private Thread networkThread = null;
    private Thread networkThread2 = null;
    private Thread networkThread3 = null;
    private Thread networkThread4 = null;

    int preSecondvalue = 0;

    public Fusion myfusion;
    public Fusion myfusion2;
    public Fusion myfusion3;
    public Fusion myfusion4;

    private bool killthread = false;
    public bool connected = false;

    void Awake()
    {
        // add copies of connection objects to this game object
        myTCP = gameObject.AddComponent<TCPConnection>();
        myTCP2 = gameObject.AddComponent<TCPConnection>();
        myTCP3 = gameObject.AddComponent<TCPConnection>();
        myTCP4 = gameObject.AddComponent<TCPConnection>();

        myfusion = gameObject.AddComponent<Fusion>();
        myfusion.instanceNumber = 1;
        myfusion2 = gameObject.AddComponent<Fusion>();
        myfusion2.instanceNumber = 2;
        myfusion3 = gameObject.AddComponent<Fusion>();
        myfusion3.instanceNumber = 3;
        myfusion4 = gameObject.AddComponent<Fusion>();
        myfusion4.instanceNumber = 4;
    }

    void Start()
    {
        int w = Screen.width, h = Screen.height;
        style = new GUIStyle();
        rect = new Rect(60, 690, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1f);
    }

    void OnDestroy()
    {
        try
        {
            killthread = true;

            if (networkThread != null && networkThread.IsAlive)
            {
                networkThread.Join(1000);
                print(String.Format("network thread finished: {0}", networkThread.ThreadState));
            }
            networkThread = null;

            if (networkThread2 != null && networkThread2.IsAlive)
            {
                networkThread2.Join(1000);
                print(String.Format("network thread 2 finished: {0}", networkThread2.ThreadState));
            }
            networkThread2 = null;

            if (networkThread3 != null && networkThread3.IsAlive)
            {
                networkThread3.Join(1000);
                print(String.Format("network thread 3 finished: {0}", networkThread3.ThreadState));
            }
            networkThread3 = null;

            if (networkThread4 != null && networkThread4.IsAlive)
            {
                networkThread4.Join(1000);
                print(String.Format("network thread 4 finished: {0}", networkThread4.ThreadState));
            }
            networkThread4 = null;
        }
        catch (Exception err)
        {
            Debug.Log("Error in OnDestroy: " + err.ToString());
        }

        print("ondestroy");
    }
    void Update()
    {
        if (Input.GetKey("g"))
        {
            myfusion.resetGyroOffset();
        }
        if (timeStarted == true)
        {
            timer += Time.deltaTime * timemultiplier;
        }
    }

    void OnGUI()
    {
        // if connection has not been made, display button to connect
        if (myTCP.socketReady == false)
        {
            if (GUILayout.Button("Connect"))
            {
                myTCP.setupSocket(31400);
                startServer();

                myTCP2.setupSocket(31401);
                startServer2();

                myTCP3.setupSocket(31402);
                startServer3();

                myTCP4.setupSocket(31403);
                startServer4();

                // try to connect
                Debug.Log("Attempting to connect..");
            }
        }

        if (myTCP.socketReady == true && myTCP2.socketReady == true)
        {
            connected = true;
        }

        // display ODR and timer
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);

        if (seconds != preSecondvalue)
        {
            preSecondvalue = seconds;

            text = string.Format("{0:0.0} s ({1:0.} odr)", seconds, sampleCount);

            if (sampleCount > 0)
            {
                odr = sampleCount;
            }
            sampleCount = 0;
        }

        //if (myfusion.movementDetected)
        //{
        //    text += "\n\nMovement detected";
        //}

        GUI.Label(rect, text, style);
    }

    public void startServer()
    {
        if (networkThread == null)
        {
            networkThread = new Thread(() =>
            {
                byte[] dataFromServer = null;

                while (killthread == false)
                {
                    try
                    {
                        dataFromServer = myTCP.readSocket();

                        if (dataFromServer != null)
                        {
                            myfusion.calculateFusion(dataFromServer);
                            sampleCount++;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.Log("Error in network thread: " + err.ToString());
                        break;
                    }
                }

                //networkThread = null;
                print("bye from thread");
                connected = false;
            });
            networkThread.Priority = System.Threading.ThreadPriority.Highest;
            //networkThread.Priority = System.Threading.ThreadPriority.AboveNormal;
            networkThread.IsBackground = true;
            networkThread.Start();
        }
    }

    public void startServer2()
    {
        if (networkThread2 == null)
        {
            networkThread2 = new Thread(() =>
            {
                byte[] dataFromServer = null;

                while (killthread == false)
                {
                    try
                    {
                        dataFromServer = myTCP2.readSocket();

                        if (dataFromServer != null)
                        {
                            myfusion2.calculateFusion(dataFromServer);
                            //sampleCount++;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.Log("Error in network thread: " + err.ToString());
                        break;
                    }
                }

                //networkThread2 = null;
                print("bye from thread 2");
                connected = false;
            });
            networkThread2.Priority = System.Threading.ThreadPriority.Highest;
            //networkThread2.Priority = System.Threading.ThreadPriority.AboveNormal;
            networkThread2.IsBackground = true;
            networkThread2.Start();
        }
    }

    public void startServer3()
    {
        if (networkThread3 == null)
        {
            networkThread3 = new Thread(() =>
            {
                byte[] dataFromServer = null;

                while (killthread == false)
                {
                    try
                    {
                        dataFromServer = myTCP3.readSocket();

                        if (dataFromServer != null)
                        {
                            myfusion3.calculateFusion(dataFromServer);
                            //sampleCount++;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.Log("Error in network thread: " + err.ToString());
                        break;
                    }
                }

                //networkThread3 = null;
                print("bye from thread 3");
                connected = false;
            });
            networkThread3.Priority = System.Threading.ThreadPriority.Highest;
            //networkThread3.Priority = System.Threading.ThreadPriority.AboveNormal;
            networkThread3.IsBackground = true;
            networkThread3.Start();
        }
    }

    public void startServer4()
    {
        if (networkThread4 == null)
        {
            networkThread4 = new Thread(() =>
            {
                byte[] dataFromServer = null;

                while (killthread == false)
                {
                    try
                    {
                        dataFromServer = myTCP4.readSocket();

                        if (dataFromServer != null)
                        {
                            myfusion4.calculateFusion(dataFromServer);
                            //sampleCount++;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.Log("Error in network thread: " + err.ToString());
                        break;
                    }
                }

                //networkThread4 = null;
                print("bye from thread 4");
                connected = false;
            });
            networkThread4.Priority = System.Threading.ThreadPriority.Highest;
            //networkThread4.Priority = System.Threading.ThreadPriority.AboveNormal;
            networkThread4.IsBackground = true;
            networkThread4.Start();
        }
    }
}