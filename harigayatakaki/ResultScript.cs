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

    RankingDeta ranking = new RankingDeta();
    [SerializeField]
    DataController Data;
    [SerializeField]
    ScoreRunking scoreRunk;
    [SerializeField]
    GameObject nameBord;
    int playScore = 99996;
    int rank = 0;
    bool rankIn = false;
    string[] playerName = new string[2];
    Name_bord namekey;
    void Start()
    {
        namekey = nameBord.GetComponent<Name_bord>();
        RankStart();
    }
    void RankStart()
    {
       
        ranking = Data.Load();
        //aaaa();

        for (int i = 0; i < 5; i++)
        {
            if (playScore >= ranking.score[i])
            { rankIn = true; rank = i;break; }
            
        }
        if (rankIn)
        {
            nameBord.SetActive(true);
            namekey.isNameInput = true;
        }
        else
        { SetRank(); }
    }
    void aaaa()
    {
        for (int i = 0; i < 5; i++)
        {
            ranking.name1[i] = name1[i];
            ranking.name2[i] = name2[i];
            ranking.score[i] = score[i];
        }
        Data.Save(ranking.name1, ranking.name2, ranking.score);
    }
    void Update()
    { InputEnd(); }

    void InputEnd()
    {
        if (namekey.checkFlag[0] && namekey.checkFlag[1])
        {
            namekey.checkFlag[0] = false;
            playerName = namekey.NameGet(); SetRank();
            
            nameBord.SetActive(false);
        }
    }
    void SetRank()
    {
        if (rankIn)
        {
            RankingDeta a = ranking;
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
    void ShowRank()
    {
        RankSetEnd();
        scoreRunk.RunkpopEvent(ranking);
    }
    void RankSetEnd()
    {
       Data.Save(ranking.name1, ranking.name2, ranking.score);
    }
}
