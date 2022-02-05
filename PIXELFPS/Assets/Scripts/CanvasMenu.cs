using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class CanvasMenu : MonoBehaviour
{
    public GameObject networkManagerObj;
    private NetManager netManager;
    private SteamLobby steamLobby;
    private bool isActive = false;
    public GameObject sideMenu, playerUI;
    
    private void Start()
    {
        netManager = networkManagerObj.GetComponent<NetManager>();
        steamLobby = networkManagerObj.GetComponent<SteamLobby>();
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
        PlayerStart();
    }

    public void SteamHost()
    {
        steamLobby.HostLobby();
        PlayerStart();
    }
    public void LocalConnect()
    {
        netManager.StartClient();
        PlayerStart();
    }

    public void SteamConnect()
    {
        steamLobby.JoinLobby();
        PlayerStart();
    }

    public void ShutDown()
    {
        netManager.StopClient();
        netManager.StopHost();
        netManager.StopServer();
        playerUI.SetActive(false);
        Instantiate(Camera.main);
    }

    void PlayerStart()
    {
        isActive = false;
        sideMenu.SetActive(isActive);
        playerUI.SetActive(true);
    }
}