using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public enum UnitType
{
    Airbase,
    Marine,
    Destroyer,
    Cruiser,
    Submarine
}


public enum Team
{
    Green,
    Red,
    Neutral
}

public class Unit : MonoBehaviour
{
    [SerializeField]
    Tile location;
    [SerializeField]
    UnitType type;
    [SerializeField]
    public Team team;
    // Start is called before the first frame update
    void Start()
    {
        location.addUnit(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //returns whether this unit has the same type as the given type
    public bool sameType(UnitType type) 
    {
        return this.type == type;
    }

    public override string ToString()
    {
        return this.type.ToString();
    }
    public void moveTo(Tile tile) 
    {
        this.location = tile; 
    }

    public bool canMoveToLand() 
    { 
        return this.type == UnitType.Marine || this.type == UnitType.Airbase;
    }

    //EFFECT: Highlights the tile associated  with this unit
    public void hightLightTile() 
    {
        this.location.lightTile(true);
    }
}
