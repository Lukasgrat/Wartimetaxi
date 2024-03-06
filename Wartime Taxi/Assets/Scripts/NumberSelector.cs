using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberSelector : MonoBehaviour
{
    [SerializeField]
    List<Button> buttons = new List<Button>();
    [SerializeField]
    InputHandler inputHandler;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < buttons.Count; x += 1) 
        {
            int y = x;
            buttons[x].onClick.AddListener(delegate { this.inputHandler.numberSelcted(y + 1); });
        }
    }

    //Given a number, enables the first x numbers of the list
    public void enableButtons(int count)
    {
        this.gameObject.SetActive(true);
        for (int x = 0; x < count; x++) 
        {
            buttons[x].gameObject.SetActive(true);
        }
    }

    //Disables all buttons in the list
    public void disableButtons()
    {
        foreach (Button button in buttons) 
        { 
            button.gameObject.SetActive(false);
        }
    }


    //Disables this gameobject and the underlying buttons
    public void thoroughDisable() 
    { 
        this.gameObject.SetActive(false);
        this.disableButtons();
    }
}
