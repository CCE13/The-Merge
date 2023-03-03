using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cubes
{
    public class MergingManager : MonoBehaviour
    {
        public Material player1Material;
        public Material player2Material;
        public List<GameObject> cubesInGame;


        public static MergingManager instance;
        public static bool S_CubeCanMerge;
        public static bool S_CubeCanSeperate;
        public static bool S_IsMerged;


        

        public static Action<bool> canMerge;
        public static Action<bool> canSeperate;

        public static GameObject S_CubeIsBeside;


        private bool _cubesAreBesideEachOther;
        private Player1CubeBehaviour _player1CubeBehaviour;


        private void Awake()
        {
            _player1CubeBehaviour = FindObjectOfType<Player1CubeBehaviour>();
            instance = this;
        }
        public void Start()
        {
            S_IsMerged = false;
            S_CubeCanMerge = false;
            S_CubeCanSeperate = false;
            CubeSearch();
            
        }
        //searches the scene for all mergable cubes and adds it into a list.
        public void CubeSearch()
        {
            foreach (OtherCubeBehaviour cubes in FindObjectsOfType<OtherCubeBehaviour>())
            {
                cubesInGame.Add(cubes.gameObject);
            }
        }

        // Update is called once per frame

        private void Update()
        {
            if (RespawnManager.sRespawning) {
                S_CubeCanMerge = false;
                S_CubeCanSeperate = false;
                canSeperate?.Invoke(S_CubeCanSeperate);
                canMerge?.Invoke(S_CubeCanMerge);
                return; }
            if (S_IsMerged)
            {
                
                CubeMergedController.S_MergeInControl = true;
                CheckIfCanSeperate();
                canSeperate?.Invoke(S_CubeCanSeperate);
            }
            else
            {
                if (RespawnManager.sRespawning) return;
                CubeMergedController.S_MergeInControl = false;
                CheckIfCanMerge();
                canMerge?.Invoke(S_CubeCanMerge);
                
            }
        }

        //checks if the player can merge with the cube.
        private void CheckIfCanMerge()
        {
            foreach (GameObject cube in cubesInGame)
            {
                bool playerIsBesideMergeCube = (_player1CubeBehaviour.transform.position - cube.transform.position).sqrMagnitude < 1.1f;
                if (playerIsBesideMergeCube)
                {
                    S_CubeIsBeside = cube.gameObject;
                    _cubesAreBesideEachOther = true;
                    break;
                }
                else
                {
                    S_CubeIsBeside = null;
                    _cubesAreBesideEachOther = false;
                }
            }

            if (_cubesAreBesideEachOther)
            {
                    S_CubeCanMerge = true;
                if(S_CubeIsBeside.activeInHierarchy == false)
                {
                    S_CubeCanMerge = false;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    S_CubeCanMerge = false;
                    Merge(S_CubeIsBeside);
                }         
            }
            else
            {
                S_CubeCanMerge = false;
            }
        }


        //checks if the player can seperate with the cube.
        private void CheckIfCanSeperate()
        {
            var cubeFalling = _player1CubeBehaviour.GetComponent<CubeMergedController>().isFalling;
            var cubeMoving = _player1CubeBehaviour.GetComponent<CubeMergedController>().isCubeMoving;
            if (cubeFalling)
            {
                S_CubeCanSeperate = false;
                return;
            }
            else if (cubeMoving)
            {
                return;
            }
            else
            {
                S_CubeCanSeperate = true;
            }

            if (!S_CubeCanSeperate) { return; }

            if (Input.GetKeyDown(KeyCode.E))
            {
                S_CubeCanSeperate = false;
                Seperate(S_CubeIsBeside);
            }
        }

        //Merging Function
        private void Merge(GameObject cubeToMergeWith)
        {
            cubeToMergeWith.transform.parent = _player1CubeBehaviour.transform;

            Destroy(cubeToMergeWith.GetComponent<Rigidbody>());
            _player1CubeBehaviour.gameObject.AddComponent<MergedCubeBehaviour>();
            cubeToMergeWith.gameObject.AddComponent<Outline>();
            S_IsMerged = true;
            S_CubeCanMerge = false;
            var outline = cubeToMergeWith.GetComponent<Outline>();
            outline.OutlineColor = new Color(0, 255, 255, 255);
            outline.OutlineWidth = 1.75f;

            //rounds the position of the merged cubes to the nearest int.
            var player1Pos = _player1CubeBehaviour.transform.position;
            var mergedCube = cubeToMergeWith.transform.position;
            _player1CubeBehaviour.transform.position = new Vector3(
                Mathf.RoundToInt(player1Pos.x),
                Mathf.RoundToInt(player1Pos.y),
                Mathf.RoundToInt(player1Pos.z)
                );
            cubeToMergeWith.transform.position = new Vector3(
                Mathf.RoundToInt(mergedCube.x),
                Mathf.RoundToInt(mergedCube.y),
                Mathf.RoundToInt(mergedCube.z)
                );
        }

        //Seperating the cubes
        public void Seperate(GameObject cubeToSeperateWith)
        {
            cubeToSeperateWith.transform.parent = null;

            var mergeBehaviour = _player1CubeBehaviour.GetComponent<MergedCubeBehaviour>();
            Destroy(mergeBehaviour);
            CubeSetup(cubeToSeperateWith);
            S_IsMerged = false;
            S_CubeCanSeperate = false;
        }

        private void CubeSetup(GameObject cubeToReset)
        {
            cubeToReset.gameObject.AddComponent<Rigidbody>();
            Destroy(cubeToReset.GetComponent<Outline>());
            Rigidbody rb = cubeToReset.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            rb.constraints
                =
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;
        }

    }
}