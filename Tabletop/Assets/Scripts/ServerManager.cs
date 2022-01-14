using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : NetworkBehaviour
{
    //public string roomName;
    //public PlayerData player;
    //public TextMeshProUGUI playerCount;
    //private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    //private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    
    void Start()
    {
        /*Cursor.visible = true;
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                Debug.Log(id + " Just Connected!");
                playersInGame.Value++;
            }
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                Debug.Log(id + " Just Disconnected!");
                playersInGame.Value--;
            }
        };*/
    }

    void Update()
    {
        //if (NetworkManager.Singleton.OnClientConnectedCallback += false)
        //{
            
        //}
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("STARTING SERVER");
        }
        else
        {
            Debug.Log("--SERVER START FAILED");
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("STARTING CLIENT");
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        }
        else
        {
            Debug.Log("--CLIENT START FAILED");
        }
    }

    public void ShutDown()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            NetworkManager.Singleton.DisconnectClient(client);
            Debug.Log("Disconnecting Client " + client);
        }
        Debug.Log("=SERVER SHUT DOWN=");
        NetworkManager.Singleton.Shutdown();
    }

    public void OnClientConnect()
    {
        
    }

    public void OnClientDisconnect()
    {
        
    }

    public void OnJoinedLobby()
    {
        //SceneManager.LoadScene("ClientMenu");
    }
    

    public void CreatePlayer()
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
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