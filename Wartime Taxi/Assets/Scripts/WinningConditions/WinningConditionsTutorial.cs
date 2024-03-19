using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningConditionsTutorial : WinningConditions
{
    [SerializeField]
    TutorialText tutorialText;
    // Start is called before the first frame update
    void Start()
    {
        continueButton.onClick.AddListener(
          delegate
          {
              this.displayConditions.gameObject.SetActive(false);
          });
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    
    //returns the count of the player who won. If no one has, return -1
    public override int hasMet(List<Player> players)
    { 
        if(this.tutorialText.currentPhase() == -1) { return 0; }
        return -1;
    }
    //Changes the conditions based on the current circumstances
    //given the list of players
    public override void updateStandings(List<Player> players)
    {
    }
}