using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UItrackSet : MonoBehaviour
{
    public Button nextBtn;
    public Button previousBtn;
    public Button trackBtn;
    public Text trackIdText;

    public Text uniqueId;
    public Text type;
    public Text charge;
    public Text energy;
    public Text parentId;
    public Text pid;
    public Text signedPt;
    public Text mass;
    public Text momentum;
    public Text helix;
    public Text theta;
    public Text phi;

    public static GameObject[] childGameObjects;
    static Data data;
    public int index;

    string all = "All tracks";
    string selected = "Track id ";

    // Use this for initialization
    void Start()
    {
        index = 0;
    }

    void OnEnable()
    {
        nextBtn.onClick.AddListener(SetNextTrack);
        previousBtn.onClick.AddListener(SetPreviousTrack);
        trackBtn.onClick.AddListener(SetTrackInfo);
    }

    void OnDisable()
    {
        nextBtn.onClick.RemoveAllListeners();
        previousBtn.onClick.RemoveAllListeners();
        trackBtn.onClick.RemoveAllListeners();
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
        {
            index--;

            foreach (GameObject x in childGameObjects)
            {
                if (x.name != index.ToString())
                    x.SetActive(false);
                else
                    x.SetActive(true);
            }

            trackIdText.text = selected + childGameObjects[index].name;
        }
        else if (index == 0)
        {
            index--;

            childGameObjects.ToList<GameObject>().ForEach(x => x.SetActive(true));
            trackIdText.text = all;
        }
    }

    void SetTrackInfo()
    {
        string text = trackBtn.GetComponentInChildren<Text>().text;
        int index;
        Data.FTrack track;

        if (!text.Equals(all))
        {
            text = text.Substring(9);

            index = data.trackIterators[System.Int32.Parse(text)];
            track = data.tracksList.fTracks[index];

            SetInfoComponents(track);
        }
        else
        {
            SetInfoComponents(new Data.FTrack());
        }
    }
    public static void SetGameObject(GameObject[] goarr)
    {
        childGameObjects = goarr;
    }

    public static void SetData(Data d)
    {
        data = d;
    }

    void SetInfoComponents(Data.FTrack track)
    {
        uniqueId.text = track.fUniqueID.ToString();
        type.text = track.fType.ToString();
        charge.text = track.fCharge.ToString();
        energy.text = track.fE.ToString();
        parentId.text = track.fParentID.ToString();
        pid.text = track.fPID.ToString();
        signedPt.text = track.fSignedPT.ToString();
        mass.text = track.fMass.ToString();
        momentum.text = data.CalculateMomentumValue(
            track.fMomentum[0],
            track.fMomentum[1],
            track.fMomentum[2])
            .ToString();
        helix.text = track.fHelixCurvature.ToString();
        theta.text = track.fTheta.ToString();
        phi.text = track.fPhi.ToString();
    }
}
