//using Photon.Pun;
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
        //Debug.Log("Players in game: " + PlayersManager.Instance.PlayersInGame);
    }

    public void StartServer()
    {
        
    }

    public void StartClient() // yep it true tie up quip trip tip pip rip yip quote prop rep toy rot
    {
        //PhotonNetwork.ConnectUsingSettings();
    }

    public void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinLobby();
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