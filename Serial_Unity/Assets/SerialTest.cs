using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // Requires .NET 4 in Project Settings
using UnityEngine.UI;
using System.Linq;
using System;
using System.Text;

public class SerialTest : MonoBehaviour
{
    public Dropdown PortsDropdown;
    public Text ConnectionText;
    public Text ResponseText;
    private SerialPort _serial;
    private List<string> _ports;

    void Start()
    {
        RefreshPortsDropdown();
    }

    void Update()
    {
        if (_serial != null && _serial.IsOpen)
        {
            int bytesToRead = _serial.BytesToRead;
            if (bytesToRead > 0)
            {
                Debug.Log($"Bytes to read: {bytesToRead}");
                byte[] buff = new byte[bytesToRead];
                int read = _serial.Read(buff, 0, bytesToRead);
                if (read > 0)
                {
                    Debug.Log($"Read {read} bytes");
                    PrintBytes(ref buff);
                }
                else
                {
                    Debug.Log("Didn't read anything. :(");
                }
            }
        }
    }

    private void PrintBytes(ref byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append($"{b} ");
        }
        string str = sb.ToString();
        Debug.Log(str);
        ResponseText.text = str;
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public void RefreshPortsDropdown()
    {
        // Remove all the previous options
        PortsDropdown.ClearOptions();

        // Get port names
        string[] portNames = SerialPort.GetPortNames();
        _ports = portNames.ToList();

        // Add the port names to our options
        PortsDropdown.AddOptions(_ports);
    }

    public void ConnectToPort()
    {
        string port = _ports[PortsDropdown.value];

        // disconnect previous port if any
        Disconnect();

        try
        {
            _serial = new SerialPort(port, 115200)
            {
                Encoding = System.Text.Encoding.UTF8,
                DtrEnable = true
            };
            _serial.Open();

            ConnectionText.text = $"Connected to {port}";
            Debug.Log(ConnectionText.text);
        }
        catch (Exception e)
        {
            ConnectionText.text = e.Message;
        }
    }

    public void Disconnect()
    {
        if (_serial != null)
        {
            // close the connection if it is open
            if (_serial.IsOpen)
            {
                _serial.Close();
            }

            // release any resources being used
            _serial.Dispose();
            _serial = null;

            if (ConnectionText != null)
            {
                ConnectionText.text = "";
            }
            Debug.Log("Disconnected");
        }
    }

    public void Ping()
    {
        byte[] outBuffer = new byte[1];
        outBuffer[0] = 1;
        _serial.Write(outBuffer, 0, 1);
        Debug.Log("Ping");
    }
}
