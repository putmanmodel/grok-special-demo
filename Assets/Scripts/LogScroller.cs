using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LogScroller : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private ScrollRect scrollRect;

    public void AppendLog(string entry)
    {
        logText.text += entry + "\n";
        StartCoroutine(RefreshAndTopline());
    }

    private IEnumerator RefreshAndTopline()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        scrollRect.content.anchoredPosition = Vector2.zero;
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
