using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{ // https://www.youtube.com/watch?v=QlbBC07dqnE
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private const string HostAddressKey = "HostAddress";
    private NetManager netManager;
    public Button steamHost;
    public static CSteamID LobbyId { get; private set; }

    void Start()
    {
        netManager = GetComponent<NetManager>();
        if (SteamManager.Initialized)
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            steamHost.interactable = true;
        }
        else steamHost.interactable = false;
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, netManager.maxConnections);
    }

    public void JoinLobby()
    {
        //SteamMatchmaking.JoinLobby();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            netManager.Error("COULD NOT CREATE LOBBY");
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        netManager.StartHost();
        SteamMatchmaking.SetLobbyData(LobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (!NetworkServer.active)
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            netManager.networkAddress = hostAddress;
            netManager.StartClient();
        }
    }
}
