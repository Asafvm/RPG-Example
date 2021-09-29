using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagment {
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        public void FadeOutInstant()
        {
            canvasGroup.alpha = 1;
        }
        
        public IEnumerator FadeOut(float time)
        {
            while(canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Mathf.Clamp01(Time.deltaTime / time);
                yield return null;
            }
        }
        public IEnumerator FadeIn(float time)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Mathf.Clamp01(Time.deltaTime / time);
                yield return null;
            }
        }
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();         
        }

    
    }
}
