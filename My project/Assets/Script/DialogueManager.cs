using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public CanvasGroup dialogueCanvas;
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;

    public MonoBehaviour playermovement;
    public Rigidbody2D rb;
    public Animator playeranim;

    private void Awake()
    {
        Instance = this; // cukup ini
        lines = new Queue<DialogueLine>();
        HideDialogue();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // 🔥 UI HILANG? STOP AMAN
        if (dialogueCanvas == null || dialogueArea == null)
        {
            Debug.LogWarning("UI Dialogue tidak ada, dialog dibatalkan");
            return;
        }

        if (dialogue == null || dialogue.dialogueLines.Count == 0)
            return;

        isDialogueActive = true;
        ShowDialogue();

        if (playermovement) playermovement.enabled = false;
        if (rb) rb.velocity = Vector2.zero;
        if (playeranim) playeranim.enabled = false;

        lines.Clear();

        foreach (var line in dialogue.dialogueLines)
            lines.Enqueue(line);

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (!isDialogueActive || lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.line));
    }

    IEnumerator TypeSentence(string sentence)
    {
        if (dialogueArea == null) yield break;

        dialogueArea.text = "";

        foreach (char letter in sentence)
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        StopAllCoroutines();
        HideDialogue();

        if (playermovement) playermovement.enabled = true;
        if (playeranim) playeranim.enabled = true;
    }

    void ShowDialogue()
    {
        dialogueCanvas.alpha = 1;
        dialogueCanvas.interactable = true;
        dialogueCanvas.blocksRaycasts = true;
    }

    void HideDialogue()
    {
        dialogueCanvas.alpha = 0;
        dialogueCanvas.interactable = false;
        dialogueCanvas.blocksRaycasts = false;
    }

    // 🔥 DIPANGGIL SAAT PLAYER MATI
    public void CancelDialogue()
    {
        StopAllCoroutines();
        HideDialogue();
        isDialogueActive = false;
    }
}
