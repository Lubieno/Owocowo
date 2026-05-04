using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPNetworkManager : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;

    [Header("Ustawienia Sieci")]
    public string serverIP = "127.0.0.1";
    public int port = 12345;

    void Start()
    {
        udpClient = new UdpClient();
        // W prawdziwej grze tutaj ³¹czymy siê z IP serwera

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                // Tutaj bêdziemy przetwarzaæ dane (np. pozycje innych graczy)
                Debug.Log("Odebrano: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    public void SendData(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, serverIP, port);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        udpClient.Close();
    }
}
