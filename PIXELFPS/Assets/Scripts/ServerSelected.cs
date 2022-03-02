using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelected : MonoBehaviour
{
    [HideInInspector] public ServerData server;
    private NetworkActions netAct;
    private Button joinButton;
    private TextMeshProUGUI name, players, ping;

    public void LobbySelected()
    {
        joinButton = GameObject.Find("Join Lobby Button").GetComponent<Button>();
        joinButton.interactable = true;
        netAct.currentSelectedServer = server.info;
    }

    void Start()
    {
        name = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        players = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        ping = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        transform.localScale = new Vector3(1, 1, 1);
        name.text = server.info.EndPoint.Address.ToString(); //REPLACE WITH SERVER.NAME
        players.text = "0" + "/" + server.maxPlayers;
        //ping.text = "Ping: ";
        netAct = GameObject.Find("NetManager").GetComponent<NetworkActions>();
    }
}
