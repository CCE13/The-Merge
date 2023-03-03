using System.Collections;
using UnityEngine;
using Cubes;

public class RespawnManager : MonoBehaviour
{
    public float secondsToWait;
    public int numberOfDeath;
    public static RespawnManager instance;
    public static bool sRespawning;

    private void Start()
    {
        instance = this;
        sRespawning = false;
    }

    //Respawns the cube provided to its original starting positing.
    //the isCube bool and isDeath is to represent the main player, where the number of deaths will be counted.
    public void Respawning(GameObject cube, Vector3 startPos,Material material, bool isCube, bool isDeath)
    {
        if (isCube && sRespawning) { return; }
        StartCoroutine(Respawn(cube, startPos,material, isCube,isDeath));
    }
    private IEnumerator Respawn(GameObject cube, Vector3 startPos, Material material, bool isCube, bool isDeath)
    {

        if (isCube)
        {
            sRespawning = true;
        }
        var outline = cube.GetComponent<Outline>();
        if(outline != null)
        {
            outline.OutlineWidth = 0f;
        }
        if (CubeMergedController.S_MergeInControl && isCube)
        {
            MergingManager.instance.Seperate(MergingManager.S_CubeIsBeside);
        }
        for (float i = -0.2f; i < 1; i += Time.deltaTime)
        {
            material.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(material.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
            yield return new WaitForSeconds(0.0005f);
        }

        cube.SetActive(false);
        ResetCube(cube, startPos);
        cube.SetActive(true);
        yield return new WaitForSeconds(secondsToWait);
        for (float i = 1; i > -0.2; i -= Time.deltaTime)
        {
            material.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(material.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
            yield return new WaitForSeconds(0.0005f);

        }
        if (isCube)
        {
            if (isDeath)
            {
                numberOfDeath++;
            }   
            sRespawning = false;
        }
    }

    //resets the cube to its original position, rotation and scale. Freezes the cube on all its rotations and its x and y positions.
    private void ResetCube(GameObject cube, Vector3 startPos)
    {
        cube.transform.parent = null;
        cube.transform.position = startPos;
        cube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cube.transform.rotation = Quaternion.Euler(Vector3.zero);
        cube.transform.localScale = new Vector3(1, 1, 1);
    }
}