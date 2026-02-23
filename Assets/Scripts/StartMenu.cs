using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [Header("Character Selection")]
    public Image characterDisplay;
    public Sprite[] characterSprites;
    public int currentCharacterIndex = 0;

    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button startGameButton;
    public Button instructionButton;
    public Button highScoreButton;

    [Header("Scene Names")]
    public string gameSceneName = "TransitionScene";
    public string instructionSceneName = "InstructionScene";
    public string highScoreSceneName = "HighScoreScene";

    void Start()
    {
        UpdateCharacterDisplay();

        if (leftButton != null)
            leftButton.onClick.AddListener(PreviousCharacter);

        if (rightButton != null)
            rightButton.onClick.AddListener(NextCharacter);

        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);

        if (instructionButton != null)
            instructionButton.onClick.AddListener(OpenInstruction);

        if (highScoreButton != null)
            highScoreButton.onClick.AddListener(OpenHighScore);
    }


    public void PreviousCharacter()
    {
        if (characterSprites.Length == 0) return;

        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
            currentCharacterIndex = characterSprites.Length - 1;

        UpdateCharacterDisplay();
    }

    public void NextCharacter()
    {
        if (characterSprites.Length == 0) return;

        currentCharacterIndex++;
        if (currentCharacterIndex >= characterSprites.Length)
            currentCharacterIndex = 0;

        UpdateCharacterDisplay();
    }

    public void UpdateCharacterDisplay()
    {
        if (characterSprites.Length > 0 && characterDisplay != null)
        {
            characterDisplay.sprite = characterSprites[currentCharacterIndex];
        }
    }

    public void StartGame()
    {
        Debug.Log("Start Game Button Clicked!");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenInstruction()
    {
        Debug.Log("Opening Instruction Scene...");
        SceneManager.LoadScene(instructionSceneName);
    }

    public void OpenHighScore()
    {
        Debug.Log("Opening High Score Scene...");
        SceneManager.LoadScene(highScoreSceneName);
    }
}
