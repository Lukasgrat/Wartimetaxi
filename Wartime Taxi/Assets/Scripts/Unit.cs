using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public enum UnitType
{
    Airbase,
    Marine,
    Destroyer,
    Cruiser,
    Submarine,
    None,
}


public enum Team
{
    Green,
    Red,
    Neutral
}

public class Unit : MonoBehaviour
{
    [SerializeField]
    internal Tile location;
    [SerializeField]
    internal UnitType type;
    [SerializeField]
    internal Team team;
    [SerializeField]
    internal int health = 4;
    [SerializeField]
    internal int MAXHEALTH = 4;
    FakeUnit fake;
    internal bool hasFake = false;


    Player player;
    // Start is called before the first frame update
    void Start()
    {
        location.addUnit(this);
    }


    //Creates a new Unit
    public void resetUnit(Tile location, int MAXHEALTH, UnitType type, Team team, Player p) 
    {
        this.moveTo(location);
        this.MAXHEALTH = MAXHEALTH;
        this.type = type;
        this.team = team;
        health = MAXHEALTH;
        this.player = null;
        this.setPlayer(p);
    }

    //sets the given player(can only be done once)
    public void setPlayer(Player p)
    {
        if (this.player != null) 
        {
            throw new Exception("Cannot alter player after set");
        }
        this.player = p;
    }

    //returns the associated player of this unit
    public Player getPlayer() 
    {
        if (this.player == null) 
        {
            throw new Exception("Error, the player has not been set for unit" + this.gameObject.name);
        }
        return this.player;
    }


    //returns whether this unit has the same team as the given team

    public bool sameTeam(Team team) { return this.team == team; }


    //returns if this unit is the given type
    public bool sameType(UnitType type) 
    { 
        return this.type.Equals(type);
    }

    //returns if this unit is the same type
    public bool sameType(Unit u)
    {
        return u.sameType(this.type);
    }

    //EFFECT: Converts the unit from a Marine to an Airfield and vice versa
    public void grade(bool isUp) 
    {
        if (isUp && this.type == UnitType.Marine) 
        {
            this.type = UnitType.Airbase;
        }
        else if (!isUp && this.type == UnitType.Airbase) 
        { 
            this.type = UnitType.Marine;
        }
    }

    //returns whether the given team is opposing to this unit
    public bool opposingTeam(Team team) 
    {
        if (this.team == Team.Green && team == Team.Red)
        {
            return true;
        }
        else if (this.team == Team.Red && team == Team.Green) 
        {
            return true;
        }
        return false;
    }

    public void moveTo(Tile tile)
    {
        this.location = tile;

        if (tile != null)
        {
            tile.changeTeam(this.team);
            if (tile.isAirBase)
            {
                this.grade(true);
            }
            else
            {
                this.grade(false);
            }
        }

    }

    public bool canMoveToLand() 
    { 
        return this.type == UnitType.Marine || this.type == UnitType.Airbase;
    }

    //Returns whether this unit can hit that unit

    public virtual bool canShoot(Unit that) 
    {
        if (that.opposingTeam(this.team) && that.isVunerable(this.type)) 
        {
            if (this.type == UnitType.Airbase) 
            {
                return that.unitInRangeOfTile(2, this.location);
            }
            return that.unitInRangeOfTile(1, this.location);
        }
        return false;
    }
    //Returns if this unit is within the given range of the given tile
    public bool unitInRangeOfTile(int n, Tile t) 
    {
        return t.inRange(n, this.location);
    }

    public bool canMoveOff() 
    {
        return this.location.canMoveOffOf(this);
    }

    //Returns whether this piece is vunerable to that type of piece

    public bool isVunerable(UnitType ut) 
    {
        if (ut == this.type) 
        {
            return true;
        }
        switch (ut) 
        { 
            case(UnitType.Marine):
                return this.type == UnitType.Airbase;
            case(UnitType.Airbase):
                return this.type != UnitType.Destroyer;
            case (UnitType.Cruiser):
                return this.type == UnitType.Airbase ||
                    this.type == UnitType.Marine ||
                    this.type == UnitType.Destroyer;
            case (UnitType.Destroyer):
                return this.type == UnitType.Submarine;
            case (UnitType.Submarine):
                return this.type == UnitType.Cruiser;
        }
        throw new System.Exception("Missing case for:" + ut);
    }

    //EFFECT Adds the location of this unit to the given list
    public void addLocationTo(List<Tile> tiles) 
    {
        if (!tiles.Contains(this.location)) 
        { 
        tiles.Add(this.location);
        }
    }


    public void addFake(FakeUnit unit) 
    { 
        unit.setParentUnit(this);
        this.fake = unit;
        this.hasFake = true;
    }

    public void removeFake() 
    {
        this.hasFake = false;
    }


    //EFFECT: Decreases the unit's health by the given amount, capping at 0 health
    public virtual void shot(int damage) 
    {
        if (this.health - damage < 0)
        {
            this.health = 0;
        }
        else 
        {
            this.health -= damage;
        }
        
    }

    //Returns if this unit can survive the given amount of damage
    //If the damage is zero, it implies its requesting if the unit is alive
    public virtual bool canSurviveShot(int damage) 
    {
        return this.health - damage > 0;
    }

    //Returns the display string for this unit, given which team is asking
    //if its the enemy team, it will not say if its real or fake
    public virtual string displayString(Team team) 
    {
        if (this.type == UnitType.Submarine && !this.opposingTeam(team))
        {
            return this.team + " Real " + this.type + "\n Health: " + this.health + " Max:" + this.MAXHEALTH;

        }
        else
        {
            return this.team + " " + this.type + "\n Health: " + this.health + " Max:" + this.MAXHEALTH;
        }
    }

