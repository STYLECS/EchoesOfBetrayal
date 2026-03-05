using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;

    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    [Header("Trigger Settings")]
    public bool useOnce = true; // ✅ pilih di Inspector

    private bool hasTriggered = false;

    public void TriggerDialogue()
    {
        // 🔒 pengaman agar tidak spam
        if (useOnce && hasTriggered) return;

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager belum ada di Scene!");
            return;
        }

        if (dialogue == null || dialogue.dialogueLines == null || dialogue.dialogueLines.Count == 0)
        {
            Debug.LogError("Dialogue belum diisi atau kosong!");
            return;
        }

        DialogueManager.Instance.StartDialogue(dialogue);

        if (useOnce)
        {
            hasTriggered = true;
            Destroy(gameObject); // 🔥 hilang setelah dipakai
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
}
