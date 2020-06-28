using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransition : Singleton<SceneTransition>
{
    [Header("Scene Transition")]
    public GameObject transitionScreen;
    public float sceneTransitionTime;
    // Start is called before the first frame update
    void Start()
    {
        PlayIntroAnimation();
    }

    public void LoadScene(string scene)
    {
        // play scene transition
        PlayOutroAnimation();
        loadSceneParams sceneParams = new loadSceneParams();
        sceneParams.scene = scene;
        sceneParams.waitTime = sceneTransitionTime;
        StartCoroutine("LoadSceneAfterDelay", sceneParams);

    }

    class loadSceneParams
    {
        public string scene;
        public float waitTime;
    }
    IEnumerator LoadSceneAfterDelay(loadSceneParams sceneParams)
    {
        PlayOutroAnimation();
        yield return new WaitForSeconds(sceneParams.waitTime);

        SceneManager.LoadScene(sceneParams.scene);
    }
    public void PlayIntroAnimation()
    {
        transitionScreen.GetComponent<Animator>().SetTrigger("Intro");
    }

    public void PlayOutroAnimation()
    {
        transitionScreen.GetComponent<Animator>().SetTrigger("Outtro");
    }
}
