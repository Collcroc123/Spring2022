using Mirror;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    public GameObject castPoint;
    private Animator anim; // Attack Animation
    [SyncVar] private bool canAttack = true; // Limits Attack Speed
    public SpellData currentSpell; // Currently Equipped Spell
    public float punchForce = 400; // How Hard to Push Players
    public float punchDamage = 5; // How Much Damage Punches Do
    [SyncVar] private GameObject target;

    private void Start()
    {
        anim = GameObject.Find("RHand").GetComponent<Animator>();
    }

    private void Update()
    {
        if (isLocalPlayer || hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Client CanAttack: " + canAttack);
                if (canAttack) anim.Play("Attack4F");
                CmdFire();
            }
            if (Input.GetKeyDown(KeyCode.F) && canAttack) Melee();
        }
    }

    [Command] // this is called on the server
    void CmdFire()
    {
        Debug.Log("Server CanAttack = " + canAttack);
        if (canAttack)
        {
            if (currentSpell != null)
            {
                Debug.Log("MAGIC!");
                canAttack = false;
                GameObject projectile = Instantiate(currentSpell.prefab, castPoint.transform.position, castPoint.transform.rotation);
                projectile.GetComponent<Spellcast>().spell = currentSpell;
                projectile.GetComponent<Spellcast>().spell.player = (int)netId;
                NetworkServer.Spawn(projectile);
                Invoke(nameof(AttackCooldown), currentSpell.rate);
            }
            else Melee();
        }
    }
    
    [Command]
    void Melee()
    {
        Debug.Log("PUNCH!");
        canAttack = false;
        Punch(castPoint.transform.rotation.eulerAngles, punchForce);
        Invoke(nameof(AttackCooldown), 1);
    }
    
    [Command]
    private void AttackCooldown()
    {
        canAttack = true;
        AttackCooldownRpc();
    }
    
    [TargetRpc]
    private void AttackCooldownRpc()
    {
        canAttack = true;
    }

    public void Punch(Vector3 dir, float force)
    {
        if (canAttack) anim.Play("Attack4F"); // CHANGE TO PUNCH
        if (target != null) CmdPunch(target, dir, force);
    }
    
    [Command]
    public void CmdPunch(GameObject target, Vector3 dir, float force)
    {
        NetworkIdentity targetID = target.GetComponent<NetworkIdentity>();
        RpcPunch(targetID.connectionToClient, dir, force);
    }
    
    [TargetRpc]
    public void RpcPunch(NetworkConnection tg, Vector3 dir, float force)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        Vector3 direction = castPoint.transform.forward;
        direction.y = 0.75f;
        if (rb != null) rb.AddForce(direction * force);
        //Debug.Log(direction);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) target = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) target = null;
    }
}