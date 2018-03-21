using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

using SensorFusionSharpIF;
using System.IO;

public class Fusion : MonoBehaviour {
    private KionixFusionIF sF = null;
    private MyWorkerThread workerThread = null;
    private float gx, gy, gz, ax, ay, az, mx, my, mz;

    private float grx = 0, gry = 0, grz = 0; // value before offset correction
    private float gxo = 0, gyo = 0, gzo = 0; // offset value
    private float gtx, gty, gtz;             // temporary gyro values
    private float cfx = 0, cfy = 0, cfz = 0, cfw = 1; // "c" fusion quaternion
    private float gyroRawMultiplier = 0.001090830782496456f; // for getting gyro raw data
    private System.Object dataLock = new System.Object();
    private UInt64 samplecount = 0;

    public Quaternion myq;
    private Quaternion reset_myq;
    private Quaternion storedQuaternion;
    private Quaternion lastQuaternion;
    private Quaternion prevQuaternion;
    private Quaternion origQuaternion = new Quaternion(0, 0, 0, 1);
    private bool gyroOffsetResetNeeded = true; // set to true when space pressed

    private float prevAx, prevAy, prevAz = 0;
    public bool movementDetected = false;
    public int instanceNumber = 1;

    // Use this for initialization
    void Start () {
        myq = new Quaternion();

        myq.SetEulerRotation(0, 0, 0);

        reset_myq = new Quaternion(0, 0, 0, 1);

        // initialize Kionix sensor fusion
        if (sF == null)
        {
            if (Application.isEditor)
            {
                sF = new KionixFusionIF(instanceNumber, Path.Combine(Application.dataPath, "GameJamTemplate\\Plugins"));
            }
            else
            {
                sF = new KionixFusionIF(instanceNumber, Path.Combine(Application.dataPath, "Plugins"));
            }
            
            // setup event handler if needed
            sF.QuaternionChanged += OnQuaternionEvent;

            int fusionStatus;
            sF.InitializeFusion((UInt16)SensorFusionType.FUSION_TYPE_6_AXIS_ACC_GYRO);
            fusionStatus = sF.StartFusion();

            workerThread = new MyWorkerThread();
            workerThread.Priority = System.Threading.ThreadPriority.AboveNormal;
            //workerThread.Priority = System.Threading.ThreadPriority.Highest;
            workerThread.Start();
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey("space"))
        {
            gyroOffsetResetNeeded = true;     // enable automatic gyro offset reset on game startup

            print("resetpos");
            // store current quaternion taking cube position into account
            storedQuaternion = lastQuaternion;

            storedQuaternion = Quaternion.Inverse(storedQuaternion);
            reset_myq = storedQuaternion * origQuaternion;
        }
	}

    float getQuaternionValue(string qString)
    {
        uint num = uint.Parse(qString, System.Globalization.NumberStyles.AllowHexSpecifier);
        byte[] floatVals = BitConverter.GetBytes(num);
        Array.Reverse(floatVals);
        return BitConverter.ToSingle(floatVals, 0);
    }

    //void calculate(string serverSays)
    void calculate(byte[] bytesFromServer)
    {
        // just return sensor fusion is not yet ready
        if (sF == null)
        {
            return;
        }

        samplecount += 1;

        var bytes = ConvertFuncs.ToByteArray(bytesFromServer);

        // TODO: Add other stream data options/data lengths
        if (bytes.Length == 9)
        {
            // get Gyroscope data
            gtx = bytes[0];
            gty = bytes[1];
            gtz = bytes[2];
            gx = gtx * gyroRawMultiplier;
            gy = gty * gyroRawMultiplier;
            gz = gtz * gyroRawMultiplier;

            grx = gx;gry = gy;grz = gz;
            gx -= gxo;gy -= gyo;gz -= gzo;

            // get Accelerometer data
            ax = bytes[3];
            ay = bytes[4];
            az = bytes[5];

            // store previous values
            if (prevAx != 0 || prevAy != 0 || prevAz != 0)
            {
                //print(String.Format("acc difference: {0}, {1}, {2}", Math.Abs(ax - prevAx), Math.Abs(ay - prevAy), Math.Abs(az - prevAz)));
                if (Math.Abs(ax - prevAx) >= 100 || Math.Abs(ay - prevAy) >= 100 || Math.Abs(az - prevAz) >= 100)
                {    
                    movementDetected = true;
                }
                else
                {
                    movementDetected = false;
                }
            }

            prevAx = ax;
            prevAy = ay;
            prevAz = az;

            // get Magnetometer data
            mx = bytes[6];
            my = bytes[7];
            mz = bytes[8];
        }

        // run sensor fusion with raw sensor data
        runFusion(bytesFromServer);
    }

    int runFusion(byte[] sensorData)
    {
        int fusionStatus = -1;

        if (sensorData.Length != 20)
        {
            print("Invalid fusion data!!!");
            return -1;
        }

        fusionStatus = sF.InputSensorData(sensorData);
        fusionStatus = sF.RunFusion();

        return fusionStatus;
    }

    public void calculateFusion(byte[] rawData)
    {
        calculate(rawData);   
    }

    public UInt64 getSampleCount()
    {
        return samplecount;
    }

