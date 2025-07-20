// Assets/Scripts/MemoryComponent.cs

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MemoryComponent : MonoBehaviour
{
    public float[] currentVector;
    public List<string> emotionalTags = new List<string>();
    public MemoryLogUI logUI;

    void Awake()
    {
        // Ensure we have a 10-element vector
        if (currentVector == null || currentVector.Length != 10)
            currentVector = new float[10];

        // Auto-wire the UI logger if not set in Inspector
        if (logUI == null)
            logUI = FindObjectOfType<MemoryLogUI>();
    }

    public void LogReverbEntry(
        string dsl,
        float[] delta,
        string reason,
        string[] tags,
        float intensity,
        string source)
    {
        // Only log NPC_A’s axis[0] to the scroll view
        if (gameObject.name != "NPC_A")
            return;

        float value = (currentVector != null && currentVector.Length > 0)
            ? currentVector[0]
            : 0f;

        string formatted = value >= 0
            ? $"+{value:F2}"
            : $"{value:F2}";

        Debug.Log($"[NPC_A] axis[0]: {formatted}");

        if (logUI != null)
            logUI.AppendLog("NPC_A", value, formatted, source, reason);
    }

    public void RememberTag(string tag)
    {
        if (!emotionalTags.Contains(tag))
            emotionalTags.Add(tag);
    }

    public bool HasEmotionalTag(string tag)
    {
        return emotionalTags.Contains(tag);
    }
}
