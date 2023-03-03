using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI;

namespace Cubes
{
    public class PushBlockBehaviour : MonoBehaviour, IDissolve
    {
        public GameObject buttonToPress;
        private Vector3 _startPos;
        private Material material;


        public static event Action<bool> onButton;
        //set-up 
        //assignment of variables needed
        private void Start()
        {
            
            _startPos = transform.position;
            material = GetComponent<Renderer>().material;
            StartCoroutine(TransitionIn());
            LevelManager.gameEnded += Dissolve;
            UiController.restarting += Restarting;
        }
        private void OnDestroy()
        {
            LevelManager.gameEnded -=Dissolve;
            UiController.restarting -= Restarting;
        }

        //Restarting the game.
        public void Restarting()
        {
            RespawnManager.instance.Respawning(gameObject, _startPos, material, false,false);
        }

        private void Update()
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 1f);
            if ((hit.collider != null) && hit.collider.CompareTag("KillPlane"))
            {
                RespawnManager.instance.Respawning(gameObject, _startPos, material,  false, false);
            }
            if (hit.collider != null && hit.collider.gameObject == buttonToPress)
            {
                if (CubeMergedController.S_MergeInControl) { onButton?.Invoke(false); return; }
                onButton?.Invoke(true);
            }
            else
            {
                onButton?.Invoke(false);
            }
        }
        public void Dissolve()
        {
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

