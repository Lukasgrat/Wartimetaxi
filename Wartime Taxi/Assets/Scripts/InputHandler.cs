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
    // Start is called before the first frame update
    void Start()
    {
        this.cam = Camera.main;
        playerList = new List<Player>();
        playerList.Add(new Player(Team.Green, STARTINGDRAW, cardGenerator));
        playerList.Add(new Player(Team.Red, STARTINGDRAW, cardGenerator));
        this.drawCardButton.onClick.AddListener(drawCard);
        STARTINGDRAW = cards.transform.position;
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
}
