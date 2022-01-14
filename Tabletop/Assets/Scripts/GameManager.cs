using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int cardColor, cardNumber;
    private int drawSize, discardSize;
    public bool infiniteDeck;

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
    
}
