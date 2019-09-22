//-----------------------------------------------------------------------
// <copyright file="AugmentedImageExampleController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.AugmentedImage
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controller for AugmentedImage example.
    /// </summary>
    /// <remarks>
    /// In this sample, we assume all images are static or moving slowly with
    /// a large occupation of the screen. If the target is actively moving,
    /// we recommend to check <see cref="AugmentedImage.TrackingMethod"/> and
    /// render only when the tracking method equals to
    /// <see cref="AugmentedImageTrackingMethod"/>.<c>FullTracking</c>.
    /// See details in <a href="https://developers.google.com/ar/develop/c/augmented-images/">
    /// Recognize and Augment Images</a>
    /// </remarks>
    public class AugmentedImageExampleController : MonoBehaviour
    {
        /// <summary>
        /// A prefab for visualizing an AugmentedImage.
        /// </summary>
        //public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;

        /// <summary>
        /// The overlay containing the fit to scan user guide.
        /// </summary>
        public GameObject FitToScanOverlay;

        private Lines lines;
        private bool isHit = false;
        private Dictionary<string, int> values;

        private Dictionary<int, AugmentedImageVisualizer> m_Visualizers
            = new Dictionary<int, AugmentedImageVisualizer>();

        private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();
        public void Start()
        {
            lines = new Lines();
            values = new Dictionary<string, int>();
            for (int i = 1; i <= 31; i++)
            {
                values.Add(i.ToString(), 0);
                values.Add(i.ToString() + " emgu", 0);
                values.Add(i.ToString() + " eq", 0);
            }
        }

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Enable ARCore to target 60fps camera capture frame rate on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;
        }

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            // Get updated augmented images for this frame.
            Session.GetTrackables<AugmentedImage>(
                m_TempAugmentedImages, TrackableQueryFilter.Updated);

            //Debug.Log("Before");

            if (!isHit)
            // Create visualizers and anchors for updated augmented images that are tracking and do
            // not previously have a visualizer. Remove visualizers for stopped images.
            {
                foreach (var image in m_TempAugmentedImages)
                {
                    Debug.Log("Tracking " + image.Name);

                    AugmentedImageVisualizer visualizer = null;
                    m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);

                    Debug.Log("DB index " + image.DatabaseIndex);
                    Debug.Log("DB trackingState " + image.TrackingState.ToString());

                    if (image.TrackingState == TrackingState.Tracking && visualizer == null)
                    {
                        //Debug.Log("HEREEEEEEEEEEe");
                        // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                        Anchor anchor = image.CreateAnchor(image.CenterPose);

                        GameObject simulation = new GameObject();
                        simulation.transform.parent = anchor.transform;

                        //visualizer = (AugmentedImageVisualizer)Instantiate(
                        //AugmentedImageVisualizerPrefab, anchor.transform);
                        visualizer = new AugmentedImageVisualizer();
                        visualizer.Image = image;
                        m_Visualizers.Add(image.DatabaseIndex, visualizer);

                        Debug.Log("FOUND " + image.Name);
                        values[image.Name]++;
                        //string text = "";

                        //foreach (string value in values.Keys)
                        //{
                        //    text = text + value + " " + values[value] + "\n";
                        //}

                        //Debug.Log(text);
                        if (!isHit)
                        {
                            Debug.Log("Starting");

                            StartCoroutine(lines.DrawLines(simulation, anchor.transform.position, new GameObject(), anchor.transform.rotation));
                            isHit = true;
                        }
                    }
                    //else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
                    //{
                    //    m_Visualizers.Remove(image.DatabaseIndex);
                    //    GameObject.Destroy(visualizer.gameObject);
                    //}
                }
            }

            // Show the fit-to-scan overlay if there are no images that are Tracking.
            //foreach (var visualizer in m_Visualizers.Values)
            //{
            //    if (visualizer.Image.TrackingState == TrackingState.Tracking)
            //    {
            //        FitToScanOverlay.SetActive(false);
            //        return;
            //    }
            //}

            //FitToScanOverlay.SetActive(true);
        }
    }
}
