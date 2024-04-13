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
    public override bool makeMove(Player currentPlayer, Player opposingPlayer,
        UnitGenerator unitGenerator, GameObject cards, int maxCardCount) 
    {
        if (this.tt.currentPhase() < 1) { return true; }
        else if (this.tt.currentPhase() == 11 && !this.destroyer.onLocation(this.tile2))
        {
            if (counter == 0)
            {
                Action act1 = new DrawCard(currentPlayer, cards);
                act1.PlayMove();
            }
            else if (counter == 1)
            {
                Action act2 = new OrderAction(new Move(this.tile1, this.tile2, this.destroyer, currentPlayer), currentPlayer);
                act2.PlayMove();
            }
            counter += 1;
        }
        else if (this.tt.currentPhase() == 13 && !this.destroyer.onLocation(this.tile1))
        {
            if (counter == 2)
            {
                Action act2 = new OrderAction(new Move(this.tile2, this.tile1, this.destroyer, currentPlayer), currentPlayer);
                act2.PlayMove();
            }
            else if (counter == 3)
            {
                Action act1 = new DrawCard(currentPlayer, cards);
                act1.PlayMove();
            }
           
            counter += 1;
        }
        return true;
    }

    public override int determineDrawValue(Player p) { return 0; }
    public override int determineDiscardValue(CardType cardType, Player p) { return 0; }
    public override int determineMovementValue(Tile currentTile, Tile nextTile, Unit unit, Player player) { return 0; }
    public override int determineShootValue(Unit shooter, Unit target) { return 0; }
    public override int determineSplitValue(Unit unit, Tile currentTile, Tile nextTile, Player p, int splitHealth) { return 0; }
}
