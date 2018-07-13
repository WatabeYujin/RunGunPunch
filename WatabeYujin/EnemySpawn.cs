using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {
    [SerializeField]
    private List<GameObject> spawnPrefub;       //スポーンを行う順にPrefubを入れる
    [SerializeField]
    private float spawnInterval = 100;          //スポーンを行う間隔
    [SerializeField]
    private Transform spawnPosition;            //prefubをスポーンさせる位置

    private float timeCount;                    //経過時間をカウント、スポーン毎に0になる。
    private bool isGameActive = true;           //ゲームが動いているか
    private int spawnCount = 0;                 //スポーンを行った回数

	void Start () {
        SpawnEvent();
    }	
	
	void Update () {
        TimeCount();
        if (!SpawnTimeCheck(timeCount)) return;
        SpawnEvent();
    }

    /// <summary>
    /// deltaTimeによる時間のカウントを行う
    /// </summary>
    void TimeCount()
    {
        if (!isGameActive) return;
        timeCount += Time.deltaTime;
    }

    /// <summary>
    /// スポーンを行うタイミングのチェックを行う
    /// </summary>
    /// <param name="time">調べる時間</param>
    /// <returns>スポーンできる場合trueを返し,不可能な場合falseを返す</returns>
    bool SpawnTimeCheck(float time)
    {
        if(spawnInterval <= time)
        {
            timeCount = 0;
            return true;
        }
        else return false;
    }

    /// <summary>
    /// 実際にprefubをスポーンさせる
    /// </summary>
    /// <returns>スポーンできた場合true,できなかった場合falseを返す</returns>
    bool SpawnEvent()
    {
        if (spawnCount > spawnPrefub.Count) return false;
        if (!spawnPrefub[spawnCount]) return false;
        
        GameObject spawnObj = Instantiate(spawnPrefub[spawnCount]);
        spawnObj.transform.position = spawnPosition.position;
        spawnCount++;
        return true;
    }
}
