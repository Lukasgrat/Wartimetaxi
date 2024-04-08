using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinningConditions : MonoBehaviour
{
    [SerializeField]
    internal Button continueButton;
    [SerializeField]
    internal Image displayConditions;
    [SerializeField]
    TextMeshProUGUI turnText;
    [SerializeField]
    GameObject turnsDisplay;
    [SerializeField]
    int TURNSNEEDED;
    int turnsLeft;
    [SerializeField]
    List<Tile> importantTiles;
    [SerializeField]
    int tilesNeeded;
    internal bool mainMenuOn = true;
    [SerializeField]
    internal Button mainMenuButton;


    //ASSUMES THAT THE PLAYER LIST GIVEN HAS CONSISTENT INDEXES
    int winningPlayerIndex = -1;
    void Start()
    {
        continueButton.onClick.AddListener(
            delegate 
            {
                this.displayConditions.gameObject.SetActive(false);
                mainMenuOn = false;
        });
        mainMenuButton.onClick.AddListener(
            delegate 
            {
                mainMenuOn = !mainMenuOn;
                this.displayConditions.gameObject.SetActive(mainMenuOn);
            }
            );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            mainMenuOn = !mainMenuOn;
            this.displayConditions.gameObject.SetActive(mainMenuOn);
        }
    }

    //returns the count of the player who won. If no one has, return -1
    public virtual int hasMet(List<Player> players)
    {
        if (this.turnsLeft == 0)
        {
            return this.controlsTiles(players);
        }
        int isAlive = -1;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].unitCount() > 0) 
            {
                if (isAlive != -1)
                {
                    return -1;
                }
                else 
                {
                    isAlive = i;
                }
            }
        }
        return isAlive;
    }

    //Given a list of players, returns the index of the player controls the needed tiles
    //If no one has, returns -1
    int controlsTiles(List<Player> players) 
    {
        for (int i = 0; i < players.Count; i++)
        {
            int tilesControlled = 0;
            foreach (Tile t in importantTiles)
            {
                if (players[i].controlsTile(t))
                {
                    tilesControlled++;
                }
            }
            if (tilesControlled >= this.tilesNeeded) 
            {
                return i;
            }
        }
        return -1;
    }

    //Changes the conditions based on the current circumstances
    //given the list of players
    public virtual void updateStandings(List<Player> players) 
    {
        int currentController = this.controlsTiles(players);
        if (currentController == -1)
        {
            this.turnsLeft = this.TURNSNEEDED;
            this.turnsDisplay.SetActive(false);
        }
        else if (currentController != this.winningPlayerIndex)
        {
            this.turnsLeft = this.TURNSNEEDED;
            this.winningPlayerIndex = currentController;
            this.turnsDisplay.SetActive(true);
        }
        else 
        {
            this.turnsLeft -= 1;
        }
        turnText.text = this.turnsLeft.ToString();
    }
}