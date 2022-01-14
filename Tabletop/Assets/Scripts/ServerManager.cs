using UnityEngine;
using Mirror;

public class ServerManager : NetworkManager
{
    //public string roomName;
    //public PlayerData player;
    //public TextMeshProUGUI playerCount;
    //private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    //private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();

    public override void OnStartServer()
    {
        Debug.Log("STARTING SERVER");
    }

    public override void OnStopServer()
    {
        Debug.Log("STOPPING SERVER");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("CONNECTED TO SERVER");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("DISCONNECTED FROM SERVER");
    }

    public void CreatePlayer()
    {
        
    }

    public void DeletePlayer()
    {
        
    }

    public void CreateRoom()
    {
        // Create room using typed in name
    }

    public void JoinRoom()
    {
        // Get name of room you click on
    }

    public void OnJoinedRoom()
    {
        // Un-hide lobby UI, fill in info
    }
}