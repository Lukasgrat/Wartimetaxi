using System.Collections;
using System.Collections.Generic;
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
    int health = 4;
    // Start is called before the first frame update
    void Start()
    {
        location.addUnit(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   //returns whether this unit has the same type as the given type
    public bool sameType(UnitType type) 
    {
        return this.type == type;
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

    public override string ToString()
    {
        return this.type.ToString();
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
        tiles.Add(this.location);
    }


    //Shoots this ship for 1 health, returnning if it survived the shot
    public bool survivedShot() 
    {
        this.health -= 1;
        Debug.Log(this.health);
        return this.health > 0;
    }

    //Removes u from its given locaiton
    public void removeLocation() 
    { 
        this.location.removeUnit(this);
    }
}
