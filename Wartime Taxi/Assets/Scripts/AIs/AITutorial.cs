using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AITutorial : AI
{
    [SerializeField]
    TutorialText tt;
    int counter = 0;
    [SerializeField]
    Unit destroyer;
    [SerializeField]
    Tile tile1;
    [SerializeField]
    Tile tile2;
    [SerializeField]
    Tile tile3;
    public override void makeMove(Player currentPlayer, Player opposingPlayer,
        FakeUnit fakeUnit, Unit unit, GameObject cards, int maxCardCount) 
    {
        if (this.tt.currentPhase() < 1) { return; }
        else if (this.tt.currentPhase() == 11 && !this.destroyer.onLocation(this.tile2))
        {
            if (counter == 0)
            {
                Action act1 = new DrawCard(currentPlayer, cards);
                act1.PlayMove();
            }
            else if (counter == 1)
            {
                Action act2 = new OrderAction(new Move(this.tile1, this.tile2, this.destroyer), currentPlayer);
                act2.PlayMove();
            }
            counter += 1;
            return;
        }
        else if (this.tt.currentPhase() == 13 && !this.destroyer.onLocation(this.tile1))
        {
            if (counter == 2)
            {
                Action act2 = new OrderAction(new Move(this.tile2, this.tile1, this.destroyer), currentPlayer);
                act2.PlayMove();
            }
            else if (counter == 3)
            {
                Action act1 = new DrawCard(currentPlayer, cards);
                act1.PlayMove();
            }
           
            counter += 1;
            return;
        }
        else 
        {
            return;
        }
        List<Action> list = currentPlayer.possibleActions(
            maxCardCount, cards, opposingPlayer, fakeUnit, unit).OrderBy(x => Guid.NewGuid()).ToList();

        this.quicksort(list);
        list[0].PlayMove();
    }
}
