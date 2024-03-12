using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField]
    Button selfButton;
    [SerializeField]
    int scene;
    // Start is called before the first frame update
    void Start()
    {
        selfButton.onClick.AddListener(delegate { SceneManager.LoadScene(scene); });
    }
}
