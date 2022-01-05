using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Client : MonoBehaviour
{   // https://youtu.be/LVve99EuECM
    private const string ServerIP = "127.0.0.1";  // The IP of the server
    private const int MaxUsers = 8;               // Max number of users on server
    private const int ServerPort = 7678;          // Port the PC/Mobile players connects to
    private const int WebPort = 7679;             // Port the WebGL players connect to
    private const int BufferSize = 1024;          // Amount of information sent by clients in bytes, bigger = slower
    private int reliableChannelID;                // Important messages, will not be lost, slower unlike Unreliable
    private int hostID;                           // ID for PC/Mobile host
    private int connectionID;                     // ID for user connection
    private bool isConnected;                     // Is client connected
    private byte error;                           // Stores errors

    private void Start()
    {
        //Connect();
    }
    
    public void Connect()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannelID = cc.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(cc, MaxUsers);
        
        // Client Only
        hostID = NetworkTransport.AddHost(topo, 0);
#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL Client
        connectionID = NetworkTransport.Connect(hostID, ServerIP, WebPort, 0, out error);
#else
        // Standalone Client
        NetworkTransport.Connect(hostID, ServerIP, ServerPort, 0, out error);
#endif
        Debug.Log("Attempting Connection To " + ServerIP);
        isConnected = true;
    }
    
    private void Update()
    {
        if (!isConnected) 
            return;
        
        byte[] recBuffer = new byte[BufferSize]; // Buffer that transports data
        int recHostID;     // Web or standalone
        int connectionID;  // What device message is from
        int channelID;     // What channel message is from
        int dataSize;      // Message size
        
        NetworkEventType type = NetworkTransport.Receive(out recHostID, out connectionID, out channelID, recBuffer,recBuffer.Length, out dataSize, out error);

        switch (type)
        {
            case NetworkEventType.Nothing:
            {
                break;
            }
            case NetworkEventType.ConnectEvent:
            {
                Debug.Log("You Have Connected To The Server");
                break;
            }
            case NetworkEventType.DisconnectEvent:
            {
                Debug.Log("You Have Disconnected From The Server");
                break;
            }
            case NetworkEventType.DataEvent:
            {
                Debug.Log("User " + connectionID + " Has Sent Data, Message: " + System.Text.Encoding.UTF8.GetString(recBuffer) + ", Size: " + dataSize);
                break;
            }
            default:
            case NetworkEventType.BroadcastEvent:
            {
                Debug.Log("Unknown Network Message Type Received: " + type);
                break;
            }
                
        }
    }
}
