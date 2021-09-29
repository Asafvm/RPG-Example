using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Saving;

using UnityEngine;

namespace RPG.SceneManagment {
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "QuickSave";

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }
        IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutInstant();
            yield return fader.FadeIn(.2f);

        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F5))
            {
                QuickSave();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                QuickLoad();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void QuickLoad()
        {

            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void QuickSave()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
