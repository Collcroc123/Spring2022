using UnityEngine;
using Cursor = UnityEngine.Cursor;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isActive = false;
    public GameObject sideMenu;
    public GameObject playerUI;
    
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu") sideMenu.SetActive(true);
        else playerUI.SetActive(true);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Menu") return;
        if (Input.GetKeyDown(KeyCode.Escape))
        { // Show or Hide Menu When Escape is Pressed
            isActive = !isActive;
            sideMenu.SetActive(isActive);
            if (isActive) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
    }
}