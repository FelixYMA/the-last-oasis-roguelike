using UI;
using UnityEngine;

namespace GamePlay
{
    public class Chest : MonoBehaviour
    {
        private static readonly int Open = Animator.StringToHash("Open");
        private Animator m_Animator;
        private bool m_PlayerInRange, m_IsOpen;
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
            m_Animator = GetComponent<Animator>();
            m_IsOpen = m_PlayerInRange = false;
        }

        private void Update()
        {
            if (m_PlayerInRange && !m_IsOpen && Input.GetKeyDown(KeyCode.E))
            {
                m_Animator.SetBool(Open, true);
                m_IsOpen = true;
                GameUI.Ist.ShowSelectPanel(true);
            }
            
        }
    }
}