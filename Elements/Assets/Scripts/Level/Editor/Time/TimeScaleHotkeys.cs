using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WordSorter.Core.Editor
{
    [InitializeOnLoad]
    public static class TimeScaleHotkeys
    {
        static TimeScaleHotkeys() => InputSystem.onAfterUpdate += Update;

        private static void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var focused = EditorWindow.focusedWindow;

            if (focused == null || focused.GetType().Name != "GameView")
            {
                return;
            }

            var keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return;
            }

            if (keyboard.qKey.wasPressedThisFrame)
            {
                SetTimeScale(0.01f);
            }

            if (keyboard.wKey.wasPressedThisFrame)
            {
                SetTimeScale(0.1f);
            }

            if (keyboard.eKey.wasPressedThisFrame)
            {
                SetTimeScale(0.5f);
            }

            if (keyboard.rKey.wasPressedThisFrame)
            {
                SetTimeScale(1f);
            }

            if (keyboard.tKey.wasPressedThisFrame)
            {
                SetTimeScale(2f);
            }

            if (keyboard.yKey.wasPressedThisFrame)
            {
                SetTimeScale(5f);
            }
        }

        private static void SetTimeScale(float value)
        {
            Time.timeScale = value;
            Debug.Log($"TimeScale set to {value}");
        }
    }
}