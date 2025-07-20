using UnityEngine;

[RequireComponent(typeof(SpandaNPC))]
public class HarmonizationModule : MonoBehaviour
{
    [Header("Harmonization Settings")]
    public int axisIndex = 0;
    [Range(0f, 1f)] public float smoothingAlpha = 0.4f;
    public float decayRate = 1.0f;

    private SpandaNPC spandaNPC;
    private float currentHarmony;

    void Start()
    {
        spandaNPC = GetComponent<SpandaNPC>();
        currentHarmony = spandaNPC.memory.currentVector[axisIndex];
    }

    void Update()
    {
        // 1. Listen for pulses
        if (Input.GetKeyDown(KeyCode.Space))
            AddPulse(-1f);

        if (Input.GetKeyDown(KeyCode.Return))
            AddPulse(+1f);

        // 2. Apply decay toward zero (equilibrium)
        currentHarmony *= Mathf.Exp(-decayRate * Time.deltaTime);
        currentHarmony = Mathf.Clamp(currentHarmony, -1f, 1f);

        // 3. Write back and update visuals
        spandaNPC.memory.currentVector[axisIndex] = currentHarmony;
        spandaNPC.UpdateGlowOrbs();
    }

    void AddPulse(float pulse)
    {
        currentHarmony = Mathf.Lerp(currentHarmony, pulse, smoothingAlpha);
    }
}