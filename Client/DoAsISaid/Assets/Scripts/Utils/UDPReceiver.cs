using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    public static UDPReceiver Instance = null;

    [SerializeField]
    int sendPort;

    [SerializeField]
    int listenPort;

    Thread receiveThread;
    UdpClient client;

    public string LastReceivedPacket => lastReceivedUDPPacket;
    string lastReceivedUDPPacket = "";

    //string allReceivedUDPPackets = "";
   
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

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Start()
    {
        init();
    }


#if UNITY_EDITOR
    bool isUseFacialTrack = false;

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
#endif
  
    void OnGUI()
    {
        Rect rectObj=new Rect(40,10,200,400);
        GUIStyle style = new GUIStyle();

        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# Listen to 127.0.0.1:" + listenPort + " #\n"
                + "\nLast Packet: \n" + lastReceivedUDPPacket + "\n"
        , style); ;
    }
       
    // init
    private void init()
    {
        //print("Sending to 127.0.0.1 : " + sendPort);
        print("Listen to 127.0.0.1 : " + listenPort);
 
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

                //print(">> " + text);
               
                lastReceivedUDPPacket = text;
                //allReceivedUDPPackets = allReceivedUDPPackets + text;
               
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
   
    public string getLatestUDPPacket()
    {
        //allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
}
