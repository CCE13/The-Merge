using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubes
{
    public abstract class CubeSuperController : MonoBehaviour
    {
        public abstract void RollCube(Vector3 dir);

        public abstract IEnumerator Moving(Vector3 anchor, Vector3 axis,Vector3 dir);
        public abstract void CheckInput();



        /// <summary>
        /// Raycast function to check for the <paramref name="tagToCheckFor"/>
        /// </summary>
        /// <param name="positionToRaycastFrom"></param>
        /// <param name="directionToRaycastTowards"></param>
        /// <param name="lengthOfRaycast"></param>
        /// <param name="tagToCheckFor"></param>
        /// <returns></returns>
        public abstract bool RaycastChecking(Vector3 positionToRaycastFrom,Vector3 directionToRaycastTowards, float lengthOfRaycast,string tagToCheckFor);
    }
}

