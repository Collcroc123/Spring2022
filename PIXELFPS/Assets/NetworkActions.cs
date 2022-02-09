using UnityEngine;
using Mirror;

public class NetworkActions : NetworkManager
{
    public GameAction OnShutdown;

    public void HostLobby()
    {
        StartHost();
    }

    public void JoinLobby()
    {
        StartClient();
    }
    
    public void ShutDown()
    {
        Debug.Log("SHUTTING DOWN CLIENTS AND SERVERS");
        StopClient();
        StopHost();
        StopServer();
        Instantiate(Camera.main);
        OnShutdown.RaiseAction();
    }
}