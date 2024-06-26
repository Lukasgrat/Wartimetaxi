using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI tutorialInfoText;
    [SerializeField]
    Button forwardButton;
    [SerializeField]
    Button backButton;
    [SerializeField]
    List<string> info;
    [SerializeField]
    List<int> blockedConditions;
    [SerializeField]
    InputHandler InputHandler;
    [SerializeField]
    Image pointingAtAction;
    [SerializeField]
    GameObject pointingAtIsland;
    [SerializeField]
    GameObject pointingAtRedMarine;
    [SerializeField]
    GameObject pointingAtGreenMarine;
    [SerializeField]
    Image pointingAtTypes;
    [SerializeField]
    Image pointingAtDraw;
    [SerializeField]
    GameObject pointingAtUnit;
    [SerializeField]
    GameObject pointingAtDeck;
    [SerializeField]
    GameObject pointingAtDestroyer;
    [SerializeField]
    Unit cruiser;
    [SerializeField]
    Tile northwestApproach;
    [SerializeField]
    Unit enemyDestroyer;
    [SerializeField]
    Tile airbase;
    [SerializeField]
    Unit redMarine;
    
    int startingText = 0;
    int currentText = 0;
    bool hasHovered = false;
    // Start is called before the first frame update
    void Start()
    {
        this.forwardButton.onClick.AddListener(delegate { this.changeText(1); });
        this.backButton.onClick.AddListener(delegate { this.changeText(-1); });
    }

    // Update is called once per frame
    void Update()
    {
        if (this.blockedConditions.Count > 0 && this.blockedConditions[0] == this.currentText)
        {
            this.forwardButton.gameObject.SetActive(false);
        }
        else if(currentText < this.info.Count - 1)
        {
            this.forwardButton.gameObject.SetActive(true);
        }
        this.tutorialDisplay(this.currentText);
    }

    void changeText(int change)
    {
        if (currentText + change < startingText)
        {
            throw new System.Exception("Attempted change for text fails going backward");
        }
        if (currentText + change >= info.Count)
        {
            throw new System.Exception("Attempted change for text fails going forward");
        }
        currentText += change;
        this.tutorialInfoText.text = this.info[currentText];
        if (currentText == 0)
        {
            this.backButton.gameObject.SetActive(false);
        }
        else 
        {
            this.backButton.gameObject.SetActive(true);
        }
        if (currentText < this.info.Count - 1) 
        {
            this.forwardButton.gameObject.SetActive(true);
        }
        else 
        {
            this.forwardButton.gameObject.SetActive(false);
        }
    }

    public void changeConditions()
    {
        if (this.blockedConditions.Count > 0) 
        {
            if (this.blockedConditions[0] == 1
                && this.hasHovered) 
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 3
                && this.InputHandler.currentPlayer().canDrawCard(5))
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 5
                && !this.InputHandler.currentPlayer().canDrawCard(6))
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 8
                && this.cruiser.onLocation(this.northwestApproach))
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 11
                && !this.enemyDestroyer.canSurviveShot(2))
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 13
                && !this.InputHandler.currentPlayer().canDrawCard(3))
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 17
                && this.InputHandler.currentPlayer().containsFake())
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 20
                && this.InputHandler.currentPlayer().unitCount() == 5)
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 22
                && this.airbase.unitType() == UnitType.Airbase)
            {
                this.blockedConditions.RemoveAt(0);
            }
            else if (this.blockedConditions[0] == 23
                && !this.redMarine.canSurviveShot(3))
            {
                this.blockedConditions.RemoveAt(0);
            }
        }
    }

    //handles the display of all extra UI related to the tutorial
    void tutorialDisplay(int state) 
    {
        this.pointingAtAction.gameObject.SetActive(state == 2);
        this.pointingAtUnit.gameObject.SetActive(state == 1);
        this.pointingAtDraw.gameObject.SetActive(state == 5);
        this.pointingAtDeck.gameObject.SetActive(state == 7);
        this.pointingAtTypes.gameObject.SetActive(state == 9);
        this.pointingAtGreenMarine.gameObject.SetActive(state == 20);
        this.pointingAtDestroyer.gameObject.SetActive(state == 21);
        this.pointingAtIsland.gameObject.SetActive(state == 22);
        this.pointingAtRedMarine.gameObject.SetActive(state == 23);
    }

    //returns what phase of the tutorial the player is in
    //returns -1 if the tutorial is complete
    public int currentPhase() 
    {
        if (this.blockedConditions.Count == 0) 
        {
            return -1;
        }
        return this.blockedConditions[0];
    }

    public void hovered() 
    {
        this.hasHovered = this.currentText == 1;
    }
}
