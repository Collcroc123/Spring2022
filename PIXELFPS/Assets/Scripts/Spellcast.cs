using System.Collections;
using UnityEngine;

public class Spellcast : MonoBehaviour
{
    [HideInInspector] public SpellData spell; // Current gun
    [HideInInspector] public GameObject castSpawn; // Location where bullet spawns
    private Renderer texture; // Bullet's texture
    private Rigidbody rbody; // Bullet's rigidbody
    private AudioSource source; // Gunshot sound
    //public GameObject hitAnim; // Sound and particles
    // Private Light bulletLight;
    
    void Start()
    { // Gives cast attributes depending on what gun is fired
        texture = GetComponentInChildren<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody>();
        texture.material = spell.spellTexture;
        rbody.velocity = castSpawn.transform.forward * spell.castSpeed;
        //source.clip = spell.castSound[Random.Range(0, spell.castSound.Length - 1)];
        source.clip = spell.castSound[0];
        source.pitch = Random.Range(0.9f, 1.1f);
        source.Play();
    }

    private void OnTriggerEnter(Collider other)
    { // Checks if spell hits something
        if (other.CompareTag("Enemy") && gameObject.CompareTag("FriendlySpell"))
        {
            Hit();
        }
        else if (other.CompareTag("Player") && gameObject.CompareTag("EnemySpell"))
        {
            Hit();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Door") || other.CompareTag("Crate"))
        {
            if (!spell.canBounce)
            {
                Hit();
            }
        }
    }
    
    private void Hit()
    {
        //Instantiate(hitAnim, gameObject.transform.position, Quaternion.Euler(90, 0 ,0));
        Destroy(gameObject);
    }
    
    private void OnTriggerExit(Collider other)
    { //deletes spell if it hits nothing after 5 secs
        //StartCoroutine(WaitFor(5));
    }

    private IEnumerator WaitFor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}