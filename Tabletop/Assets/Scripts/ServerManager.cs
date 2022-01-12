using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : NetworkBehaviour
{
    //[SerializeField]
    //private bool isServer, isClient, isHost, isRunning;
    //public string roomName;
    //public PlayerData player;
    public TextMeshProUGUI playerCount;
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    
    void Start()
    {
        //Cursor.visible = true;
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                //Debug.Log(id + " Just Connected!");
                Logger.Instance.LogInfo(id + " Just Connected!");
                playersInGame.Value++;
            }
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                //Debug.Log(id + " Just Disconnected!");
                Logger.Instance.LogInfo(id + " Just Disconnected!");
                playersInGame.Value--;
            }
        };
    }

    void Update()
    {
        //Debug.Log("Players in game: " + PlayerManager.Instance.PlayersInGame);
        //playerCount.text = "Players in game: " + PlayerManager.Instance.PlayersInGame;
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            //Debug.Log("STARTING SERVER");
            Logger.Instance.LogInfo("STARTING SERVER");
        }
        else
        {
            //Debug.Log("---SERVER START FAILED");
            Logger.Instance.LogInfo("---SERVER START FAILED");
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            //Debug.Log("STARTING CLIENT");
            Logger.Instance.LogInfo("STARTING CLIENT");
        }
        else
        {
            //Debug.Log("---CLIENT START FAILED");
            Logger.Instance.LogInfo("---CLIENT START FAILED");
        }
    }

    public void OnConnectedToMaster()
    {
        
    }

    public void OnJoinedLobby()
    {
        //SceneManager.LoadScene("ClientMenu");
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