using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cubes;

public class DialogueController : MonoBehaviour
{
    public string Name;

    [TextArea(3, 10)]
    public string[] Dialogues;
    public float typingSpeed;
    public float timeBetweenDialogues;
    public static bool isRunning;

    public bool Running
    {
        get { return isRunning; }
    }
    public Text text;


    private int _index;
    // Start is called before the first frame update
    private void OnValidate()
    {
        //sets the game object name to the name tyed in the inspector.
        gameObject.name = Name;
    }
    void Start()
    {
        text.text = string.Empty;
    }
    public void StartDialogue()
    {
        _index = 0;
        isRunning = true;
        StartCoroutine(DialogueRunning());
    }


    IEnumerator DialogueRunning()
    {
        CubeController.sCanMove = false;
        CubeMergedController.canMove = false;

        foreach (char letter in Dialogues[_index].ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(timeBetweenDialogues);
        NextLine();
        
    }

    void NextLine()
    {
        //checks if there are any other dialogues to run
        if(_index< Dialogues.Length - 1)
        {
            _index++;
            text.text = string.Empty;
            StartCoroutine(DialogueRunning());
        }
        else
        {
            text.text = string.Empty;
            CubeController.sCanMove = true;
            CubeMergedController.canMove = true;
            isRunning = false;
        }
    }
}
