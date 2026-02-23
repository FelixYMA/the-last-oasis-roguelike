using UnityEngine;

namespace GamePlay
{
    public class Exit : MonoBehaviour
    {
        private bool m_PlayerInRange;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) m_PlayerInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) m_PlayerInRange = false;
        }

        private void Start()
        {
            m_PlayerInRange = false;
        }

        private void Update()
        {
            if (m_PlayerInRange && Input.GetKeyDown(KeyCode.E)) GameManager.Ist.NextScene();
        }
    }
}