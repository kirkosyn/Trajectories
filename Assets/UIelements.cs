using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIelements : MonoBehaviour
{

    public Slider minMomSlider;
    public Slider maxMomSlider;
    public Text minMomVal;
    public Text maxMomVal;
    public Dropdown dataSetBoxList;
    public Dropdown eventBoxList;
    public Button start;

    public static float minMomentum;
    public static float maxMomentum;
    public static int fPID;
    public static string eventNum;

    // Use this for initialization
    void Start()
    {
        dataSetBoxList.value = 0;
        eventBoxList.value = 0;
        minMomSlider.value = 0;
        maxMomSlider.value = 5;

        SetSliderMax();
        SetSliderMin();
        SetData();
        SetEvent();
    }

    private void OnEnable()
    {
        dataSetBoxList.onValueChanged.AddListener(delegate { SetData(); });
        eventBoxList.onValueChanged.AddListener(delegate { SetEvent(); });
        minMomSlider.onValueChanged.AddListener(delegate { SetSliderMin(); });
        maxMomSlider.onValueChanged.AddListener(delegate { SetSliderMax(); });
        start.onClick.AddListener(StartVisualization);
    }

    private void OnDisable()
    {
        dataSetBoxList.onValueChanged.RemoveAllListeners();
        eventBoxList.onValueChanged.RemoveAllListeners();
        minMomSlider.onValueChanged.RemoveListener(delegate { SetSliderMin(); });
        maxMomSlider.onValueChanged.RemoveListener(delegate { SetSliderMax(); });
        start.onClick.RemoveAllListeners();
    }

    void SetSliderMin()
    {
        minMomVal.text = minMomSlider.value.ToString();
        minMomentum = minMomSlider.value;
    }
    void SetSliderMax()
    {
        maxMomVal.text = maxMomSlider.value.ToString();
        maxMomentum = maxMomSlider.value;
    }

    void SetData()
    {
        string option = dataSetBoxList.options[dataSetBoxList.value].text;

        if (!option.Equals("All sets"))
            fPID = int.Parse(option.Substring(6));
        else fPID = -1;
    }

    void SetEvent()
    {
        eventNum = eventBoxList.options[eventBoxList.value].text;
    }

    void StartVisualization()
    {
        foreach (var image in GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.m_TempAugmentedImages)
        {
            GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.m_Visualizers.Remove(image.DatabaseIndex);
        }

        if (GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.simulation != null)
        {
            GameObject.Destroy(GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.simulation.GetComponentInChildren<GameObject>());
            GameObject.Destroy(GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.simulation.gameObject);
        }

        if (GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.visualizer != null)
            GameObject.Destroy(GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.visualizer.gameObject);

        if (GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.anchor != null)
        {
            Destroy(GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.anchor);
            GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.anchor = new GoogleARCore.Anchor();
        }

        foreach (GameObject go in UItrackSet.childGameObjects)
        {
            if (go != null)
                Destroy(go.gameObject);
        }
        GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.isHit = false;
        GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.m_Visualizers 
            = new Dictionary<int, GoogleARCore.Examples.AugmentedImage.AugmentedImageVisualizer>();
        GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.visualizer = null;
        GoogleARCore.Examples.AugmentedImage.AugmentedImageExampleController.m_TempAugmentedImages = new List<GoogleARCore.AugmentedImage>();
    }

    // Update is called once per frame
    void Update() { }
}
