using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using System.Linq;

public class Player
{
    List<Unit> units;
    Team team;
    List<Card> deck;
    Vector3 startingPoint;
    [SerializeField]
    CardGenerator generator;
    int CARDSPACING = 110;
    int standardMaxHealth = 4;

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
            card.gameObject.SetActive(shouldShow);
        }
    }

    //Returns if this player controls the given tile
    public bool controlsTile(Tile t) 
    {
        return t.sameTeam(this.team);
    }

    //Returns the amount of units this player has left
    public int unitCount() 
    {
        return this.units.Count;
    }

    //Returns if this player is of the given team
    public bool sameTeam(Team team) 
    {
        return this.team == team;
    }

    //Returns whether this player can draw a card given the current maximum
    public bool canDrawCard(int maxCards)
    {
        return this.deck.Count < maxCards;
    }
    public void addUnit(Unit u)
    {
        if (!u.sameTeam(this.team))
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
            new Vector3((this.deck.Count * CARDSPACING) + parent.transform.position.x,
            parent.transform.position.y,
            parent.transform.position.z),
            new Quaternion(), parent.transform));
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
                    new Vector3(card.transform.position.x - CARDSPACING,
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

    //Returns all possible actions this player can perform,
    //given the maximum number of cards, and where to store cards
    public List<Action> possibleActions(int maxCards, GameObject parent, Player enemy, FakeUnit fakeTemplateUnit, Unit templateUnit) 
    {
        List<Action> actions = new List<Action>();
        if (this.canDrawCard(maxCards))
            actions.Add(new DrawCard(this, parent));

        bool hasShoot = false;
        bool hasSplit = false;
        bool hasMove = false;
        foreach (Card c in this.deck) 
        {
            if (c.sameType(CardType.Move))
            {
                hasMove = true;
            }
            else if (c.sameType(CardType.Split))
            {
                hasSplit = true;
            }
            else if (c.sameType(CardType.Shoot)) 
            {
                hasShoot = true;
            }
        }
        if (hasShoot) 
        {
            actions.Add(new Discard(CardType.Shoot, this));
            foreach (Unit u in this.units) 
            {
                foreach (Unit vunerable in u.possibleShots(enemy.units)) 
                {
                    Debug.Log(vunerable.gameObject.name);
                    actions.Add(new OrderAction(new Shoot(u, vunerable, enemy), this));
                }
            }
        }
        if (hasSplit)
        {
            actions.Add(new Discard(CardType.Split, this));
            foreach (Unit u in this.units) 
            {
                foreach (Tile t in u.movements())
                {
                    if (u.type == UnitType.Submarine && !this.containsFake())
                    {
                        actions.Add(new OrderAction(
                            u.makeFakeSplitOrder(t, this, fakeTemplateUnit, true), 
                            this));
                        actions.Add(new OrderAction(
                            u.makeFakeSplitOrder(t, this, fakeTemplateUnit, false), 
                            this));
                    }
                    else
                    {
                        foreach (int health in u.possibleHealthSplits())
                        {
                            actions.Add(new OrderAction(
                                u.makeSplitOrder(t, this, health, templateUnit),
                                this));
                        }
                    }

                }
            }
        }
        if (hasMove)
        {
            actions.Add(new Discard(CardType.Move, this));
            foreach(Unit u in this.units) 
            {
                foreach (Tile t in u.movements()) 
                {
                    actions.Add(new OrderAction(u.makeMoveOrder(t), this));
                }
            }
        }
        return actions;
    }

    //Returns if this player has any fake units associate with it
    public bool containsFake() 
    {
        foreach(Unit u in this.units) 
        {
            if (!u.isReal()) 
            {
                return true;
            }
        }
        return false;
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

    public List<Tile> canSplitFrom()
    {
        List<Tile> returnTiles = new List<Tile>();
        foreach (Unit u in units)
        {
            if (u.canMoveOff() && u.canSurviveShot(1))
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
        u.selfDestruction();
    }

    //returns all tiles vunerable to the give unit
    public List<Tile> vunerableTiles(Unit u) 
    {
        List<Tile> tiles = new List<Tile>();
       foreach (Unit unit in this.units) 
        {
            if (u.canShoot(unit)) 
            {
                unit.addLocationTo(tiles);
            }
        }
        return tiles;
    }

    //Attempts to repair all units that are able to be repaired
    public void repairAll() 
    {
        foreach (Unit u in this.units) 
        {
            u.updateMaxiumum(this, this.standardMaxHealth);
            u.recover();
        }
    }

    //Given  Unittype, returns how much the assumed
    //unit's health should increase to meet the maximum
    //IE If there is a sub with 2 health in there, 
    //the function will return 2 so that when the maximum
    //health increases, it will meet the standard of 4

    public int totalHealth(UnitType type) 
    {
        int x = 0;
        foreach (Unit u in this.units) 
        { 
            if(u.sameType(type)) 
            {
                x = u.addMaxHealthTo(x);
            }
        }
        return this.standardMaxHealth - x;
    }

    //Checks to see if there are any units that can be consolidated, doing so if there are
    public void consolidate() 
    {
        if (this.units.Count < 2) 
        {
            return;
        }
        for (int i = 0; i < this.units.Count-1; i++) 
        { 
            for(int j = i + 1; j < this.units.Count; j++)
            {
                if (this.units[i].sameLocation(this.units[j])) 
                {
                    if (this.units[i].assimilate(this.units[j])) 
                    {
                        this.destroyUnit(this.units[j]);
                        j--;
                    }
                }
            }
        }
    }

    //Returns all possible moves the player can make in this state

}
