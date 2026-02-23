using GamePlay;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool isBoss;
    public int damage;
    private float m_Timer;

    private void Start()
    {
        var cur = (int)GameManager.Ist.curScene;
        if (damage < 1) damage = Random.Range(cur + (isBoss ? 5 : 1), 2 * cur + (isBoss ? 5 : 1)); // 1 - 15
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name} Attack at {other.name}, tag: {other.tag}");
        if (!other.CompareTag("Player") || Time.time - m_Timer < 1.5f) return;
        m_Timer = Time.time;
        other.gameObject.GetComponent<PlayerController>()?.TakeDamage(damage);
    }
}
