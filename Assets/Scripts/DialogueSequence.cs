using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using GamePlay;


public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;
    public string[] dialogueLines;
    public event Action OnDialogueComplete;


    public Button choiceButton;

    private int index = 0;
    private bool isTyping = false;
    private bool waitingForChoice = false;

    void Start()
    {
        choiceButton.gameObject.SetActive(false);

        if (dialogueLines.Length > 0)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            Debug.LogWarning("Dialogue lines are empty! Please assign dialogue lines in the inspector.");
        }

        if (GameManager.Ist != null)
        {
        GameManager.Ist.RegisterDialogueSystem(this);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTyping && !waitingForChoice)
        {
            DisplayNextLine();
        }
    }

    IEnumerator TypeLine()
    {
        if (index < 0 || index >= dialogueLines.Length)
        {
            Debug.LogError("Dialogue index is out of bounds.");
            yield break;
        }

        isTyping = true;
        dialogueText.text = "";

        foreach (char c in dialogueLines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (index == 3) 
        {
            waitingForChoice = true;
            choiceButton.gameObject.SetActive(true);
        }
    }

    private void FinishDialogue()
    {
        OnDialogueComplete?.Invoke();
    }

    public void DisplayNextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else{
            FinishDialogue();
        }
    }

    public void OnChoiceClicked()
    {
        choiceButton.gameObject.SetActive(false);
        waitingForChoice = false;
        DisplayNextLine();
    }
}
