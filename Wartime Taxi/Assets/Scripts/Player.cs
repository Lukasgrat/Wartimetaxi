using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;

public class Player
{
    List<Unit> units;
    Team team;
    List<Card> deck;
    Vector3 startingPoint;
    [SerializeField]
    CardGenerator generator;

    Player(Team team, List<Unit> units, List<Card> deck, CardGenerator generator)
    {
        this.units = units;
        this.team = team;
        this.deck = deck;
        this.startingPoint = new Vector3(0,0,0);
        this.generator = generator;
    }
    public Player(Team team, Vector3 startingPoint, CardGenerator generator)
    {
        this.team = team;
        this.units = new List<Unit>();
        this.deck = new List<Card>();
        this.startingPoint = startingPoint;
        this.generator = generator;
    }

    //EFFECT displays all cards in hand
    public void showCards(bool shouldShow)
    {
        foreach (Card card in this.deck)
        {
            card.enabled = shouldShow;
        }
    }
    //Returns whether this player can draw a card given the current maximum
    public bool canDrawCard(int maxCards) 
    { 
        return this.deck.Count < maxCards;
    }

    //Draws a card given its parent
    public void drawCard(GameObject parent) 
    {
        Card gameObject = generator.generateCard();
        this.deck.Add(Object.Instantiate(gameObject, 
            new Vector3((this.deck.Count * 80) + 500, 0, 0)
            , new Quaternion(), parent.transform));
    }
}
