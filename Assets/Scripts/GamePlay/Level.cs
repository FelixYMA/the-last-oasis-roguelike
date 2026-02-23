using UnityEngine;

namespace GamePlay
{
    public class Level : MonoBehaviour
    {
        public GameObject sword, spear;

        private void OnDestroy()
        {
            if (GameManager.Ist && GameManager.Ist.bgmAudio) GameManager.Ist.bgmAudio.Stop();
            if (GameManager.Ist) GameManager.Ist.SaveScore();
        }

        private void Start()
        {
            if (GameManager.Ist)
            {
                GameManager.Ist.bgmAudio.clip = GameManager.Ist.curScene == SceneName.Level5 ? GameManager.Ist.level5Bgm : GameManager.Ist.levelBgm;
                GameManager.Ist.bgmAudio.Play();
            }
            sword.SetActive(false);
            spear.SetActive(false);
            // Init
            InitPlayer();
        }

        private void InitPlayer()
        {
            if (GameManager.Ist.playerCharacter.Equals("spear"))
            {
                spear.SetActive(true);
                sword.SetActive(false);
            }
            else
            {
                sword.SetActive(true);
                spear.SetActive(false);
            }
        }
    }
}