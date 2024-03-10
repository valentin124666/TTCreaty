using UnityEngine;

namespace Tools
{
    public class TouchUtility : MonoBehaviour
    {
        public static bool Enabled = true;

        public static int TouchCount
        {
            get
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Input.touchCount;
#else
                if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0)  || Input.GetMouseButtonUp(0))
                    return 1;
                
                return 0;
#endif
            }
        }

        public static Touch GetTouch(int index)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Input.GetTouch(index);
#else
            var touch = new Touch();

            if (index != 0) return touch;
            
            if (Input.GetMouseButtonDown(0))
            {
                touch.position = Input.mousePosition;
                touch.phase = TouchPhase.Began;
                touch.fingerId = 0;
            }
            else if (Input.GetMouseButton(0))
            {
                touch.position = Input.mousePosition;
                touch.phase = TouchPhase.Moved;
                touch.fingerId = 0;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                touch.position = Input.mousePosition;
                touch.phase = TouchPhase.Ended;
                touch.fingerId = 0;
            }

            return touch;
#endif
        }
    }
}