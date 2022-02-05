using UnityEngine;
using UnityEngine.UI;

public class ServerSelected : MonoBehaviour
{
    private Button joinButton;

    public void LobbySelected()
    {
        joinButton = GameObject.Find("Lobby Button").GetComponent<Button>();
        joinButton.interactable = true;
        //then tell joinButton what the server IP is so when it's clicked it can connect
    }
}
