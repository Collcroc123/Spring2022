using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Server : MonoBehaviour
{   // https://youtu.be/LVve99EuECM
    private const string ServerIP = "127.0.0.1";  // The IP of the server
    private const int MaxUsers = 8;               // Max number of users on server
    private const int ServerPort = 7678;          // Port the PC/Mobile players connects to
    private const int WebPort = 7679;             // Port the WebGL players connect to
    private const int BufferSize = 1024;          // Amount of information sent by clients in bytes, bigger = slower
    private int reliableChannelID;                // Important messages, will not be lost, slower unlike Unreliable
    private int hostID;                           // ID for PC/Mobile host
    private int webHostID;                        // ID for WebGL host
    private bool isStarted;                       // Is server running
    private byte error;                           // Stores errors

    public GameObject playerText;
    private GameObject[] players;

    private void Start()
    {
        players = new GameObject[MaxUsers];
        //StartServer();
    }
    
    public void StartServer()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannelID = cc.AddChannel(QosType.Reliable);
        HostTopology topo = new HostTopology(cc, MaxUsers);
        
        // Server Only
        hostID = NetworkTransport.AddHost(topo, ServerPort, null);
        webHostID = NetworkTransport.AddWebsocketHost(topo, WebPort);
        Debug.Log("Starting Server on Port " + ServerPort + ", WebPort " + WebPort);
        isStarted = true;
    }
    
    public void Shutdown()
    {
        isStarted = false;
        Debug.Log("Server Shutting Down");
        NetworkTransport.Shutdown();
    }

    private void Update()
    {
        if (!isStarted) 
            return;
        
        byte[] recBuffer = new byte[BufferSize]; // Buffer that transports data
        int recHostID;     // Web or Standalone
        int connectionID;  // What user is message  from
        int channelID;     // What channel is message from
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
                Debug.Log("User " + connectionID + " Has Connected Through Host " + recHostID);
                GameObject temp = Instantiate(playerText, new Vector2(-375, 200 - connectionID * 50), Quaternion.identity);
                temp.transform.SetParent(gameObject.transform, false);
                temp.transform.Find("PlayerNum").GetComponent<TextMeshProUGUI>().text = connectionID.ToString();
                players[connectionID] = temp;
                break;
            }
            case NetworkEventType.DisconnectEvent:
            {
                Debug.Log("User " + connectionID + " Has Disconnected Through Host " + recHostID);
                Destroy(players[connectionID]);
                break;
            }
            case NetworkEventType.DataEvent:
            {
                Debug.Log("User " + connectionID + " Has Sent Data Through Host " + recHostID + ", Message: " + System.Text.Encoding.UTF8.GetString(recBuffer) + ", Size: " + dataSize);
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
