using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cubes;

public class CarryObject : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        //parents the tagged object.
        if (other.CompareTag("PushCube"))
        {
            other.transform.parent = this.transform;
        }

        //checks if the cube is merged.
        if (other.CompareTag("OtherCube") && !MergingManager.S_IsMerged)
        {
            other.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //removes the tagged object from this object.
        if (other.CompareTag("PushCube"))
        {
            other.transform.parent = null;
        }
        if (other.CompareTag("OtherCube") && !MergingManager.S_IsMerged)
        {
            other.transform.parent = null;
        }
    }
}
