using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField]
    List<Tile> adjacentTiles = new List<Tile>();
    [SerializeField]
    List<Vector3> unitPositions = new List<Vector3>();
    List<Unit> units = new List<Unit>();
    public bool isLand;
    GameObject litTile;
    Team team = Team.Neutral;
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i += 1) {
            if (transform.GetChild(i).name.Equals("Light"))
            { 
                litTile = transform.GetChild(i).gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    //EFFECT: given whether it should light the tile or not, does so
    public void lightTile(bool shouldLight)
    {
        this.litTile.SetActive(shouldLight);
    }

    //EFFECT: Lights all adjacent tiles to this tile
    public void lightAdjacent(bool shouldLight) 
    {
        foreach (Tile tile in adjacentTiles) 
        { 
            tile.lightTile(shouldLight);
        }
    }
    //EFFECT: Adds the given unit to one of the locations
    public void addUnit(Unit unit) {
        if (this.units.Count + 1 > this.unitPositions.Count) 
        {
            throw new Exception("Error, tile" + this.name +" does not have enough positions to hold" 
                + (units.Count + 1).ToString() + "units." );
        }
        unit.gameObject.transform.position = (this.unitPositions[this.units.Count])+ this.transform.position;
        this.units.Add(unit);
    
    }


    //EFFECT: Removes the given unit(with exact reference) from this Tile, shifting all other units down by one
    public void removeUnit(Unit unit)
    {
        if (this.units.Contains(unit))
        {
            this.units.Remove(unit);
            unit.moveTo(null);
            for (int x = 0; x < this.units.Count; x += 1)
            {
                this.units[x].gameObject.transform.position = this.unitPositions[x];
            }
        }

    }

    //Returns whether the given unit can move onto this tile
    public bool canMove(Unit unit, Tile nextTile)
    {
        if (nextTile.isLand && !unit.canMoveToLand()) 
        {
            return false;
        }
        return this.adjacentTiles.Contains(nextTile);
    }

    //Handles selecting this tile
    public void select() {
    }
}
