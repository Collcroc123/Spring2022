using UnityEngine;

public class MeleeTrigger : MonoBehaviour
{
    private float mass = 3; // defines the character mass
    private Vector3 impact = Vector3.zero;
    private CharacterController character;
 
    void Start()
    {
        character = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        if (character != null && impact.magnitude > 0.2) character.Move(impact * Time.deltaTime); // apply the impact force
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime); // consumes the impact energy each cycle
    }
    
    // call this function to add an impact force:
    public void Punch(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            character = other.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            character = null;
        }
    }
}
