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
    Camera cam;
    int turnNumber = 0;
    Team currentPlayerTeam = Team.Green;
    int currentPlayerIndex = 0;
    [SerializeField]
    GameObject tileHolder;
    List<Tile> tiles;
    List<Player> playerList;
    int MAXCARDCOUNT = 6;
    int startingCardCount = 5;
    [SerializeField]
    Button drawCardButton;
    Vector3 STARTINGDRAW;
    [SerializeField]
    GameObject cards;
    [SerializeField]
    CardGenerator cardGenerator;
    [SerializeField]
    GameObject units;
    [SerializeField]
    SelectionHandler selectionHandler;
    [SerializeField]
    NumberSelector numberSelector;
    CardType selectedCard = CardType.None;



    Unit initiator;
    Tile initiatedTile;
    Tile splitMovingTile;
    Tile target;



    [SerializeField]
    GameObject unitInfo;
    [SerializeField]
    TextMeshProUGUI unitInfoText;


    int MAXACTIONS = 2;
    int actionsLeft = 2;
    [SerializeField]
    TextMeshProUGUI actionText;

    [SerializeField]
    NextTurnButton nextTurnButton;

    [SerializeField]
    Unit greenTemplateUnit;
    [SerializeField]
    Unit redTemplateUnit;

    [SerializeField]
    FakeUnit greenFakeTemplateUnit;
    [SerializeField]
    FakeUnit redFakeTemplateUnit;

    [SerializeField]
    FakeButton fakeButton;


    [SerializeField]
    WinningConditions winningConditions;
    [SerializeField]
    WinningScript winningScript;

    bool AI = false;
    enum State
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
    State currentState;
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
    void drawCard()
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
    void clearLights()
    {
        foreach (Tile tile in tiles)
        {
            tile.lightTile(false);
        }
    }

    void SelectionHandler(Tile t)
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
            Order order = new Move(this.initiatedTile, t, this.initiator);
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
            t.activateMoveableUnits(this.selectionHandler);
            this.initiatedTile = t;
        }
        else if (this.currentState == State.SelectedUnit1Split)
        {
            this.currentState = State.SelectedTile2Split;
            this.numberSelector.enableButtons(this.initiator.healthToSpare());
            this.splitMovingTile = t;
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
    void playOrder(Order order)
    {
        this.changeActions(this.actionsLeft - 1);
        order.playCard();
        this.removeCard(this.selectedCard);
        this.resetState();
    }

    //Discards the given card
    public void discardCard(CardType cardType) 
    {
        this.changeActions(this.actionsLeft - 1);
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
    int getPlayerIndex(Team team) 
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
    public void numberSelcted(int num)
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

    void createRealUnit(int health)
    {
        Unit templateUnit;
        if (this.currentPlayerTeam == Team.Green)
        {
            templateUnit = this.greenTemplateUnit;
        }
        else if (this.currentPlayerTeam == Team.Red)
        {
            templateUnit = this.redTemplateUnit;
        }
        else
        {
            throw new Exception("Team " + this.currentPlayerTeam + " has no given template for units");
        }
        Order order = new Split(this.initiator, this.initiatedTile,
            this.splitMovingTile, this.currentPlayer(), health, templateUnit);
        this.playOrder(order);
        this.currentPlayer().consolidate();

    }

    //returns the current player
    Player currentPlayer() 
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
            FakeUnit templateUnit;
            if (this.currentPlayerTeam == Team.Green)
            {
                templateUnit = this.greenFakeTemplateUnit;
            }
            else if (this.currentPlayerTeam == Team.Red)
            {
                templateUnit = this.redFakeTemplateUnit;
            }
            else
            {
                throw new Exception("Team " + this.currentPlayerTeam + " has no given template for units");
            }
            Order order = new Split(this.initiator, this.initiatedTile,
                this.splitMovingTile, this.currentPlayer(), templateUnit, !isMovingFake);
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

    void nextTurn()
    {
        this.playerList[currentPlayerIndex].showCards(false);
        switch (this.currentPlayerTeam) 
        { 
            case Team.Green:
                this.currentPlayerTeam = Team.Red;
                this.currentPlayerIndex += 1;
                nextTurnButton.appear(this.currentPlayerTeam, this.boardState() * -1);
                if (this.AI)
                {
                    nextTurnButton.gameObject.SetActive(false);
                    this.AIRun();
                }
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
            Debug.Log(this.playerList.Count);
            this.winningScript.declareWinner(this.playerList[possibleWinner]);
        }
        this.playerList[currentPlayerIndex].repairAll();
        this.playerList[currentPlayerIndex].showCards(true);
        this.changeActions(this.MAXACTIONS);

        if (this.currentPlayer().unitCount() == 0)
        {
            this.nextTurn();
        }
    }

    //returns the player whose turn its not
    public Player opposingPlayer() {
        if (currentPlayerIndex == 0) {
            return this.playerList[1];
        }
        return this.playerList[0];
    }

    public void startMove()
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
    public void startShoot()
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

    public void startSplit()
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
    void AIRun() 
    {
        this.resetState();
        FakeUnit currentFake;
        Unit currentUnit;
        if (this.currentPlayer().sameTeam(Team.Red))
        {
            currentFake = this.redFakeTemplateUnit;
            currentUnit = this.redTemplateUnit;
        }
        else if (this.currentPlayer().sameTeam(Team.Green))
        {
            currentFake = this.greenFakeTemplateUnit;
            currentUnit = this.greenTemplateUnit;
        }
        else 
        {
            throw new Exception("Team not accounted for in AI");
        }
        List<Action> list = this.currentPlayer().
            possibleActions(this.MAXCARDCOUNT, this.cards, this.opposingPlayer(), currentFake, currentUnit)
            .OrderBy(x => Guid.NewGuid()).ToList();

        this.quicksort(list);
        list[0].PlayMove();

        List<Action> list2 = this.currentPlayer().
           possibleActions(this.MAXCARDCOUNT, this.cards, this.opposingPlayer(), currentFake, currentUnit)
           .OrderBy(x => Guid.NewGuid()).ToList();

        this.quicksort(list2);
        list2[0].PlayMove();
        this.nextTurn();
    }

    //Swaps two elements in a list
    void swap<T>(List<T> x, int index1, int index2) 
    {
        T item = x[index1];
        x[index1] = x[index2];
        x[index2] = item;
    }
    // Returns the index where the pivot element ultimately ends up in the sorted source
    // EFFECT: Modifies the source list in the range [loIdx, hiIdx) such that
    //         all values to the left of the pivot are less than (or equal to) the pivot
    //         and all values to the right of the pivot are greater than it
    int partition(List<Action> source, int loIdx, int hiIdx, int pivot)
        {
            int curLo = loIdx;
            int curHi = hiIdx - 1;
            while (curLo < curHi)
            {
                // Advance curLo until we find a too-big value (or overshoot the end of the list)
                while (curLo < hiIdx && source[curLo].value() >=  pivot)
                {
                    curLo = curLo + 1;
                }
                // Advance curHi until we find a too-small value (or undershoot the start of the list)
                while (curHi >= loIdx && source[curHi].value() < pivot)
                {
                    curHi = curHi - 1;
                }
                if (curLo < curHi)
                {
                    swap(source, curLo, curHi);
                }
            }
            swap(source, loIdx, curHi); // place the pivot in the remaining spot
            return curHi;
        }
    // In ArrayUtils
    // EFFECT: Sorts the given ArrayList according to the given comparator
    void quicksort(List<Action> arr)
    {
        quicksortHelp(arr, 0, arr.Count);
    }

    // EFFECT: sorts the source array according to comp, in the range of indices [loIdx, hiIdx)
    void quicksortHelp(List<Action> source, int loIdx, int hiIdx)
    {
        if (loIdx >= hiIdx)
        {
            return;
        }
        int pivot = source[loIdx].value();
        int pivotIdx = partition(source, loIdx, hiIdx, pivot);
        // Step 3: sort both halves of the list
        quicksortHelp(source, loIdx, pivotIdx);
        quicksortHelp(source, pivotIdx + 1, hiIdx);
    }

    
}
