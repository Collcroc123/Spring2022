using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Server : MonoBehaviour
{   // https://youtu.be/LVve99EuECM
    private const string ServerIP = "127.0.0.1";  // The IP of the server
    private const int MaxUsers = 8;               // Max number of users on server
    private const int ServerPort = 7678;          // Port the Standalone players connects to
    private const int WebPort = 7679;             // Port the WebGL players connect to
    private const int BufferSize = 1024;          // Amount of information sent by clients in bytes, bigger = slower
    private int reliableChannel;                  // Important messages, will not be lost, slower unlike Unreliable
    private int hostID;                           // ID for PC/Mobile host
    private int webHostID;                        // ID for WebGL host
    private bool isStarted;                       // Is server running
    private byte error;                           // Stores errors? (Never Used?)

    public GameObject playerText;                 // Player Icon & Name
    private GameObject[] players;                 // Array of Players in Server
    public Settings settings;

    private void Start()
    {
        players = new GameObject[MaxUsers]; // Stores Players in Server
    }
    
    public void StartServer()
    { // Creates New Server (unless one is running)
        NetworkTransport.Init();                                               // Initializes Server
        ConnectionConfig cc = new ConnectionConfig();                          // Creates Connection Config (settings)
        reliableChannel = cc.AddChannel(QosType.Reliable);                     // Creates a Reliable Channel (guaranteed to receive)
        HostTopology topo = new HostTopology(cc, MaxUsers);  // Creates Host Topology (host info)
        hostID = NetworkTransport.AddHost(topo, ServerPort, null);          // Creates Host for Standalone
        webHostID = NetworkTransport.AddWebsocketHost(topo, WebPort);          // Creates Host for WebGL
        isStarted = true;                                                      // Server is Started
        Debug.Log("ServerPort " + ServerPort + ", WebPort" + WebPort);
    }
    
    public void Shutdown()
    { // Shuts Down Current Server (if one is running)
        NetworkTransport.Shutdown(); // Shuts Down Server
        isStarted = false;           // Server is No Longer Started
        Debug.Log("Server Shutting Down");
    }

    private void Update()
    {
        if (!isStarted) 
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
            { // Triggers when a client connects to the server
                Debug.Log("User " + connectionID + " Has Connected Through Host " + recHostID);
                //CreatePlayer(connectionID); // Creates an Icon and Name for Player
                break;
            }
            case NetworkEventType.DisconnectEvent:
            { // Triggers when a client disconnects from the server
                Debug.Log("User " + connectionID + " Has Disconnected Through Host " + recHostID);
                Destroy(players[connectionID]); // Destroys Player's Icon and Name
                break;
            }
            case NetworkEventType.DataEvent:
            { // Triggers when a client sends data to the server
                Debug.Log("User " + connectionID + " Has Sent Data");
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(buffer);
                NetMessage message = (NetMessage)formatter.Deserialize(ms);
                OnData(connectionID, channelID, recHostID, message);
                //PlayerData player = (PlayerData)formatter.Deserialize(ms);
                //CreatePlayer(connectionID, player);
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

    private void CreatePlayer(int connectionID, PlayerData player)
    {
        GameObject temp = Instantiate(playerText, new Vector2(-375, 200 - connectionID * 50), Quaternion.identity);
        temp.transform.SetParent(gameObject.transform, false);
        temp.transform.Find("PlayerNum").GetComponent<TextMeshProUGUI>().text = connectionID.ToString();
        temp.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = player.userName;
        temp.transform.Find("PlayerIcon").GetComponent<RawImage>().texture = settings.icons[player.playerIcon];
        players[connectionID] = temp;
    }
    
    private void OnData(int connectionID, int channelID, int recHostID, NetMessage message)
    {
        switch (message.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            
            case NetOP.CreatePlayer:
                CreateAccount((Net_CreatePlayer) message);
                break;
        }
    }
    
    public void SendClient(int recHostID, int connectionID, NetMessage message)
    {
        // Hold Data Here
        byte[] buffer = new byte[BufferSize];
        
        // Crush Data to Byte Array
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, message);

        if (recHostID == 0)
            NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, BufferSize, out error);
        else
            NetworkTransport.Send(webHostID, connectionID, reliableChannel, buffer, BufferSize, out error);
    }
    
    private void CreateAccount(Net_CreatePlayer account)
    {
        Debug.Log("Create Account: \nUsername: " + account.Username + " \nPassword: " + account.Password + " \nEmail: " + account.Email + " \nHave a Great Day!");
    }
}
