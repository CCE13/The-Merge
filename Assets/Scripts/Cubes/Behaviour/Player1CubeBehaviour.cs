using UnityEngine;
using System.Collections;
using System;
using UI;

namespace Cubes
{
    [RequireComponent(typeof(Rigidbody),typeof(Outline))]
    public class Player1CubeBehaviour : CubeController,IDissolve
    {
        public Material material;
        public static event Action<bool> onButton;
        //set-up
        public void Awake()
        {
            //stores a reference to the rigidbody and the outline component
            rb = GetComponent<Rigidbody>();
            outline = GetComponent<Outline>();
            material = GetComponent<Renderer>().material;
            LevelManager.gameEnded += Dissolve;
            UiController.restarting += Restarting;
        }

        private void OnDestroy()
        {
            LevelManager.gameEnded -= Dissolve;
            UiController.restarting -= Restarting;
        }
        //Restarting the game.
        public void Restarting()
        {
            RespawnManager.instance.Respawning(gameObject, startPos, material, true, false);
        }
        //asignment of values
        public void Start()
        {
            StartCoroutine(TransitionIn());
            startPos = transform.position;
            sCanMove = true;
        }

        public void Update()
        {
            SetOutline();
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 1f);
            if ((hit.collider != null) && hit.collider.CompareTag("KillPlane"))
            {
                RespawnManager.instance.Respawning(gameObject, startPos, material, true, true);
            }
            if(hit.collider != null && hit.collider.gameObject == buttonToPress)
            {
                if (CubeMergedController.S_MergeInControl) { onButton?.Invoke(false); return; }
                onButton?.Invoke(true);
            }
            else
            {
                onButton?.Invoke(false);
            }
        }

        private void SetOutline()
        {
            if (RespawnManager.sRespawning)
            {
                outline.OutlineWidth = 0f;

            }
            else
            {
                outline.OutlineWidth = 1.75f;
                CheckInput();

            }
            
        }

        public void Dissolve()
        {
            outline.enabled = false;
            StartCoroutine(TransitionOut());
        }

        public IEnumerator TransitionOut()
        {
            for (float i = -0.2f; i < 1; i += Time.deltaTime)
            {
                material.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(material.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
                yield return null;
            }
        }

        public IEnumerator TransitionIn()
        {
            for (float i = 1; i > -0.2; i -= Time.deltaTime)
            {
                material.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(material.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
                yield return null;

            }
        }
    }
}