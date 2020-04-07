using MathNet.Numerics;
using MathNet.Numerics.Differentiation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Socket : MonoBehaviour
{
    [SerializeField] Transform[] bottomLegs = null;
    [SerializeField] Transform[] topLegs = null;

    // Use this for initialization
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    String Host = "localhost";
    Int32 Port = 5500;

    private String[] positionLegs = null;
    private String[] endEffPosition = null;

    void Start()
    {
        //setupSocket();
    }

    private void setupSocket()
    {
        try
        {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            socketReady = true;

            //byte[] values = new byte[positionLegs.Length * sizeof(float)];
            //Buffer.BlockCopy(positionLegs, 0, values, 0, values.Length);
            //theStream.Write(values, 0, values.Length);

            Byte[] data;
            setupVariables();
            for (int i = 0; i < positionLegs.Length; i++)
            {
                data = System.Text.Encoding.ASCII.GetBytes(positionLegs[i]);
                theStream.Write(data, 0, positionLegs[i].Length);
                Debug.Log(positionLegs[i]);
            }

            for (int i = 0; i < 6; i++)
            {
                data = new Byte[5];
                theStream.Read(data, 0, data.Length);
                Debug.Log("LALAL " + data);
            }
            
            Debug.Log("socket is sent");
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }

    private void setupVariables()
    {
        positionLegs = new String[18];
        endEffPosition = new String[6];

        for (int i = 0; i < 6; i++)
        {
            double x = Math.Truncate((topLegs[i].position.x - bottomLegs[i].position.x) * 100) / 100;
            positionLegs[i * 3] = setupFormat(x);

            double y = Math.Truncate((topLegs[i].position.z - bottomLegs[i].position.z) * 100) / 100;
            positionLegs[i * 3 + 1] = setupFormat(y);

            double z = Math.Truncate((topLegs[i].position.y - bottomLegs[i].position.y) * 100) / 100;
            positionLegs[i * 3 + 2] = setupFormat(z);
        }
    }

    private string setupFormat(double value)
    {
        string s = null;
        if (value <= 0)
            s = string.Format("{0:N2}", value);
        else
            s = string.Format("{0:N3}", value);
        return s;
    }
}
