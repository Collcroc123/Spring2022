using UnityEngine;
using Mirror;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    private SpellArrayData spells;
    public TMP_Text healthTxt;
    public int respawnTime = 5;
    private int maxHealth = 100;
    [SyncVar] private int health;
    private NetworkActions netActs;
    private AudioSource audio;
    public AudioClip hurt;
    private NetworkIdentity targetID;
    private PlayerMovement movement;
    private PlayerAttack attack;
    private GameObject head;
    private GameObject body;

    void Start()
    {
        targetID = GetComponent<NetworkIdentity>();
        spells = Resources.Load<SpellArrayData>("SpellList");
        health = maxHealth;
        netActs = FindObjectOfType<NetworkActions>();
        audio = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        head = transform.GetChild(0).gameObject;
        body = transform.GetChild(1).gameObject;
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
            Spellcast spellcast = other.GetComponent<Spellcast>();
            if (spellcast.player != (int)netId) 
            {
                health -= spells.var[spellcast.spellNumber].damage;
                audio.clip = hurt;
                audio.Play();
                if (health <= 0)
                {
                    RespawnPlayer(targetID.connectionToClient, gameObject, respawnTime); //netActs.
                    //Invoke(nameof(ResetHealth), respawnTime);// SET HEALTH AFTER RESPAWN!!!
                }
            }
        }
    }

    private void ResetHealth()
    {
        health = maxHealth;
    }
    
    [TargetRpc]
    public void RespawnPlayer(NetworkConnection tg, GameObject player, int time)
    { // MIGHT NOT WORK, TEST ASAP
        Debug.Log("DEAD");
        player.transform.position = NetworkManager.startPositions[Random.Range(0, NetworkManager.startPositions.Count)].position;
        movement.enabled = false;
        attack.enabled = false;
        head.SetActive(false);
        body.SetActive(false);
        Invoke(nameof(WaitRespawn), time);
    }
    
    private void WaitRespawn(GameObject player)
    {
        movement.enabled = true;
        attack.enabled = true;
        head.SetActive(true);
        body.SetActive(true);
        health = maxHealth;
        Debug.Log("ALIVE");
    }
}