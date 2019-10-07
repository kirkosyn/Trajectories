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
    GameObject trackElementInLines;
    LineRenderer lr;
    Data.FTrack fTrack;
    float reduction;
    Vector3 nextPose;
    readonly Material material = new Material(Shader.Find("Sprites/Default"));
    Data.TracksParamsList tracksParamsList;
    Data.TracksParams track;
    int maxVal;
    readonly float width = 0.008f;
    readonly float time = 0.1f; //0.001f; 
    float maxMomentum;
    float minMomentum;

    //rysowanie trajektorii
    public IEnumerator DrawLines(GameObject gameObject, Vector3 pose,
        GameObject prefab, Quaternion rotation, float img_width, float img_height)
    {
        data = new Data(UIelements.minMomentum, UIelements.maxMomentum, UIelements.fPID);
        //pose += new Vector3(0, 0.25f, 0);

        reduction = 500 / img_height / img_width;

        LoadResources(gameObject, prefab, pose, reduction);

        for (int i = 0; i < maxVal; i++)
        {
            for (int j = 0; j < length; j++)
            {
                track = tracksParamsList.tracksParams[data.trackIterators[j]];

                if (!track.isEnd)
                {
                    fTrack = data.tracksList.fTracks[data.trackIterators[j]];
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.transform.parent = childGameObjects[j].transform;
                    go.AddComponent<LineRenderer>();

                    lr = go.GetComponent<LineRenderer>() as LineRenderer;

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

                    tracksParamsList.tracksParams[data.trackIterators[j]] = new Data.TracksParams(track);

                }

            }

            Debug.Log("Czas " + Time.time);
            yield return new WaitForSecondsRealtime(time);
            Debug.Log("Czas2 " + Time.time);
        }

        UItrackSet.SetData(data);
        UItrackSet.SetGameObject(childGameObjects);
    }

    //załadowanie danych o śladach
    private void LoadResources(GameObject gameObject, GameObject prefab, Vector3 pose, float reduction)
    {
        data.LoadFile();
        data.SetTheParams(pose, reduction);
        length = data.trackIterators.Count;
        maxVal = data.maxVal;
        tracksParamsList = data.tracksParamsList;
        childGameObjects = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            childGameObjects[i] = Instantiate(prefab) as GameObject;
            childGameObjects[i].transform.parent = gameObject.transform;
            childGameObjects[i].name = i.ToString();
        }
    }

    //ustawienie odcinka (część trajektorii)
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
