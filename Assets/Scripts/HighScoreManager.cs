using UnityEngine;
using TMPro;
using System.Linq;
using GamePlay;
using Unity.Mathematics.Geometry;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentContainer; 
    public GameObject entryPrefab; 
    public Button backButton; 

    void Start()
    {
        if (backButton) backButton.onClick.AddListener(() => gameObject.SetActive(false));
        ShowScores();
    }

    void ShowScores()
    {
        var dict = GameManager.Ist.rankList.dict;
        if (dict.Count < 1) return;
        var pairs = dict.ToList();
        pairs.Sort((a, b) => b.Value.CompareTo(a.Value));
        var count = Mathf.Min(10, pairs.Count);
        for (var i = 0; i < count; i++)
        {
            var p = pairs[i];
            var entry = Instantiate(entryPrefab, contentContainer);
            var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts is {Length: >= 3})
            {
                texts[0].text = (i + 1).ToString();
                texts[1].text = p.Key;
                texts[2].text = p.Value.ToString();
            }
            var img = entry.GetComponent<Image>();
            if (img) img.color = i % 2 == 0 ? new Color(1, 1, 1, 0.22f) : new Color(1, 1, 1, 0);
            entry.SetActive(true);
        }
    }
}
