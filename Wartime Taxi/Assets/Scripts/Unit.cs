using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public enum UnitType
{
    Airbase,
    Marine,
    Destroyer,
    Cruiser,
    Submarine
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
    Tile location;
    [SerializeField]
    UnitType type;
    [SerializeField]
    public Team team;
    [SerializeField]
    int health = 4;
    [SerializeField]
    int MAXHEALTH = 4;
    FakeUnit fake;
    bool hasFake = false;
    // Start is called before the first frame update
    void Start()
    {
        location.addUnit(this);
    }
    //Creates a new Unit
    public void resetUnit(Tile location, int MAXHEALTH, UnitType type, Team team) 
    {
        this.location = location;
        this.MAXHEALTH = MAXHEALTH;
        this.type = type;
        this.team = team;
        health = MAXHEALTH;
    }
    
    //returns whether this unit has the same team as the given team

    public bool sameTeam(Team team) { return this.team == team; }


    //returns if this unit is the given type
    public bool sameType(UnitType type) 
    { 
        return this.type.Equals(type);
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
    }

    public bool canMoveToLand() 
    { 
        return this.type == UnitType.Marine || this.type == UnitType.Airbase;
    }

    //EFFECT: Highlights the tile associated  with this unit
    public void hightLightTile() 
    {
        this.location.lightTile(true);
    }


    //Returns whether this unit can hit that unit

    public bool canShoot(Unit that) 
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
                    this.type == UnitType.Marine;
            case (UnitType.Destroyer):
                return this.type == UnitType.Submarine;
            case (UnitType.Submarine):
                return this.type == UnitType.Cruiser ||
                    this.type == UnitType.Destroyer;
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


    public void addFact(FakeUnit unit) 
    { 
        unit.setParentUnit(this);
        this.fake = unit;
        this.hasFake = true;
    }

    public void removeFake() 
    {
        this.hasFake = false;
    }

    //Shoots this ship for 1 health, returnning if it survived the shot
    public virtual bool survivedShot() 
    {
        this.health -= 1;
        return this.health > 0;
    }

    public bool hasFakeUnit() 
    { 
        return this.hasFake;
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

    //EFFECT: Returns if this unit can off of the given tile to any of the other tiles
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
    public void split(Tile tile, int health, bool isFake, Unit templateUnit) 
    {
        if (health > this.health) 
        {
            throw new Exception("Cannot remove " + health + " health for a split with" +
                "a max health of " + this.health + " health.");
        }
        if (this.type == UnitType.Submarine)
        {
            //TODO
            this.health -= health;
            this.MAXHEALTH -= health;
            templateUnit.resetUnit(tile, health, this.type, this.team);
        }
        else
        {
            this.health -= health;
            this.MAXHEALTH -= health;
            templateUnit.resetUnit(tile, health, this.type, this.team);
        }

    }

    //Returns whether this unit is real or not
    public virtual bool isReal() 
    {
        return true; 
    }

    //Returns the sum of the given maximum health and this unit's health
    public int addMaxHealthTo(int max) 
    { 
        return this.MAXHEALTH + max;
    }

    //EFFECT increases this unit's maximum health and
    //standard health, assuming they are the same type
    //returning if it was successful
    public bool assimilate(Unit u) 
    {
        if (this.type != u.type) 
        {
            return false;
        }
        this.health += u.health;
        this.MAXHEALTH += u.MAXHEALTH;
        return true;
    }

    //Returns if this and that unit have the same location
    public bool sameLocation(Unit that) 
    {
        return this.location == that.location;
    }
}
