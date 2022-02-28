using UnityEngine;

[CreateAssetMenu(menuName = "Datas/SpellData")]
public class SpellData : ScriptableObject
{
    public int player;
    public GameObject cast; //spellcast prefab to spawn
    public float rate, force, cost, recharge, duration; //spell & cast attributes
    public int damage; //spell damage
    public bool canBounce;
    public Sprite texture; //bullet color
    public AudioClip[] sound; //gunshot sounds
    //public AudioClip reloadSound; //reload sound
}