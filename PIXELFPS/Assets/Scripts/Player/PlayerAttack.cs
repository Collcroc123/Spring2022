using Mirror;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    public GameObject castPoint;                // Where to Cast From
    private Animator anim;                      // Attack Animation
    [SyncVar] private bool canAttack = true;    // Limits Attack Speed
    public SpellData currentSpell;              // Currently Equipped Spell
    public float punchForce = 400;              // How Hard to Push Players
    public float punchDamage = 5;               // How Much Damage Punches Do
    [SyncVar] private GameObject target;        // Who to Push

    private void Start()
    {
        anim = GameObject.Find("RHand").GetComponent<Animator>();
    }

    private void Update()
    {
        if (isLocalPlayer || hasAuthority)
        {
            if (canAttack)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0)) Fire(castPoint.transform.rotation);
                if (Input.GetKeyDown(KeyCode.F)) Punch(castPoint.transform.forward);
            }
        }
    }

    void Fire(Quaternion rot)
    {
        if (currentSpell != null)
        {
            canAttack = false;
            anim.Play("Attack4F");
            CmdFire(rot);
            Invoke(nameof(AttackCooldown), currentSpell.rate);
        }
        //else Punch();
    }

    [Command]
    void CmdFire(Quaternion rot)
    {
        GameObject projectile = Instantiate(currentSpell.prefab, castPoint.transform.position, rot);
        projectile.GetComponent<Spellcast>().spell = currentSpell;
        projectile.GetComponent<Spellcast>().spell.player = (int)netId;
        NetworkServer.Spawn(projectile);
    }
    
    void Punch(Vector3 dir)
    {
        canAttack = false;
        anim.Play("Attack4F"); // CHANGE TO PUNCH
        if (target != null) CmdPunch(target, punchForce, dir);
        Invoke(nameof(AttackCooldown), 1);
    }

    [Command]
    public void CmdPunch(GameObject targ, float force, Vector3 dir)
    {
        NetworkIdentity targetID = targ.GetComponent<NetworkIdentity>();
        RpcPunch(targetID.connectionToClient, targ, force, dir);
    }
    
    [TargetRpc]
    public void RpcPunch(NetworkConnection tg, GameObject targ, float force, Vector3 dir)
    {
        if (targ != null)
        {
            Rigidbody rb = targ.GetComponent<Rigidbody>();
            Vector3 direction = dir; //rot
            direction.y = 0.75f;
            if (rb != null) rb.AddForce(direction * force);
        }
    }
    
    private void AttackCooldown()
    {
        canAttack = true;
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