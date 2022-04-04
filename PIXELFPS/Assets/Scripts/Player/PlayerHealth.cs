using UnityEngine;
using Mirror;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    public TMP_Text healthTxt;
    public int respawnTime = 5;
    private int maxHealth = 100;
    [SyncVar] private int health;
    private NetworkActions netActs;
    
    void Start()
    {
        health = maxHealth;
        netActs = FindObjectOfType<NetworkActions>();
    }
    
    void Update()
    {
        healthTxt.text = health.ToString();
    }
    
    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Spellcast>())
        {
            SpellData spell = other.GetComponent<Spellcast>().spell;
            if (spell.player != (int)netId) 
            {
                health -= spell.damage;
                if (health <= 0)
                {
                    netActs.RespawnPlayer(gameObject, respawnTime);
                    Invoke(nameof(ResetHealth), respawnTime);// SET HEALTH AFTER RESPAWN!!!
                }
            }
        }
    }

    private void ResetHealth()
    {
        health = maxHealth;
    }
}