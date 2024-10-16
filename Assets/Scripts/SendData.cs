
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SendData : MonoBehaviour
{
    private SerialPort port;
    [SerializeField]
    private string COM;
    [SerializeField]
    private int baudRate;
    [SerializeField]
    // Start is called before the first frame update
    void Start()
    {
        port = new SerialPort(COM, baudRate);
        if (!port.IsOpen) {
            port.Open();
            Debug.Log("Port opened");
        }
    }

    public void SendToArduino(HandData handData) {
        string data = ""; 
        data = ConfigureData(handData);

        if (data != null && data != "" && port.IsOpen)
        {
            port.WriteLine(data);
            Debug.Log("Sent to Arduino: " + data);
        }

    }

    // Arduino will receive a string like this:
    // Left:32.32
    private string ConfigureData(HandData handData) {
        //string formattedData = $"{handData.handedness}:";
        string formattedData = "";

        for (int i = 1; i < 2; i++)
        {
            Vector3 pos = handData.positions[i];
            Quaternion rot = handData.rotations[i];

            //formattedData += $"Joint{i} Pos: {pos.x:F2}, {pos.y:F2}, {pos.z:F2} ";
            //formattedData += $"Rot: {rot.x:F2}, {rot.y:F2}, {rot.z:F2}, {rot.w:F2} | ";
            formattedData += $"{rot.x:F2}";

        }

        return formattedData.TrimEnd(); 
    }


    void OnApplicationQuit()
    {
        // Close the serial port when the application ends
        if (port.IsOpen)
        {
            port.Close();
            Debug.Log("Serial Port Closed");
        }
    }
}