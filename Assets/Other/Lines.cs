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
    //Color color;
    GameObject[] childGameObjects;
    List<GameObject> cgoCopy;
    GameObject trackElementInLines;
    LineRenderer lr;
    Data.FTrack fTrack;
    readonly int reduction = 500;
    Vector3 nextPose;
    //Vector3 shift;
    readonly Material material = new Material(Shader.Find("Sprites/Default"));
    //List<int> lengthList;
    Data.TracksParamsList tracksParamsList;
    Data.TracksParams track;
    int maxVal;
    //Vector3 offset = new Vector3(-0.5f, 0, 0);
    readonly float width = 0.008f;
    readonly float time = 0.001f; //0.5f
    float maxMomentum;
    float minMomentum;
    //List<int> trackIterators = new List<int>();

    /*public struct TracksParams
    {
        public bool isEnd;
        public int pointsVal;
        public Vector3 shift;
        public Vector3 actualPose;
        public Color color;

        public TracksParams(bool x, int y, Vector3 z, Vector3 q, Color w)
        {
            isEnd = x;
            pointsVal = y;
            shift = z;
            actualPose = q;
            color = w;
        }
    }*/

    /*public class TracksParamsList
    {
        public TracksParams[] tracksParams;
    }*/

    public IEnumerator DrawLines(GameObject gameObject, Vector3 pose,
        GameObject prefab, Quaternion rotation) //, GameObject aliceModel, GameObject trackElement)//, int i)
    {
        data = new Data(UIelements.minMomentum, UIelements.maxMomentum, UIelements.fPID);
        pose += new Vector3(0, 0.25f, 0);
        LoadResources(gameObject, pose);

        track = tracksParamsList.tracksParams[0];

        cgoCopy = new List<GameObject>();

        //GameObject aliceModelObject = Instantiate(aliceModel, track.actualPose + offset, rotation) as GameObject;
        //aliceModelObject.transform.parent = gameObject.transform;
        //aliceModelObject.transform.Rotate(0, 90f, 0, Space.Self);

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
                        //trackElementInLines = Instantiate(trackElement, track.actualPose, rotation,
                        //  childGameObjects[j].transform.parent) as GameObject;
                        lr = childGameObjects[j].GetComponent<LineRenderer>() as LineRenderer;
                        cgoCopy.Add(childGameObjects[j]);
                        //childGameObjects[j].AddComponent<CapsuleCollider>();

                        SetLine(lr, track.color);

                        lr.SetPosition(0, track.actualPose);

                        nextPose = new Vector3(fTrack.fPolyX[i] / reduction,
                                fTrack.fPolyY[i] / reduction,
                                fTrack.fPolyZ[i] / reduction) - track.shift;

                        lr.SetPosition(1, nextPose);

                        //SetCapsule(trackElementInLines, track.color, track.actualPose, nextPose);

                        //SetCollider(childGameObjects[j], i, j, track.actualPose, nextPose); //, lr);

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

    /*private void SetCapsule(GameObject capsule, Color32 color, Vector3 actualPose, Vector3 nextPose)
    {
        Material material = capsule.GetComponent<Material>() as Material;
        material.SetColor("_Color", color);        

        //capCol.height = (nextPose - actualPose).magnitude + 2 * (width + 0.002f); 
        //capCol.radius = width + 0.002f; 
    }*/

    //ustawienie colliderów na poszczególne komponenty trajektorii
    private void SetCollider(GameObject collidersSpace, int i, int j, Vector3 actualPose, Vector3 nextPose)//, LineRenderer lr)
    {
        GameObject colliderBox = new GameObject();
        //colliderBox.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        //BoxCollider meshCollider; // = childGameObjects[j].AddComponent<BoxCollider>();
        CapsuleCollider meshCollider;
        //colliderBox.AddComponent<BoxCollider>();
        colliderBox.AddComponent<CapsuleCollider>();
        //meshCollider = colliderBox.GetComponent<BoxCollider>() as BoxCollider;
        meshCollider = colliderBox.GetComponent<CapsuleCollider>() as CapsuleCollider;


        meshCollider.transform.parent = colliderBox.transform;
        colliderBox.transform.parent = collidersSpace.transform;

        meshCollider.isTrigger = true;
        meshCollider.tag = "Line";
        meshCollider.name = string.Concat("line" + " " + i + " " + j);
        // string.Concat("line" + " " + i + " " + j);
        //meshCollider.size = new Vector3(1, System.Math.Abs(nextPose.y - actualPose.y), width);
        meshCollider.height = (nextPose - actualPose).magnitude + 2 * (width + 0.002f);
        meshCollider.radius = width + 0.002f;

        meshCollider.transform.position = actualPose + (nextPose - actualPose) / 2;
        meshCollider.transform.rotation = Quaternion.LookRotation(nextPose);
        /*colliderBox.transform.Rotate(Vector3.SignedAngle(actualPose, nextPose, Vector3.right),
            Vector3.SignedAngle(actualPose, nextPose, Vector3.up),
            Vector3.SignedAngle(actualPose, nextPose, Vector3.forward));*/
    }

    // Use this for initialization
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        //    if (GameObject.Find("minMomSlider"))
        //        minMomentum = (GameObject.FindObjectsOfType(typeof(Slider)).ElementAt(0) as Slider).value;
        //    if (GameObject.Find("maxMomSlider"))
        //        maxMomentum = (GameObject.FindObjectsOfType(typeof(Slider)).ElementAt(1) as Slider).value;
        //Debug.Log("min " + minMomentum + " max " + maxMomentum);
    }

}