    public void resetGyroOffset()
    {
        // sets current rotation speed to offset
        lock (dataLock)
        {
            float m = 1f;
            gxo = grx*m;
            gyo = gry*m;
            gzo = grz*m;
        }
        return;
    }

    public Vector3 getAccXYZ()
    {
        Vector3 acc = new Vector3();
        lock (dataLock)
        {
            acc.Set(ax, ay, az);
        }
        return acc;
    }

    public Vector3 getMagXYZ()
    {
        Vector3 mag = new Vector3();
        lock (dataLock)
        {
            mag.Set(mx, my, mz);
        }
        return mag;
    }

    public Vector3 getGyroXYZ()
    {
        Vector3 gyro = new Vector3();
        lock (dataLock)
        {
            gyro.Set(gx, gy, gz);
        }
        return gyro;
    }

    public Quaternion getQuaternion()
    {
        Quaternion q;
        lock (dataLock) {
            q = new Quaternion(myq.x, myq.y, myq.z, myq.w);
        }
        return q;
    }

    private void OnQuaternionEvent(object sender, QuaternionEvent e)
    {
        // get quaternion values
        cfw = e.W;
        cfx = e.X;
        cfy = e.Y;
        cfz = e.Z;

        //print(String.Format("quaternion data: {0}, {1}, {2}, {3}", cfw, cfx, cfy, cfz));
        lock (dataLock)
        {
            myq.w = cfw;
            myq.x = -cfx;
            myq.y = -cfy;
            myq.z = cfz;

            // store last quaternion coming from fusion
            lastQuaternion = myq;

            // change quaternion according to stored position
            myq = reset_myq * myq;

            // calculate angle for gyro offset reset
            float quatAngle = Quaternion.Angle(prevQuaternion, myq);

            if (gyroOffsetResetNeeded && quatAngle <= 0.04)
            {
                //print("Angle: " + quatAngle);
                print("Automatic gyro offset reset done");
                resetGyroOffset();
                gyroOffsetResetNeeded = false;
            }

            // store quaternion for angle check
            prevQuaternion = myq;
        }
    }
}

public static class ConvertFuncs
{
    public static int[] ToByteArray(byte[] bytes)
    {
        byte[] byteData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] sensorData = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        int index = 0;
        byte messageLength = 0;
        byte messageType = 0;
        int totalLength = bytes.Length;

        for (int i = 0; i < totalLength; i++)
        {
            if (i == 0 || i == 1)
            {
                if (i == 0)
                {
                    messageLength = bytes[i];
                }
                else if (i == 1)
                {
                    messageType = bytes[i];
                }

                continue;
            }

            byteData[index] = bytes[i];
            index++;
        }

        index = 0;

        for (int i = 0; i < totalLength - 2; i += 2)
        {
            byte LSB = byteData[i];
            byte MSB = byteData[i + 1];

            // 8bit unsigned bytes 16bit signed one
            short intValue = (short)((MSB << 8) | LSB);

            sensorData[index] = intValue;
            index++;
        }

        return sensorData;
    }        
}

public class MyDataQueue<T>
{
    private readonly Queue<T> queue = new Queue<T>();
    private object LOCK = new object();
    private Action Callback = null;

    public void SetCallBack(Action callBackFunction)
    {
        Callback = callBackFunction;
    }

    public virtual void Enqueue(T item)
    {
        lock (LOCK)
        {
            queue.Enqueue(item);
            //OnEnqueued();

            if (Callback != null)
            {
                Callback();
            }
        }
    }
    public virtual int Count
    {
        get
        {
            return queue.Count;
        }
    }
    public virtual T Dequeue()
    {
        lock (LOCK)
        {
            T item = queue.Dequeue();
            return item;
        }
    }

    public virtual void Clear()
    {
        lock (LOCK)
        {
            queue.Clear();
        }
    }

    public virtual T[] Empty()
    {
        T[] queueContent = null;

        lock (LOCK)
        {
            queueContent = queue.ToArray();
            queue.Clear();
        }

        return queueContent;
    }
}

public abstract class BaseThread
{
    protected Thread _thread;
    protected MyDataQueue<Action> commandQueue = new MyDataQueue<Action>();

    protected BaseThread()
    {
        _thread = new Thread(new ThreadStart(this.RunThread));
        _thread.IsBackground = true;
    }

    // Thread methods / properties
    public void Start() { _thread.Start(); }
    public void Join() { _thread.Join(); }
    public void Abort() { _thread.Abort(); }
    public bool IsAlive { get { return _thread.IsAlive; } }

    public System.Threading.ThreadPriority Priority
    {
        get { return _thread.Priority; }
        set { _thread.Priority = value; }
    }

    // Override in base class
    public abstract void RunThread();
}

public class MyWorkerThread : BaseThread
{    
    public void Add(Action command)
    {
        commandQueue.Enqueue(command);
    }

    public int Count
    {
        get
        {
            return commandQueue.Count;
        }
    }

    public override void RunThread()
    {
        while (IsAlive)
        {
            if (commandQueue.Count > 0)
            {
                Action command = commandQueue.Dequeue();
                try
                {
                    command.Invoke();
                }
                catch (ThreadAbortException)
                { }
                catch (Exception err)
                { }
            }
            else
            {
                Thread.Sleep(new TimeSpan(10000));
            }
        }
    }
}