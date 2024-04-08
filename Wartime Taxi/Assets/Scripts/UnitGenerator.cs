using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    [SerializeField]
    Unit grnTemplateUnit;
    [SerializeField]
    Unit redTemplateUnit;
    [SerializeField]
    Unit greenSubTemplate;
    [SerializeField]
    Unit redSubTemplate;
    [SerializeField]
    Unit greenDestroyerTemplate;
    [SerializeField]
    Unit redDestroyerTemplate;
    [SerializeField]
    Unit greenCruiserTemplate;
    [SerializeField]
    Unit redCruiserTemplate;
    [SerializeField]
    FakeUnit greenFakeSubTemplate;
    [SerializeField]
    FakeUnit redFakeSubTemplate;
    [SerializeField]
    GameObject Units;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Given a team, unittype, and whether it is fake or not, creates a unit of the respective qualities and returns it
    public Unit createUnit(Unit u)
    {
        Unit newUnit;
        if (u.sameTeam(Team.Green)) { newUnit = this.selectGreenUnit(u); }
        else if (u.sameTeam(Team.Red)) { newUnit = this.selectRedUnit(u); }
        else 
        {
            throw new Exception("Invalid team attempted to be created");
        }
        Unit finishedUnit = UnityEngine.Object.Instantiate(newUnit,
           Vector3.zero,
            new Quaternion(), Units.transform);

        return finishedUnit;
    }

    Unit selectGreenUnit(Unit u) 
    {
        if (u.sameType(UnitType.Submarine))
        {
            return this.greenSubTemplate;
        }
        else if (u.sameType(UnitType.Destroyer))
        {
            return this.greenDestroyerTemplate;
        }
        else if (u.sameType(UnitType.Cruiser))
        {
            return this.greenCruiserTemplate;
        }
        else
        {
            return this.grnTemplateUnit;
        }
    }
    Unit selectRedUnit(Unit u)
    {
        if (u.sameType(UnitType.Submarine))
        {
            return this.redSubTemplate;
        }
        else if (u.sameType(UnitType.Destroyer))
        {
            return this.redDestroyerTemplate;
        }
        else if (u.sameType(UnitType.Cruiser))
        {
            return this.redCruiserTemplate;
        }
        else 
        {
            return this.redTemplateUnit;
        }
    }

    public FakeUnit createFakeUnit(Unit u)
    {
        FakeUnit newUnit;
        if (u.sameTeam(Team.Green)) { newUnit = this.greenFakeSubTemplate; }
        else { newUnit = this.redFakeSubTemplate; }
        FakeUnit finishedUnit = UnityEngine.Object.Instantiate(newUnit,
           Vector3.zero,
            new Quaternion(), Units.transform);

        return finishedUnit;
    }
}
