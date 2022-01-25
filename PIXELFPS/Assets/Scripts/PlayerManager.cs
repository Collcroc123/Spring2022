using UnityEngine;
using Steamworks;
using Mirror;
using TMPro;

public class PlayerManager : NetworkBehaviour
{ // https://www.youtube.com/watch?v=_QajrabyTJc
    public Camera mainCamera;
    private string userName;
    public TextMeshPro nameTxt;
    
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
    
    [SyncVar] public int health = 100;
    public TextMeshPro healthTxt;
    
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        if (SteamManager.Initialized)
        {
            userName = SteamFriends.GetPersonaName();
            nameTxt.text = userName;
        }
        if (isLocalPlayer)
        {
            mainCamera.enabled = true;
        }
        else
        {
            Destroy(mainCamera);
        }
    }
    
    private void Update()
    {
        healthTxt.text = health.ToString();
        if (isLocalPlayer)
        {
            MovePlayer();
            MoveCamera();
        }
    }

    private void MovePlayer()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }

    private void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
