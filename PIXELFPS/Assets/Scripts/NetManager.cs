using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class NetManager : NetworkManager
{
    public GameObject errorWindow;
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    { // Gets and Sets new player's Steam ID
        base.OnServerAddPlayer(conn);
        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.LobbyId, numPlayers - 1);
        var playerManager = conn.identity.GetComponent<PlayerManager>();
        playerManager.SetSteamId(steamId.m_SteamID);
    }

    public void Error(string text)
    {
        errorWindow.SetActive(true);
        errorWindow.GetComponentInChildren<TextMeshPro>().text = "ERROR: \n" + text;
    }
}
