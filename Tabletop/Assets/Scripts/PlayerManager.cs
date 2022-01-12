using DilmerGames.Core.Singletons;
using Unity.Collections;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour //Singleton<PlayerManager>
{
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    public bool IsServer = true;

    
    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    private void Start()
    {
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

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerName.Value = "Player";
        }
    }
}