using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{ 
    Move,
    Split,
    Shoot,
    None
}
public class Card : MonoBehaviour
{
    [SerializeField]
    CardType cardType;
    public InputHandler inputHandler;
    public Card(CardType cardType, InputHandler inputHandler)
    {
        this.cardType = cardType;
    }

    public void setInputHandler(InputHandler inputHandler)
    {
        this.inputHandler = inputHandler;
    }

    public bool sameType(CardType type) 
    { 
        return this.cardType == type;
    }
    // Start is called before the first frame update
    
    void Start()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate { this.selectCard(); });
    }
    //activates this card
    void selectCard()
    {
        switch (this.cardType) 
        {
            case CardType.Move:
                this.inputHandler.startMove();
                break;
            case CardType.Shoot:
                this.inputHandler.startShoot();
                break;
            case CardType.Split:
                this.inputHandler.startSplit();
                break;
            default:
                throw new System.Exception("Error, card state " + this.cardType + "has not been accounted for.");
        }

    }
}
