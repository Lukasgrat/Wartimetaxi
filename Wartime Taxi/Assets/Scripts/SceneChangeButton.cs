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
    [SerializeField]
    string prefChanged;
    [SerializeField]
    int value;
    // Start is called before the first frame update
    void Start()
    {
        selfButton.onClick.AddListener(delegate {
            if (prefChanged.Equals("AI"))
            {
                PlayerPrefs.SetInt(prefChanged, value);
            }
            SceneManager.LoadScene(scene); });
    }
}
