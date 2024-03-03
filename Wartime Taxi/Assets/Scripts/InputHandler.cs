using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Camera cam;
    int turnNumber = 0;
    Team currentPlayer = Team.Green;
    [SerializeField]
    List<Tile> tiles;
    List<Card> cards;
    List<Player> playerList;
    // Start is called before the first frame update
    void Start()
    {
        this.cam = Camera.main;
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
                break;
            case Team.Red: 
                this.currentPlayer = Team.Green;
                turnNumber += 1;
                break;
            default:
                throw new System.Exception("Error, invalid state for current team reached");
        }
    }
}
