using UnityEngine;
using System;
using System.Net.Sockets;


public class TCPConnection : MonoBehaviour
{
    //a true/false variable for connection status
    public bool socketReady = false;

    TcpClient mySocket;
    NetworkStream theStream;
    
    //try to initiate connection
    public void setupSocket(int conPort = 31400, string conHost = "192.168.255.2")
    {
        try
        {
            mySocket = new TcpClient(conHost, conPort);
            theStream = mySocket.GetStream();            
            theStream.ReadTimeout = 500;

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    public byte[] readSocket()
    {
        int msglen = 0;
        int bufferLength = 256;
        byte[] resultBytes = null;

        if (theStream != null && theStream.DataAvailable)
        {
            Byte[] inStream = new Byte[bufferLength];

            // read length first
            msglen = theStream.Read(inStream, 0, 1);

            if (msglen == 1)
            {
                int dataLength = inStream[0];
                resultBytes = new byte[dataLength];
                msglen += theStream.Read(inStream, 0, dataLength);

                Array.Copy(inStream, 0, resultBytes, 0, dataLength);
            }
        }

        return resultBytes;
    }

    // disconnect from the socket
    public void closeSocket()
    {
        if (!socketReady)
            return;

        mySocket.Close();
        socketReady = false;
    }
}

