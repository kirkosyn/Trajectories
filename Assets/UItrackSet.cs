using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UItrackSet : MonoBehaviour
{

    public Button nextBtn;
    public Button previousBtn;
    public Text trackIdText;
    static GameObject[] childGameObjects;
    public int index;

    // Use this for initialization
    void Start()
    {
        index = 0;
    }

    void OnEnable()
    {
        nextBtn.onClick.AddListener(SetNextTrack);
        previousBtn.onClick.AddListener(SetPreviousTrack);
    }

    void OnDisable()
    {
        nextBtn.onClick.RemoveAllListeners();
        previousBtn.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update() { }

    void SetNextTrack()
    {
        if (index < childGameObjects.Length - 1)
            index = index + 1;

        foreach (GameObject x in childGameObjects)
        {
            if (x.name != index.ToString())
                x.SetActive(false);
            else
                x.SetActive(true);
        }

        trackIdText.text = "Track id " + childGameObjects[index].name;
    }

    void SetPreviousTrack()
    {
        if (index > 0)
            index = index - 1;

        foreach (GameObject x in childGameObjects)
        {
            if (x.name != index.ToString())
                x.SetActive(false);
            else
                x.SetActive(true);
        }

        trackIdText.text = "Track id " + childGameObjects[index].name;
    }

    public static void SetGameObject(GameObject[] goarr)
    {
        childGameObjects = goarr;
    }
}
