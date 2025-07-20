using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(MemoryComponent))]
public class SpandaNPC : MonoBehaviour
{
    public static bool demoOverrideOrbs = false;
    public static int[] demoOrbIndices = new int[] { 0 };

    [Header("Core Components")]
    public MemoryComponent memory;
    private AffinityModule affinityModule;
    private PersonalBubbleModule bubble;

    [Header("Body Visuals")]
    public Renderer bodyRenderer;
    public Color baseColor = Color.white;

    [Header("Glow Orb Settings")]
    public GameObject glowOrbPrefab;
    public Transform glowOrbAnchor;
    public GlowOrbController[] glowOrbControllers;

    [Header("Axis Labels (10)")]
    public string[] axisZones = new string[10]
    {
        "Love/Hate", "Harmony/Conflict", "Joy/Despair", "Courage/Fear", "Trust/Suspicion",
        "Loyalty/Betrayal", "Attraction/Repulsion", "Intimacy/Distance", "Peace/Chaos", "Gratitude/Resentment"
    };

    void Awake()
    {
        memory = GetComponent<MemoryComponent>();
        affinityModule = GetComponent<AffinityModule>();
        bubble = GetComponent<PersonalBubbleModule>();
    }

    void Start()
    {
        switch (name)
        {
            case "NPC_A": baseColor = new Color(1f, 0.7f, 0.7f); break;
            case "NPC_B": baseColor = new Color(0.6f, 0.6f, 1f); break;
            case "NPC_C": baseColor = new Color(0.6f, 1f, 0.6f); break;
            case "NPC_D": baseColor = new Color(1f, 1f, 0.6f); break;
            case "NPC_E": baseColor = new Color(1f, 0.6f, 1f); break;
            case "NPC_F": baseColor = new Color(1f, 0.8f, 0.4f); break;
        }

        if (bodyRenderer != null)
            bodyRenderer.material.color = baseColor;

        if (memory.currentVector == null || memory.currentVector.Length != axisZones.Length)
            memory.currentVector = new float[axisZones.Length];

        if (name == "NPC_F")
        {
            memory.currentVector = new float[10]
            { 0.4f, 0.3f, 0.5f, 0.2f, 0.1f, 0.3f, 0f, 0.2f, 0.1f, 0.3f };
            memory.emotionalTags = new List<string> { "friend", "protector", "safe" };
            memory.RememberTag("friend");
        }
        else if (name == "NPC_DEBUG")
        {
            memory.currentVector = new float[10]
            { 1f, 0.6f, 0.2f, -0.2f, -0.6f, -1f, 0.4f, -0.4f, 0f, 0.8f };
        }

        glowOrbControllers = new GlowOrbController[axisZones.Length];
        if (glowOrbPrefab != null && glowOrbAnchor != null)
        {
            for (int i = 0; i < axisZones.Length; i++)
            {
                if (demoOverrideOrbs && !demoOrbIndices.Contains(i))
                    continue;

                GameObject orb = Instantiate(glowOrbPrefab, glowOrbAnchor);
                orb.transform.localScale = Vector3.one * 0.2f;

                var ctrl = orb.GetComponent<GlowOrbController>();
                if (ctrl != null)
                {
                    ctrl.orbitCenter = transform;
                    ctrl.axisIndex = i;
                    ctrl.zone = axisZones[i];
                    ctrl.startAngleDeg = i * (360f / axisZones.Length);
                    ctrl.enableDebug = true;
                    glowOrbControllers[i] = ctrl;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) { FillAll(-0.9f); UpdateGlowOrbs(); }
        if (Input.GetKeyDown(KeyCode.R)) { FillAll(0.9f); UpdateGlowOrbs(); }
        if (Input.GetKeyDown(KeyCode.P)) { FillAll(0f); UpdateGlowOrbs(); }

        // 🚀 Manual test spike with Enter or Space
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            float[] delta = new float[10];
            string[] symbols = new string[10];
            float deltaSum = 0f;

            for (int i = 0; i < 10; i++)
            {
                delta[i] = (i == 0) ? 1f : 0f;
                deltaSum += Mathf.Abs(delta[i]);
                symbols[i] = delta[i] > 0.5f ? "A" : delta[i] < -0.5f ? "V" : "$";
                memory.currentVector[i] = Mathf.Clamp(memory.currentVector[i] + delta[i], -1f, 1f);
            }

            string dsl = string.Join(",", symbols);
            memory.LogReverbEntry(dsl, delta, "harmony pulse", new[] { "demo" }, deltaSum, "manual");
            UpdateGlowOrbs();
        }
    }

    void FillAll(float value)
    {
        for (int i = 0; i < memory.currentVector.Length; i++)
            memory.currentVector[i] = value;
    }

    public void UpdateGlowOrbs()
    {
        if (glowOrbControllers == null || memory?.currentVector == null) return;
        for (int i = 0; i < glowOrbControllers.Length; i++)
        {
            var orb = glowOrbControllers[i];
            if (orb != null)
                orb.SetValue(memory.currentVector[i]);
        }
    }

    public void ShowAllOrbs(bool state = true)
    {
        if (glowOrbControllers == null) return;
        foreach (var orb in glowOrbControllers)
            orb?.gameObject.SetActive(state);
    }

    public void ShowOnlyOrb(int axisIndex)
    {
        if (glowOrbControllers == null) return;
        for (int i = 0; i < glowOrbControllers.Length; i++)
            glowOrbControllers[i]?.gameObject.SetActive(i == axisIndex);
    }

    public void ReceivePulse(float[] sourceVector, GameObject sourceNPC)
    {
        if (memory == null || affinityModule == null) return;
        if (bubble == null || !bubble.CanReact()) return;

        float[] myVec = memory.currentVector;
        float affinityValue = affinityModule.CalculateAffinity(myVec, sourceVector);
        float dist = Vector3.Distance(transform.position, sourceNPC.transform.position);
        if (dist > bubble.bubbleRadius) return;

        bubble.MarkReacted();
        if (sourceNPC.name == "NPC_B")
            memory.RememberTag("bully");
        if (sourceNPC.name == "NPC_E" && memory.HasEmotionalTag("bully"))
            Debug.Log($"{name} reminded of previous bully by {sourceNPC.name}.");

        if (memory.HasEmotionalTag("friend") && sourceNPC.name == "NPC_F")
        {
            float healStrength = 0.25f;
            for (int i = 0; i < myVec.Length; i++)
            {
                float d = -Mathf.Sign(myVec[i]) * healStrength;
                myVec[i] = Mathf.Clamp(myVec[i] + d, -1f, 1f);
            }
            UpdateGlowOrbs();
            return;
        }

        float[] oldVec = (float[])myVec.Clone();
        float[] deltaVec = new float[myVec.Length];
        string[] shortSymbols = new string[myVec.Length];
        float deltaSum = 0f;

        for (int i = 0; i < myVec.Length; i++)
        {
            float mod = sourceVector[i]
                      * (1f - bubble.pulseResistance)
                      * bubble.receptivityIndex;
            if (bubble.pulseReinforcementFactor > 0f)
                mod += sourceVector[i] * bubble.pulseReinforcementFactor;

            float newVal = Mathf.Clamp(myVec[i] + mod, -1f, 1f);
            deltaVec[i] = newVal - myVec[i];
            myVec[i] = newVal;

            deltaSum += Mathf.Abs(deltaVec[i]);
            shortSymbols[i] = deltaVec[i] > 0.5f ? "A" : deltaVec[i] < -0.5f ? "V" : "$";
        }

        if (deltaSum > 0.01f)
        {
            string dsl = string.Join(",", shortSymbols);
            memory.LogReverbEntry(dsl, deltaVec, "harmony pulse", new[] { "reacted" }, deltaSum, sourceNPC.name);
        }

        UpdateGlowOrbs();
    }
}