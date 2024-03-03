using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum CardType
{ 
    Move,
    Split,
    Shoot
}
public class Card : MonoBehaviour
{
    [SerializeField]
    CardType cardType;

    // Start is called before the first frame update
    
    void Start()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(this.enableCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void enableCard()
    { 
       
    }
}
