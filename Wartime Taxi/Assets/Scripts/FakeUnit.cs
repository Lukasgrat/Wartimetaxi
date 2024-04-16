using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FakeUnit : Unit
{
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
        location.addUnit(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Returns whether this unit can hit that unit
    public override bool canShoot(Unit that) 
    {
        return false;
    }
    //Returns whether this unit is real or not
    public override bool isReal()
    {
        return false;
    }

    //Shoots this ship for 1 health, returnning if it survived the shot
    public override bool canSurviveShot(int damage)
    {
        return  false;
    }
    public override void shot(int damage) 
    {
        this.health = 0;
    }

    //Returns the display string for this unit, given which team is asking
    //if its the enemy team, it will not say if its real or fake
    public override string displayString(Team team)
    {
        if (this.type == UnitType.Submarine && !this.opposingTeam(team))
        {
            return this.team + " Fake " + this.type;
        }
        else
        {
            return this.team + " " + this.type + "\n Health: " + this.health + " / " + this.MAXHEALTH;
        }
    }

    //Self destructs this unit if its a fake unit
    //Given this is a fake unit, it does
    public override void selfDestruction()
    {
        this.removeLocation();
        this.realUnit.removeFake();
        Object.Destroy(this.gameObject);
    }

    //Returns the sum of the given maximum health and this unit's max health
    public override int addMaxHealthTo(int max)
    {
        return max;
    }
}
