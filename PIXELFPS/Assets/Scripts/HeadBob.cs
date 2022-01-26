using UnityEngine;

public class HeadBob : MonoBehaviour
{ // https://www.youtube.com/watch?v=3ejFD3eortE
    public Transform headTransform;
    public Transform cameraTransform;
    public float bobFrequency = 5f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.1f;
    [Range(0,1)]public float headBobSmoothing = 0.1f;
    private bool isWalking;
    private float walkingTime;
    private Vector3 targetCamPos;
    private PlayerManager playerMan;

    void Start()
    {
        playerMan = GetComponent<PlayerManager>();
    }

    void Update()
    {
        if (!playerMan.isWalking || !playerMan.isGrounded) walkingTime = 0;
        else walkingTime += Time.deltaTime;
        targetCamPos = headTransform.position + CalculateHeadBobOffset(walkingTime);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCamPos, headBobSmoothing);
        if ((cameraTransform.position - targetCamPos).magnitude <= 0.001)
            cameraTransform.position = targetCamPos;
    }
    
    private Vector3 CalculateHeadBobOffset(float t)
    {
        float horizontalOffset = 0;
        float verticalOffset = 0;
        Vector3 offset = Vector3.zero;
        if (t > 0)
        {
            horizontalOffset = Mathf.Cos(t * bobFrequency) * bobHorizontalAmplitude;
            verticalOffset = Mathf.Sin(t * bobFrequency* 2) * bobVerticalAmplitude;
            offset = headTransform.right * horizontalOffset + headTransform.up * verticalOffset;
        }
        return offset;
    }
}