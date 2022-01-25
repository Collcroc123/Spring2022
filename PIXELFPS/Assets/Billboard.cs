using TMPro;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera mainCam;
    private SpriteRenderer sprite;
    private float angle;
    public Sprite[] spriteSheet;
    public Transform player;
    public TextMeshPro angleTxt;

    void Start()
    {
        mainCam = Camera.main;
        sprite = GetComponent<SpriteRenderer>();
        player = transform.parent;
    }
    
    void Update()
    {
        if (mainCam != null) // CLIENT SIDE STILL USING SELF CAMERA RATHER THAN CLIENT CAM
        {
            transform.LookAt(mainCam.transform);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            if (spriteSheet.Length == 8)
            {
                Vector3 direction = mainCam.transform.position - player.position;
                angle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg;

                if (player.eulerAngles.y < 180)
                {
                    angle -= player.eulerAngles.y;
                }
                else
                {
                    angle -= player.eulerAngles.y - 360;
                }
                
                //Debug.Log(angle);
                if (angle > -22.5 && angle <= 22.5)
                    sprite.sprite = spriteSheet[0];
                else if (angle > 22.5 && angle <= 67.5)
                    sprite.sprite = spriteSheet[1];
                else if (angle > 67.5 && angle <= 112.5)
                    sprite.sprite = spriteSheet[2];
                else if (angle > 112.5 && angle <= 157.5)
                    sprite.sprite = spriteSheet[3];
                else if (angle > 157.5 || angle <= -157.5)
                    sprite.sprite = spriteSheet[4];
                else if (angle > -157.5 && angle <= -112.5)
                    sprite.sprite = spriteSheet[5];
                else if (angle > -112.5 && angle <= -67.5)
                    sprite.sprite = spriteSheet[6];
                else if (angle > -67.5 && angle <= -22.5)
                    sprite.sprite = spriteSheet[7];
                
                angleTxt.text = angle.ToString(); //angle.ToString();
            }
        }
        else
        {
            Debug.Log("NO MAIN CAM");
            mainCam = Camera.main;
        }
    }
}