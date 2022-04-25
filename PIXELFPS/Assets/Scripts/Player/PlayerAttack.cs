using Mirror;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    public GameObject castPoint;                // Where to Cast From
    private Animator anim;                      // Attack Animation
    [SyncVar] private bool canAttack = true;    // Limits Attack Speed
    private SpellArrayData currentSpell;        // List of Spells
    [SyncVar] public int spell;                 // Currently Equipped Spell
    public float punchForce = 400;              // How Hard to Push Players
    public float punchDamage = 5;               // How Much Damage Punches Do
    [SyncVar] private GameObject target;        // Who to Push
    private AudioSource audio;

    private void Awake()
    {
        anim = GameObject.Find("RHand").GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        currentSpell = Resources.Load<SpellArrayData>("SpellList");
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
        if (spell != 0)
        {
            canAttack = false;
            anim.Play("Attack4F");
            Debug.Log("SPELL IS " + spell);
            CmdFire(rot, spell);
            Invoke(nameof(AttackCooldown), currentSpell.var[spell].rate);
        }
        //else Punch();
    }

    [Command]
    void CmdFire(Quaternion rot, int spellNum)
    {
        Debug.Log("COMMAND SPELL IS " + spell);
        Debug.Log("SPELLNUM IS " + spellNum);
        GameObject projectile = Instantiate(currentSpell.var[spellNum].prefab, castPoint.transform.position, rot);
        projectile.GetComponent<Spellcast>().spellNumber = spellNum;
        projectile.GetComponent<Spellcast>().player = (int)netId;
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