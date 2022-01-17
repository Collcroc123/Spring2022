using UnityEngine;
using Mirror;

public class Connector : NetworkBehaviour
{
    public Server server;
    public Client client;
    private float lastCardLoc;
    public GameObject card;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    [Command]
    public void NextTurn()
    {
        server.GetCurrentCard();
        // send to current player
    }
    
    [Command]
    public void Draw(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Debug.Log("DRAWING CARD");
            NewCard(server.GetTopCard());
            server.RefillDraw();
        }
    }
    
    [TargetRpc]
    public void NewCard(UnoCard unoCard)
    {
        if (card != null)
        {
            lastCardLoc += 0.5f;
            GameObject newCard = Instantiate(card, new Vector3(lastCardLoc, 0, 0), Quaternion.Euler(0, 0, 0));
            newCard.GetComponent<CardInfo>().cardData = unoCard;
        }
        else
        {
            client.EndTurn();
            NextTurn();
        }
    }

    [Command]
    public void Play(UnoCard card)
    {
        
    }
}
