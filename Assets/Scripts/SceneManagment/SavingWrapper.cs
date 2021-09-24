using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Saving;

using UnityEngine;

namespace RPG.SceneManagment {
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "QuickSave";

        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutInstant();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
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
        }

        public void QuickLoad()
        {

            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void QuickSave()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
    }
}
