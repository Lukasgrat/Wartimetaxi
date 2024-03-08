using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

interface Order
{
    bool isValid();
    void playCard();

    int value();
}

class Move : Order {
    Tile currentTile;
    Tile nextTile;
    Unit unit;
    public Move(Tile currentTile, Tile nextTile, Unit unit) 
    {
        this.currentTile = currentTile;
        this.nextTile = nextTile;
        this.unit = unit; 
    }
    public bool isValid() 
    {
        return this.currentTile.canMove(this.unit, this.nextTile);
    }
    public void playCard() 
    {
        this.currentTile.removeUnit(this.unit);
        this.nextTile.addUnit(this.unit);
        if (this.currentTile.isAirBase && !this.nextTile.isAirBase) 
        {
            this.unit.grade(false);
        }
        else if (!this.currentTile.isAirBase && this.nextTile.isAirBase)
        {
            this.unit.grade(true);
        }
    }

    public int value()
    {
        return 0;
    }
}

class Shoot : Order 
{
    Unit shooter;
    Unit target;
    Player targetedPlayer;
    public Shoot(Unit shooter, Unit target, Player targetedPlayer) 
    { 
        this.shooter = shooter;
        this.target = target;
        this.targetedPlayer = targetedPlayer;
    }

    public bool isValid()
    {
        return shooter.canShoot(this.target);
    }

    public void playCard()
    {
        target.shot(1);
        if (!this.target.isReal())
        {
            this.targetedPlayer.destroyUnit(this.target);
        }
        else
        {
            if (this.target.hasFake)
            {
                this.targetedPlayer.destroyUnit(this.target.getFake());
            }
            if (!target.canSurviveShot(0))
            {
                targetedPlayer.destroyUnit(this.target);
            }
        }
    }

    public int value()
    {
        return 0;
    }
}

class Split : Order 
{
    Unit u;
    Tile currentTile;
    Tile nextTile;
    int splitHealth;
    Player p;
    bool isFake;
    Unit templateUnit;
    FakeUnit fakeTemplateUnit;
    bool shouldSwapPlaces;
    public Split(Unit u, Tile currentTile, Tile nextTile, Player p, int splitHealth, Unit templateUnit) 
    { 
        this.u = u;
        this.nextTile = nextTile;
        this.p = p;
        this.currentTile = currentTile;
        this.splitHealth = splitHealth;
        this.templateUnit = templateUnit;
        this.isFake = false;
    }

    public Split(Unit u, Tile currentTile, Tile nextTile, Player p, FakeUnit fakeTemplateUnit, bool shouldSwapPlaces) 
    {
        this.u = u;
        this.currentTile = currentTile;
        this.nextTile = nextTile;
        this.p = p;
        this.fakeTemplateUnit = fakeTemplateUnit;
        this.shouldSwapPlaces = shouldSwapPlaces;
        this.isFake = true;
    }


    public void playCard() 
    {
        if (!isFake)
        {
            this.splitReal();
        }
        else 
        { 
            this.splitFake();
        }
    }

    void splitReal()
    {
        Unit newUnit = this.templateUnit;
        this.u.split(this.nextTile, splitHealth, newUnit);
        GameObject unitsParent = GameObject.Find("Units");
        Unit finishedUnit = Object.Instantiate(newUnit,
           Vector3.zero,
            new Quaternion(), unitsParent.transform);
        p.addUnit(finishedUnit);

    }

    void splitFake() 
    {
        FakeUnit newUnit = this.fakeTemplateUnit;
        this.u.split(this.nextTile, newUnit, this.shouldSwapPlaces);
        GameObject unitsParent = GameObject.Find("Units");
        FakeUnit finishedUnit = Object.Instantiate(newUnit,
            Vector3.zero,
            new Quaternion(), unitsParent.transform);
        p.addUnit(finishedUnit);
        this.u.addFake(finishedUnit);
    }


    public bool isValid() 
    {
        return u.healthToSpare() >= this.splitHealth && this.currentTile.canMove(this.u, this.nextTile);
    }

    public int value() 
    {
        return 0;
    }
}

