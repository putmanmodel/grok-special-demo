using UnityEngine;

public class OrbDemoManager : MonoBehaviour
{
    public SpandaNPC npcA;
    public SpandaNPC npcB;
    public SpandaNPC npcC;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float[] delta = new float[10];
            string[] syms = new string[10];
            for (int i = 0; i < 10; i++)
            {
                delta[i] = (i == 0) ? -1f : 0f;
                syms[i] = delta[i] < -0.5f ? "V" : "$";
            }

            npcA.ReceivePulse(delta, npcB.gameObject);
            npcA.memory.LogReverbEntry(string.Join(",", syms), delta, "negative spike", new[] { "demo" }, 1f, "manual");
            npcA.UpdateGlowOrbs();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            float[] delta = new float[10];
            string[] syms = new string[10];
            for (int i = 0; i < 10; i++)
            {
                delta[i] = (i == 0) ? 1f : 0f;
                syms[i] = delta[i] > 0.5f ? "A" : "$";
            }

            npcA.ReceivePulse(delta, npcC.gameObject);
            npcA.memory.LogReverbEntry(string.Join(",", syms), delta, "positive harmony pulse", new[] { "demo" }, 1f, "manual");
            npcA.UpdateGlowOrbs();
        }
    }
}