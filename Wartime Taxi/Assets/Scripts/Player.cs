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

    //EFFECT: Removes the first instance of the given type of card within this person's hand
    // If none are found, does nothing
    public void removeCard(CardType type)
    {
        bool hasDestroyed = false;
        for (int x = 0; x < this.deck.Count; x += 1)
        {
            Card card = this.deck[x];
            if (hasDestroyed)
            {
                card.transform.position =
                    new Vector3(card.transform.position.x - 80,
                    card.transform.position.y,
                    card.transform.position.z);
            }
            else if (card.sameType(type))
            {
                hasDestroyed = true;
                Object.Destroy(card.gameObject);
                this.deck.RemoveAt(x);
                x -= 1;
            }
        }
    }

    //Returns the list of tiles of this player where units can shoot the given units
    public List<Tile> canMoveFrom()
    {
        List<Tile> returnTiles = new List<Tile>();
        foreach (Unit u in units)
        {
            if (u.canMoveOff()) 
            {
                u.addLocationTo(returnTiles);
            }
        }
        return returnTiles;
    }


    //Returns the list of tiles of the attackers of this player be shot by the given player in any form
    public List<Tile> canBeShotBy(Player that)
    {
        return that.canShoot(this.units);
    }

    //Returns the list of tiles of this player where units can shoot the given units
    public List<Tile> canShoot(List<Unit> units) 
    {
        List<Tile> returnTiles = new List<Tile>();
        foreach (Unit target in units) 
        {
            foreach (Unit unit in this.units) 
            {
                if (unit.canShoot(target)) 
                {
                    unit.addLocationTo(returnTiles);
                }
            }
        }
        return returnTiles;
    }

    //Removes the unit u from this player
    public void destroyUnit(Unit u) 
    {
        this.units.Remove(u);
        u.removeLocation();
        Object.Destroy(u.gameObject);
    }
}
