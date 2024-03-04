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
    public void addUnit(Unit u)
    {
        if (u.team != this.team) 
        {
            throw new System.Exception("Team mismatch for player and added unit");
        }
        this.units.Add(u);
    }

    //Draws a card given its parent
    public void drawCard(GameObject parent) 
    {
        Card gameObject = generator.generateCard();
        this.deck.Add(Object.Instantiate(gameObject, 
            new Vector3((this.deck.Count * 80) + 500, 0, 0)
            , new Quaternion(), parent.transform));
    }

    //EFFECT: highlights the tiles that contain these units
    public void hightLightUnitTiles() 
    {
        foreach (Unit unit in this.units) 
        {
            unit.hightLightTile();
        }
    }
}
