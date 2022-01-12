using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    private bool isServer, isClient, isHost, isRunning;
    public string roomName;
    public PlayerData player;
    
    void Start()
    {
        
    }

    void Update()
    {
        //Debug.Log("Players in game: " + PlayerManager.Instance.PlayersInGame);
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("STARTING SERVER");
        }
        else
        {
            Debug.Log("---SERVER START FAILED");
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("STARTING CLIENT");
        }
        else
        {
            Debug.Log("---CLIENT START FIALED");
        }
    }

    public void OnConnectedToMaster()
    {
        
    }

    public void OnJoinedLobby()
    {
        SceneManager.LoadScene("ClientMenu");
    }
    

    public void CreatePlayer()
    {
        
    }

    public void DeletePlayer()
    {
        
    }

    public void CreateRoom()
    {
        //PhotonNetwork.CreateRoom(roomName);
        // Create room using typed in name
    }

    public void JoinRoom()
    {
        //PhotonNetwork.JoinRoom(roomName);
        // Get name of room you click on
    }

    public void OnJoinedRoom()
    {
        //PhotonNetwork.LoadLevel("Game");
        // Un-hide lobby UI, fill in info
    }
}