    //Removes u from its given locaton
    public void removeLocation() 
    { 
        this.location.removeUnit(this);
    }

    // Returns if this unit can off of the given tile to any of the other tiles
    public bool canMoveOffOf(Tile tile, List<Tile> adjacentTiles)
    {
        foreach (Tile t in adjacentTiles)
        {
            if (t.canMove(this, tile))
            {
                return true;
            }
        }
        return false;
    }


    //Returns all the tiles this unit can move to 
    public List<Tile> movements() 
    {
        List<Tile> tiles = this.location.movementOptions(this);
        return tiles;
    }

    //Returns all units this unit can shoot at, given the enemy list of units
    public List<Unit> possibleShots(List<Unit> units) 
    {
        List<Unit> returnUnits = new List<Unit>();
        foreach(Unit that in units)
        {
            if (this.canShoot(that)) 
            {
                returnUnits.Add(that);
            }
        }
        return returnUnits;
    }

    //Updates the maximum health of this unit to meet in line with the standard maximum
    public void updateMaxiumum(Player p, int standard)
    {
        if (this.MAXHEALTH == standard) 
        {
            return;
        }
        this.MAXHEALTH += p.totalHealth(this.type);
    }

    //EFFECT: Increases the health of this unit by one up to the maximum
    public void recover() 
    {
        if (this.location.canRecover() &&  this.health < this.MAXHEALTH) 
        {
            this.health += 1;
        }
    }

    //Returns the amount of health this unit can spare
    public int healthToSpare() 
    {
        return this.health - 1;
    }

    //Splits this unit to alter the given one with the given health and Tile,
    //And if the new one is a fake(ignored for all units except submarine)
    public void split(Tile tile, int health, Unit templateUnit) 
    {
        if (health > this.health) 
        {
            throw new Exception("Cannot remove " + health + " health for a split with" +
                "a max health of " + this.health + " health.");
        }
        this.health -= health;
        this.MAXHEALTH -= health;
        templateUnit.resetUnit(tile, health, this.type, this.team, this.player);
    }

    //Splits this unit to alter the given one with the given health and Tile,
    //And if the new one is a fake(ignored for all units except submarine)
    public void split(Tile tile, FakeUnit templateUnit, bool shouldSwapPlaces)
    {
        if (shouldSwapPlaces)
        {
            templateUnit.resetUnit(this.location, this.health, this.type, this.team, this.player);
            Order moveOrder = new Move(this.location, tile, this);
            moveOrder.playCard();
        }
        else 
        {
            templateUnit.resetUnit(tile, this.health, this.type, this.team, this.player);
        }
    }


    //Returns whether this unit is real or not
    public virtual bool isReal() 
    {
        return true; 
    }

    //Returns the possible healths this unit can split off of
    public List<int> possibleHealthSplits()
    {
        List<int> returnList= new List<int>();
        if (!this.isReal()) 
        {
            return returnList;
        }
        for (int x = this.health - 1; x > 1; x -= 1) 
        {
            returnList.Add(x);
        }
        return returnList;
    }


    //Returns the sum of the given maximum health and this unit's max health
    public int addMaxHealthTo(int max) 
    { 
        return this.MAXHEALTH + max;
    }

    //EFFECT increases this unit's maximum health and
    //standard health, assuming they are the same type
    //returning if it was successful
    public void assimilate(Unit u) 
    {
        if (this.type != u.type) 
        {
            throw new Exception("Error: Attempted units to assimiliate are not of the same type");
        }
        if (!u.isReal()) 
        {
            return;
        } 
        this.health += u.health;
        this.MAXHEALTH += u.MAXHEALTH;
        if (!this.hasFake && u.hasFake) 
        {
            this.fake = u.fake;
            this.fake.setParentUnit(this);
        }
    }

    //Returns if this and that unit have the same location
    public bool sameLocation(Unit that) 
    {
        return this.location == that.location;
    }

    //Makes a move order out of the given next tile to move to

    public Order makeMoveOrder(Tile nextTile) 
    {
        return new Move(this.location, nextTile, this);
    }

    //Makes a fake split order out of the given health, nexttile, swapping, and respective templates
    public Order makeFakeSplitOrder(Tile nextTile, Player p, FakeUnit unit, bool shouldSwapPlaces) 
    {
        return new Split(this, this.location, nextTile, p, unit, shouldSwapPlaces);
    }

    public Order makeSplitOrder(Tile nextTile, Player p, int health, Unit unit) 
    {
        return new Split(this, this.location, nextTile, p, health, unit);
    }


 
    //Self destructs this unit if its a fake unit
    //Given this is a real unit, does nothing
    public virtual void selfDestruction() 
    {

        this.removeLocation();
        Destroy(this.gameObject);
    }

    public void unlinkFake() 
    {
        this.fake = null;
        this.hasFake = false;
    }

    //Attempts to get the fake unit from this unit,
    //if it has none, throws error
    public FakeUnit getFake() 
    {
        if (this.hasFake)
        {
            return this.fake;
        }
        else 
        {
            throw new Exception("Error: attempted to retrieve non-existant fake unit from" + this.name);
        }
    }



    //Returns if this unit is on the given location
    public bool onLocation(Tile t)
    {
        return this.location == t;
    }
}
