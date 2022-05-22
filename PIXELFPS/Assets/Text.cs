using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Text : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            WindowsVoice.theVoice.speak("[dah%3C600,20%3E][dah%3C600,20%3E][dah%3C600,20%3E][dah%3C500,16%3E][dah%3C130,23%3E][dah%3C600,20%3E][dah%3C500,16%3E][dah%3C130,23%3E][dah%3C600,20%3E][_%3C800,17%3E][dah%3C600,27%3E][dah%3C600,27%3E][dah%3C600,27%3E][dah%3C500,28%3E][dah%3C130,23%3E][dah%3C600,19%3E][dah%3C500,16%3E][dah%3C130,23%3E][dah%3C600,20%3E]");
        }
    }
}
