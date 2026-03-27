using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Numpad music switcher for testing.
    /// Numpad 1-6: jump to specific track
    /// Numpad + / -: cycle forward / backward
    /// </summary>
    public class MusicDebugController : MonoBehaviour
    {
        [SerializeField] AudioClip[] tracks;  // assign in order: MainMenu, Calm, Intense, Boss, Victory, GameOver
        [SerializeField] string[]    labels;

        int current = 0;

        void Update()
        {
            // Numpad 1-6 — direct jump
            for (int i = 0; i < Mathf.Min(tracks.Length, 6); i++)
            {
                if (Input.GetKeyDown(KeyCode.Keypad1 + i))
                    Play(i);
            }

            // Numpad + / - — cycle
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                Play((current + 1) % tracks.Length);

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                Play((current - 1 + tracks.Length) % tracks.Length);
        }

        void Play(int index)
        {
            if (AudioManager.Instance == null || tracks == null || index >= tracks.Length) return;
            current = index;
            AudioManager.Instance.CrossFade(tracks[index], 0.5f);
            string label = (labels != null && index < labels.Length) ? labels[index] : tracks[index]?.name;
            Debug.Log($"[Music] Now playing [{index + 1}]: {label}");
        }

        void OnGUI()
        {
            GUI.color = new Color(1, 1, 1, 0.75f);
            GUILayout.BeginArea(new Rect(10, 10, 280, 180));
            GUILayout.Label("<b>🎵 Music Debug (Numpad)</b>");
            if (tracks != null)
            {
                for (int i = 0; i < tracks.Length; i++)
                {
                    string label = (labels != null && i < labels.Length) ? labels[i] : tracks[i]?.name;
                    string marker = i == current ? "▶ " : "   ";
                    GUILayout.Label($"{marker}[{i + 1}] {label}");
                }
            }
            GUILayout.Label("  [+] Next   [-] Prev");
            GUILayout.EndArea();
        }
    }
}
