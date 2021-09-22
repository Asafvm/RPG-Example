using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagment {
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;


        IEnumerator FadeOutIn()
        {
            yield return FadeOut(3f);
            yield return FadeIn(1f);
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
        // Start is called before the first frame update
        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();         
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
