using GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameStartView : MonoBehaviour
    {
        [Header("UI")] 
        public GameObject instructionPanelGo, highScorePanelGo;
        public TMP_InputField playerNameInput;
        [Header("Character Selection")]
        public GameObject sword, spear;

        [Header("Buttons")]
        public Button leftButton;
        public Button rightButton;
        public Button startGameButton;
        public Button instructionButton;
        public Button highScoreButton;

        private void OnDestroy()
        {
            if (GameManager.Ist && GameManager.Ist.bgmAudio) GameManager.Ist.bgmAudio.Stop();
        }

        void Start()
        {
            if (GameManager.Ist)
            {
                GameManager.Ist.bgmAudio.clip = GameManager.Ist.startBgm;
                GameManager.Ist.bgmAudio.Play();
            }
            // SetActive
            if (instructionPanelGo) instructionPanelGo.SetActive(false);
            if (highScorePanelGo) highScorePanelGo.SetActive(false);
            if (spear) spear.SetActive(false);
            if (sword) sword.SetActive(true);
            // AddListener
            if (leftButton) leftButton.onClick.AddListener(PreviousCharacter);
            if (rightButton) rightButton.onClick.AddListener(NextCharacter);
            if (startGameButton) startGameButton.onClick.AddListener(StartGame);
            if (instructionButton) instructionButton.onClick.AddListener(OpenInstruction);
            if (highScoreButton) highScoreButton.onClick.AddListener(OpenHighScore);
        }
        private void PreviousCharacter()
        {
            leftButton.enabled = false;
            spear.SetActive(false);
            sword.SetActive(true);
            GameManager.Ist.playerCharacter = "sword";
            leftButton.enabled = true;
        }
        private void NextCharacter()
        {
            rightButton.enabled = false;
            spear.SetActive(true);
            sword.SetActive(false);
            GameManager.Ist.playerCharacter = "spear";
            rightButton.enabled = true;
        }
        private void StartGame()
        {
            if (string.IsNullOrEmpty(playerNameInput.text))
            {
                UIManager.ShowBox("Please enter your name!").Forget();
                return;
            }
            if (PlayerPrefs.HasKey($"Score-{playerNameInput.text}"))
            {
                UIManager.ShowBox("The name already exists!").Forget();
                return;
            }
            GameManager.Ist.playerName = playerNameInput.text;
            GameManager.Ist.NextScene();
        }
        private void OpenInstruction()
        {
            instructionPanelGo.gameObject.SetActive(true);
        }
        private void OpenHighScore()
        {
            highScorePanelGo.gameObject.SetActive(true);
        }
    }
}
