using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Data
{

    public Data(float minMomentum, float maxMomentum, int fPID)
    {
        this.minMomentum = minMomentum;
        this.maxMomentum = maxMomentum;
        this.fPID = fPID;
    }

    public Data(float minMomentum, float maxMomentum)
    {
        this.minMomentum = minMomentum;
        this.maxMomentum = maxMomentum;
    }

    public Data(int fPID)
    {
        this.fPID = fPID;
    }

    public Data()
    {

    }

    [System.Serializable]
    public struct FTrack
    {
        public int fUniqueID;
        public string fType;
        public int fCharge;
        public float fE;
        public int fParentID;
        public int fPID;
        public float fSignedPT;
        public float fMass;
        public List<float> fMomentum;
        public List<float> fStartCoordinates;
        public List<float> fEndCoordinates;
        public List<float> fChildrenIDs;
        public float fHelixCurvature;
        public float fTheta;
        public float fPhi;
        public List<float> fPolyX;
        public List<float> fPolyY;
        public List<float> fPolyZ;
    }

    public struct TracksParams
    {
        public bool isEnd;
        public int pointsVal;
        public Vector3 shift;
        public Vector3 actualPose;
        public Color32 color;

        public TracksParams(bool x, int y, Vector3 z, Vector3 q, Color w)
        {
            isEnd = x;
            pointsVal = y;
            shift = z;
            actualPose = q;
            color = w;
        }
    }

    public class TracksParamsList
    {
        public TracksParams[] tracksParams;
    }

    [System.Serializable]
    public class TracksList
    {
        public FTrack[] fTracks;
    }

    public struct Momentums
    {
        public float momentum;
        public int iterator;

        public Momentums(float x, int y)
        {
            momentum = x;
            iterator = y;
        }
    }

    public Color32[] colors;


    public TracksList tracksList;
    public TracksParamsList tracksParamsList;
    public TracksParams track;
    public int length;
    public float momentumValue;
    public FTrack fTrack;
    public Color32 color;
    public readonly int reduction = 500;
    public Vector3 nextPose;
    public Vector3 shift;
    public int maxVal;
    public List<int> lengthList;
    public List<int> trackIterators = new List<int>();
    public float maxMomentum;
    public float minMomentum;
    public int fPID;
    public string resourceAddr = "collision";

    public List<Momentums> listOfMomentums;

    //wczytanie pliku
    public void LoadFile()
    {
        TextAsset listOfCollisions = Resources.Load(resourceAddr) as TextAsset;
        string listToString = listOfCollisions.ToString();
        tracksList = (TracksList)JsonUtility.FromJson(listToString, typeof(TracksList));

    }

    //ustawienie parametrów śladów
    public void SetTheParams(GameObject gameObject, Vector3 pose)
    {
        length = tracksList.fTracks.Length;
        int minVal;
        listOfMomentums = new List<Momentums>();

        tracksParamsList = new TracksParamsList()
        {
            tracksParams = new TracksParams[length]
        };

        for (int i = 0; i < length; i++)
        {
            fTrack = tracksList.fTracks[i];
            momentumValue = CalculateMomentumValue(fTrack.fMomentum[0], fTrack.fMomentum[1], fTrack.fMomentum[2]);
            
            if ((!fPID.Equals(-1) && fTrack.fPID.Equals(fPID)) || fPID.Equals(-1))
            {
                if (momentumValue > minMomentum && momentumValue < maxMomentum)
                {
                    minVal = MinimalValue(i);
                    trackIterators.Add(i);
                    listOfMomentums.Add(new Momentums(momentumValue, i));

                    shift = new Vector3(fTrack.fPolyX[0] / reduction,
                            fTrack.fPolyY[0] / reduction,
                            fTrack.fPolyZ[0] / reduction) - pose;

                    tracksParamsList.tracksParams[i] = new TracksParams(false, minVal, shift, pose, Color.blue);

                    if (maxVal < minVal)
                        maxVal = minVal;
                }
            }
            else
                continue;
        }

        listOfMomentums.Sort((s1, s2) => s1.momentum.CompareTo(s2.momentum));
        SetColorPalette(trackIterators.Count);
        SetAccordingColors();
    }

    //wyznaczenie najmniejszej długości śladu
    public int MinimalValue(int i)
    {
        lengthList = new List<int>()
        {
        tracksList.fTracks[i].fPolyX.Count,
        tracksList.fTracks[i].fPolyY.Count,
        tracksList.fTracks[i].fPolyZ.Count
        };

        return lengthList.Min();
    }

    //wyliczenie wartości wektora pędu na podstawie składowych x,y,z
    public float CalculateMomentumValue(float x, float y, float z)
    {
        float res = Mathf.Sqrt(Mathf.Pow(x, 2) +
                               Mathf.Pow(y, 2) +
                               Mathf.Pow(z, 2));
        return res;
    }


    public void SetColorPalette(int length)
    {
        colors = new Color32[length];
        int step = 255 / (length + 1);
        byte current = 0;

        for (int i = 0; i < length; i++)
        {
            colors[i] = new Color32(current, 0, 0, 255);
            current = System.Convert.ToByte(System.Convert.ToInt32(current) + step);
        }
    }

    public void SetAccordingColors()
    {
        foreach (int iterator in trackIterators)
        {
            int index = listOfMomentums.FindIndex(x => x.iterator == iterator);
            color = colors[index];
            tracksParamsList.tracksParams[iterator].color = color;
        }
    }
}