using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AIStandard : AI
{
    [SerializeField]
    List<Tile> prioritizedTiles;
    [SerializeField]
    Tile cruiserHeatmapTile;
    [SerializeField]
    Tile subarineHeatmapTile;
    [SerializeField]
    Tile dockyard;

    //Given the attributes needed to make the values and move itself, returns whether the move required removing action points or not
    public override bool makeMove(Player currentPlayer, Player opposingPlayer,
        UnitGenerator unitGenerator, GameObject cards, int maxCardCount) 
    {
        List<Action> list = currentPlayer.possibleActions(
            maxCardCount, cards, opposingPlayer, unitGenerator).OrderBy(x => Guid.NewGuid()).ToList();

        this.quicksort(list);
        list[0].PlayMove();
        return list[0].requiresAction();
    }
    public override int determineDrawValue(Player p)
    {
        if (p.canDrawCard(3))
        {
            return 20;
        }
        else if (p.getCount(CardType.Move) < 2
            && p.getCount(CardType.Shoot) < 2)
        {
            return 15;
        }
        return 5;
    }


    public override int determineDiscardValue(CardType cardType, Player p)
    {
        if (p.canDrawCard(6))
        {
            return 0;
        }
        return 5;
    }


    public override int determineMovementValue(Tile currentTile, Tile nextTile, Unit unit, Player player) 
    {

        if (unit.sameType(UnitType.Airbase))
        {
            return 1;
        }
        if (unit.sameType(UnitType.Marine))
        {
            if (player.getCount(UnitType.Marine) + player.getCount(UnitType.Airbase) == 1 && !unit.canSurviveShot(3))
            {
                List<Tile> path = currentTile.findPath(dockyard);
                if (path.Count > 0 && path[0] == nextTile)
                {
                    return 25;
                }
            } 
            if (unit.canSurviveShot(2) && player.getCount(CardType.Split) > 0)
            {
                return 3;
            }

            foreach (Tile t in prioritizedTiles)
            {
                if (!unit.sameTeam(t))
                {
                    List<Tile> path = currentTile.findPath(t);
                    if (path[0] == nextTile)
                    {
                        return 20 - path.Count;
                    }
                }
            }
            return 8;
        }
        else
        {
            if (!unit.canSurviveShot(1) && unit.MAXHEALTH > 1)
            {
                List<Tile> path = currentTile.findPath(dockyard);
                if (path.Count > 0 && path[0] == nextTile)
                {
                    return 25;
                }
            }
            else 
            {
                if (unit.sameType(UnitType.Cruiser))
                {
                    List<Tile> path = currentTile.findPath(this.cruiserHeatmapTile);
                    if (path.Count > 0 && path[0] == nextTile && path.Count > 1)
                    {
                        return 15;
                    }
                }
                else if (unit.sameType(UnitType.Submarine))
                {
                    List<Tile> path = currentTile.findPath(this.subarineHeatmapTile);
                    if (path.Count > 0 && path[0] == nextTile && path.Count > 2)
                    {
                        return 12;
                    }
                }
            }
        }

        return 4;
    }
    public override int determineShootValue(Unit shooter, Unit target) { return 25; }

    public override int determineSplitValue(Unit unit, Tile currentTile, Tile nextTile, Player p, int splitHealth) 
    { 
        if (!unit.canSurviveShot(2))
        {
            return 2;
        }
        else if (unit.sameType(UnitType.Marine) && splitHealth == 2)
        {
            return 15;
        }
        else
        {
            return 5;
        }
    }

}
