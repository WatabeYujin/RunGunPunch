using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour {
    [SerializeField]
    private Image transitionImage;
    private string sceneName;
    [SerializeField]
    private float fadeTime = 0.5f;

    const string m_AlphaPropertieName = "_Alpha";

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    public void SceneMoveEvent(string scene)
    {
        sceneName = scene;
        StartCoroutine(SceneMoveIEnumerator());
    }

    IEnumerator SceneMoveIEnumerator()
    {
        yield return StartCoroutine(FadeIn());
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn()
    {
        transitionImage.rectTransform.localScale = new Vector3(1, 1, 1);
        transitionImage.enabled = true;
        Material m_material = transitionImage.material;
        float current = 0;
        while (current < fadeTime)
        {
            m_material.SetFloat(m_AlphaPropertieName, current / fadeTime);
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        m_material.SetFloat(m_AlphaPropertieName, 1);
    }

    IEnumerator FadeOut()
    {
        transitionImage.rectTransform.localScale = new Vector3(-1, 1, 1);
        transitionImage.enabled = true;
        Material m_material = transitionImage.material;
        float current = 0;
        while (current < fadeTime)
        {
            m_material.SetFloat(m_AlphaPropertieName, 1 - current / fadeTime);
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        m_material.SetFloat(m_AlphaPropertieName, 0);
        transitionImage.enabled = false;
    }
}
