using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    List<Unit> units;
    Team team;
    List<Card> deck;

    Player(Team team, List<Unit> units, List<Card> deck)
    {
        this.units = units;
        this.team = team;
        this.deck = deck;
    }
    Player(Team team) : this(team, new List<Unit>(), new List<Card>()) { }
}
