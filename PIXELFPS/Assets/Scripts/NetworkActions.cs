using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;
using Steamworks;
using Mirror;
using TMPro;

public class NetworkActions : NetworkManager
{
    public GameAction OnPlayerAdded;
    public GameAction OnShutdown;
    public GameObject errorWindow;
    
    public GameObject scrollView;
    public GameObject serverPrefab;
    private NetworkDiscovery netDisc;
    private List<GameObject> serverList = new List<GameObject>();
    private int currentServer;
    public ServerResponse currentSelectedServer;
    //readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    
/*#if UNITY_EDITOR
    void OnValidate()
    {
        if (netDisc == null)
        {
            netDisc = GetComponent<NetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(netDisc.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, netDisc }, "Set NetworkDiscovery");
        }
    }
#endif*/

    void Start()
    {
        netDisc = GetComponent<NetworkDiscovery>();
    }

    public void FindServers()
    {
        Debug.Log("FINDING LOCAL LOBBIES");
        ClearServerList();
        netDisc.StartDiscovery();
    }
    
    public void HostServer()
    {
        Debug.Log("HOSTING LOCAL LOBBY");
        ClearServerList();
        StartHost();
        netDisc.AdvertiseServer();
    }
    
    public void JoinServer()
    {
        Debug.Log("JOINING LOCAL LOBBY");
        netDisc.StopDiscovery();
        StartClient(currentSelectedServer.uri);
    }
    
    public void OnDiscoveredServer(ServerResponse info)
    { // Note that you can check the versioning to decide if you can connect to the server or not using this method
        //discoveredServers[info.serverId] = info;
        Debug.Log("SERVER FOUND: " + info.EndPoint.Address);
        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
        {
            GameObject serverEntry = Instantiate(serverPrefab, scrollView.transform, true);
            ServerSelected servSel = serverEntry.GetComponent<ServerSelected>();
            servSel.server = ScriptableObject.CreateInstance<ServerData>();
            servSel.server.info = info;
            serverList.Add(serverEntry);
        }
    }
    

    private void ClearServerList()
    {
        Debug.Log("CLEARED SERVER LIST");
        //discoveredServers.Clear();
        foreach (GameObject svr in serverList) Destroy(svr);
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
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    { // Gets and Sets new player's Steam ID
        base.OnServerAddPlayer(conn);
        OnPlayerAdded.RaiseAction();
        if (SteamManager.Initialized)
        {
            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamActions.LobbyId, numPlayers - 1);
            var playerManager = conn.identity.GetComponent<PlayerManager>();
            playerManager.SetSteamId(steamId.m_SteamID);
        }
    }
    
    public void Error(string text)
    {
        errorWindow.SetActive(true);
        errorWindow.GetComponentInChildren<TextMeshProUGUI>().text = "ERROR: \n" + text;
    }
}