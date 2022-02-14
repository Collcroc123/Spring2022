using UnityEngine;

[CreateAssetMenu(menuName = "Datas/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string name = "Player";
    public int health = 100;

    public void SetName(string newName)
    {
        name = newName;
        if (name == null) name = "Player";
    }
}