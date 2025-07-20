using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoryLogUI : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public ScrollRect scrollRect;
    private string fullLog = "";

    public void AppendLog(
        string npcName,
        float value,
        string deltaString,
        string source,
        string reason)
    {
        // Compose your line
        string line = $"[{npcName}] axis[0]: {value:+0.00;-0.00;0.00}";
        fullLog += line + "\n";

        // Update the text field
        if (logText != null)
            logText.text = fullLog;

        // Force Unity to rebuild the layout, then scroll to top
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            // If your content pivot is top, normalizedPosition=1 is the top
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
