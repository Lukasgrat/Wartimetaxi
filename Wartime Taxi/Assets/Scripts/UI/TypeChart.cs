using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TypeChart : MonoBehaviour
{
    [SerializeField]
    Image typechart;

    private void Update()
    {
    }
    public void showTypeChart()
    {
        typechart.gameObject.SetActive(true);
    }

    public void hideTypeChart()
    {
        typechart.gameObject.SetActive(false);
    }
}
