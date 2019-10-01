using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Lines : MonoBehaviour
{

    public Data data;
    int length;
    GameObject[] childGameObjects;
    List<GameObject> cgoCopy;
    GameObject trackElementInLines;
    LineRenderer lr;
    Data.FTrack fTrack;
    readonly int reduction = 500;
    Vector3 nextPose;
    readonly Material material = new Material(Shader.Find("Sprites/Default"));
    Data.TracksParamsList tracksParamsList;
    Data.TracksParams track;
    int maxVal;
    readonly float width = 0.008f;
    readonly float time = 0.001f; //0.5f
    float maxMomentum;
    float minMomentum;

    public IEnumerator DrawLines(GameObject gameObject, Vector3 pose,
        GameObject prefab, Quaternion rotation)
    {
        data = new Data(UIelements.minMomentum, UIelements.maxMomentum, UIelements.fPID);
        pose += new Vector3(0, 0.25f, 0);
        LoadResources(gameObject, pose);

        track = tracksParamsList.tracksParams[0];

        cgoCopy = new List<GameObject>();
       

        for (int i = 0; i < maxVal; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (data.trackIterators.Exists(x => x == j) == true)
                {
                    track = tracksParamsList.tracksParams[j];

                    if (!track.isEnd)
                    {
                        fTrack = data.tracksList.fTracks[j];
                        childGameObjects[j] = Instantiate(prefab) as GameObject;
                        childGameObjects[j].transform.parent = gameObject.transform;
                        childGameObjects[j].AddComponent<LineRenderer>();
                        childGameObjects[j].name = j.ToString();

                        lr = childGameObjects[j].GetComponent<LineRenderer>() as LineRenderer;
                        cgoCopy.Add(childGameObjects[j]);

                        SetLine(lr, track.color);

                        lr.SetPosition(0, track.actualPose);

                        nextPose = new Vector3(fTrack.fPolyX[i] / reduction,
                                fTrack.fPolyY[i] / reduction,
                                fTrack.fPolyZ[i] / reduction) - track.shift;

                        lr.SetPosition(1, nextPose);

                        track.actualPose = nextPose;

                        if (track.pointsVal == i + 1)
                        {
                            track.isEnd = true;
                        }

                        tracksParamsList.tracksParams[j] = track;

                    }
                }
            }

            Debug.Log("Czas " + Time.time);
            yield return new WaitForSecondsRealtime(time);
            Debug.Log("Czas2 " + Time.time);
        }

        UItrackSet.SetGameObject(cgoCopy.ToArray());
    }

    private void LoadResources(GameObject gameObject, Vector3 pose)
    {
        data.LoadFile();
        data.SetTheParams(gameObject, pose);
        childGameObjects = new GameObject[data.length];
        tracksParamsList = data.tracksParamsList;
        length = data.length;
        maxVal = data.maxVal;
    }

    private void SetLine(LineRenderer lr, Color32 col)
    {
        lr.material = material;

        lr.startColor = col;
        lr.endColor = col;

        lr.startWidth = width;
        lr.endWidth = width;

        lr.useWorldSpace = true;
        lr.positionCount = 2;
    }

    //ustawienie colliderów na poszczególne komponenty trajektorii
    private void SetCollider(GameObject collidersSpace, int i, int j, Vector3 actualPose, Vector3 nextPose)
    {
        GameObject colliderBox = new GameObject();
        CapsuleCollider meshCollider;
        colliderBox.AddComponent<CapsuleCollider>();
        meshCollider = colliderBox.GetComponent<CapsuleCollider>() as CapsuleCollider;


        meshCollider.transform.parent = colliderBox.transform;
        colliderBox.transform.parent = collidersSpace.transform;

        meshCollider.isTrigger = true;
        meshCollider.tag = "Line";
        meshCollider.name = string.Concat("line" + " " + i + " " + j);
        meshCollider.height = (nextPose - actualPose).magnitude + 2 * (width + 0.002f);
        meshCollider.radius = width + 0.002f;

        meshCollider.transform.position = actualPose + (nextPose - actualPose) / 2;
        meshCollider.transform.rotation = Quaternion.LookRotation(nextPose);
    }

    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update() { }

}
