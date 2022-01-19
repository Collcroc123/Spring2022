using UnityEngine;
using Mirror;

public class ServerManager : NetworkManager
{
    public Connector connector;

    public override void OnStartServer()
    {
        connector.ServerStarted();
    }

    public override void OnStopServer()
    {
        connector.ServerStopped();
    }

    public override void OnClientConnect(NetworkConnection client)
    {
        connector.ClientConnected(client);
    }

    public override void OnClientDisconnect(NetworkConnection client)
    {
        connector.ClientDisconnected(client);
    }
}