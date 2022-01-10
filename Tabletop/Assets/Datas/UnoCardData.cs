using UnityEngine;

[CreateAssetMenu]
public class UnoCardData : ScriptableObject
{
    public int color; // Black = 0, Red = 1, Blue = 2, Green = 3, Yellow = 4
    public int number; // 0-9, +2 = 10, Reverse = 11, Skip = 12, Blacks(Wild = 1, +4 = 2, Blank = 3)
    
    // if (color == 0 && number > 3) { Debug.Log("INVALID CARD: " + color + "-" + number); }
    // if (number < 0 || color < 0 || number > 12 || color > 4) { Debug.Log("INVALID CARD: " + color + "-" + number); }
}
