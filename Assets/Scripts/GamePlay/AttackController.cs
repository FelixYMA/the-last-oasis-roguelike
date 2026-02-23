using UI;
using UnityEngine;

namespace GamePlay
{
    public class AttackController : MonoBehaviour
    {
        private float m_Timer;
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"Player Attack at {other.name}, tag: {other.tag}");
            if (!other.CompareTag("Enemy") || Time.time - m_Timer < 1f) return;
            m_Timer = Time.time;
            GameManager.Ist.uiAudio.PlayOneShot(GameManager.Ist.hitAudio);
            GameUI.Ist.HitNumber(other.transform.position, GameUI.Ist.attack).Forget();
            other.gameObject.GetComponent<Enemy>()?.TakeDamage(GameUI.Ist.attack);
            other.gameObject.GetComponent<BossL4>()?.TakeDamage(GameUI.Ist.attack);
            other.gameObject.GetComponent<BossL5>()?.TakeDamage(GameUI.Ist.attack);
        }
    }
}
