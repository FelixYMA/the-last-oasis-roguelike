using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;
    private bool isTalking = false;

    void Start()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] dialogueLines)
    {
        isTalking = true;
        dialoguePanel.SetActive(true);
        sentences.Clear();

        foreach (var line in dialogueLines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isTalking = false;
    }

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.E))
        {
            DisplayNextSentence();
        }
    }
}
