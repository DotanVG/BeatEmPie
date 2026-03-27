using UnityEngine;
using UnityEngine.InputSystem;

namespace BeatEmPie
{
    /// <summary>
    /// Numpad music switcher for testing.
    /// Numpad 1-6 / Row 1-6: jump to specific track
    /// Numpad + / -: cycle forward / backward
    /// Song name appears top-right briefly then fades out.
    /// </summary>
    public class MusicDebugController : MonoBehaviour
    {
        [SerializeField] AudioClip[] tracks;
        [SerializeField] string[]    labels;

        [SerializeField] float displayDuration = 2.0f;
        [SerializeField] float fadeDuration    = 0.8f;

        int   current      = 0;
        float displayTimer = 0f;
        string displayText = "";

        // Cached key references
        Key[] numpadKeys = {
            Key.Numpad1, Key.Numpad2, Key.Numpad3,
            Key.Numpad4, Key.Numpad5, Key.Numpad6,
        };
        Key[] rowKeys = {
            Key.Digit1, Key.Digit2, Key.Digit3,
            Key.Digit4, Key.Digit5, Key.Digit6,
        };

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            int count = tracks != null ? Mathf.Min(tracks.Length, 6) : 0;

            // Numpad 1-6 or row 1-6
            for (int i = 0; i < count; i++)
            {
                if (kb[numpadKeys[i]].wasPressedThisFrame || kb[rowKeys[i]].wasPressedThisFrame)
                {
                    Play(i);
                    return;
                }
            }

            // Numpad + / -
            if (kb[Key.NumpadPlus].wasPressedThisFrame || kb[Key.Equals].wasPressedThisFrame)
            {
                Play((current + 1) % tracks.Length);
                return;
            }
            if (kb[Key.NumpadMinus].wasPressedThisFrame || kb[Key.Minus].wasPressedThisFrame)
            {
                Play((current - 1 + tracks.Length) % tracks.Length);
                return;
            }

            // Tick display timer
            if (displayTimer > 0f)
                displayTimer -= Time.unscaledDeltaTime;
        }

        void Play(int index)
        {
            if (AudioManager.Instance == null || tracks == null || index >= tracks.Length) return;
            current = index;
            AudioManager.Instance.CrossFade(tracks[index], 0.5f);

            displayText  = (labels != null && index < labels.Length) ? labels[index] : tracks[index]?.name ?? "";
            displayTimer = displayDuration + fadeDuration;

            Debug.Log($"[Music] [{index + 1}] {displayText}");
        }

        void OnGUI()
        {
            if (displayTimer <= 0f) return;

            float alpha = displayTimer > fadeDuration
                ? 1f
                : displayTimer / fadeDuration;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
            };
            style.normal.textColor = new Color(1f, 1f, 0.7f, alpha);

            float w = 320f, h = 30f;
            float x = Screen.width - w - 16f;
            float y = 16f;

            // Shadow
            var shadowStyle = new GUIStyle(style);
            shadowStyle.normal.textColor = new Color(0f, 0f, 0f, alpha * 0.6f);
            GUI.Label(new Rect(x + 1, y + 1, w, h), $"♪  {displayText}", shadowStyle);

            GUI.Label(new Rect(x, y, w, h), $"♪  {displayText}", style);
        }
    }
}
