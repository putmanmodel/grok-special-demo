using UnityEngine;
using TMPro;

public class ConflictMessageDisplay : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    private float displayTime = 2.5f;
    private float timer = 0f;

    void Awake()
    {
        if (messageText == null)
            messageText = GetComponentInChildren<TextMeshProUGUI>();

        messageText.text = "";
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && messageText != null)
            {
                messageText.text = "";
            }
        }
    }

    public void ShowConflictMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            timer = displayTime;
        }
    }
}