using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenControl : MonoBehaviour
{
    public Slider slider;
    //public GameObject[] screenshots;
    public Animator anim;

    AsyncOperation async;

    void Start()
    {
        StartCoroutine(LoadingScreen());
        //screenshots[Random.Range(0, screenshots.Length)].SetActive(true);
    }

    IEnumerator LoadingScreen()
    {
        async = SceneManager.LoadSceneAsync(EnterLoading.sceneToLoad);
        async.allowSceneActivation = false;
        while (async.isDone == false)
        {
            slider.value = async.progress;
            if (async.progress == 0.9f)
            {
                yield return new WaitForSecondsRealtime(2);
                slider.value = 1f;
                anim.Play("LoadingFadeOut");
                yield return new WaitForSecondsRealtime(2);
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}