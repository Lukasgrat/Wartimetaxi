using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    Button stationaryButton;
    [SerializeField]
    Button movingButton;
    [SerializeField]
    InputHandler inputHandler;
    // Start is called before the first frame update
    void Start()
    {
        this.stationaryButton.onClick.AddListener(delegate { this.inputHandler.selectedFake(false); });
        this.movingButton.onClick.AddListener(delegate { this.inputHandler.selectedFake(true); });
    }
}
