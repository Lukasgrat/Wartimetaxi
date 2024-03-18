using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AIStandard : AI
{
    public override void makeMove(Player currentPlayer, Player opposingPlayer,
        FakeUnit fakeUnit, Unit unit, GameObject cards, int maxCardCount) 
    {
        List<Action> list = currentPlayer.possibleActions(
            maxCardCount, cards, opposingPlayer, fakeUnit, unit).OrderBy(x => Guid.NewGuid()).ToList();

        this.quicksort(list);
        list[0].PlayMove();
    }
}
