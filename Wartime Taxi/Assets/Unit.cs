using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    Tile location;
    [SerializeField]
    UnitType type;
    [SerializeField]
    Team team;
    enum UnitType
    {
        Artillery,
        Marine,
        Destroyer,
        Cruiser,
        Submarine
    }
    enum Team
    { 
        Green,
        Red
    }
    // Start is called before the first frame update
    void Start()
    {
        location.addUnit(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override string ToString() {
        return this.type.ToString();
    }
}
