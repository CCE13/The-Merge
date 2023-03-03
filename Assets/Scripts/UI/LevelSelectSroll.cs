using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectSroll : MonoBehaviour
{
    [SerializeField]private ScrollRect scrollRect;
    [SerializeField]private float horizontalNormalizedPositionToAdd;


    private float _currentHorizontalPosition;
    private bool _isMoving;

    public static float S_savedPosition;

    // Start is called before the first frame update
    private void Start()
    {
        if (LevelManager.sLevelEnded){
            scrollRect.horizontalNormalizedPosition = S_savedPosition;
        }
        else
        {
            S_savedPosition = scrollRect.horizontalNormalizedPosition;
        }
        
    }

    //moves the scroll rect towards the right

    public void NextLevel()
    {
        if (scrollRect.horizontalNormalizedPosition >= 0.95) { return; }
        if (_isMoving) { return; }
        _currentHorizontalPosition = scrollRect.horizontalNormalizedPosition;
        StartCoroutine(ChangingLevel(_currentHorizontalPosition + horizontalNormalizedPositionToAdd));
    }

    //moves the scroll rect towards the left
    public void PreviousLevel()
    {
        if (scrollRect.horizontalNormalizedPosition <= 0.05) { return; }
        if (_isMoving) { return; }
        _currentHorizontalPosition = scrollRect.horizontalNormalizedPosition;
        StartCoroutine(ChangingLevel(_currentHorizontalPosition - horizontalNormalizedPositionToAdd));
    }
    private IEnumerator ChangingLevel(float targetPos)
    {
        _isMoving = true;
        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_currentHorizontalPosition, targetPos, t);
            yield return null;
        }   
        if (scrollRect.horizontalNormalizedPosition != targetPos)
        {
               scrollRect.horizontalNormalizedPosition = targetPos;
        }
        _isMoving = false;
        S_savedPosition = scrollRect.horizontalNormalizedPosition;
    }
}
