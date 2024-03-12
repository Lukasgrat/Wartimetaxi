using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinningScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI winningText;
    [SerializeField]
    GameObject winDisplay;
    
    //Given a player, adjusts the winscreen to display their victory
    public void declareWinner(Player p) 
    {
        if (p.sameTeam(Team.Green))
        {
            winningText.color = Color.green;
            winningText.text = "Green has won";
        }
        else if (p.sameTeam(Team.Red))
        {
            winningText.color = Color.red;
            winningText.text = "Red has won";
        }
        else 
        {
            throw new System.Exception("Error, player's team is not accounted for when declaring winner");
        }
    }
}
