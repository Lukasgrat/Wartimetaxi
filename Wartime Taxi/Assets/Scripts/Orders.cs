using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface Order
{
    bool isValid();
    void playCard();

    int value(AI ai);

    CardType getType();

    void destroyCard(Player p);
}

class Move : Order {
    Tile currentTile;
    Tile nextTile;
    Unit unit;
    Player player;
    public Move(Tile currentTile, Tile nextTile, Unit unit, Player player) 
    {
        this.currentTile = currentTile;
        this.nextTile = nextTile;
        this.unit = unit; 
        this.player = player;
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

    public CardType getType() 
    {
        return CardType.Move;
    }

    public int value(AI ai)
    {
        return ai.determineMovementValue(this.currentTile, this.nextTile, this.unit, this.player);
    }


    public void destroyCard(Player p) 
    {
        p.removeCard(this.getType());
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

    public int value(AI ai)
    {
        return ai.determineShootValue(shooter, target);
    }


    public CardType getType()
    {
        return CardType.Shoot;
    }

    public void destroyCard(Player p)
    {
        p.removeCard(this.getType());
    }
}

class Split : Order 
{
    Unit unit;
    Tile currentTile;
    Tile nextTile;
    int splitHealth;
    Player p;
    bool isFake;
    UnitGenerator UG;
    bool shouldSwapPlaces;
    public Split(Unit unit, Tile currentTile, Tile nextTile, Player p, int splitHealth, UnitGenerator UG) 
    { 
        this.unit = unit;
        this.nextTile = nextTile;
        this.p = p;
        this.currentTile = currentTile;
        this.splitHealth = splitHealth;
        this.UG = UG;
        this.isFake = false;
    }

    public Split(Unit unit, Tile currentTile, Tile nextTile, Player p, UnitGenerator UG, bool shouldSwapPlaces) 
    {
        this.unit = unit;
        this.currentTile = currentTile;
        this.nextTile = nextTile;
        this.p = p;
        this.UG = UG;
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

        this.p.consolidate();
    }

    void splitReal()
    {
        Unit newUnit = this.UG.createUnit(this.unit);
        this.unit.split(this.nextTile, splitHealth, newUnit);
        p.addUnit(newUnit);

    }

    void splitFake()
    {
        FakeUnit newUnit = this.UG.createFakeUnit(this.unit);
        this.unit.fakeSplit(this.nextTile, newUnit, this.shouldSwapPlaces);
        p.addUnit(newUnit);
        this.unit.addFake(newUnit);
    }


    public bool isValid() 
    {
        return unit.healthToSpare() >= this.splitHealth && this.currentTile.canMove(this.unit, this.nextTile);
    }

    public int value(AI ai) 
    {
        return ai.determineSplitValue(unit, this.currentTile, this.nextTile, this.p, this.splitHealth);
    }


    public CardType getType()
    {
        return CardType.Split;
    }

    public void destroyCard(Player p)
    {
        p.removeCard(this.getType());
    }
}

