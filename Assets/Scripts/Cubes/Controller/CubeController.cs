 using System.Collections;
using UnityEngine;

namespace Cubes
{
    public abstract class CubeController : CubeSuperController
    {
        [SerializeField] private int _rollingSpeed;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Outline outline;
        [HideInInspector] public Vector3 startPos;
        public static bool sCanMove;
        public bool IsFalling => Mathf.RoundToInt(rb.velocity.y) < 0f;

        public bool isCubeMoving;
        public GameObject buttonToPress;



        //checks if the cube can roll based of its surroundings.
        public override void RollCube(Vector3 dir)
        {
            bool pathBlocked = RaycastChecking(transform.position, Vector3.up, 1, "PushBlock")
                 || RaycastChecking(transform.position, Vector3.up, 1, "OtherBlock")
                 || RaycastChecking(transform.position + Vector3.up, dir, 1, "Ground")
                 || RaycastChecking(transform.position + Vector3.up, dir, 1, "PushCube")
                 || RaycastChecking(transform.position + Vector3.up, dir, 1, "OtherCube");
            bool canClimb = RaycastChecking(transform.position, dir, 1, "Ground") 
                || RaycastChecking(transform.position, dir, 1, "OtherCube" );
            bool canClimbBlock = RaycastChecking(transform.position, dir, 1, "PushCube");
            bool canPush = RaycastChecking(transform.position, dir, 1, "PushCube") && !RaycastChecking(transform.position + dir, dir, 1, "Ground") && !RaycastChecking(transform.position + dir, dir, 1, "OtherCube");

            if (pathBlocked){ return; }

            if (canClimb||canClimbBlock && !canPush)
            {
                ClimbWall(dir);
            }
            else if (canPush)
            {
                PushBlock(dir, PushCast(dir));
            }
            else
            {
                MoveNormally(dir);
            }
        }

        //checks input.
        public override void CheckInput()
        {
            //check if the cube is merged
            if (MergingManager.S_IsMerged) { return; }
            if (isCubeMoving) { return; }
            if (!sCanMove) { return; }

            if (IsFalling) { return; }

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



        public override bool RaycastChecking(Vector3 positionToRaycastFrom, Vector3 directionToRaycastTowards, float lengthOfRaycast, string tagToCheckFor)
        {
            RaycastHit hit;
            if (Physics.Raycast(positionToRaycastFrom, directionToRaycastTowards, out hit, lengthOfRaycast))
            {
                if (hit.transform.gameObject.tag == tagToCheckFor)
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

        //Returns a the push cube if the raycast hits the object.
        public GameObject PushCast(Vector3 directionToRaycastTowards)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToRaycastTowards, out hit, 1))
            {
                if (hit.transform.gameObject.tag == "PushCube")
                {
                    return hit.transform.gameObject;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #region Movement Calculation

        //moves the cube based off the axis and anchor.
        public void MoveNormally(Vector3 directionToMoveTowards)
        {
            var anchor = transform.position + (Vector3.down + directionToMoveTowards) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, directionToMoveTowards);
            StartCoroutine(Moving(anchor, axis,directionToMoveTowards));
        }

        //pushes the cube and moves the cube based off the asix anchor and direction.
        public void PushBlock(Vector3 directionToMoveTowards, GameObject cubeToPush)
        {
            var anchor = transform.position + (Vector3.down + directionToMoveTowards) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, directionToMoveTowards);
            StartCoroutine(PushBlock(anchor, axis,cubeToPush,directionToMoveTowards));
        }

        //moves the cube up the wall based off the axis and anchor.
        public void ClimbWall(Vector3 directionToClimbTowards)
        {
            var anchor = transform.position + (Vector3.up + directionToClimbTowards) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, directionToClimbTowards);
            StartCoroutine(Climbing(anchor, axis));
        }
        private IEnumerator PushBlock(Vector3 anchor,Vector3 axis,GameObject cubeToPush,Vector3 pushDirection)
        {
            isCubeMoving = true;
            

            for (int i = 0; i < (90 / _rollingSpeed); i++)
            {
                cubeToPush.transform.position = Vector3.Lerp(cubeToPush.transform.position, cubeToPush.transform.position + pushDirection,0.1f);
                transform.RotateAround(anchor, axis, _rollingSpeed);
                yield return new WaitForSeconds(0.01f);
            }
            
            isCubeMoving = false;

            cubeToPush.transform.position = new Vector3
            (
            Mathf.RoundToInt(cubeToPush.transform.position.x),
            Mathf.RoundToInt(cubeToPush.transform.position.y),
            Mathf.RoundToInt(cubeToPush.transform.position.z));
            transform.position = new Vector3
                (
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));

        }

        public override IEnumerator Moving(Vector3 anchor, Vector3 axis, Vector3 dir)
        {
            isCubeMoving = true;

            for (int i = 0; i < (90 / _rollingSpeed); i++)
            {
                transform.RotateAround(anchor, axis, _rollingSpeed);
                yield return new WaitForSeconds(0.01f);
            }

            isCubeMoving = false;

            transform.position = new Vector3
                (
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));
        }

        public IEnumerator Climbing(Vector3 anchor, Vector3 axis)
        {
            isCubeMoving = true;
            rb.useGravity = false;

            for (int i = 0; i < (180 / _rollingSpeed); i++)
            {
                transform.RotateAround(anchor, axis, _rollingSpeed);
                yield return new WaitForSeconds(0.01f);
            }

            isCubeMoving = false;
            rb.useGravity = true;

            transform.position = new Vector3
                (
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z));
        }

        #endregion
    }
}