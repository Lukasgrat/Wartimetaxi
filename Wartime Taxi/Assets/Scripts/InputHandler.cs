using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    Camera cam;
    int turnNumber = 0;
    Team currentPlayer = Team.Green;
    int currentPlayerIndex = 0;
    [SerializeField]
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
    CardType selectedCard = CardType.None;
    


    Unit initiator;
    Tile initiatedTile;
    Tile target;
    Unit unitTarget;
    
    
    
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
        
    }
    State currentState;
    // Start is called before the first frame update
    void Start()
    {
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
            if (units.transform.GetChild(x).TryGetComponent<Unit>(out Unit u))
            {
                if (u.team == Team.Green)
                {
                    this.playerList[0].addUnit(u);
                }
                else if(u.team == Team.Red) 
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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.TryGetComponent<Tile>(out Tile t))
                {
                    this.SelectionHandler(t);
                    if (this.currentState == State.SelectedMove 
                        || this.currentState == State.SelectedUnit1Move
                        || this.currentState == State.SelectedShoot
                        || this.currentState == State.SelectedUnit1Shoot)
                    {
                        this.resetState();
                    }
                }
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
                    this.unitInfoText.text = u.ToString();
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
            Player player = this.playerList[this.currentPlayerIndex];
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
        if (t.isLighten() && this.currentState == State.SelectedMove)
        {

            this.currentState = State.SelectedTile1Move;
            t.activateMoveableUnits(this.selectionHandler);
            this.initiatedTile = t;
            this.clearLights();
        }
        else if (t.isLighten() && this.currentState == State.SelectedUnit1Move)
        {
            Order order = new Move(this.initiatedTile, t, this.initiator);
            this.changeActions(this.actionsLeft - 1); 
            order.playCard();
            this.playerList[this.currentPlayerIndex].removeCard(this.selectedCard);
            this.resetState(); 
        }
        else if (t.isLighten() && this.currentState == State.SelectedShoot) {
            this.currentState = State.SelectedTile1Shoot;
            t.activateShootableUnits(this.selectionHandler);
            this.initiatedTile = t;
            this.clearLights();
        }
        else if (t.isLighten() && this.currentState == State.SelectedUnit1Shoot)
        {
            this.target = t;
            this.currentState = State.SelectedTile2Shoot;
            t.activateVunerableUnits(this.selectionHandler, initiator);
            this.initiatedTile = t;
            this.clearLights();
        }
        else
        {
            resetState();
        }
    }

    //EFFECT: Resets the built up state from a move
    public void resetState()
    {
        this.currentState = State.Idle;
        this.selectedCard = CardType.None;
        this.clearLights();
        this.selectionHandler.disableButtons();
    }

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
            Order order = new Shoot(this.initiator, this.initiatedTile.returnUnit(ut), this.opposingPlayer());
            this.changeActions(this.actionsLeft - 1);
            order.playCard();
            this.playerList[this.currentPlayerIndex].removeCard(this.selectedCard);
            this.selectionHandler.disableButtons();
            this.resetState();
        }
    }

    void nextTurn()
    {
        this.playerList[currentPlayerIndex].showCards(false);
        switch (this.currentPlayer) 
        { 
            case Team.Green:
                this.currentPlayer = Team.Red;
                this.currentPlayerIndex += 1;
                nextTurnButton.appear(this.currentPlayer, this.boardState() * -1);
                break;
            case Team.Red:
                this.currentPlayer = Team.Green;
                turnNumber += 1;
                this.currentPlayerIndex = 0;
                nextTurnButton.appear(this.currentPlayer, this.boardState());
                break;
            default:
                throw new System.Exception("Error, invalid state for current team reached");
        }
        this.playerList[currentPlayerIndex].repairAll();
        this.playerList[currentPlayerIndex].showCards(true);
        this.changeActions(this.MAXACTIONS);
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
}
