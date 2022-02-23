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
    
    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start()
    {
        texture = GetComponentInChildren<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody>();
        texture.sprite = spell.spellTexture;
        rbody.AddForce(transform.forward * spell.force);
        //source.clip = spell.castSound[Random.Range(0, spell.castSound.Length - 1)];
        //source.clip = spell.castSound[0];
        //source.pitch = Random.Range(0.9f, 1.1f);
        //source.Play();
    }
    
    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        Debug.Log("IM DESTROYED!!!");
        //Instantiate(hitAnim, gameObject.transform.position, Quaternion.Euler(90, 0 ,0));
        NetworkServer.Destroy(gameObject);
    }
    
    // ServerCallback because we don't want a warning
    // if OnTriggerEnter is called on the client
    [ServerCallback]
    void OnTriggerEnter(Collider co)
    {
        //Instantiate(hitAnim, gameObject.transform.position, Quaternion.Euler(90, 0 ,0));
        if (co.CompareTag("Player") == false) NetworkServer.Destroy(gameObject);
    }
}