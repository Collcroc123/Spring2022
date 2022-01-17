using UnityEngine;
using Mirror;

public class ServerManager : NetworkManager
{
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
}