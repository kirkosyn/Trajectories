using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Data
{

    public Data(float minMomentum, float maxMomentum)
    {
        this.minMomentum = minMomentum;
        this.maxMomentum = maxMomentum;
    }

    public Data()
    {

    }

    /*public struct ColorParams
    {
        public float saturation;
        public float hue;
        public float value;

        public ColorParams(float x, float y, float z)
        {
            hue = x;
            saturation = y;
            value = z;
        }
    }*/

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
        int iterator = 0;

        //SetColorPalette();
        SortMomentums();

        tracksParamsList = new TracksParamsList()
        {
            tracksParams = new TracksParams[length]
        };

        for (int i = 0; i < length; i++)
        {
            fTrack = tracksList.fTracks[i];
            momentumValue = CalculateMomentumValue(fTrack.fMomentum[0], fTrack.fMomentum[1], fTrack.fMomentum[2]);

            if (momentumValue > minMomentum && momentumValue < maxMomentum)
            {
                minVal = MinimalValue(i);
                trackIterators.Add(iterator);
                iterator++;
                Debug.Log("iterator = " + iterator);

                shift = new Vector3(fTrack.fPolyX[0] / reduction,
                        fTrack.fPolyY[0] / reduction,
                        fTrack.fPolyZ[0] / reduction) - pose;

                //color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                //int index = listOfMomentums.FindIndex(x => x.iterator == i);

                //color = colors[index];

                tracksParamsList.tracksParams[i] = new TracksParams(false, minVal, shift, pose, Color.blue);

                if (maxVal < minVal)
                    maxVal = minVal;
            }
        }

        SetColorPalette(trackIterators.Count());
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
            //Debug.Log("color: " + current.ToString());
        }
    }

    public void SetAccordingColors()
    {
        for (int i = 0; i < trackIterators.Count(); i++)
        {
            int j = trackIterators[i];
            Debug.Log("i = " + i + " j = " + j);
            int index = listOfMomentums.FindIndex(x => x.iterator == j);
            Debug.Log("index = " + index);
            color = colors[index];
            tracksParamsList.tracksParams[j].color = color;
        }
    }

    public void SortMomentums()
    {
        listOfMomentums = new List<Momentums>();
        int iterator = 0;

        foreach (FTrack element in tracksList.fTracks)
        {
            momentumValue = CalculateMomentumValue(element.fMomentum[0], element.fMomentum[1], element.fMomentum[2]);

            if (momentumValue < maxMomentum && momentumValue > minMomentum)
            {
                listOfMomentums.Add(new Momentums(momentumValue, iterator));
                iterator++;
            }

        }

        listOfMomentums.Sort((s1, s2) => s1.momentum.CompareTo(s2.momentum));
    }
}
