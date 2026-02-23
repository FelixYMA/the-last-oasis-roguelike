using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage; 
    private PlayerController player;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    public void UpdateHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}
