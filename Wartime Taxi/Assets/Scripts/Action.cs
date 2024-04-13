using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Action 
{
    void PlayMove();

    int value(AI ai);

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

    public int value(AI ai) 
    {
        return order.value(ai);
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

    public int value(AI ai) 
    {
        return ai.determineDiscardValue(cardType, p);
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

    public int value(AI ai) 
    {
        return ai.determineDrawValue(this.p);
    }

    public bool requiresAction()
    {
        return true;
    }
}