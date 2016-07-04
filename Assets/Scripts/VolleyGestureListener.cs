using UnityEngine;
using UnityEngine.UI;

//using Windows.Kinect;
using System.Collections;
using System;

namespace Volley
{
    public class VolleyGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
    {
        [Tooltip("Text to display gesture-listener messages and gesture information.")]
        public Text gestureInfo;

        [Header("Gestures to detect")]
        public KinectGestures.Gestures[] gesturesToDetect;

        [Tooltip("Message to display to prompt player to begin game.")]
        public string startMessage;

        [Tooltip("Message to display to prompt player to stand in front of camera")]
        public string findingMessage;

        // private bool to track if progress message has been displayed
        private bool progressDisplayed;
        private float progressGestureTime;


        public void UserDetected(long userId, int userIndex)
        {
            // as an example - detect these user specific gestures
            KinectManager manager = KinectManager.Instance;

            // Check if primary user still exists
            

            foreach(var gesture in gesturesToDetect)
                manager.DetectGesture(userId, gesture);

            if (gestureInfo != null)
            {
                gestureInfo.text = startMessage;
            }
        }

        public void UserLost(long userId, int userIndex)
        {
            if (gestureInfo != null)
            {
                gestureInfo.text = findingMessage;
            }

            // Reset score
            GameManager.Instance.Reset();
        }

        public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture,
                                      float progress, KinectInterop.JointType joint, Vector3 screenPos)
        {
            if ((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
            {
                if (gestureInfo != null)
                {
                    string sGestureText = string.Format("{0} - {1:F0}%", gesture, screenPos.z * 100f);
                    gestureInfo.text = sGestureText;

                    progressDisplayed = true;
                    progressGestureTime = Time.realtimeSinceStartup;
                }
            }
            else if ((gesture == KinectGestures.Gestures.Wheel) && progress > 0.5f)
            {
                if (gestureInfo != null)
                {
                    string sGestureText = string.Format("{0} - {1:F0} degrees", gesture, screenPos.z);
                    gestureInfo.text = sGestureText;

                    progressDisplayed = true;
                    progressGestureTime = Time.realtimeSinceStartup;
                }
            }
            else if ((gesture == KinectGestures.Gestures.HandsTogether) && progress > 0.5f)
            {
                if (gestureInfo != null)
                {
                    string sGestureText = string.Format("{0} - distance {1:F2}%", gesture, screenPos.z);
                    gestureInfo.text = sGestureText;

                    progressDisplayed = true;
                    progressGestureTime = Time.realtimeSinceStartup;
                }
            }
        }

        public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                      KinectInterop.JointType joint, Vector3 screenPos)
        {
            if (progressDisplayed)
                return true;

            string sGestureText = gesture + " detected";
            if (gestureInfo != null)
            {
                if (gesture == KinectGestures.Gestures.UnderhandLeftToss)
                {
                    GameObject ball = VolleySpawner.Instance.Spawn(KinectManager.Instance.GetJointKinectPosition(userId, (int)joint));
                    //Logger.Log("Spawning " + ball.GetHashCode());
                }

                //gestureInfo.text = sGestureText;
            }



            return true;
        }

        public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
                                      KinectInterop.JointType joint)
        {
            if (progressDisplayed)
            {
                progressDisplayed = false;

                if (gestureInfo != null)
                {
                    gestureInfo.text = string.Empty;
                }
            }

            if (gesture == KinectGestures.Gestures.HandsTogether)
            {
                if (gestureInfo != null)
                {
                    string sGestureText = gesture + " cancelled";
                    gestureInfo.text = sGestureText;
                }
            }

            return true;
        }

        public void Update()
        {
            if (progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
            {
                progressDisplayed = false;

                if (gestureInfo != null)
                {
                    gestureInfo.text = string.Empty;
                }

                Debug.Log("Forced progress to end.");
            }
        }

    }
}

