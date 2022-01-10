using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;



public class Client : MonoBehaviour
{   // https://youtu.be/LVve99EuECM
    private const string ServerIP = "127.0.0.1";  // The IP of the server
    private const int MaxUsers = 8;               // Max number of users on server
    private const int ServerPort = 7678;          // Port the PC/Mobile players connects to
    private const int WebPort = 7679;             // Port the WebGL players connect to
    private const int BufferSize = 1024;          // Amount of information sent by clients in bytes, bigger = slower
    private int reliableChannel;                // Important messages, will not be lost, slower unlike Unreliable
    private int hostID;                           // ID for PC/Mobile host
    private int connectionID;                     // ID for user connection
    private bool isConnected;                     // Is client connected
    private byte error;                           // Stores errors

    public PlayerData player;

    private void Start()
    {
        //Connect();
    }
    
    public void Connect()
    { // Connects to Server (if running)
        NetworkTransport.Init();                                               // Starts Connection
        ConnectionConfig cc = new ConnectionConfig();                          // Creates Connection Config (settings)
        reliableChannel = cc.AddChannel(QosType.Reliable);                     // Creates a Reliable Channel (guaranteed to receive)
        HostTopology topo = new HostTopology(cc, MaxUsers);  // Creates Host Topology (host info)
        hostID = NetworkTransport.AddHost(topo, 0);                       // NA
#if UNITY_WEBGL && !UNITY_EDITOR                                               // Connects to Server Depending on Platform
        // WebGL Client
        connectionID = NetworkTransport.Connect(hostID, ServerIP, WebPort, 0, out error);
#else
        // Standalone Client
        connectionID = NetworkTransport.Connect(hostID, ServerIP, ServerPort, 0, out error);
#endif
        Debug.Log("Attempting Connection To " + ServerIP);
        isConnected = true; // Connected to Server
    }

    public void Shutdown()
    { // Disconnects Client (if connected)
        NetworkTransport.Shutdown(); // Shuts Down Server
        isConnected = false;         // Client is No Longer Connected
        Debug.Log("Client Disconnecting");
    }

    private void Update()
    {
        if (!isConnected) 
            return; // Stops Everything Below if Server is Not Started
        
        int recHostID;                         // Web or Standalone
        int connectionID;                      // What User is Message From
        int channelID;                         // What Channel is Message From
        int dataSize;                          // Message Size
        byte[] buffer = new byte[BufferSize];  // Buffer That Transports Data
        
        // Call the Network, Return an Answer
        NetworkEventType type = NetworkTransport.Receive(out recHostID, out connectionID, out channelID, buffer,buffer.Length, out dataSize, out error);

        switch (type)
        { // Response to Above Answer
            case NetworkEventType.Nothing:
            { // Triggers when nothing is happening
                break;
            }
            case NetworkEventType.ConnectEvent:
            { // Triggers when this client connects to a server
                Debug.Log("You Have Connected To The Server");
                break;
            }
            case NetworkEventType.DisconnectEvent:
            { // Triggers when this client disconnects from a server
                Debug.Log("You Have Disconnected From The Server");
                break;
            }
            case NetworkEventType.DataEvent:
            { // Triggers when this client sends data to a server
                Debug.Log("You (User " + connectionID + ") Have Sent Data");
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(buffer);
                NetMessage message = (NetMessage)formatter.Deserialize(ms);
                OnData(connectionID, channelID, recHostID, message);
                break;
            }
            default:
            case NetworkEventType.BroadcastEvent:
            { // Unknown
                Debug.Log("Unknown Network Message Type Received: " + type);
                break;
            }
        }
    }
    
    private void OnData(int connectionID, int channelID, int recHostID, NetMessage message)
    {
        switch (message.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
        }
    }
    
    public void SendServer(NetMessage message)
    {
        // Hold Data Here
        byte[] buffer = new byte[BufferSize];
        
        // Crush Data to Byte Array
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, message);

        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, BufferSize, out error);
    }

    public void TESTCREATEACCOUNT()
    {
        Net_CreatePlayer account = new Net_CreatePlayer();
        account.Username = "Gamer";
        account.Password = "12345";
        account.Email = "Gamer@Gmail.com";
        SendServer(account);
    }
    
    /*public void SendServer(PlayerData player)
    {
        // Hold Data Here
        byte[] buffer = new byte[BufferSize];
        
        // Crush Data to Byte Array
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, player); // CANNOT SERIALIZE SCRIPTABLEOBJECTS!

        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, BufferSize, out error);
    }

    public void TESTCREATEACCOUNT()
    {
        SendServer(player);
    }*/
}