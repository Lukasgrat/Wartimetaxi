using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class Tile : MonoBehaviour
{
    [SerializeField]
    List<Tile> adjacentTiles = new List<Tile>();
    [SerializeField]
    List<Vector3> unitPositions = new List<Vector3>();
    [SerializeField]
    List<Unit> units = new List<Unit>();
    public bool isLand;
    public bool isAirBase;
    [SerializeField]
    bool isDock;
    GameObject litTile;
    Team team = Team.Neutral;
    bool isLit = false;
    [SerializeField]
    AirFieldGenerator airFieldGenerator;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i += 1) {
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


    //Returns whether this tile has the same team as the given team
    public bool sameTeam(Team team) 
    { 
        return this.team == team;
    }

    //Returns whether this tile is lit or not
    public bool isLighten()
    {
        return this.isLit;
    }

    //EFFECT: given whether it should light the tile or not, does so
    public void lightTile(bool shouldLight)
    {
        this.litTile.SetActive(shouldLight);
        this.isLit = shouldLight;
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
    public void addUnit(Unit unit)
    {
        if (this.units.Count + 1 > this.unitPositions.Count)
        {
            throw new Exception("Error, tile" + this.name + " does not have enough positions to hold"
                + (units.Count + 1).ToString() + "units.");
        }
        unit.gameObject.transform.position = (this.unitPositions[this.units.Count]) + this.transform.position;
        this.units.Add(unit);

        unit.moveTo(this);
    }

    //Returns the type of the units on the tile. If there is more than one
    //or no units on the tile, returns a UnitType.None
    public UnitType unitType() 
    {
        if (this.units.Count != 1) 
        {
            return UnitType.None;
        }
        return this.units[0].type;
    }
    //Gets the first unit off of the tile

    //Given a team, attempts to change the team of the tile
    //note, due to how the game is played, the teams cannot swap immediantly from
    //Green to red and vice versa, so it will throw an error if that occurs
    public void changeTeam(Team team) 
    {
        if (this.opposingTeam(team)) 
        {
            throw new Exception("Error: Invalid swapping of teams on tile " + this.gameObject.name);
        }
        this.team = team;
        if (this.isAirBase)
        {
            swapAirfield();
        }
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
                this.units[x].gameObject.transform.position = this.unitPositions[x] + this.transform.position;
            }
        };
        if (this.units.Count == 0)
        {
            this.team = Team.Neutral;
            if (this.isAirBase) 
            {
                swapAirfield();
            }
        }

    }
    //EFFECT:Lights all tiles that the given unit can move to, assuming they are on this tile
    public void lightMoveable(Unit unit)
    {
        
        foreach (Tile t in this.adjacentTiles)
        {
            if (!unit.opposingTeam(t.team) && t.canMove(unit, this))
            {
                t.lightTile(true);
            }
        }
    }

    //EFFECT:Lights all tiles that the given unit can shoot at
    public void lightShootable(Unit unit)
    {
        foreach (Tile t in this.adjacentTiles) 
        {
            if (unit.opposingTeam(t.team) && t.isVunerable(unit)) 
            {
                t.lightTile(true);
            }
            if (unit.sameType(UnitType.Airbase)) 
            {
                foreach (Tile t2 in t.adjacentTiles)
                {
                    if (unit.opposingTeam(t2.team) && t2.isVunerable(unit))
                    {
                        t2.lightTile(true);
                    }
                }
            }
        }
    }

    //Returns if this can be shot by this unit
    public bool isVunerable(Unit unit) 
    { 
        bool returnBool = false;
        foreach (Unit u in this.units) 
        { 
            returnBool = returnBool || unit.canShoot(u);
        }
        return returnBool;
    }

    //Returns whether the given unit can move onto this tile from the given previous tile
    public bool canMove(Unit unit, Tile prevTile)
    {
        if (this.isLand && !unit.canMoveToLand())
        {
            return false;
        }
        if (unit.opposingTeam(this.team)) 
        {
            return false;
        }
        return this.adjacentTiles.Contains(prevTile);
    }

    //EFFECT: Given a Selectionhandler, activites all buttons of the units that are on this tile
    public void activateUnits(SelectionHandler handler) 
    {
        handler.enableButtons(this.units);
    }

    //Returns all moveable units from this tile
    public List<Unit> moveableUnits() 
    { 
        List<Unit> moveableUnits = new List<Unit>();
        foreach (Unit u in this.units) 
        {
            if (u.canMoveOffOf(this, this.adjacentTiles)) 
            { 
                moveableUnits.Add(u);
            }

        }
        return moveableUnits;
    }

    //Returns all splitable units from this tile
    public List<Unit> splitableUnits()
    {
        List<Unit> splitableUnits = new List<Unit>();
        foreach (Unit u in this.units)
        {
            if (u.canMoveOffOf(this, this.adjacentTiles) 
                && u.canSurviveShot(1))
            {
                splitableUnits.Add(u);
            }

        }
        return splitableUnits;
    }


    //EFFECT: Given a Selection handler, highlights all unit buttons that can be used
    public void activateMoveableUnits(SelectionHandler handler) 
    { 
        handler.enableButtons(this.moveableUnits());
        
    }

    //EFFECT: Given a Selection handler, highlights all unit buttons that can be used
    public void activateSplitableUnits(SelectionHandler handler)
    {
        handler.enableButtons(this.splitableUnits());

    }

    //Returns all units that can shoot form this tile
    public List<Unit> shootableUnits() 
    {
        List<Unit> shootableUnits = new List<Unit>();
        foreach (Unit u in this.units)
        {
            foreach (Tile t in this.adjacentTiles)
            { 
                if (u.opposingTeam(t.team) && t.isVunerable(u) && !shootableUnits.Contains(u)) 
                {
                    shootableUnits.Add(u);
                }
                if (u.sameType(UnitType.Airbase)) 
                {
                    foreach (Tile t2 in t.adjacentTiles) 
                    {
                        if (u.opposingTeam(t2.team) && t2.isVunerable(u) && !shootableUnits.Contains(u))
                        {
                            shootableUnits.Add(u);
                        }
                    }
                }
            }
        }
        return shootableUnits;
    }

    //EFFECT: Given a Selection handler, highlights all unit buttons that can be used
    public void activateShootableUnits(SelectionHandler handler)
    {
        handler.enableButtons(this.shootableUnits());

    }

    //EFFECT: Given a unit, returns if this unit can move to any other tile adjacent to this one
    public bool canMoveOffOf(Unit u) 
    {
        foreach (Tile t in this.adjacentTiles)
        {
            if (t.canMove(u, this))
            {
                return true;
            }
        }
        return false;
    }

    //EFFECT: Given a Selectionhandler and the firing unit,
    //activiates all buttons of the vunerable units that are on this tile

    public void activateVunerableUnits(SelectionHandler handler, Unit attacker) 
    { 
        List<Unit> vunerableUnits = new List<Unit>();
        foreach (Unit u in this.units) 
        {
            if (attacker.canShoot(u)) 
            { 
                vunerableUnits.Add(u);
            }
        }
        handler.enableButtons(vunerableUnits);
    }


    //Returns the unit of the given type on this tile
    //Throws an error if not found
    public Unit returnUnit(UnitType ut) 
    { 
        foreach (Unit unit in this.units) 
        {
            if (unit.sameType(ut)) 
            { 
                return unit;
            }
        }
        throw new Exception("Error: Unit not found in tile");
    }

    //Swaps the airfield on this tile to the current team
    void swapAirfield()
    {
        GameObject airfield = null;
        for (int i = 0; i < transform.childCount; i += 1)
        {
            if (transform.GetChild(i).name.Equals("Airfield"))
            {
                List<String> airfieldNames = new List<String>();
                airfieldNames.Add("AirField-Green");
                airfieldNames.Add("AirField-Red");
                airfieldNames.Add("AirField-Neutral");
                airfield = transform.GetChild(i).gameObject;
                for (int index = 0; index < airfield.transform.childCount; index += 1) 
                {
                    if (airfieldNames.Contains(airfield.transform.GetChild(index).name)) 
                    {
                        Destroy(airfield.transform.GetChild(index).gameObject);
                    }
                }
            }
        }
        if (airfield != null)
        {
            this.airFieldGenerator.generateAirField(this.team, airfield);
            return;
        }
        else
        {
            throw new Exception("Airfield Missing on tile:" + this.name);
        }
    }


    //Returns whether this tile is within n tiles of the given tile
    public bool inRange(int n, Tile t) 
    {
        if (n < 0) 
        {
            throw new Exception("Negative range given to tile");
        }
        else if (n == 0)
        {
            return t == this;
        }
        else if (n == 1)
        {
            return this.adjacentTiles.Contains(t);
        }
        bool returnBool = false;
        foreach (Tile tile in this.adjacentTiles) 
        {
            returnBool = returnBool || tile.inRange(n - 1, t);
        }
        return returnBool;
    }

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

    public bool canRecover() 
    {
        if (!this.isDock) 
        {
            return false;
        }
        foreach (Tile t in adjacentTiles) 
        {
            if (t.opposingTeam(this.team)) 
            {
                return false;
            }
        }
        return true;
    }


    //Returns the list of tiles that the given unit can move from, assuming they are on this tile
    public List<Tile> movementOptions(Unit u) 
    {
        List<Tile> returnTiles = new List<Tile> ();
        foreach (Tile t in this.adjacentTiles) 
        {
            if (t.canMove(u, this))
            {
                returnTiles.Add(t);
            }
        }
        return returnTiles;
    }
}
