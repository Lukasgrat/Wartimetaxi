using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputHandlerTutorial : InputHandler
{
    [SerializeField]
    TutorialText tutorialText;
    // Start is called before the first frame update
    void Start()
    {
        this.AI = true;
        this.tiles = new List<Tile>();
        for (int x = 0; x < tileHolder.transform.childCount; x += 1)
        {
            if (tileHolder.transform.GetChild(x).TryGetComponent(out Tile t))
            {
                this.tiles.Add(t);
            }
        }
        this.cam = Camera.main;
        playerList = new List<Player>();
        STARTINGDRAW = cards.transform.position;
        playerList.Add(new Player(Team.Green, STARTINGDRAW, cardGenerator));
        for (int i = 0; i < this.startingCardCount; i++)
        {
            this.playerList[0].drawCard(this.cards);
        }
        playerList.Add(new Player(Team.Red, STARTINGDRAW, cardGenerator));
        for (int i = 0; i < this.startingCardCount; i++)
        {
            this.playerList[1].drawCard(this.cards);
        }
        this.playerList[1].showCards(false);
        for (int x = 0; x < units.transform.childCount; x++)
        {
            if (units.transform.GetChild(x).TryGetComponent(out Unit u))
            {
                if (u.sameTeam(Team.Green))
                {
                    this.playerList[0].addUnit(u);
                }
                else if (u.sameTeam(Team.Red))
                {
                    this.playerList[1].addUnit(u);
                }
            }
        }
        this.drawCardButton.onClick.AddListener(drawCard);
    }

    // Update is called once per frame
    void Update()
    {
        if (actionsLeft == 0) { this.nextTurn(); }
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            bool hasLightenTile = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.TryGetComponent(out Tile t) && t.isLighten())
                {
                    hasLightenTile = true;
                    this.SelectionHandler(t);
                }
            }
            if (!hasLightenTile)
            {
                this.resetState();
            }
        }
        else
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            bool madeActive = false;
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.TryGetComponent<Unit>(out Unit u))
                {
                    this.unitInfoText.text = u.displayString(this.currentPlayerTeam);
                    if (u.sameTeam(Team.Green))
                    {
                        this.unitInfoText.color = Color.green;
                    }
                    else if (u.sameTeam(Team.Red))
                    {
                        this.unitInfoText.color = Color.red;
                    }
                    else
                    {
                        this.unitInfoText.color = Color.white;
                    }
                    this.tutorialText.hovered();
                    madeActive = true;
                    break;
                }

            }
            this.unitInfo.SetActive(madeActive);
        }
        int possibleWinner = this.winningConditions.hasMet(this.playerList);
        if (possibleWinner != -1)
        {
            this.winningScript.declareWinner(this.playerList[possibleWinner]);
        }
    }
    internal new void drawCard()
    {
        if (this.actionsLeft != 0) {
            EventSystem.current.SetSelectedGameObject(null);
            Player player = this.currentPlayer();
            if (player.canDrawCard(this.MAXCARDCOUNT))
            {
                player.drawCard(this.cards);
                this.changeActions(this.actionsLeft - 1);
            }
        }
    }

    //Discards the given card
    public override void discardCard(CardType cardType)
    {
        if (this.tutorialText.currentPhase() > 5)
        {
            this.changeActions(this.actionsLeft - 1);
            this.resetState();
            this.removeCard(cardType);
        }
        else if (this.tutorialText.currentPhase() == 5 && cardType == CardType.Move)
        {
            this.changeActions(this.actionsLeft - 1);
            this.resetState();
            this.removeCard(cardType);
        }
    }


    public override void startMove()
    {
        this.resetState();
        if (this.actionsLeft != 0 && this.tutorialText.currentPhase() > 5) { 
            this.selectedCard = CardType.Move;
            List<Tile> hightLightList = this.playerList[currentPlayerIndex].canMoveFrom();
            foreach (Tile t in hightLightList)
            {
                t.lightTile(true);
            }
            this.currentState = State.SelectedMove;
        }
    }
    public override void startShoot()
    {
        this.resetState();
        if (this.actionsLeft != 0 && this.tutorialText.currentPhase() > 8) { 
            this.selectedCard = CardType.Shoot;
            this.currentState = State.SelectedShoot;
            List<Tile> hightLightList = this.opposingPlayer().canBeShotBy(this.playerList[currentPlayerIndex]);
            foreach (Tile t in hightLightList) 
            {
                t.lightTile(true);
            }
        }
    }

    public override void startSplit()
    { 
        this.resetState();
        if (this.actionsLeft != 0 && this.tutorialText.currentPhase() > 13) 
        {
            this.selectedCard = CardType.Split;
            this.currentState = State.SelectedSplit;
            List<Tile> hightLightList = this.playerList[currentPlayerIndex].canSplitFrom();
            foreach (Tile t in hightLightList)
            {
                t.lightTile(true);
            }
        }
    }
}
