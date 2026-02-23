using UnityEngine;
using TMPro;

public class ControlHint : MonoBehaviour
{
    public TextMeshProUGUI hintText;

    void Start()
    {
        hintText.text = "Enter - Heavy Attack\nRight Shift - Light Attack";
    }

    public void HideHint()
    {
        hintText.gameObject.SetActive(false);
    }

    public void ShowHint()
    {
        hintText.gameObject.SetActive(true);
    }
}
