using nn.hid;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RankingDeta
{
    public string[] name1 = new string[5];
    public string[] name2 = new string[5];
    public int[] score = new int[5];
}
public class ResultScript : MonoBehaviour
{
    string[] name1 = { "AAA", "BBB", "CCC", "DDD", "EEE" };
    string[] name2 = { "FFF", "GGG", "HHH", "III", "JJJ" };
    int[] score = { 555, 444, 333, 222, 111 };

    //RankingDeta ranking = new RankingDeta();
    /*
    [SerializeField]
    DataController Data;*/
    [SerializeField]
    JoyConControl joyCon;
    [SerializeField]
    ScoreRunking scoreRunk;
    [SerializeField]
    GameObject nameBord, scoreBord, nextText;
    [SerializeField]
    Text scoreText;
    public static int[] runkscore= { 250,200,200,150,10 };
    public static string[] p1name = { "WTB", "TEC","RGP","POPO","AAA" } ;
    public static string[] p2name = { "YJN", "HC", "RGP", "RORO", "AA" };
    int playScore = 0;
    int rank = 0;
    bool rankIn = false;
    bool bordFlag = false;
    string[] playerName = new string[2];
    Name_bord namekey;
    
    void Start()
    {
        playScore = PlaySceneManager.GetScore();
        namekey = nameBord.GetComponent<Name_bord>();
        ScoreShow();
    }

    void Update()
    {
        InputEnd();
        ScoreClose();
    }
    void ScoreShow()
    {
        scoreText.text = playScore.ToString();
        scoreBord.SetActive(true);
        bordFlag = true;
        StartCoroutine(CodeFlash());
    }
    void ScoreClose()
    {
        if (!bordFlag)
        { return; }
        else
        {
            if (joyCon.ButtonGet(NpadButton.ZL, Style.Up) || joyCon.ButtonGet(NpadButton.ZR, Style.Up))
            {
                bordFlag = false; scoreBord.SetActive(false);
                RankStart();
            }
        }
    }
    IEnumerator CodeFlash()
    {
        bool m_active = true;
        while (bordFlag)
        {
            m_active = !m_active;
            nextText.SetActive(m_active);
            yield return new WaitForSeconds(0.5f);
        }
    }
    void RankStart()
    {
       /*
        try
        {
            //ranking = Data.Load();
        }
        catch
        {
            DummyData();
        }*/
        


        for (int i = 0; i < 5; i++)
        {
            if (playScore >= runkscore[i])
            { rankIn = true; rank = i; break; }

        }
        if (rankIn)
        {
            nameBord.SetActive(true);
            namekey.isNameInput = true;
        }
        else
        { SetRank(); }
    }
    /*
    void DummyData()
    {
        for (int i = 0; i < 5; i++)
        {
            ranking.name1[i] = name1[i];
            ranking.name2[i] = name2[i];
            ranking.score[i] = score[i];
        }
        Data.Save(ranking.name1, ranking.name2, ranking.score);
    }*/

    void InputEnd()
    {
        if (namekey.checkFlag[0] && namekey.checkFlag[1])
        {
            namekey.checkFlag[0] = false;
            playerName = namekey.NameGet(); SetRank();

            nameBord.SetActive(false);
        }
    }
    /*
    void SetRank()
    {
        if (rankIn)
        {
            for (int i = 4; i > rank; i--)
            {

                ranking.name1[i] = ranking.name1[i - 1];
                ranking.name2[i] = ranking.name2[i - 1];
                ranking.score[i] = ranking.score[i - 1];
            }
            ranking.name1[rank] = playerName[0];
            ranking.name2[rank] = playerName[1];
            ranking.score[rank] = playScore;
        }
        ShowRank();
    }
    */
    void SetRank()
    {
        if (rankIn)
        {
            for (int i = 4; i > rank; i--)
            {

                p1name[i] = p1name[i - 1];
                p2name[i] = p2name[i - 1];
                runkscore[i] = runkscore[i - 1];
            }
            p1name[rank] = playerName[0];
            p2name[rank] = playerName[1];
            runkscore[rank] = playScore;
        }
        ShowRank();
    }
    void ShowRank()
    {
        //RankSetEnd();
        //scoreRunk.RunkpopEvent(ranking);
        scoreRunk.RunkpopEvent();
    }
    /*
    void RankSetEnd()
    {
      Data.Save(ranking.name1, ranking.name2, ranking.score);
    }*/
}
