using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubes
{
    public class MergedCubeBehaviour :  CubeMergedController
    {
        private Rigidbody rb;
        // Update is called once per frame
        private void Start()
        {
            isFalling = false;
            canMove = true;
            S_MergeInControl = false;
            rb = GetComponent<Rigidbody>();
        }
        void Update()
        {
            CheckIfStacked();
            //check if merge control is true
            if (isFalling) return;
            if (isCubeMoving) { return; }
            if(rb.velocity.magnitude > 0) { return; }
            CheckInput();
        }
    }
}
