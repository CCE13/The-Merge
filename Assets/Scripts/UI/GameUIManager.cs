using UnityEngine;
using UnityEngine.UI;
using Cubes;

public class GameUIManager : MonoBehaviour
{
    public Image cubeStatus;

    [Header("Text Animations")]
    public Animator mergeText;
    public Animator seperateText;

    // Start is called before the first frame update
    private void Start()
    {
        cubeStatus.color = Color.red;
        MergingManager.canMerge += MergeChecker;
        MergingManager.canSeperate += SeperateChecker;
    }
    private void OnDestroy()
    {
        MergingManager.canMerge -= MergeChecker;
        MergingManager.canSeperate -= SeperateChecker;
    }

    public void MergeChecker(bool CubeCanMerge)
    {
        mergeText.SetBool("canMerge", CubeCanMerge);
        if (CubeCanMerge)
        {
            cubeStatus.color = Color.green;
        }
        else
        {
            cubeStatus.color = Color.red;
        }
    }

    public void SeperateChecker(bool CubeCanSeperate)
    {
        seperateText.SetBool("canSeperate", CubeCanSeperate);
        if (CubeCanSeperate)
        {
            cubeStatus.color = Color.green;
        }
        else
        {
            cubeStatus.color = Color.red;
        }
    }
}