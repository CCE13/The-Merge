using UnityEngine;
using UnityEngine.Events;
using Cubes;
using UI;

public class ButtonController : MonoBehaviour
{
    public bool buttonPressed;


    private void Start()
    {
        UiController.restarting += ResetBool;
    }
    private void OnDestroy()
    {
        UiController.restarting -= ResetBool;
    }

    private void ResetBool()
    {
        buttonPressed = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cube")|| other.CompareTag("OtherCube")|| other.CompareTag("PushCube"))
        {
            var MergedCube = FindObjectOfType<MergedCubeBehaviour>();
            if (MergedCube != null && MergedCube.isFalling) { buttonPressed = false; return; }
            buttonPressed = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube") || other.CompareTag("OtherCube") || other.CompareTag("PushCube"))
        {
            buttonPressed = false;
        }
    }
}