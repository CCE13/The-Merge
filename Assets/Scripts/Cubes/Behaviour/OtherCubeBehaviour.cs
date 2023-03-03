using UnityEngine;
using System.Collections;
using UI;

namespace Cubes
{
    public class OtherCubeBehaviour : MonoBehaviour,IDissolve
    {
        private Vector3 _startPos;
        public Material material;
        private void Awake()
        {
            material = GetComponent<Renderer>().material;

        }
        private void Start()
        {
            
            _startPos = transform.position;
            StartCoroutine(TransitionIn());
            UiController.restarting += Restarting;
        }
        private void OnDestroy()
        {
            UiController.restarting -= Restarting;
        }

        //restarting the game.
        public void Restarting()
        {
            RespawnManager.instance.Respawning(gameObject, _startPos, material, false, false);
        }

        //desparning the cube.
        public IEnumerator TransitionOut()
        {
            for (float i = -0.2f; i > 1; i += Time.deltaTime)
            {
                material.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(material.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
                yield return null;
            }
        }

        //spawning the cube.
        public IEnumerator TransitionIn()
        {
            for (float i = 1; i > -0.2; i -= Time.deltaTime)
            {
                material.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(material.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
                yield return null;

            }
        }
        private void Update()
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 1f);
            if ((hit.collider != null) && hit.collider.CompareTag("KillPlane"))
            {
                RespawnManager.instance.Respawning(gameObject, _startPos, material, false, false);
            }
        }

        public void Dissolve()
        {
            throw new System.Exception();
        }
    }
}