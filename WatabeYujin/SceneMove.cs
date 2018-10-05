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
    [SerializeField]
    private Sprite[] transitionSprites;
    const string m_AlphaPropertieName = "_Alpha";
    Color fadeDefaltColor = new Color(181 / 255,255/255,255/255);
    const string m_ColorName = "_Color";
    private int transitionID = 0;
    private void Start()
    {
        StartCoroutine(FadeOut(fadeDefaltColor));
    }

    public void SceneMoveEvent(string scene, int transitionid = -1)
    {
        sceneName = scene;
        if(transitionid != -1)transitionID = transitionid;
        StartCoroutine(SceneMoveIEnumerator());
    }

    public void FadeInEvent(Color fadecolor, int transitionid = -1)
    {
        if (fadecolor == new Color(0, 0, 0)) fadecolor = fadeDefaltColor;
        if (transitionid != -1) transitionID = transitionid;
        StartCoroutine(FadeIn(fadecolor));
    }

    public void FadeOutEvent(Color fadecolor, int transitionid = -1)
    {
        if (fadecolor == new Color(0, 0, 0)) fadecolor = fadeDefaltColor;
        if (transitionid != -1) transitionID = transitionid;
        StartCoroutine(FadeOut(fadecolor));
    }

    IEnumerator SceneMoveIEnumerator()
    {
        yield return StartCoroutine(FadeIn(fadeDefaltColor));
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn(Color fadecolor)
    {
        if (fadecolor == Color.black) fadecolor = fadeDefaltColor;
        Debug.Log("FadeIn");
        transitionImage.rectTransform.localScale = new Vector3(1, 1, 1);
        Material m_material = transitionImage.material;
        float current = 0;
        m_material.SetColor(m_ColorName, fadecolor);
        transitionImage.sprite = transitionSprites[transitionID];
        while (current < fadeTime)
        {
            m_material.SetFloat(m_AlphaPropertieName, current / fadeTime);
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        m_material.SetFloat(m_AlphaPropertieName, 1);
    }

    IEnumerator FadeOut(Color fadecolor)
    {
        Debug.Log("FadeOut");
        if (fadecolor == Color.black) fadecolor = fadeDefaltColor;
        transitionImage.rectTransform.localScale = new Vector3(-1, 1, 1);
        Material m_material = transitionImage.material;
        m_material.SetColor(m_ColorName,fadecolor);
        float current = 0;

        transitionImage.sprite = transitionSprites[transitionID];
        while (current < fadeTime)
        {
            m_material.SetFloat(m_AlphaPropertieName, 1f - current / fadeTime);
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        m_material.SetFloat(m_AlphaPropertieName, 0);
    }
}
