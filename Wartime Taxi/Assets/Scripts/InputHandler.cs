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

public class InputHandler : MonoBehaviour
{
    internal Camera cam;
    internal int turnNumber = 0;
    internal Team currentPlayerTeam = Team.Green;
    internal int currentPlayerIndex = 0;
    [SerializeField]
    internal GameObject tileHolder;
    internal List<Tile> tiles;
    internal List<Player> playerList;
    internal int MAXCARDCOUNT = 6;
    internal int startingCardCount = 5;
    [SerializeField]
    internal Button drawCardButton;
    internal Vector3 STARTINGDRAW;
    [SerializeField]
    internal GameObject cards;
    [SerializeField]
    internal CardGenerator cardGenerator;
    [SerializeField]
    internal AI ai;
    [SerializeField]
    internal GameObject units;
    [SerializeField]
    internal SelectionHandler selectionHandler;
    [SerializeField]
    internal NumberSelector numberSelector;
    internal CardType selectedCard = CardType.None;



    internal Unit initiator;
    internal Tile initiatedTile;
    internal Tile splitMovingTile;
    internal Tile target;



    [SerializeField]
    internal GameObject unitInfo;
    [SerializeField]
    internal TextMeshProUGUI unitInfoText;


    internal int MAXACTIONS = 2;
    internal int actionsLeft = 2;
    [SerializeField]
    internal TextMeshProUGUI actionText;

    [SerializeField]
    internal NextTurnButton nextTurnButton;

    [SerializeField]
    internal FakeButton fakeButton;


    [SerializeField]
    internal WinningConditions winningConditions;
    [SerializeField]
    internal WinningScript winningScript;

    [SerializeField]
    UnitGenerator unitGenerator;

    internal bool AI = false;
    internal enum State
    {
        Idle,

        SelectedMove,
        SelectedTile1Move,
        SelectedUnit1Move,

        SelectedShoot,
        SelectedTile1Shoot,
        SelectedUnit1Shoot,
        SelectedTile2Shoot,

