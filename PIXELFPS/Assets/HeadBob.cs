using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float bobFrequency = 5f;
    public float bobXAmplitude = 0.2f;
    public float bobYAmplitude = 0.2f;
    [Range(0,1)] public float headBobSmoothing = 0.1f;
    public BoolData moving, grounded, sliding;
    private Transform headTransform;
    private GameObject mainCamera;
    private Vector3 targetCamPos;
    private float walkingTime;
    

    // Start is called before the first frame update
    void Start()
    {
        headTransform = gameObject.transform.parent;
        mainCamera = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Bob();
    }
    
    private void Bob()
    { // Bobs Camera Around While Moving
        if (moving.var && grounded.var && !sliding.var)
        {
            walkingTime += Time.deltaTime;
            targetCamPos = headTransform.position + CalculateBobOffset(walkingTime);
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
            offset = headTransform.right * horizontalOffset + headTransform.up * verticalOffset;
        }
        return offset;
    }
}