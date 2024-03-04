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
    InputHandler inputHandler;
    public Card generateCard() 
    {
        int num = Random.Range(0, 10);
        if (num < 6)
        {
            moveCard.setInputHandler(this.inputHandler);
            return moveCard;
        }
        else 
        {
            shootCard.setInputHandler(this.inputHandler);
            return shootCard;
        }
    }
}
