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

    // Update is called once per frame
    void Update()
    {
        nextBtn.onClick.AddListener(SetNextTrack);
        previousBtn.onClick.AddListener(SetPreviousTrack);
    }

    void SetNextTrack()
    {
        if (index < childGameObjects.Length)
            index++;
        
        childGameObjects.Where(x => x.name.Equals(childGameObjects[index].name)).ToList<GameObject>().ForEach(x => x.SetActive(true));
        childGameObjects.Where(x => !x.name.Equals(childGameObjects[index].name)).ToList<GameObject>().ForEach(x => x.SetActive(false));
        
        trackIdText.text = "Track id " + childGameObjects[index].name;
    }

    void SetPreviousTrack()
    {
        if (index > 0)
            index--;

        childGameObjects.Where(x => x.name.Equals(childGameObjects[index].name)).ToList<GameObject>().ForEach(x => x.SetActive(true));
        childGameObjects.Where(x => !x.name.Equals(childGameObjects[index].name)).ToList<GameObject>().ForEach(x => x.SetActive(false));

        trackIdText.text = "Track id " + childGameObjects[index].name;
    }

    public static void SetGameObject(GameObject[] goarr)
    {        
        childGameObjects = goarr;
    }
}
