using UnityEngine;

[CreateAssetMenu(menuName = "Datas/SpellData")]
public class SpellData : ScriptableObject
{
    public GameObject cast; //spellcast prefab to spawn
    public float castRate, castSpeed, manaCost, rechargeSpeed; //spell & cast attributes
    public int spellDamage; //spell damage
    public bool canBounce;
    public Material spellTexture; //bullet color
    public AudioClip[] castSound; //gunshot sounds
    //public AudioClip reloadSound; //reload sound
}