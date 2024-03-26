using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirFieldGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject AirfieldNeutral;
    [SerializeField]
    GameObject AirfieldGreen;
    [SerializeField]
    GameObject AirfieldRed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Generates an airfield of the given type on the given parent
    public void generateAirField(Team team, GameObject parent) 
    {
        GameObject newAirfield;
        if (team == Team.Neutral)
        {
            newAirfield = this.AirfieldNeutral;
        }
        else if (team == Team.Red)
        {
            newAirfield = this.AirfieldRed;
        }
        else if (team == Team.Green) 
        {
            newAirfield = this.AirfieldGreen;
        }
        else
        {
            throw new System.Exception("Error: Team " + team + " not accounted for");
        }

        Object.Instantiate(newAirfield,
        new Vector3(parent.transform.position.x,
        parent.transform.position.y,
        parent.transform.position.z),
        parent.transform.rotation, parent.transform);
    }
}
