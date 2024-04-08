using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AIStandard : AI
{
    [SerializeField]
    List<Tile> prioritizedTiles;

    //Given the attributes needed to make the values and move itself, returns whether the move required removing action points or not
    public override bool makeMove(Player currentPlayer, Player opposingPlayer,
        UnitGenerator unitGenerator, GameObject cards, int maxCardCount) 
    {
        List<Action> list = currentPlayer.possibleActions(
            maxCardCount, cards, opposingPlayer, unitGenerator).OrderBy(x => Guid.NewGuid()).ToList();

        this.quicksort(list, prioritizedTiles);
        list[0].PlayMove();
        return list[0].requiresAction();
    }
}
