using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour
{
    [SerializeField]
    Button selectMarine;
    [SerializeField]
    Button selectCruiser;
    [SerializeField]
    Button selectDestroyer;
    [SerializeField]
    Button selectSubmarine;
    [SerializeField]
    Button selectAirfield;

    //enables all buttons associated with the given units, clearing all others 
    public void enableButtons(List<Unit> units) 
    {
        this.disableButtons();
        foreach (Unit unit in units) 
        {
            if (unit.sameType(UnitType.Marine))
            {
                this.selectMarine.gameObject.SetActive(true);
            }
            else if (unit.sameType(UnitType.Cruiser))
            {
                this.selectCruiser.gameObject.SetActive(true);
            }
            else if (unit.sameType(UnitType.Destroyer))
            {
                this.selectDestroyer.gameObject.SetActive(true);
            }
            else if (unit.sameType(UnitType.Airbase))
            {
                this.selectAirfield.gameObject.SetActive(true);
            }
            else if (unit.sameType(UnitType.Submarine))
            {
                this.selectSubmarine.gameObject.SetActive(true);
            }
        }
    }
    public void disableButtons() 
    {
        this.selectMarine.gameObject.SetActive(false);
        this.selectCruiser.gameObject.SetActive(false);
        this.selectDestroyer.gameObject.SetActive(false);
        this.selectAirfield.gameObject.SetActive(false);
        this.selectSubmarine.gameObject.SetActive(false);
    }
}
