using System.Collections;
using UnityEngine;
using Steamworks;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{ // https://www.youtube.com/watch?v=_QajrabyTJc
    #region General
    public TextMeshPro healthBar;
    public PlayerData player;
    public SpellData currentSpell;
    public int respawnTime = 5;
    private int maxHealth = 4;
    [SyncVar] public int health = 4;
    [SyncVar(hook = nameof(PlayerColor))] public Color playerColor;
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded; //protected
    private NetworkActions netActs;
    private bool canShoot = true;
    #endregion
    
    #region Movement
    [Header("Movement")]
    public float speed = 10f;
    public float gravity = -18f;
    public float jumpHeight = 4f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private CharacterController controller;
    private bool isWalking, isGrounded;
    private Vector3 velocity;
    private float xRotation;
    #endregion
    
    #region Camera
    [Header("Camera")]
    public float mouseSensitivity = 500f;
    public float bobFrequency = 5f;
    public float bobXAmplitude = 0.1f;
    public float bobYAmplitude = 0.1f;
    [Range(0,1)] public float headBobSmoothing = 0.1f;
    private Transform headTransform;
    private GameObject mainCamera;
    private Transform castPoint;
    private float walkingTime;
    private Vector3 targetCamPos;
    #endregion
    
    public override void OnStartClient()
    { // If Steam Running and Avatar Loaded, Tell Player Data to Get Avatar
        if (SteamManager.Initialized) avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(player.OnAvatarImageLoaded);
    }

    private void Start()
    {
        netActs = FindObjectOfType<NetworkActions>();
        controller = GetComponent<CharacterController>();
        headTransform = gameObject.transform.GetChild(0);
        mainCamera = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        castPoint = gameObject.transform.GetChild(0).GetChild(0).GetChild(0);
        Camera cam = mainCamera.GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        if (isLocalPlayer) GetComponentInChildren<SpriteRenderer>().enabled = false;
        if (isLocalPlayer)
        {
            if (Camera.main.gameObject != null) Destroy(Camera.main.gameObject);
            cam.enabled = true;
            gameObject.tag = "Player";
        }
        else cam.enabled = false;
        playerColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);
        //speed *= netActs.currentSelectedServer.playerSpeedMultiplier/100;
        //gravity *= netActs.currentSelectedServer.gravityMultiplier/100;
        //respawnTime *= netActs.currentSelectedServer.respawnTime;
    }
    
    private void Update()
    { //healthTxt.text = health.ToString();
        healthBar.text = new string('-', health);
        if (isLocalPlayer || hasAuthority)
        {
            MovePlayer();
            MoveCamera();
            if (mainCamera != null) HeadBob();
            if (Input.GetKeyDown(KeyCode.Mouse0)) CmdFire();
        }
    }

    #region Camera & Movement
    private void MovePlayer()
    { // Moves Player Based on Keyboard Input and Status
        Vector3 location = gameObject.transform.position;
        location.y = gameObject.transform.position.y - 1.2f;
        isGrounded = Physics.CheckSphere(location, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (x != 0 || z != 0) isWalking = true;
        else isWalking = false;
        if (Input.GetKeyDown(KeyCode.LeftShift)) speed = 18f;
        else if (Input.GetKeyDown(KeyCode.LeftControl)) speed = 6f;
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

    private void HeadBob()
    { // Bobs Camera Around While Moving
        if (!isWalking || !isGrounded) walkingTime = 0;
        else walkingTime += Time.deltaTime;
        targetCamPos = headTransform.position + CalculateHeadBobOffset(walkingTime);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, headBobSmoothing);
        if ((mainCamera.transform.position - targetCamPos).magnitude <= 0.001) mainCamera.transform.position = targetCamPos;
    }
    
    private Vector3 CalculateHeadBobOffset(float t)
    { // Calculates How The Camera Should Bob
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
    #endregion
    
    private void PlayerColor(Color oldColor, Color newColor)
    { // Syncs Player Color
        GetComponentInChildren<SpriteRenderer>().color = newColor;
        if (isLocalPlayer)
        {
            GameObject.Find("LHand").GetComponent<Image>().color = newColor;
            GameObject.Find("RHand").GetComponent<Image>().color = newColor;
        }
    }
    
    [Command] // this is called on the server
    void CmdFire()
    {
        if (canShoot)
        {
            //canShoot = false;
            GameObject projectile = Instantiate(currentSpell.prefab, castPoint.transform.position, castPoint.transform.rotation);
            projectile.GetComponent<Spellcast>().spell.player = (int)netId;
            NetworkServer.Spawn(projectile);
            //RpcOnFire();
            //CastCooldown(currentSpell.rate);
        }
    }

    [ClientRpc] // this is called on the player that fired for all observers
    void RpcOnFire()
    {
        //animator.SetTrigger("Shoot");
    }
    
    private IEnumerator CastCooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canShoot = true;
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        SpellData spell = other.GetComponent<Spellcast>().spell;
        if (spell.player != (int)netId) 
        {
            health -= spell.damage;
            if (health <= 0)
            {
                netActs.RespawnPlayer(gameObject, respawnTime);
                StartCoroutine(ResetHealth(respawnTime)); // SET HEALTH AFTER RESPAWN!!!
            }
        }
    }

    private IEnumerator ResetHealth(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        health = maxHealth;
    }

    #region STEAM
    /*
    public void SetSteamId(ulong steamId)
    { // Saves this player's Steam ID
        this.steamId = steamId;
    }
    
    private void SteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    { // Loads and Sets User's Steam Name and Avatar
        var cSteamId = new CSteamID(newSteamId);
        player.steamName = SteamFriends.GetFriendPersonaName(cSteamId);
        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);
        if (imageId != -1) player.steamImage = GetSteamImageAsTexture(imageId);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    { // Loads in User's Steam Avatar if Delayed
        if (callback.m_steamID.m_SteamID == steamId) player.steamImage = GetSteamImageAsTexture(callback.m_iImage);
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
    }*/
    #endregion
}