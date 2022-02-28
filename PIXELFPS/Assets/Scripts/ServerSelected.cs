using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelected : MonoBehaviour
{
    [HideInInspector] public ServerData server;
    private NetworkActions netAct;
    private Button joinButton;
    public TextMeshProUGUI name, players;

    public void LobbySelected()
    {
        joinButton = GameObject.Find("Join Lobby Button").GetComponent<Button>();
        joinButton.interactable = true;
        netAct.currentSelectedServer = server.info;
        //then tell joinButton what the server IP is so when it's clicked it can connect
    }

    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
        name.text = server.info.EndPoint.Address.ToString();//REPLACE WITH SERVER.NAME
        players.text = "0" + "/" + server.maxPlayers;
        netAct = GameObject.Find("NetManager").GetComponent<NetworkActions>();
    }
}
