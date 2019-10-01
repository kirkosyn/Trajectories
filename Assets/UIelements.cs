﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIelements : MonoBehaviour {

    public Slider minMomSlider;
    public Slider maxMomSlider;
    public Text minMomVal;
    public Text maxMomVal;
    public Dropdown dataSetBoxList;

    public static float minMomentum;
    public static float maxMomentum;
    public static int fPID;

	// Use this for initialization
	void Start () {
        minMomSlider.name = "minMomSlider";
        maxMomSlider.name = "maxMomSlider";
        dataSetBoxList.name = "dataSetBoxList";
	}
	
	// Update is called once per frame
	void Update () {
        minMomVal.text = minMomSlider.value.ToString();
        maxMomVal.text = maxMomSlider.value.ToString();

        minMomentum = minMomSlider.value;
        maxMomentum = maxMomSlider.value;
        string option = dataSetBoxList.options[dataSetBoxList.value].text;
        
        if (!option.Equals("All sets"))
            fPID = int.Parse(option.Substring(6));
        else fPID = -1;
    }
}
