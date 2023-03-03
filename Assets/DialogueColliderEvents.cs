using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cubes;

public class DialogueColliderEvents : MonoBehaviour
{
    public UnityEvent onPressed;
    public UnityEvent onExit;
    public bool played
    {
        get;
        private set;
    }

    public bool playOnExit;
    private DialogueController _dialogue;

    

    private void Start()
    {
        played = false;
        _dialogue = GetComponent<DialogueController>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (playOnExit) { return; }
        if (played) { return; }
        //invokes event when the cube enters the trigger.
        if (other.CompareTag("Cube"))
        {
            _dialogue.StartDialogue();
            onPressed?.Invoke();
            played = true;
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (!playOnExit) { return; }
        if (played) { return; }

        //invokes event when the cube exits the trigger.
        if (other.CompareTag("Cube"))
        {
            _dialogue.StartDialogue();
            onExit?.Invoke();
            played = true;
        }

    }
}
