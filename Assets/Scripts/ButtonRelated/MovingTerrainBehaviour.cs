using UnityEngine;

public class MovingTerrainBehaviour : MovingTerrainController
{

    //moves the terrain to the transform position + targetPos.
    public void MoveTerrain()
    {
        if (transform.position == targetPos) { return; }
        terrainMoving = true;
        if (doorPos != Vector3.zero)
        {
            StartCoroutine(DoorOpen(doorPos, targetPos+startPos, openRate));
        }
        else
        {
            StartCoroutine(DoorOpen(transform.position, targetPos+startPos, openRate));
        }
    }

    //moves the terrain back to its original position.
    public void TerrainReturn()
    {
        terrainMoving = false;
        if (doorPos != Vector3.zero)
        {
            StartCoroutine(DoorClose(doorPos, startPos, openRate));
        }
        else
        {
            StartCoroutine(DoorClose(transform.position, startPos, openRate));
        }
    }
}