using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Action 
{
    void PlayMove();

    int value();
}

class OrderAction : Action
{
    Order order;
    Player p;
    public OrderAction(Order order, Player p) 
    {
        this.order = order;
    }
    public void PlayMove() 
    {
        this.order.playCard();
    }

    public int value() 
    {
        return order.value();
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

    public int value() 
    {
        if (this.p.canDrawCard(6)) 
        {
            return 0;
        }
        return 5;
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

    public int value() 
    {
        if (this.p.canDrawCard(3))
        {
            return 15;
        }
        return 5;
    }
}