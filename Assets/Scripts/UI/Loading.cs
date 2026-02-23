using GamePlay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Loading : MonoBehaviour
    {
        public RectTransform txtRt;
        public Text txt;
        private float m_Timer;
        private void Start()
        {
            m_Timer = Time.time;
            txt.gameObject.SetActive(false);
            txt.text = GameManager.Ist.curScene switch
            {
                SceneName.Level1 => "Level 1",
                SceneName.Level2 => "Level 2",
                SceneName.Level3 => "Level 3",
                SceneName.Level4 => "Level 4",
                SceneName.Level5 => "Level 5",
                _ => txt.text
            };
            txt.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Time.time - m_Timer >= 2.0f) SceneManager.LoadScene(GameManager.Ist.curScene.ToString());
        }
    }
}
