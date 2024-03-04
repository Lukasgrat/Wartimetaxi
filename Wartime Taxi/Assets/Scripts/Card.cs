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


    // Start is called before the first frame update
    
    void Start()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate { this.selectCard(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //activates this card
    void selectCard()
    {
        this.inputHandler.startMove();
    }
}
