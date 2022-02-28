using Mirror;
using UnityEngine;

public class Spellcast : NetworkBehaviour
{
    public SpellData spell; // Current Spell
    private SpriteRenderer texture; // Spell's texture
    private Rigidbody rbody; // Spell's rigidbody
    private AudioSource source; // Spellcast sound
    //public GameObject hitAnim; // Sound and particles

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), spell.duration);
    }
    
    void Start()
    { // Set velocity for server and client so we don't have to sync the position since both now simulate it.
        texture = GetComponentInChildren<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody>();
        texture.sprite = spell.texture;
        rbody.AddForce(transform.forward * spell.force);
        //source.clip = spell.castSound[Random.Range(0, spell.castSound.Length - 1)];
        //source.clip = spell.castSound[0];
        //source.pitch = Random.Range(0.9f, 1.1f);
        //source.Play();
    }
    
    [Server]
    void DestroySelf()
    { // Destroy for everyone on the server
        //Instantiate(hitAnim, gameObject.transform.position, gameObject.transform.rotation);
        NetworkServer.Destroy(gameObject);
    }
    
    [ServerCallback]
    void OnTriggerEnter(Collider co)
    { // ServerCallback because we don't want a warning if OnTriggerEnter is called on the client
        //Instantiate(hitAnim, gameObject.transform.position, gameObject.transform.rotation);
        if (co.GetComponent<PlayerManager>().netId != spell.player) NetworkServer.Destroy(gameObject);
    }
}