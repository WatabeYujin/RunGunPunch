using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRunking : MonoBehaviour {
    [SerializeField]
    private Text[] text = new Text[5];
    //RankingDeta runking = new RankingDeta();
    [SerializeField]
    private TitleScene titlescene;
    
    
    void Start () {
        
	}
    /*
    public void RunkpopEvent(RankingDeta runkDate)
    {
        
        runking = runkDate;
        StartCoroutine(RunkPop());
    }*/
    public void RunkpopEvent()
    {
        StartCoroutine(RunkPop());
    }

    IEnumerator RunkPop() {
        for (int i = 0; i < 5; i++)
        {
            TextSet(text[i],i);
            yield return StartCoroutine(TextMove(i));
            if(i == 0)
                yield return new WaitForSeconds(0.7f);
            else
                yield return new WaitForSeconds(0.3f);
        }
        titlescene.enabled = true;
    }

    IEnumerator TextMove(int runk)
    {
        const float m_textMoveSpeed = 25;
        while (text[runk].transform.position.x > Screen.width / 2)
        {
            Debug.Log(text[runk].transform.position.x);
            text[runk].transform.position -= Vector3.right * m_textMoveSpeed;
            yield return null;
        }
    }

    void TextSet(Text text,int runk)
    {
        const int m_fontSize = 70;
        const int m_fontSizeNumberOne = 90;
        if (runk == 0)
            text.fontSize = m_fontSizeNumberOne;
        else
            text.fontSize = m_fontSize;
        text.text = ResultScript.p1name[runk].ToString() + "＆" + ResultScript.p2name[runk].ToString() + "：" + ResultScript.runkscore[runk].ToString();
    }
}
