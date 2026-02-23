using UnityEngine;
using TMPro;

public class ShowItemText : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public string itemMessage = "Find a Item with info！";
    public float displayDuration = 2f;

    private void Start()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMessage();
        }
    }

    void ShowMessage()
    {
        if (messageText != null)
        {
            messageText.text = itemMessage;
            messageText.gameObject.SetActive(true);
            Invoke("HideMessage", displayDuration);
        }
    }

    void HideMessage()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
}
