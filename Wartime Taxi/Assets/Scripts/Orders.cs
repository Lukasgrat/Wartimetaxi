using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

interface Orders
{
    bool isValid(Unit unit);
    void playCard(Unit unit);

    int value(Unit unit);
}

class Move : Orders {
    Tile currentTile;
    Tile nextTile;
    public Move(Tile currentTile, Tile nextTile) 
    {
        this.currentTile = currentTile;
        this.nextTile = nextTile;
    }
    public bool isValid(Unit unit) 
    {
        return this.currentTile.canMove(unit, this.nextTile);
    }
    public void playCard(Unit unit) 
    {
        this.currentTile.removeUnit(unit);
        this.nextTile.addUnit(unit);
    }

    public int value(Unit unit)
    {
        return 0;
    }
}