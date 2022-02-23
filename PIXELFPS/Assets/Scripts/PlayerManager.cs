using UnityEngine;
using Steamworks;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{ // https://www.youtube.com/watch?v=_QajrabyTJc
    public RawImage userPicture;
    public TextMeshPro nameTxt;
    public TextMeshPro healthTxt;
    [SyncVar] public int health = 100;
    [SyncVar(hook = nameof(PlayerColor))] public Color playerColor;
    [SyncVar(hook = nameof(PlayerName))] public string userName;
    public PlayerData playerData;
    [SyncVar(hook = nameof(SteamIdUpdated))] private ulong steamId;
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    [Header("Movement")]
    public CharacterController controller;
    public float mouseSensitivity = 500f;
    public float speed = 12f;
    public float gravity = -18f;
    public float jumpHeight = 4f;
    
    [Header("Jumping")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isWalking, isGrounded; //[HideInInspector] public
    private Vector3 velocity;
    private float xRotation;
    
    [Header("Camera")]
    public Transform headTransform;
    public GameObject mainCamera;
    public float bobFrequency = 5f;
    public float bobXAmplitude = 0.1f;
    public float bobYAmplitude = 0.1f;
    [Range(0,1)]public float headBobSmoothing = 0.1f;
    private float walkingTime;
    private Vector3 targetCamPos;

    public override void OnStartClient()
    {
        if (SteamManager.Initialized) avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (isLocalPlayer) GetComponentInChildren<SpriteRenderer>().enabled = false;
        if (isLocalPlayer && Camera.main.gameObject != null)
        {
            Destroy(Camera.main.gameObject);
            mainCamera.SetActive(true);
        }
        else Destroy(mainCamera);
        playerColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);
        userName = playerData.name;
    }
    
    private void Update()
    {
        healthTxt.text = health.ToString();
        
        if (hasAuthority)
        {
            MovePlayer();
            MoveCamera();
            if (mainCamera != null)
            {
                if (!isWalking || !isGrounded) walkingTime = 0;
                else walkingTime += Time.deltaTime;
                targetCamPos = headTransform.position + CalculateHeadBobOffset(walkingTime);
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, headBobSmoothing);
                if ((mainCamera.transform.position - targetCamPos).magnitude <= 0.001) mainCamera.transform.position = targetCamPos;
            }
        }
    }

    private void MovePlayer()
    { // Moves Player Based on Keyboard Input and Status
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (x != 0 || z != 0) isWalking = true;
        else isWalking = false;

        if (Input.GetKeyDown(KeyCode.LeftControl)) speed = 6f;
        else speed = 12f;
        
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }

    private void MoveCamera()
    { // Moves Camera Based on Mouse Movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    private Vector3 CalculateHeadBobOffset(float t)
    { // Bobs camera when you move
        float horizontalOffset = 0;
        float verticalOffset = 0;
        Vector3 offset = Vector3.zero;
        if (t > 0)
        {
            horizontalOffset = Mathf.Cos(t * bobFrequency) * bobXAmplitude;
            verticalOffset = Mathf.Sin(t * bobFrequency* 2) * bobYAmplitude;
            offset = headTransform.right * horizontalOffset + headTransform.up * verticalOffset;
        }
        return offset;
    }

    private void PlayerColor(Color oldColor, Color newColor)
    {
        GetComponentInChildren<SpriteRenderer>().color = newColor;
        if (isLocalPlayer)
        {
            GameObject.Find("LHand").GetComponent<Image>().color = newColor;
            GameObject.Find("RHand").GetComponent<Image>().color = newColor;
        }
    }

    private void PlayerName(string oldName, string newName)
    {
        nameTxt.text = newName;
    }
    
    #region STEAM
    
    public void SetSteamId(ulong steamId)
    { // Saves this player's Steam ID
        this.steamId = steamId;
    }
    
    private void SteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    { // Loads and Sets User's Steam Name and Avatar
        var cSteamId = new CSteamID(newSteamId);
        userName = SteamFriends.GetFriendPersonaName(cSteamId);
        nameTxt.text = userName;

        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);
        if (imageId != -1)
        {
            userPicture.texture = GetSteamImageAsTexture(imageId);
        }
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    { // Loads in User's Steam Avatar if Delayed
        if (callback.m_steamID.m_SteamID == steamId)
        {
            userPicture.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    { // Converts Steam Avatar Into Usable Image for Unity
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[(width * height) * 4];
            isValid = SteamUtils.GetImageRGBA(iImage, image, (int) (width * height * 4));
            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        return texture;
    }
    
    #endregion
}