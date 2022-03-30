using UnityEngine;

public class MeleeTrigger : MonoBehaviour
{
    private Rigidbody rb;

    void Update()
    {
        //if (character != null && impact.magnitude > 0.2) character.Move(impact * Time.deltaTime); // apply the impact force
        //impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime); // consumes the impact energy each cycle
    }
    
    // call this function to add an impact force:
    public void Punch(Vector3 dir, float force)
    {
        Vector3 direction = transform.forward;
        direction.y = 0.75f;
        Debug.Log(direction);
        if (rb != null) rb.AddForce(direction * force);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) rb = other.GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) rb = null;
    }
}
