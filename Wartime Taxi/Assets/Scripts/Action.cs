using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Action 
{
    void PlayMove();

    int value(List<Tile> prioritizedTiles);

    //returns if this action required an action point
    bool requiresAction();
}

class OrderAction : Action
{
    Order order;
    Player p;
    public OrderAction(Order order, Player p) 
    {
        this.order = order;
        this.p = p;
    }
    public void PlayMove() 
    {
        this.order.playCard();
        this.order.destroyCard(this.p);
    }

    public int value(List<Tile> prioritizedTiles) 
    {
        return order.value(prioritizedTiles);
    }

    public bool requiresAction() 
    {
        return true;
    }
}

class Discard : Action
{
    CardType cardType;
    Player p;
    public Discard(CardType cardType,Player p) 
    {
        this.cardType = cardType;
        this.p = p;
    }

    public void PlayMove()
    {
        this.p.removeCard(this.cardType);
    }

    public int value(List<Tile> prioritizedTiles) 
    {
        if (this.p.canDrawCard(6)) 
        {
            return 0;
        }
        return 5;
    }

    public bool requiresAction()
    {
        return false;
    }
}

class DrawCard : Action 
{
    Player p;
    GameObject parent;
    public DrawCard(Player p, GameObject parent)
    {
        this.p = p;
        this.parent = parent;
    }

    public void PlayMove() 
    {
        this.p.drawCard(this.parent);
    }

    public int value(List<Tile> prioritizedTiles) 
    {
        if (this.p.canDrawCard(3))
        {
            return 20;
        }
        else if (this.p.getCount(CardType.Move) < 2 
            && this.p.getCount(CardType.Shoot) < 2) 
        {
            return 15;
        }
        return 5;
    }

    public bool requiresAction()
    {
        return true;
    }
}