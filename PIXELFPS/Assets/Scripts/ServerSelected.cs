using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelected : NetworkBehaviour
{
    [SyncVar] public ServerData server; //[HideInInspector] 
    private NetworkActions netAct;
    private Button joinButton;
    private TextMeshProUGUI nameTxt, playersTxt, pingTxt;

    public void LobbySelected()
    {
        joinButton = GameObject.Find("Join Lobby Button").GetComponent<Button>();
        joinButton.interactable = true;
        netAct.currentSelectedServer = server;
    }

    void Start()
    {
        nameTxt = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        playersTxt = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        pingTxt = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        transform.localScale = new Vector3(1, 1, 1);
        if (server.serverName != null) nameTxt.text = server.serverName;
        else nameTxt.text = server.info.EndPoint.Address.ToString();
        playersTxt.text = "0" + "/" + server.maxPlayers;
        //ping.text = "Ping: ";
        netAct = GameObject.Find("NetManager").GetComponent<NetworkActions>();
    }
}
