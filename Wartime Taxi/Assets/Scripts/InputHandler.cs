using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField]
    int MAXCARDCOUNT = 6;
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
        playerList.Add(new Player(Team.Red, STARTINGDRAW, cardGenerator));
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
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        if(Input.GetMouseButtonDown(0)) 
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) 
            {
                if (hit.transform.gameObject.TryGetComponent<Tile>(out Tile t))
                {
                    this.SelectionHandler(t);
                }
            }
        }
    }
    void drawCard() 
    {
        Player player = this.playerList[this.currentPlayerIndex];
        if (player.canDrawCard(this.MAXCARDCOUNT)) 
        {
            player.drawCard(this.cards);
        }
    }
    void clearLights()
    { 
        foreach (Tile tile in tiles)
        {
            tile.lightTile(false);
        }
    }

    void moveUnit() {
    
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
            order.playCard();
            this.playerList[this.currentPlayerIndex].removeCard(this.selectedCard);
            this.selectionHandler.disableButtons();
            this.resetState();
        }
    }

    void nextTurn() 
    {
        switch (this.currentPlayer) 
        { 
            case Team.Green:
                this.currentPlayer = Team.Red;
                this.playerList[currentPlayerIndex].showCards(false);
                this.currentPlayerIndex += 1;
                this.playerList[currentPlayerIndex].showCards(true);
                break;
            case Team.Red:
                this.playerList[currentPlayerIndex].showCards(false);
                this.currentPlayer = Team.Green;
                turnNumber += 1;
                this.currentPlayerIndex = 0;
                this.playerList[currentPlayerIndex].showCards(true);
                break;
            default:
                throw new System.Exception("Error, invalid state for current team reached");
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
        this.selectedCard = CardType.Move;
        List<Tile> hightLightList = this.playerList[currentPlayerIndex].canMoveFrom();
        foreach (Tile t in hightLightList)
        {
            t.lightTile(true);
        }
        this.currentState = State.SelectedMove;
    }
    public void startShoot()
    {
        this.resetState();
        this.selectedCard = CardType.Shoot;
        this.currentState = State.SelectedShoot;
        List<Tile> hightLightList = this.opposingPlayer().canBeShotBy(this.playerList[currentPlayerIndex]);
        foreach (Tile t in hightLightList) 
        {
            t.lightTile(true);
        }
    }
}
