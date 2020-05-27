using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public DialogueManager dialogueManager;

    private bool dialogueTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(dialogueTriggered != true)
        {
            TriggerDialogue();
            dialogueTriggered = true;
        }
    }

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }
}
