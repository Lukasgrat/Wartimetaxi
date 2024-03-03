using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{ 
    Move,
    Split,
    Shoot
}
public class Card : MonoBehaviour
{
    [SerializeField]
    CardType cardType;
    public Card(CardType cardType)
    {
        this.cardType = cardType;
    }


    // Start is called before the first frame update
    
    void Start()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(this.selectCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //activates this card
    void selectCard()
    { 
     
    }

    //EFFECT: sets the type of the card when its null
    void startingType(CardType type) 
    {
        if (this.cardType == null) 
        {
            
        }
    }
}
