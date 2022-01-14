using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private int cardColor, cardNumber;
    private int drawSize, discardSize;
    public bool infiniteDeck, blanks;
    //private UnoCard[] deck;
    public CardDeck deck;
    public GameObject card;
    
    private void Start()
    {
        CreateDeck();
        InstantiateDeck();
        if (isLocalPlayer)
        { // If the client is running this code...
            //lol
        }
    }
    
    public void CheckCardPlayability(int playedColor, int playedNumber)
    {
        if (playedColor == cardColor || playedNumber == cardNumber)
        {
            
        }
    }

    public void Draw(int num)
    {
        for (int i = 0; i < num; i++)
        {
            //draw cards here
            RefillDraw();
        }
    }

    public void EndTurn(bool skip)
    {
        // code to end current player's turn, skip bool
    }

    public void RefillDraw()
    {
        if (!infiniteDeck && drawSize <= 0)
        {
            if (discardSize > 0)
            {
                drawSize = discardSize;
                discardSize = 0;
            }
            else
            {
                Debug.Log("Draw Pile & Discard Pile Both Empty!");
                EndTurn(false);
            }
        }
        else
        {
            return;
        }
    }

    public void CreateDeck()
    {
        int cardNumber = 0;    // Tracks the current card type
        int cardNumCount = 0;  // Tracks how many of each card type
        int cardColor = 0;     // Tracks the current card color
        int deckSize = 108;
        if (blanks) { deckSize += 4; }
        Debug.Log("Beginning Deck Creation...");
        deck.unoDeck = new UnoCard[deckSize];
        for (int i = 0; i < deckSize; i++)
        {
            if (cardColor == 0)
            { // If cards are black
                deck.unoDeck[i] = ScriptableObject.CreateInstance<UnoCard>();
                deck.unoDeck[i].color = cardColor;
                deck.unoDeck[i].number = cardNumber;
                deck.unoDeck[i].GetImage();
                Debug.Log("Color: " + cardColor + " Number: " + cardNumber);
                cardNumCount++;
                if (cardNumCount == 4)
                { // Makes sure there's only four of each black card
                    cardNumCount = 0;
                    cardNumber++;
                }
                if (cardNumber == 2 && !blanks)
                { // Ends black cards if no blanks
                    cardColor++;
                    cardNumber = 0;
                }
                else if (cardNumber == 3 && blanks)
                { // Ends black cards if blanks
                    cardColor++;
                    cardNumber = 0;
                }
            }
            else
            {
                deck.unoDeck[i] = ScriptableObject.CreateInstance<UnoCard>();
                deck.unoDeck[i].color = cardColor;
                deck.unoDeck[i].number = cardNumber;
                deck.unoDeck[i].GetImage();
                Debug.Log("Color: " + cardColor + " Number: " + cardNumber);
                if (cardNumber == 0)
                { // Makes sure there's only one 0 card per color
                    cardNumber++;
                }
                else
                { // Makes two of all other cards
                    cardNumCount++;
                }
            
                if (cardNumCount == 2)
                { // Makes sure there's only two of each card per color
                    cardNumCount = 0;
                    cardNumber++;
                }
            
                if (cardNumber > 12)
                { // Changes card color when finished with current color
                    cardNumber = 0;
                    cardColor++;
                }
            }
        }
    }

    public void InstantiateDeck()
    {
        for (int i = 0; i < deck.unoDeck.Length; i++)
        {
            Instantiate(card, new Vector3(0, i*0.01f, 0), Quaternion.Euler(90f, 0, 0));
        }
    }
}