using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Camera cam;
    int turn = 0;
    [SerializeField]
    List<Tile> tiles;

    // Start is called before the first frame update
    void Start()
    {
        this.cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        if(Input.GetMouseButtonDown(0)) 
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) 
            {
                if (hit.transform.gameObject.TryGetComponent<Tile>(out Tile t))
                {
                    Debug.Log("here");
                }
            }
        }
    }
    void clearLights()
    { 
        foreach (Tile tile in tiles)
        {
            tile.lightTile(false);
        }
    }

    void moveUnit() {
    
    }
}
