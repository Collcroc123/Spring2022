// Useful for Text Meshes that should face the camera.
using UnityEngine;

namespace Mirror.Examples.Tanks
{
    public class FaceCamera : MonoBehaviour
    {
        void LateUpdate()
        {
            if (Camera.main != null)
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}