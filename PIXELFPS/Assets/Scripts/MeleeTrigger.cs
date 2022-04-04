using Mirror;
using UnityEngine;

public class MeleeTrigger : NetworkBehaviour
{
    [SyncVar] private GameObject target;

    public void Punch(Vector3 dir, float force)
    {
        CmdPunch(target, dir, force);
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
        Vector3 direction = transform.forward;
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