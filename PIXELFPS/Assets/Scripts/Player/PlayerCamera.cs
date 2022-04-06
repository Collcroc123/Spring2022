using Mirror;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    private PlayerMovement player;
    private Transform orientation;
    private Transform headTF, cameraTF, mainCamera;
    private Vector3 targetCamPos;
    private float walkingTime;
    private float xRotation;
    private float sensMultiplier = 1f;
    public float sensitivity = 100f;
    public float bobFrequency = 5f;
    public float bobXAmplitude = 0.2f;
    public float bobYAmplitude = 0.2f;
    [Range(0,1)] public float headBobSmoothing = 0.1f;

    void Start()
    {
        player = transform.GetComponent<PlayerMovement>();
        orientation = transform.GetChild(1);
        headTF = transform.GetChild(0);
        cameraTF = headTF.GetChild(0);
        mainCamera = cameraTF.GetChild(0);
        if(!isLocalPlayer) Destroy(mainCamera);
    }
    
    void Update()
    {
        if (isLocalPlayer || hasAuthority)
        {
            cameraTF.transform.position = player.transform.position;
            Look();
            if (mainCamera != null) Bob();
        }
    }
    
    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = cameraTF.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        cameraTF.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void Bob()
    { // Bobs Camera Around While Moving
        if (player.moving && player.grounded && !player.sliding)
        {
            walkingTime += Time.deltaTime;
            targetCamPos = headTF.position + CalculateBobOffset(walkingTime);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, headBobSmoothing);
            if ((mainCamera.transform.position - targetCamPos).magnitude <= 0.001) mainCamera.transform.position = targetCamPos;
        }
        else walkingTime = 0;
    }
    
    private Vector3 CalculateBobOffset(float t)
    { // Calculates How The Camera Should Bob
        float horizontalOffset = 0;
        float verticalOffset = 0;
        Vector3 offset = Vector3.zero;
        if (t > 0)
        {
            horizontalOffset = Mathf.Cos(t * bobFrequency) * bobXAmplitude;
            verticalOffset = Mathf.Sin(t * bobFrequency* 2) * bobYAmplitude;
            offset = headTF.right * horizontalOffset + headTF.up * verticalOffset;
        }
        return offset;
    }
}