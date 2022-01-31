using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public GameObject networkManagerObj;
    private NetManager netManager;
    private SteamLobby steamLobby;
    private bool isActive = false;
    private VisualElement root;
    private Button singleButton, multiButton, settingsButton, quitButton;
    private VisualElement singleMenu, multiMenu, settingsMenu, quitMenu;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        netManager = networkManagerObj.GetComponent<NetManager>();
        steamLobby = networkManagerObj.GetComponent<SteamLobby>();
        
        singleButton = root.Q<Button>("Singleplayer");
        multiButton = root.Q<Button>("Multiplayer");
        settingsButton = root.Q<Button>("Settings");
        quitButton = root.Q<Button>("Quit");

        singleMenu = root.Q<VisualElement>("SingleMenu");
        multiMenu = root.Q<VisualElement>("MultiMenu");
        settingsMenu = root.Q<VisualElement>("SettingsMenu");
        quitMenu = root.Q<VisualElement>("QuitMenu");

        singleButton.clicked += SingleplayerMenu;
        multiButton.clicked += MultiplayerMenu;
        settingsButton.clicked += SettingsMenu;
        quitButton.clicked += QuitMenu;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { // Show or Hide Menu When Escape is Pressed
            isActive = !isActive;
            if (isActive)
            {
                root.style.display = DisplayStyle.Flex;
            }
            else
            {
                root.style.display = DisplayStyle.None;
            }
        }
    }

    void SingleplayerMenu()
    {
        Debug.Log("SINGLEPLAYER MENU");
        singleMenu.style.display = DisplayStyle.Flex;
    }

    void MultiplayerMenu()
    {
        Debug.Log("MULTIPLAYER MENU");
        multiMenu.style.display = DisplayStyle.Flex;
        //steamLobby.HostLobby();
        //netManager.StartHost();
    }

    void SettingsMenu()
    {
        Debug.Log("SETTINGS MENU");
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    void QuitMenu()
    {
        Debug.Log("QUIT MENU");
        quitMenu.style.display = DisplayStyle.Flex;
    }
}