using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextTurnButton : MonoBehaviour
{

    [SerializeField]
    Button continueButton;
    [SerializeField]
    TextMeshProUGUI infoText;
    // Start is called before the first frame update
    void Start()
    {
        this.continueButton.onClick.AddListener(delegate { this.gameObject.SetActive(false); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Given the current team and how much the battle is in favor of them,
    //enables the UI with a generated prompt
    public void appear(Team team, int favor) 
    {
        string battleInfo = "";
        if (team == Team.Green)
        {
            this.infoText.color = Color.green;
            battleInfo = "Greenie, ";
        }
        else if (team == Team.Red)
        {
            this.infoText.color = Color.red;
            battleInfo = "Redsocks, ";
        }
        else 
        {
            throw new System.Exception("Team " + team + " not accounted for in next turn UI.");
        }
        battleInfo += "you're up.";
        /*
        if (favor == 0)
        {
            battleInfo += "The battle is still in the air!";
        }
        else if (favor < 0)
        {
            battleInfo += "Turn the tide!";
        }
        else if (favor > 0) 
        {
            battleInfo += "Keep them on the ropes!";
        }*/
        this.infoText.text = battleInfo;
        this.gameObject.SetActive(true);
    }

}
