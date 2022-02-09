using Mirror;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class CanvasMenu : MonoBehaviour
{
    public GameObject networkManagerObj;
    //private NetManager netManager;
    private NetworkManager netManager;
    //private SteamLobby steamLobby;
    private bool isActive = false;
    public GameObject sideMenu;


    public GameObject quitButton, disconnectButton, playerUI;


    private void Start()
    {
        netManager = networkManagerObj.GetComponent<NetManager>();
        //steamLobby = networkManagerObj.GetComponent<SteamLobby>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { // Show or Hide Menu When Escape is Pressed
            isActive = !isActive;
            sideMenu.SetActive(isActive);
            if (isActive) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LocalHost()
    {
        netManager.StartHost();
        OnConnection();
    }

    public void LocalConnect()
    {
        netManager.StartClient();
        OnConnection();
    }
    
    public void SteamHost()
    {
        //steamLobby.HostLobby();
        OnConnection();
    }

    public void SteamConnect()
    {
        //steamLobby.JoinLobby();
        OnConnection();
    }

    public void OnConnection()
    {
        isActive = false;
        sideMenu.SetActive(isActive);
        quitButton.SetActive(false);
        disconnectButton.SetActive(true);
        playerUI.SetActive(true);
    }
}