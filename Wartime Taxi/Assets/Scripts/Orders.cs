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
        if (!target.survivedShot()) 
        {
           targetedPlayer.destroyUnit(this.target);
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
    public Split(Unit u, Tile currentTile, Tile nextTile, Player p, int splitHealth, bool isFake, Unit templateUnit) 
    { 
        this.u = u;
        this.nextTile = nextTile;
        this.p = p;
        this.currentTile = currentTile;
        this.splitHealth = splitHealth;
        this.isFake = isFake;
        this.templateUnit = templateUnit;
    }

    public void playCard() 
    {
        Unit newUnit = this.templateUnit;
        this.u.split(this.nextTile, splitHealth, this.isFake, newUnit);
        GameObject unitsParent = GameObject.Find("Units");
        Unit finishedUnit = Object.Instantiate(newUnit,
           Vector3.zero,
            new Quaternion(), unitsParent.transform);
        p.addUnit(finishedUnit);
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

