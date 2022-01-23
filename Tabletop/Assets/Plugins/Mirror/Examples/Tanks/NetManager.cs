using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetManager : NetworkManager
{
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
}
