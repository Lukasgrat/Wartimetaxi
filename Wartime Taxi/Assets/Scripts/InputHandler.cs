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
    CardType selectedCard;


    Unit initiator;
    Tile target;
    Unit unitTarget;

    enum State 
    { 
        Idle,
        SelectedMove,
        SelectedTile1Move,
        SelectedUnit1Move,
        SelectedTile2Move,
        
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
            t.activateUnits(this.selectionHandler);
        }
        else 
        {
            this.currentState = State.Idle;
            this.selectedCard = CardType.None;
            this.clearLights();
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
    public void startMove() 
    {
        this.selectedCard = CardType.Move;
        this.clearLights();
        this.playerList[currentPlayerIndex].hightLightUnitTiles();
        this.currentState = State.SelectedMove;
    }
}
