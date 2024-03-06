using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{

    [SerializeField]
    Card moveCard;
    [SerializeField]
    Card shootCard;
    [SerializeField]
    Card splitCard;
    [SerializeField]
    InputHandler inputHandler;
    public Card generateCard() 
    {
        int num = Random.Range(0, 10);
        if (num < 4)
        {
            moveCard.setInputHandler(this.inputHandler);
            return moveCard;
        }
        else if (num < 7) 
        { 
            splitCard.setInputHandler(this.inputHandler);
            return splitCard;
        }
        else
        {
            shootCard.setInputHandler(this.inputHandler);
            return shootCard;
        }
    }
}
