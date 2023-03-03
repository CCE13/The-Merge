using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubes
{
    public abstract class CubeMergedController : CubeSuperController
    {
        [SerializeField] private int rollingSpeed = 10;
        public static bool S_MergeInControl;
        public bool cubeStacked;
        public GameObject Player1 => FindObjectOfType<Player1CubeBehaviour>().gameObject;
        public GameObject cubeMergedWith => transform.GetChild(0).gameObject;

        public bool isFalling;
        public static bool canMove;
        public bool isCubeMoving;

        
        //checks if the cube can be rolled
        public override void RollCube(Vector3 dir)
        {
            bool cannotMove = RaycastChecking(transform.position, dir, 1, "Ground")
                || RaycastChecking(transform.position, dir, 1, "OtherCube")
                ||RaycastChecking(cubeMergedWith.transform.position,dir,1,"OtherCube")
                || cubeStacked && RaycastChecking(transform.position, dir, 2, "Ground")
                || cubeStacked && RaycastChecking(transform.position, dir, 2, "PushCube")
                || cubeStacked && RaycastChecking(transform.position, dir, 2, "OtherCube")
                || cubeStacked && RaycastChecking(cubeMergedWith.transform.position, dir, 2, "PushCube")
                || cubeStacked && RaycastChecking(cubeMergedWith.transform.position, dir, 2, "Ground") 
                || cubeStacked && RaycastChecking(cubeMergedWith.transform.position, dir, 2, "OtherCube")
                || RaycastChecking(cubeMergedWith.transform.position, dir, 1, "Ground")
                || RaycastChecking(transform.position, dir, 1, "PushCube")
                || RaycastChecking(cubeMergedWith.transform.position, dir, 1, "PushCube");
            bool useOtherCubeAnchor = (RaycastChecking(cubeMergedWith.transform.position, Vector3.down, 1, "Ground")
                || RaycastChecking(cubeMergedWith.transform.position, Vector3.down, 1, "OtherCube") || RaycastChecking(cubeMergedWith.transform.position, Vector3.down, 1, "PushCube"))
                && !RaycastChecking(Player1.transform.position, -dir, 1, "Cube");

            if (cannotMove)
            {
                Debug.Log("Cant Move, Something is in the way!");
                return;
            }

            if (useOtherCubeAnchor)
            {
                MoveNormally(dir, cubeMergedWith.transform.position);
                Debug.Log(" Move HIGH");    
            }
            else
            {
                MoveNormally(dir, transform.position);
                Debug.Log(" move low");
            }
        }
        #region InputChecking

        /// <summary>
        /// Checks the input of the player
        /// </summary>
        public override void CheckInput()
        {

            if (!canMove) { return; }

            if (Input.GetKeyDown(KeyCode.W))
            {
                RollCube(Vector3.forward);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                RollCube(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                RollCube(Vector3.back);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                RollCube(Vector3.right);
            }
        }
        #endregion

        #region Movement

        //moves the merged cube based of the anchor and axis.
        public void MoveNormally(Vector3 directionToMoveTowards, Vector3 whichPlayer)
        {
            var anchor = whichPlayer + (Vector3.down + directionToMoveTowards) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, directionToMoveTowards);
            StartCoroutine(Moving(anchor, axis,directionToMoveTowards));
        }

        public override IEnumerator Moving(Vector3 anchor, Vector3 axis, Vector3 dir)
        {
            isCubeMoving = true;
            for (int i = 0; i < (90 / rollingSpeed); i++)
            {
                transform.RotateAround(anchor, axis, rollingSpeed);
                yield return new WaitForSeconds(0.01f);
            }

            
            transform.position = new Vector3
                (
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));

            cubeMergedWith.transform.position = new Vector3
                (
                Mathf.RoundToInt(cubeMergedWith.transform.position.x),
                Mathf.RoundToInt(cubeMergedWith.transform.position.y),
                Mathf.RoundToInt(cubeMergedWith.transform.position.z));
            Fall(dir,axis);
            isCubeMoving = false;
        }
        #endregion

        #region CubeFalling

        //checks if the player can fall.
        public void Fall(Vector3 dir,Vector3 axis)
        {
            bool playerIsOnTop = Mathf.FloorToInt(Player1.transform.position.y) > Mathf.FloorToInt(cubeMergedWith.transform.position.y);
            if (CheckIfMergedCubeOnGround() && CheckIfPlayer1OnGround())
            {
                return;
            }
            if (!CheckIfMergedCubeOnGround() && CheckIfPlayer1OnGround())
            {
                if(playerIsOnTop)
                {
                    Falling(Player1.transform.position, dir, axis);
                }
                else
                {
                    Falling(cubeMergedWith.transform.position, dir, axis);
                }
            }

            if (CheckIfMergedCubeOnGround() && !CheckIfPlayer1OnGround())
            {
                if (playerIsOnTop)
                {
                    Falling(Player1.transform.position, dir, axis);
                }
                else
                {
                    Falling(cubeMergedWith.transform.position, dir, axis);
                }
            }
            if (!CheckIfMergedCubeOnGround() && !CheckIfPlayer1OnGround())
            {
                if (playerIsOnTop)
                {
                    Falling(Player1.transform.position, dir, axis);
                }
                else if(Player1.transform.position.y == cubeMergedWith.transform.position.y)
                {
                    Falling((Player1.transform.position + cubeMergedWith.transform.position) / 2, dir, axis);
                    
                }
                else
                {
                    Falling(cubeMergedWith.transform.position, dir, axis);
                }
                
            }
        }
        /// <summary>
        /// Falls towards the <paramref name="dir"/> at the <paramref name="positionToAddForceAt"/>
        /// </summary>
        /// <param name="positionToAddForceAt"></param>
        /// <param name="dir"></param>
        private void Falling(Vector3 positionToAddForceAt, Vector3 dir,Vector3 axis)
        {
            Rigidbody rb = Player1.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.AddTorque(axis * 15);
            rb.AddForceAtPosition((Vector3.down + dir) * 3, positionToAddForceAt, ForceMode.VelocityChange);
            isFalling = true;
            canMove = false;
        }

        #endregion

        /// <summary>
        /// Check if the MergeChecker Cube is stacked
        /// </summary>
        public void CheckIfStacked()
        {
            //sets to true as long as one of the cubes is ontop of another.
            if (Mathf.RoundToInt(Player1.transform.position.y) - 1 == Mathf.RoundToInt(cubeMergedWith.transform.position.y))
            {
                cubeStacked = true;   
            }
            else if((Mathf.RoundToInt(Player1.transform.position.y) + 1 == Mathf.RoundToInt(cubeMergedWith.transform.position.y)))
            {
                cubeStacked = true;
            }
            else if((Mathf.RoundToInt(Player1.transform.position.y) == Mathf.RoundToInt(cubeMergedWith.transform.position.y)))
            {
                cubeStacked = false;
            }
        }

        #region Raycast Checking
        public override bool RaycastChecking(Vector3 positionToRaycastFrom, Vector3 directionToRaycastTowards, float lengthOfRaycast, string tagToCheckFor)
        {
            RaycastHit hit;
            if (Physics.Raycast(positionToRaycastFrom, directionToRaycastTowards, out hit, lengthOfRaycast))
            {
                if (hit.transform.gameObject.CompareTag(tagToCheckFor))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// returns true if the child of the gameobject hit is equal to the <paramref name="tagToCheckFor"/>.
        /// </summary>
        /// <param name="positionToRaycastFrom"></param>
        /// <param name="directionToRaycastTowards"></param>
        /// <param name="lengthOfRaycast"></param>
        /// <param name="tagToCheckFor"></param>
        /// <returns></returns>
        public bool RaycastCheckingChild(Vector3 positionToRaycastFrom, Vector3 directionToRaycastTowards, float lengthOfRaycast, string tagToCheckFor)
        {
            RaycastHit hit;
            if (Physics.Raycast(positionToRaycastFrom, directionToRaycastTowards, out hit, lengthOfRaycast))
            {
                if (hit.collider.transform.CompareTag(tagToCheckFor))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if Player1 is on the ground.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfPlayer1OnGround()
        {
            if (RaycastChecking(Player1.transform.position, Vector3.down, 10, "Ground")
                || RaycastChecking(Player1.transform.position, Vector3.down, 10, "PushCube")
                || RaycastChecking(Player1.transform.position, Vector3.down, 10, "OtherCube")
                ||RaycastCheckingChild(Player1.transform.position, Vector3.down, 10, "OtherCube"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the MergedCube is on the ground.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfMergedCubeOnGround()
        {
            if (RaycastChecking(cubeMergedWith.transform.position, Vector3.down, 10, "Ground") 
                || RaycastChecking(cubeMergedWith.transform.position, Vector3.down, 10, "OtherCube")
                || RaycastChecking(cubeMergedWith.transform.position, Vector3.down, 10, "PushCube")
                ||RaycastChecking(cubeMergedWith.transform.position,Vector3.down, 10, "Cube"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}

