using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nn.hid;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    [SerializeField]
    JoyConControl joyCon;
    void Update()
    {

        if (joyCon.ButtonGet(NpadButton.ZR,Style.Down) || joyCon.ButtonGet(NpadButton.ZL,Style.Down) || Input.GetKeyDown(KeyCode.A))
        {
             StartCoroutine(AddScene());
        }
    }
    IEnumerator AddScene()
    {
        //シーンを非同期で追加する
        SceneManager.LoadScene(SceneName.GamePlay, LoadSceneMode.Additive);
        //シーン名を指定する
        Scene scene = SceneManager.GetSceneByName(SceneName.GamePlay);
        Debug.Log(scene);
        while (!scene.isLoaded)
        {
            yield return null;
        }
        //指定したシーン名をアクティブにする
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync(SceneName.Title);
    }
}
