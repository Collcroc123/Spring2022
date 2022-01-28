using System;
using UnityEngine;
using TMPro;

public class Billboard : MonoBehaviour
{
    public Sprite[] spriteSheet;
    private Camera mainCam;
    private SpriteRenderer sprite;
    private float angle;
    public TextMeshPro angleTxt;


    void Start()
    {
        mainCam = Camera.main;
        sprite = GetComponent<SpriteRenderer>();
    }
    
    void LateUpdate()
    {
        if (mainCam != null)
        {
            transform.LookAt(mainCam.transform);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
            if (spriteSheet.Length == 8)
            {
                Vector3 direction = mainCam.transform.position - transform.parent.position;
                angle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg;
                if (transform.parent.eulerAngles.y - angle < 180) angle -= transform.parent.eulerAngles.y;
                else angle -= transform.parent.eulerAngles.y - 360;
                angleTxt.text = angle.ToString();
                
                int index = (int)MathF.Round(angle/45);
                if (index < 0) index += 8;
                sprite.sprite = spriteSheet[index];
            }
        }
        else mainCam = Camera.main;
    }
}

/*
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
    sprite.sprite = spriteSheet[7];*/