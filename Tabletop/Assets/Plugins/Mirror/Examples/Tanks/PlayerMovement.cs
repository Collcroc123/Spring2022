using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public Vector3 movement = new Vector3();
    
    void Start()
    {
        
    }
    
    [Client]
    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdMove();
            }
        }
    }
    
    [Command]
    private void CmdMove()
    {
        RpcMove();
    }

    [ClientRpc]
    private void RpcMove()
    {
        transform.Translate(movement);
    }
}