/*for (int i = 0; i < length; i++)
        {
            fTrack = data.tracksList.fTracks[i];
            nextPose = pose;


            for (int j = 0; j < minVal; j++)
            {
                lr = childGameObjects[i].GetComponent<LineRenderer>() as LineRenderer;

                lr.SetPosition(0, nextPose);

                nextPose = new Vector3(fTrack.fPolyX[j] / reduction,
                        fTrack.fPolyY[j] / reduction,
                        fTrack.fPolyZ[j] / reduction) - shift;

                lr.SetPosition(1, nextPose);

                Debug.Log("Czas " + Time.time);
                yield return new WaitForSecondsRealtime(0.2f);
                Debug.Log("Czas2 " + Time.time);
            }

        }*/

/*public void SetTheParams(GameObject gameObject, Vector3 pose, GameObject prefab)
    {
        data = new Data();
        data.LoadFile();

        childGameObjects = new GameObject[data.length];

        /*length = data.tracksList.fTracks.Length;
        
        childGameObjects = new GameObject[length];
        tracksParamsList = new TracksParamsList()
        {
            tracksParams = new TracksParams[length]
        };*/

/*for (int i = 0; i < length; i++)
{
    fTrack = data.tracksList.fTracks[i];
    //float momentumValue = CalculateMomentumValue(fTrack.fMomentum[0], fTrack.fMomentum[1], fTrack.fMomentum[2]);


    //if (momentumValue > minMomentum && momentumValue < maxMomentum)
    //{
    //int minVal = MinimalValue(i);

    //trackIterators.Add(i);

    shift = new Vector3(fTrack.fPolyX[0] / reduction,
                fTrack.fPolyY[0] / reduction,
                fTrack.fPolyZ[0] / reduction) - pose;

    color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

    tracksParamsList.tracksParams[i] = new TracksParams(false, minVal, shift, pose, color);

    if (maxVal < minVal)
        maxVal = minVal;
    //}
}*/

/*private int MinimalValue(int i)
{
lengthList = new List<int>()
{
data.tracksList.fTracks[i].fPolyX.Count,
data.tracksList.fTracks[i].fPolyY.Count,
data.tracksList.fTracks[i].fPolyZ.Count
};

return lengthList.Min();
}*/
