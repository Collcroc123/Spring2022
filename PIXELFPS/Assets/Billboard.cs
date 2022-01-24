using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;
    private SpriteRenderer sprite;
    public Sprite[] spriteSheet;

    void Start()
    {
        mainCam = Camera.main;
        sprite = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (mainCam != null)
        {
            transform.LookAt(mainCam.transform);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            
            Vector3 direction = mainCam.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg;
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
        }
        else
        {
            mainCam = Camera.main;
        }
    }
}