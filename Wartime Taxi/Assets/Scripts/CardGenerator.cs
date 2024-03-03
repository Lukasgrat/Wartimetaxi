using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{

    [SerializeField]
    GameObject moveCard;
    public Card generateCard() 
    {
        return moveCard.GetComponent<Card>();
    }
}
