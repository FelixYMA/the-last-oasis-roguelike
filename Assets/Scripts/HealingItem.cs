using UnityEngine;
using TMPro;

public class HealingItem : MonoBehaviour
{
    public int healAmount = 10;
    public TextMeshProUGUI messageText;
    public float displayDuration = 2f;

    void Start(){
        if(messageText!=null){
            messageText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMessage("+ " + healAmount.ToString());
            Destroy(gameObject);
        }
    }

    void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
            Invoke(nameof(HideMessage), displayDuration);
        }
    }

    void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}
