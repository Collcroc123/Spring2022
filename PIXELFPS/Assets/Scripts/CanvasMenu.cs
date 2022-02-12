using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class CanvasMenu : MonoBehaviour
{
    private bool isActive = false;
    public GameObject sideMenu;
    public GameObject quitButton, disconnectButton, playerUI;


    private void Start()
    {
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

    public void OnConnection()
    {
        isActive = false;
        sideMenu.SetActive(isActive);
        quitButton.SetActive(false);
        disconnectButton.SetActive(true);
        playerUI.SetActive(true);
    }
}