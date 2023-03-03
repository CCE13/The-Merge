using System.Collections;
using UnityEngine;

public class MovingTerrainController : MonoBehaviour
{
    protected Vector3 doorPos;

   [SerializeField] protected Vector3 targetPos;
    public Vector3 startPos
    {
        get;
        private set;
    }

    public float openRate;
    public bool terrainMoving;


    // Start is called before the first frame update


    private void Start()
    {
        startPos = transform.position;
    }

    //opening the door.
    public IEnumerator DoorOpen(Vector3 beginPos, Vector3 endPos, float time)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            transform.position = Vector3.Lerp(beginPos, endPos, t);
            doorPos = transform.position;
            if (!terrainMoving)
            {
                break;
            }
            yield return null;
        }
    }

    //closing the door.
    public IEnumerator DoorClose(Vector3 beginPos, Vector3 endPos, float time)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            transform.position = Vector3.Lerp(beginPos, endPos, t);
            doorPos = transform.position;
            if (terrainMoving)
            {
                break;
            }
            yield return null;
        }
    }
}