using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
//using Steamworks;

// RigidBody Based Movement by Dani
// https://www.youtube.com/watch?v=XAC8U9-dTZU
public class PlayerMovement : NetworkBehaviour 
{
    [Header("General")]
    public Transform playerCam;
    public Transform orientation;
    private Rigidbody rb;
    public PlayerData player;

    [Header("Movement")]
    public float moveSpeed = 4000;
    public float maxSpeed = 15;
    public BoolData grounded, moving, sliding; //tracks if active
    public LayerMask whatIsGround;
    public float counterMovement = 0.175f;
    public float maxSlopeAngle = 35f;
    public float jumpForce = 600f;
    [SyncVar] private bool readyToJump = true; //tracks jump cooldown
    private float jumpCooldown = 0.25f;
    private float threshold = 0.01f;
    private float x, y;
    private bool jumping, sprinting, crouching; //tracks input
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    [Header("Crouch/Slide")]
    private Vector3 crouchScale = new Vector3(1, 0.75f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    
    [Header("Rotation")]
    private float xRotation;
    public float sensitivity = 100f;
    private float sensMultiplier = 1f;

    [Header("Cosmetic")]
    [SyncVar(hook = nameof(PlayerColor))] public Color playerColor;
    //protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Camera cam = playerCam.GetChild(0).GetComponent<Camera>();
        if (isLocalPlayer)
        {
            orientation.GetComponentInChildren<SpriteRenderer>().enabled = false;
            //if (Camera.main.gameObject != null) NetworkServer.Destroy(Camera.main.gameObject);
            //cam.enabled = true;
            gameObject.tag = "Player";
        }
        else Destroy(cam.gameObject);
        playerColor = Color.HSVToRGB(UnityEngine.Random.Range(0.0f, 1.0f), 1.0f, 1.0f);
        //speed *= netActs.settings.playerSpeedMultiplier/100;
        //gravity *= netActs.settings.gravityMultiplier/100;
        //respawnTime *= netActs.settings.respawnTime;
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void FixedUpdate()
    {
        if (isLocalPlayer || hasAuthority) Movement();
    }

    private void Update()
    {
        if (isLocalPlayer || hasAuthority)
        {
            MyInput();
            Look();
        }
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        if (x != 0 || y != 0) moving.var = true;
        else moving.var = false;
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.LeftShift)) StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftShift)) StopCrouch();
    }
    
    #region Crouch
    private void StartCrouch()
    {
        sliding.var = true;
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f && grounded.var) rb.AddForce(orientation.transform.forward * slideForce);
    }

    private void StopCrouch()
    {
        sliding.var = false;
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
    #endregion
    
    #region Movement
    private void Movement()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10); // Extra gravity
        // Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;
        CounterMovement(x, y, mag); // Counteract sliding and sloppy movement
        if (readyToJump && grounded.var && jumping) Jump(); // If holding jump && ready to jump, then jump
        float maxSpeed = this.maxSpeed; // Set max speed
        if (crouching && grounded.var && readyToJump)
        { // If sliding down a ramp, add force down so player stays grounded and also builds speed
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;
        
        float multiplier = 1f, multiplierV = 1f; // Some multipliers
        
        if (!grounded.var)
        { // Movement in air
            //multiplier = 0.5f;
            //multiplierV = 0.5f;
        }
        if (grounded.var && crouching) multiplierV = 0f; // Movement while sliding
        
        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }
    #endregion

    #region Jump
    private void Jump()
    {
        if (grounded.var && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f) rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump()
    {
        readyToJump = true;
    }

    private bool cancellingGrounded;
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal))
            {
                grounded.var = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }
    
    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private void StopGrounded()
    {
        grounded.var = false;
    }
    
    #endregion
    
    #region Looking
    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }
    
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;
        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        return new Vector2(xMag, yMag);
    }
    #endregion

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded.var || jumping) return;
        if (crouching)
        { // Slow down sliding
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }
        
        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }
    
    

    private void PlayerColor(Color oldColor, Color newColor)
    { // Syncs Player Color
        GetComponentInChildren<SpriteRenderer>().color = newColor;
        if (isLocalPlayer)
        {
            GameObject.Find("LHand").GetComponent<Image>().color = newColor;
            GameObject.Find("RHand").GetComponent<Image>().color = newColor;
        }
    }
    
    /*public override void OnStartClient()
    { // If Steam Running and Avatar Loaded, Tell Player Data to Get Avatar
        if (SteamManager.Initialized) avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(player.OnAvatarImageLoaded);
    }*/
}
