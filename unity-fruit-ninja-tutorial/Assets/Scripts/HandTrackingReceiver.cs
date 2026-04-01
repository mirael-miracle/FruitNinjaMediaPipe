using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class HandTrackingReceiver : MonoBehaviour
{
    private UdpClient client;
    private Thread receiveThread;

    private int port = 5055;

    public static Vector2 Position = new Vector2(0.5f, 0.5f);
    public static string Gesture = "none";

    void Start()
    {
        client = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("UDP Receiver started on port " + port);
    }

    void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        Debug.Log("Receiver alive");
        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref anyIP);
                string json = Encoding.UTF8.GetString(data);
                Debug.Log("RAW DATA RECEIVED: " + json);
                ParseJson(json);
            }
            catch { }
        }
    }

    void ParseJson(string json)
    {
        json = json.Replace("{", "").Replace("}", "").Replace("\"", "").Trim();

        string[] parts = json.Split(',');

        float x = 0.5f, y = 0.5f;
        string gesture = "none";

        foreach (var part in parts)
        {
            string[] kv = part.Split(':');
            if (kv.Length != 2) continue;

            string key = kv[0].Trim();
            string val = kv[1].Trim();

            if (key == "x")
                float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out x);
            else if (key == "y")
                float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out y);
            else if (key == "gesture")
                gesture = val;
        }

        if (gesture != "none")
        {
            Position = new Vector2(x, y);   // update state if only hand in camera
        }
        Gesture = gesture;   // update gesture every frame for state control
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        if (client != null)
            client.Close();
    }
}