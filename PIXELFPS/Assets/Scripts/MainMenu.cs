using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MainMenu : MonoBehaviour
{
    public GameObject networkManagerObj;
    private NetManager netManager;
    private SteamLobby steamLobby;
    private bool isActive = false;
    private VisualElement root;
    private Button singleButton, multiButton, settingsButton, quitButton, localButton, steamButton;
    private VisualElement sideMenu, singleMenu, multiMenu, settingsMenu, quitMenu;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        netManager = networkManagerObj.GetComponent<NetManager>();
        steamLobby = networkManagerObj.GetComponent<SteamLobby>();

        sideMenu = root.Q<VisualElement>("MenuButtons");
        singleButton = root.Q<Button>("Singleplayer");
        multiButton = root.Q<Button>("Multiplayer");
        settingsButton = root.Q<Button>("Settings");
        quitButton = root.Q<Button>("Quit");

        singleMenu = root.Q<VisualElement>("SingleMenu");
        multiMenu = root.Q<VisualElement>("MultiMenu");
        settingsMenu = root.Q<VisualElement>("SettingsMenu");

        localButton = root.Q<Button>("LocalButton");
        steamButton = root.Q<Button>("SteamButton");
        
        singleButton.clicked += SingleplayerMenu;
        multiButton.clicked += MultiplayerMenu;
        settingsButton.clicked += SettingsMenu;
        quitButton.clicked += Quit;
        localButton.clicked += LocalConnect;
        steamButton.clicked += SteamConnect;
        
        sideMenu.AddToClassList("SlideOut");
        FadeAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { // Show or Hide Menu When Escape is Pressed
            isActive = !isActive;
            if (isActive)
            {
                Cursor.lockState = CursorLockMode.None;
                sideMenu.RemoveFromClassList("SlideOut");
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                FadeAll();
                sideMenu.AddToClassList("SlideOut");
            }
        }
    }

    void SingleplayerMenu()
    {
        FadeAll();
        Debug.Log("SINGLEPLAYER MENU");
        singleMenu.RemoveFromClassList("FadeOut");
    }

    void MultiplayerMenu()
    {
        FadeAll();
        Debug.Log("MULTIPLAYER MENU");
        multiMenu.RemoveFromClassList("FadeOut");
        //steamLobby.HostLobby();
        //netManager.StartHost();
    }

    void SettingsMenu()
    {
        FadeAll();
        Debug.Log("SETTINGS MENU");
        settingsMenu.RemoveFromClassList("FadeOut");
    }

    void Quit()
    {
        Application.Quit();
    }

    void FadeAll()
    {
        singleMenu.AddToClassList("FadeOut");
        multiMenu.AddToClassList("FadeOut");
        settingsMenu.AddToClassList("FadeOut");
    }

    void LocalConnect()
    {
        netManager.StartHost();
        FadeAll();
        isActive = false;
    }

    void SteamConnect()
    {
        steamLobby.HostLobby();
        FadeAll();
        isActive = false;
    }
}