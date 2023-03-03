using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cubes;

public class ButtonEventManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent
    {
        public string name;
        public List<GameObject> buttonsRequired;
        public bool allButtonsPressed;
        public UnityEvent onPressed;
        public UnityEvent onExit;
    }

    public ButtonEvent[] buttonEvents;
    // Update is called once per frame
    void Update()
    {
        //goes through each buttonEvent class to check if the buttons are pressed and if an event should occur.
        foreach (ButtonEvent buttonEvents in buttonEvents)
        {
            CheckIfAllButtonsPressed(buttonEvents);
            EventPlayer(buttonEvents);
        }
        
    }

    //check if all the buttons in the buttonevent class is pressed

    public void CheckIfAllButtonsPressed(ButtonEvent buttonEvents)
    {
        foreach (GameObject button in buttonEvents.buttonsRequired)
        {
            if (button.GetComponent<ButtonController>().buttonPressed)
            {
                buttonEvents.allButtonsPressed = true;
            }
            else
            {
                buttonEvents.allButtonsPressed = false;
                break;
            }
        }
    }

    
    //invokes events when all buttons in the buttonevent class are pressed;
    public void EventPlayer(ButtonEvent buttonEvents)
    {
        if (buttonEvents.allButtonsPressed)
        {
            buttonEvents.onPressed?.Invoke();
            
        }
        else
        {
            buttonEvents.onExit?.Invoke();
        }
    }
}
