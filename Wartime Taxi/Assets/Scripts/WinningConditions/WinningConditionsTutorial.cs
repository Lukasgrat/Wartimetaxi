using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningConditionsTutorial : WinningConditions
{
    [SerializeField]
    TutorialText tutorialText;
    [SerializeField]
    Unit redMarine;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(
           delegate
           {
               mainMenuOn = !mainMenuOn;
               this.displayConditions.gameObject.SetActive(mainMenuOn);
           }
           );
        continueButton.onClick.AddListener(
          delegate
          {
              this.displayConditions.gameObject.SetActive(false);
              mainMenuOn = false;
          });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mainMenuOn = !mainMenuOn;
            this.displayConditions.gameObject.SetActive(mainMenuOn);
        }
    }
    
    
    //returns the count of the player who won. If no one has, return -1
    public override int hasMet(List<Player> players)
    { 
        if(this.tutorialText.currentPhase() == -1 || 
            (this.tutorialText.currentPhase() == 23 && !this.redMarine.canSurviveShot(3)))
        { 
            return 0;
        }
        return -1;
    }
    //Changes the conditions based on the current circumstances
    //given the list of players
    public override void updateStandings(List<Player> players, int currentPlayerIndex)
    {
    }
}