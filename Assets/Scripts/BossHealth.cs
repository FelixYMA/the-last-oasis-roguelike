using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private Slider m_HealthSlider;

    private void Start()
    {
        currentHealth = maxHealth;
        m_HealthSlider = GetComponentInChildren<Slider>();
        m_HealthSlider.maxValue = maxHealth;
        m_HealthSlider.value = currentHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (m_HealthSlider) m_HealthSlider.value = currentHealth;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject);
        // 出现宝箱
        // SceneManager.LoadScene("Level2");
    }
}
