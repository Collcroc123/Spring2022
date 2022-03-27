using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class PlayerManager : NetworkBehaviour
{ // https://www.youtube.com/watch?v=_QajrabyTJc
    #region General
    public TextMeshPro healthBar;
    public PlayerData player;
    public SpellData currentSpell;
    public int respawnTime = 5;
    private int maxHealth = 100;
    [SyncVar] public int health = 100;
    [SyncVar(hook = nameof(PlayerColor))] public Color playerColor;
    //protected Callback<AvatarImageLoaded_t> avatarImageLoaded; //protected
    private NetworkActions netActs;
    [SyncVar] private bool canAttack = true;
    private Animator anim;
    private MeleeTrigger melee;
    #endregion
    
    #region Movement
    [Header("Movement")]
    public float speed = 10f;
    public float gravity = -18f;
    public float jumpHeight = 7.5f;
    public LayerMask groundMask;
    public BoolData isWalking, isGrounded;
    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;
    private Rigidbody playerBody;
    private Vector3 velocity;
    private float xRotation;
    #endregion
    
    #region Camera
    [Header("Camera")]
    public float mouseSensitivity = 250f;
    private GameObject mainCamera;
    private Transform castPoint;
    private float walkingTime;
    private Vector3 targetCamPos;
    #endregion
    
    /*public override void OnStartClient()
    { // If Steam Running and Avatar Loaded, Tell Player Data to Get Avatar
        if (SteamManager.Initialized) avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(player.OnAvatarImageLoaded);
    }*/

    private void Start()
    {
        netActs = FindObjectOfType<NetworkActions>();
        playerBody = GetComponent<Rigidbody>();
        mainCamera = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        castPoint = gameObject.transform.GetChild(0).GetChild(0).GetChild(0);
        melee = castPoint.GetComponent<MeleeTrigger>();
        Camera cam = mainCamera.GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        if (isLocalPlayer) GetComponentInChildren<SpriteRenderer>().enabled = false;
        if (isLocalPlayer)
        {
            //if (Camera.main.gameObject != null) Destroy(Camera.main.gameObject);
            cam.enabled = true;
            gameObject.tag = "Player";
        }
        else cam.enabled = false;
        playerColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);
        anim = GameObject.Find("RHand").GetComponent<Animator>();
        //speed *= netActs.settings.playerSpeedMultiplier/100;
        //gravity *= netActs.settings.gravityMultiplier/100;
        //respawnTime *= netActs.settings.respawnTime;
    }
    
    private void Update()
    {
        //healthBar.text = new string('-', health);
        if (isLocalPlayer || hasAuthority)
        {
            MovePlayer();
            MovePlayerCamera();
            //transform.Rotate(0f, mainCamera.transform.rotation.y, 0f);
            //playerBody.MoveRotation(Quaternion.Euler(0f, mainCamera.transform.rotation.y, 0f));
            if (Input.GetKeyDown(KeyCode.Mouse0)) CmdFire();
            if (Input.GetKeyDown(KeyCode.F)) CmdPunch();
        }
    }

    #region Camera & Movement
    private void MovePlayer()
    { // Moves Player Based on Keyboard Input and Status
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * speed;
        playerBody.velocity = new Vector3(moveVector.x, playerBody.velocity.y, moveVector.z);
        //playerBody.MovePosition(transform.position + moveVector * Time.deltaTime);
        Vector3 location = gameObject.transform.position;
        location.y = gameObject.transform.position.y - 0.8f;
        isGrounded.var = Physics.CheckSphere(location, 0.25f, groundMask);
        if (playerMovementInput.magnitude > 0) isWalking.var = true;
        else isWalking.var = false; // playerMovementInput.x != 0 || playerMovementInput.z != 0
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
            playerBody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }
    
    private void MovePlayerCamera()
    {
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * Time.deltaTime;
        xRotation -= playerMouseInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(0f, playerMouseInput.x, 0f);
        //playerBody.MoveRotation(Quaternion.Euler(0f, playerMouseInput.x, 0f));
    }
    #endregion

    #region Attack
    [Command] // this is called on the server
    void CmdFire()
    {
        if (canAttack)
        {
            if (currentSpell != null) StartCoroutine(CastCooldown(currentSpell.rate));
            else StartCoroutine(PunchCooldown(1));
        }
    }

    [ClientRpc] // this is called on the player that fired for all observers
    void RpcOnFire()
    {
        //animator.SetTrigger("Shoot");
    }

    [Command]
    void CmdPunch()
    {
        if (canAttack) StartCoroutine(PunchCooldown(1));
    }
    
    private IEnumerator CastCooldown(float seconds)
    {
        canAttack = false;
        GameObject projectile = Instantiate(currentSpell.prefab, castPoint.transform.position, castPoint.transform.rotation);
        projectile.GetComponent<Spellcast>().spell.player = (int)netId;
        NetworkServer.Spawn(projectile);
        anim.Play("Attack4F");
        //RpcOnFire();
        yield return new WaitForSeconds(seconds);
        canAttack = true;
    }

    private IEnumerator PunchCooldown(float cooldown)
    {
        canAttack= false;
        melee.Punch(castPoint.transform.rotation.eulerAngles, 1000);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
    #endregion
    
    #region Health
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