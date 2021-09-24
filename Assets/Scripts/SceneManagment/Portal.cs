using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagment
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier { A, B, C, D }

        [SerializeField] DestinationIdentifier destination;
        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private ParticleSystem teleportEffect;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] float fadeOutTime = .5f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = .5f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<NavMeshAgent>().SetDestination(transform.position);
                StartCoroutine(LoadLevel(sceneToLoad, other.gameObject));
            }
        }

        private IEnumerator LoadLevel(int sceneToLoad, GameObject player)
        {
            if (sceneToLoad < 0)
            {
                Debug.Log("Scene to load no set!");
                yield break;
            }
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>();

            //Transition effects
            if (teleportEffect != null)
            {
                teleportEffect.Play();
            }
            yield return new WaitForSeconds(.5f);
            player.SetActive(false);
            yield return fader.FadeOut(fadeOutTime);

            // Save current level
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.QuickSave();
            //Load next level
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            //Load level save file
            savingWrapper.QuickLoad();

            //Reposition player
            Portal otherPortal = GetOtherPortal();
            if (otherPortal != null)
                UpdatePlayer(otherPortal);
            //Save current position
            savingWrapper.QuickSave();
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
            Destroy(gameObject);
        }

        private static void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                return portal;
            }
            return null;
        }
    }
}
