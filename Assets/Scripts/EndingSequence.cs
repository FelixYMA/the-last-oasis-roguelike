using System.Collections;
using TMPro;
using UnityEngine;
using System;
using GamePlay;

public class EndingSequence : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;
    public string[] dialogueLines;
    public event Action OnEndingDialogueComplete;

    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
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
            GameManager.Ist.RegisterEndingSequence(this);
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

        yield return new WaitForSeconds(1.5f);
        DisplayNextLine();
    }

    private void FinishDialogue()
    {
        OnEndingDialogueComplete?.Invoke();
    }

    public void DisplayNextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            FinishDialogue();
        }
    }
}
