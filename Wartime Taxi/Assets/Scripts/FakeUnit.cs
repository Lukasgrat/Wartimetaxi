using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeUnit : Unit
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
    [SerializeField]
    Unit realUnit;

    //Adds the given unit as the parent unit of this fake unit
    //WILL THROW AN ERROR IF THE UNIT TYPES ARE NOT THE SAME
    //(A SUBMARINE CANNOT BE A FAKE OF A DESTROYER)
    public void setParentUnit(Unit parent) 
    {
        if (!parent.sameType(this.type)) 
        {
            throw new System.Exception("Attempted to make a parent that is not the type of the fake, " + this.type);
        } 
        this.realUnit = parent;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Returns whether this unit is real or not
    public override bool isReal()
    {
        return false;
    }

    //Shoots this ship for 1 health, returnning if it survived the shot
    public override bool survivedShot()
    {
        return false;
    }


    //Returns the display string for this unit, given which team is asking
    //if its the enemy team, it will not say if its real or fake
    public override string displayString(Team team)
    {
        if (this.type == UnitType.Submarine && !this.opposingTeam(team))
        {
            return this.team + " Fake " + this.type + "\n Health: " + this.health + " Max:" + this.MAXHEALTH;

        }
        else
        {
            return this.team + " " + this.type + "\n Health: " + this.health + " Max:" + this.MAXHEALTH;
        }
    }
}