        SelectedSplit,
        SelectedTile1Split,
        SelectedUnit1Split,
        SelectedTile2Split,
        SelectingFakeSplit,
    }
    internal State currentState;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("AI", 0) == 1) 
        {
            this.AI = true;
        }
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
                    return;
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
                    madeActive = true;
                    break;
                }

            }
            this.unitInfo.SetActive(madeActive);
        }
    }
    internal void drawCard()
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
    internal void clearLights()
    {
        foreach (Tile tile in tiles)
        {
            tile.lightTile(false);
        }
    }

    internal void SelectionHandler(Tile t)
    {
        if (!t.isLighten())
        {
            resetState();
        }
        else if (this.currentState == State.SelectedMove)
        {

            this.currentState = State.SelectedTile1Move;
            t.activateMoveableUnits(this.selectionHandler);
            this.initiatedTile = t;
        }
        else if (this.currentState == State.SelectedUnit1Move)
        {
            Order order = new Move(this.initiatedTile, t, this.initiator, this.currentPlayer());
            this.playOrder(order);
            this.currentPlayer().consolidate();
        }
        else if (this.currentState == State.SelectedShoot)
        {
            this.currentState = State.SelectedTile1Shoot;
            t.activateShootableUnits(this.selectionHandler);
            this.initiatedTile = t;
        }
        else if (this.currentState == State.SelectedUnit1Shoot)
        {
            this.target = t;
            this.currentState = State.SelectedTile2Shoot;
            t.activateVunerableUnits(this.selectionHandler, initiator);
        }
        else if (this.currentState == State.SelectedSplit)
        {
            this.currentState = State.SelectedTile1Split;
            t.activateSplitableUnits(this.selectionHandler);
            this.initiatedTile = t;
        }
        else if (this.currentState == State.SelectedUnit1Split)
        {
            this.currentState = State.SelectedTile2Split;
            this.numberSelector.enableButtons(this.initiator.healthToSpare());
            this.splitMovingTile = t;
            if (this.initiator.sameType(UnitType.Submarine) &&
                !this.currentPlayer().containsFake())
            {
                this.numberSelected(0);
            }
        }
        else
        {
            resetState();
        }
        if ((this.currentState == State.SelectedTile1Move
            || this.currentState == State.SelectedTile1Shoot
            || this.currentState == State.SelectedTile1Split)
            && t.unitType() != UnitType.None)
        {
            this.unitSelected(t.unitType());
        }
        else
        {
            this.clearLights();
        }
    }

    //EFFECT plays out the given order on this game
    internal void playOrder(Order order)
    {
        this.changeActions(this.actionsLeft - 1);
        order.playCard();
        this.removeCard(this.selectedCard);
        this.resetState();
    }

    //Discards the given card
    public virtual void discardCard(CardType cardType)
    {
        this.resetState();
        this.removeCard(cardType);
    }

    //Removes the card of the given type from the current player's hand
    public void removeCard(CardType cardType)
    {
        this.currentPlayer().removeCard(cardType);
    }

    //Removes the card of the given type from the given team player's hand
    public void removeCard(CardType cardType, Team team)
    {

        this.playerList[this.getPlayerIndex(team)].removeCard(cardType);
    }

    //Returns the index of the player given the Team, returnning negative 1 if none are found
    internal int getPlayerIndex(Team team) 
    {
        for (int x = 0; x < this.playerList.Count; x += 1) 
        {
            if (this.playerList[x].sameTeam(team)) 
            {
                return x;
            }
        }
        return -1;
    }

    //EFFECT: Resets the built up state from a move
    public void resetState()
    {
        this.currentState = State.Idle;
        this.selectedCard = CardType.None;
        this.fakeButton.gameObject.SetActive(false);
        this.clearLights();
        this.selectionHandler.disableButtons();
        this.numberSelector.thoroughDisable();
    }
    
    //Given a number, readies to split off that many units when the next tile is selected
    public void numberSelected(int num)
    {
        if (this.currentState == State.SelectedTile2Split)
        {
            if (this.initiator.sameType(UnitType.Submarine) && !this.currentPlayer().containsFake())
            {
                this.fakeButton.gameObject.SetActive(true);
                this.currentState = State.SelectingFakeSplit;
            }
            else 
            {
                this.createRealUnit(num);
            }
        }
        else 
        {
            throw new Exception("Error: invalid state of number selection reached");
        }
    }

    internal void createRealUnit(int health)
    {
        Order order = new Split(this.initiator, this.initiatedTile,
            this.splitMovingTile, this.currentPlayer(), health, unitGenerator);
        this.playOrder(order);
        this.currentPlayer().consolidate();

    }

    //returns the current player
    internal Player currentPlayer() 
    {
        return this.playerList[this.currentPlayerIndex];
    }


    //Activated upon a signal from an outside movement source,
    // once it is, the fake will be created and the real one moving
    // as neccessary
    public void selectedFake(bool isMovingFake) 
    {
        if (this.currentState == State.SelectingFakeSplit)
        {
            Order order = new Split(this.initiator, this.initiatedTile,
                this.splitMovingTile, this.currentPlayer(), unitGenerator, !isMovingFake);
            this.playOrder(order);
            this.currentPlayer().consolidate();
        }
        else
        {
            throw new Exception("Error: invalid state  for fake selection reached");
        }
    }

    //Given a unit, finds that unit to be used for later operations and advance the state with that unit added
    public void unitSelected(UnitType ut) 
    {
        if (this.currentState == State.SelectedTile1Move)
        {
            this.initiator = this.initiatedTile.returnUnit(ut);
            this.clearLights();
            this.initiatedTile.lightMoveable(this.initiator);
            this.selectionHandler.disableButtons();
            this.currentState = State.SelectedUnit1Move;

        }
        else if (this.currentState == State.SelectedTile1Shoot)
        {
            this.initiator = this.initiatedTile.returnUnit(ut);
            this.clearLights();
            this.initiatedTile.lightShootable(this.initiator);
            this.selectionHandler.disableButtons();
            this.currentState = State.SelectedUnit1Shoot;
        }
        else if(this.currentState == State.SelectedTile2Shoot)
        {
            Order order = new Shoot(this.initiator, this.target.returnUnit(ut), this.opposingPlayer());
            this.playOrder(order);
        }
        else if (this.currentState == State.SelectedTile1Split)
        {
            this.initiator = this.initiatedTile.returnUnit(ut);
            this.clearLights();
            this.initiatedTile.lightMoveable(this.initiator);
            this.selectionHandler.disableButtons();
            this.currentState = State.SelectedUnit1Split;
        }
    }

    internal void nextTurn()
    {
        this.playerList[currentPlayerIndex].showCards(false);
        switch (this.currentPlayerTeam) 
        { 
            case Team.Green:
                this.currentPlayerTeam = Team.Red;
                this.currentPlayerIndex += 1;
                nextTurnButton.appear(this.currentPlayerTeam, this.boardState() * -1);
                break;
            case Team.Red:
                this.currentPlayerTeam = Team.Green;
                turnNumber += 1;
                this.winningConditions.updateStandings(this.playerList);
                this.currentPlayerIndex = 0;
                nextTurnButton.appear(this.currentPlayerTeam, this.boardState());
                break;
            default:
                throw new Exception("Error, invalid state for current team reached");
        }
        int possibleWinner = this.winningConditions.hasMet(this.playerList);
        if (possibleWinner != -1) 
        {
            this.winningScript.declareWinner(this.playerList[possibleWinner]);
        }
        this.playerList[currentPlayerIndex].repairAll();
        this.playerList[currentPlayerIndex].showCards(true);
        this.changeActions(this.MAXACTIONS);

        if (this.currentPlayer().unitCount() == 0)
        {
            this.nextTurn();
        }
        if(this.currentPlayerTeam == Team.Red && this.AI)
        {
            nextTurnButton.gameObject.SetActive(false);
            this.AIRun();
        }
    }

    //returns the player whose turn its not
    public Player opposingPlayer() {
        if (currentPlayerIndex == 0) {
            return this.playerList[1];
        }
        return this.playerList[0];
    }

    public virtual void startMove()
    {
        this.resetState();
        if (this.actionsLeft != 0) { 
            this.selectedCard = CardType.Move;
            List<Tile> hightLightList = this.playerList[currentPlayerIndex].canMoveFrom();
            foreach (Tile t in hightLightList)
            {
                t.lightTile(true);
            }
            this.currentState = State.SelectedMove;
        }
    }
    public virtual void startShoot()
    {
        this.resetState();
        if (this.actionsLeft != 0) { 
            this.selectedCard = CardType.Shoot;
            this.currentState = State.SelectedShoot;
            List<Tile> hightLightList = this.opposingPlayer().canBeShotBy(this.playerList[currentPlayerIndex]);
            foreach (Tile t in hightLightList) 
            {
                t.lightTile(true);
            }
        }
    }

    public virtual void startSplit()
    { 
        this.resetState();
        if (this.actionsLeft != 0) 
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

    //EFFECT changes the actions to the given amount and displays as such
    public void changeActions(int newAction) 
    {
        this.actionsLeft = newAction;
        this.actionText.text = "Actions Lefts: " + this.actionsLeft;
    }

    //Returns the current value of the board, with positives favoring the first player and negatives the second
    public int boardState() 
    {
        return 0;
    }



    //Autoruns the current player's turn through AI logic
    internal void AIRun()
    {
        this.resetState();
        while (actionsLeft != 0)
        {
            if (this.ai.makeMove(this.currentPlayer(), this.opposingPlayer(), unitGenerator, this.cards, this.MAXCARDCOUNT))
            { 
                this.changeActions(this.actionsLeft - 1);
            }
        }
        this.nextTurn();
    }
}
