using UnityEngine;

public class BossTestDamage : MonoBehaviour
{
    private BossHealth bossHealth;

    void Start()
    {
        bossHealth = GetComponent<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogError("BossHealth component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && bossHealth != null)
        {
            bossHealth.TakeDamage(20);
        }
    }
}
