using UnityEngine;
using System.Collections;
 
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
 
public class UDPReceiver : MonoBehaviour
{
    //Test
    bool isUseFacialTrack = false;

    [SerializeField]
    int sendPort;

    [SerializeField]
    int listenPort;

    Thread receiveThread;
    UdpClient client;
 
    string lastReceivedUDPPacket = "";
    string allReceivedUDPPackets = "";
   
    // Start from shell
    private static void Main()
    {
       UDPReceiver receiveObj = new UDPReceiver();
       receiveObj.init();
 
        string text = "";

        do
        {
             text = Console.ReadLine();
        }
        while(!text.Equals("exit"));
    }

    public void Start()
    {
        init();
    }

    //Test
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            try
            {
                isUseFacialTrack = !isUseFacialTrack;
                string message = (isUseFacialTrack) ? "U" : "D";
                byte[] sendByte = Encoding.ASCII.GetBytes(message);
                client.Send(sendByte, sendByte.Length, "127.0.0.1", sendPort);
            }
            catch (SocketException)
            {
                Debug.Log("Send error...");
            }
        }
    }

  
    // OnGUI
    void OnGUI()
    {
        Rect rectObj=new Rect(40,10,200,400);
            GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj,"# UDPReceive\n127.0.0.1 "+listenPort+" #\n"
                    + "shell> nc -u 127.0.0.1 : "+listenPort+" \n"
                    + "\nLast Packet: \n"+ lastReceivedUDPPacket
                    + "\n\nAll Messages: \n"+allReceivedUDPPackets
                ,style);
    }
       
    // init
    private void init()
    {
        print("UDPSend.init()");
        print("Sending to 127.0.0.1 : " + sendPort);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + sendPort + "");
 
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));

        receiveThread.IsBackground = true;
        receiveThread.Start();
    }
 
    // receive thread
    private  void ReceiveData()
    {
        client = new UdpClient(listenPort);

        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                print(">> " + text);
               
                lastReceivedUDPPacket=text;
                allReceivedUDPPackets = allReceivedUDPPackets + text;
               
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
   
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
}
