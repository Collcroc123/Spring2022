using UnityEngine;
using Cursor = UnityEngine.Cursor;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isActive = false;
    public GameObject sideMenu;
    public GameObject playerUI;
    public GameObject disconnectButton;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelWasLoaded;
    }
    
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
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

    void OnLevelWasLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            isActive = true;
            sideMenu.SetActive(isActive);
            playerUI.SetActive(false);
            disconnectButton.SetActive(false);
        }
        else
        {
            isActive = false;
            sideMenu.SetActive(isActive);
            playerUI.SetActive(true);
            disconnectButton.SetActive(true);
        }
    }
}