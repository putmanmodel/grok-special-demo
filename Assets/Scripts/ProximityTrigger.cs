using UnityEngine;

[RequireComponent(typeof(SpandaNPC))]
public class ProximityTrigger : MonoBehaviour
{
    private SpandaNPC selfNPC;
    private MemoryComponent selfMemory;

    private void Awake()
    {
        selfNPC = GetComponent<SpandaNPC>();
        selfMemory = GetComponent<MemoryComponent>();

        if (selfMemory == null)
            Debug.LogError($"{name} missing MemoryComponent.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") && other != this)
        {
            SpandaNPC otherNPC = other.GetComponent<SpandaNPC>();
            if (otherNPC != null && selfMemory != null)
            {
                Debug.Log($"[Trigger] {selfNPC.name} entered proximity of {otherNPC.name}");

                float[] sourceVector = selfMemory.currentVector;
                otherNPC.ReceivePulse(sourceVector, this.gameObject);
            }
        }
    }
}