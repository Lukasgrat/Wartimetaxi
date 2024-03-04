using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{

    [SerializeField]
    Card moveCard;
    [SerializeField]
    InputHandler inputHandler;
    public Card generateCard() 
    {
        moveCard.setInputHandler(this.inputHandler); 
        return moveCard;
    }
}
