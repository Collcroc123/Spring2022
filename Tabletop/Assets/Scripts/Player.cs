using System;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool saidHello;

    private void Start()
    {
        if (isLocalPlayer)
        { // If the client is running this code...
            
        }
    }

    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal * 0.1f, moveVertical * 0.1f, 0);
            transform.position = transform.position + movement;
        }
    }

    void Update()
    {
        HandleMovement();

        if (isLocalPlayer && !saidHello && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Saying Hello to Server!");
            Hello();
        }
        else if (isServer && saidHello && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Saying Hello back to the Client!");
            saidHello = false;
            HelloBack();
            
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("saidHello is " + saidHello);
        }
    }

    public override void OnStartServer()
    {
        Debug.Log("Player has spawned on the server!");
    }

    [Command]
    void Hello()
    {
        Debug.Log("Received Hello from Client, Reply with X!");
        saidHello = true;
    }
    
    [TargetRpc]
    void HelloBack()
    {
        Debug.Log("Received Hello from Server!");
    }
}
