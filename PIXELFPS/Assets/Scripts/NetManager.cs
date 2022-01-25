using UnityEngine;
using Mirror;
using TMPro;

public class NetManager : NetworkManager
{
    public GameObject errorWindow;
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;
    
    public override void OnStartClient()
    {
        //CmdSendPlayerInfo()
    }

    public void QuitGame()
    {
        Debug.Log("QUITTING GAME");
        Application.Quit();
    }

    public void Error(string text)
    {
        errorWindow.SetActive(true);
        errorWindow.GetComponent<TextMeshPro>().text = "ERROR: \n" + text;
    }
}
