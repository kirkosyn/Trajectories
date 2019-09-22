using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsCheck : MonoBehaviour
{
    //GameObject circleTouch;
    //Vector3 offset;
    private const float k_ModelRotation = 180.0f;
    string hitName;
    char[] charSeparators = new char[] { ' ' };
    int numberOfVector;
    int numberOfTrack;
    
    //GameObject trajectoriesModel;
    //GameObject[] modelChildren;
    Data data;
    Rect windowRect = new Rect(50, 50, 500, 300);
    bool showWindow = false;

    float featureMass;
    float featurePolyX;
    float featurePolyY;
    float featurePolyZ;
    string featureType;

    private GUIStyle statesLabel;
    private bool initDone = false;

    // Use this for initialization
    void Start()
    {

        data = new Data();
        data.LoadFile();
        //trajectoriesModel = GameObject.FindGameObjectWithTag("Model");
        //modelChildren = trajectoriesModel.GetComponents<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Touch touch;
        touch = Input.GetTouch(0);

        if (Input.touchCount < 1 || (touch.phase != TouchPhase.Began))
        {
            return;
        }

        Ray raycast = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit raycastHit;
        //offset = Camera.main.ScreenToViewportPoint(touch.position) - Camera.main.ScreenToWorldPoint(touch.position);

        /*if (touch.pressure > 0f)
        {
            circleTouch = new GameObject();
            circleTouch.AddComponent<SpriteRenderer>();
            SpriteRenderer spriteRenderer = circleTouch.GetComponent<SpriteRenderer>() as SpriteRenderer;
            spriteRenderer.sprite = Resources.Load<Sprite>("circleobj");
            Instantiate(circleTouch, touch.position, Quaternion.Euler(touch.position));
            Destroy(circleTouch, 1f);
        }*/



        if (Physics.Raycast(raycast, out raycastHit))
        {
            Debug.Log("Something Hit");
            /*if (raycastHit.collider.name == "Line")
            {
                Debug.Log("Line clicked name");
            }*/

            //OR with Tag

            if (raycastHit.collider.CompareTag("Line"))
            {
                Debug.Log("Line clicked tag");

                hitName = raycastHit.collider.name;
                //Debug.Log(hitName);

                string[] result;
                result = hitName.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);
                //Debug.Log(result[1]);
                //Debug.Log(result[2]);
                numberOfVector = System.Int32.Parse(result[1]);
                numberOfTrack = System.Int32.Parse(result[2]);

                SetLabels();

                showWindow = true;

                //Debug.Log("Mass = " + featureMass);
                //Debug.Log("Coordinate x = " + featurePolyX);
            }

        }
        
    }

    private void SetLabels()
    {
        featureMass = data.tracksList.fTracks[numberOfTrack].fMass;
        featurePolyX = data.tracksList.fTracks[numberOfTrack].fPolyX[numberOfVector];
        featurePolyY = data.tracksList.fTracks[numberOfTrack].fPolyY[numberOfVector];
        featurePolyZ = data.tracksList.fTracks[numberOfTrack].fPolyZ[numberOfVector];
        featureType = data.tracksList.fTracks[numberOfTrack].fType;
    }
    
    void InitStyles()
    {
        initDone = true;
        statesLabel = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            margin = new RectOffset(),
            padding = new RectOffset(),
            fontSize = 30
            //fontStyle = FontStyle.Bold
        };
    }
    void OnInspectorGUI()
    {
        if (!initDone)
            InitStyles();
    }

    void OnGUI()
    {
        if (showWindow)
        {
            OnInspectorGUI();
            windowRect = GUILayout.Window(0, windowRect, WindowProc, "");
        }
    }

    void WindowProc(int windowID)
    {
        GUILayout.Label("Type = " + featureType, statesLabel);
        GUILayout.Label("Mass = " + featureMass.ToString(), statesLabel);
        GUILayout.Label("Coordinate X = " + featurePolyX.ToString(), statesLabel);
        GUILayout.Label("Coordinate Y = " + featurePolyY.ToString(), statesLabel);
        GUILayout.Label("Coordinate Z = " + featurePolyZ.ToString(), statesLabel);

        if (GUI.Button(new Rect(220, 200, 100, 80), "Close", statesLabel))
        {
            showWindow = false;
        }
    }

}
