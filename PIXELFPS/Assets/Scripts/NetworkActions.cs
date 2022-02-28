using System.Collections;
using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;
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
    private List<ServerResponse> currentServers = new List<ServerResponse>();
    public ServerResponse currentSelectedServer;

    void Start()
    {
        netDisc = GetComponent<NetworkDiscovery>();
    }

    public void FindServers()
    {
        Debug.Log("FINDING LOBBIES");
        ClearServerList();
        netDisc.StartDiscovery();
    }
    
    public void HostServer()
    {
        Debug.Log("HOSTING LOBBY");
        ClearServerList();
        StartHost();
        netDisc.AdvertiseServer();
    }
    
    public void JoinServer()
    {
        Debug.Log("JOINING LOBBY");
        netDisc.StopDiscovery();
        StartClient(currentSelectedServer.uri);
    }
    
    public void OnDiscoveredServer(ServerResponse info)
    { // Note that you can check the versioning to decide if you can connect to the server or not using this method
        //discoveredServers[info.serverId] = info;
        Debug.Log("SERVER FOUND: " + info.EndPoint.Address);
        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
        {
            if (!currentServers.Contains(info))
            { // DOES NOT WORK WITH MULTIPLE SERVERS ON ONE IP
                GameObject serverEntry = Instantiate(serverPrefab, scrollView.transform, true);
                ServerSelected servSel = serverEntry.GetComponent<ServerSelected>();
                servSel.server = ScriptableObject.CreateInstance<ServerData>();
                servSel.server.info = info;
                serverList.Add(serverEntry);
                currentServers.Add(info);
            }
        }
    }

    private void ClearServerList()
    {
        Debug.Log("CLEARED SERVER LIST");
        foreach (GameObject svr in serverList) Destroy(svr);
        currentServers.Clear();
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
    {
        base.OnServerAddPlayer(conn);
        OnPlayerAdded.RaiseAction();
    }

    public void RespawnPlayer(GameObject player, int time)
    {
        StartCoroutine(WaitRespawn(player, time));
    }

    private IEnumerator WaitRespawn(GameObject player, int time)
    {
        Debug.Log("DEAD");
        player.SetActive(false);
        yield return new WaitForSeconds(time);
        player.SetActive(true);
        Debug.Log("ALIVE");
    }
    
    public void Error(string text)
    {
        errorWindow.SetActive(true);
        errorWindow.GetComponentInChildren<TextMeshProUGUI>().text = "ERROR: \n" + text;
    }
}