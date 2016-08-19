using UnityEngine;
using UnityEngine.UI;

//using Windows.Kinect;
using System.Collections;
using System;

namespace Volley
{
    public class VolleyGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
    {
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("Text to display gesture-listener messages and gesture information.")]
        public Text gestureInfo;

        [Header("Gestures to detect")]
        public KinectGestures.Gestures[] gesturesToDetect;

        [Tooltip("Message to display to prompt player to begin game.")]
        public string startMessage;

        [Tooltip("Message to display to prompt player to stand in front of camera")]
        public string findingMessage;

        // singleton instance of the class
        private static VolleyGestureListener instance = null;

        // private bool to track if progress message has been displayed
        private bool progressDisplayed;
        private float progressGestureTime;

        /// <summary>
        /// Gets the singleton CubeGestureListener instance.
        /// </summary>
        /// <value>The CubeGestureListener instance.</value>
        public static VolleyGestureListener Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Invoked when a new user is detected. Here you can start gesture tracking by invoking KinectManager.DetectGesture()-function.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userIndex">User index</param>
        public void UserDetected(long userId, int userIndex)
        {
            // the gestures are allowed for the primary user only
            KinectManager manager = KinectManager.Instance;
            if (!manager || (userIndex != playerIndex))
                return;

            GameManager gameManager = GameManager.Instance;

            Debug.Log("[UserDetected] PrimaryUserID: " + manager.GetPrimaryUserID() + ", DetectedUserID: " + userId);

            foreach(var gesture in gesturesToDetect)
                manager.DetectGesture(userId, gesture);

            gameManager.SpawnPlayer(playerIndex);

            if (gestureInfo != null)
            {
                gestureInfo.text = startMessage;
                AudioManager.Instance.PlayOneShot(SoundType.HumanGruntOk);
                gameManager.SetState(GameState.Pregame);
            }
        }

        /// <summary>
        /// Invoked when a user gets lost. All tracked gestures for this user are cleared automatically.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userIndex">User index</param>
        public void UserLost(long userId, int userIndex)
        {
            KinectManager manager = KinectManager.Instance;
            GameManager gameManager = GameManager.Instance;

            if (gestureInfo != null)
            {
                gestureInfo.text = findingMessage;                  // Show waiting for users feedback
                gameManager.SetState(GameState.Waiting);   // Set game state to waiting
            }

            gameManager.RemovePlayer(playerIndex);
        }

        /// <summary>
        /// Invoked when a gesture is in progress.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userIndex">User index</param>
        /// <param name="gesture">Gesture type</param>
        /// <param name="progress">Gesture progress [0..1]</param>
        /// <param name="joint">Joint type</param>
        /// <param name="screenPos">Normalized viewport position</param>
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

        /// <summary>
        /// Invoked if a gesture is completed.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="userId">User ID</param>
        /// <param name="userIndex">User index</param>
        /// <param name="gesture">Gesture type</param>
        /// <param name="joint">Joint type</param>
        /// <param name="screenPos">Normalized viewport position</param>
        public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                      KinectInterop.JointType joint, Vector3 screenPos)
        {
            // the gestures are allowed for the primary user only
            if (userIndex != playerIndex)
                return false;

            if (progressDisplayed)
                return true;

            if (gestureInfo != null)
            {
                string sGestureText = gesture + " detected";
                if (gesture == KinectGestures.Gestures.UnderhandLeftToss)
                    VolleySpawner.Instance.Spawn(KinectManager.Instance.GetJointKinectPosition(userId, (int)joint));

                gestureInfo.text = sGestureText;
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

