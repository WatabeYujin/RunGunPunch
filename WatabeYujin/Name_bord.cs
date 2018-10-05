using nn.hid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Name_bord : MonoBehaviour
{
    [SerializeField]
    JoyConControl joyCon;
    [SerializeField]
    GameObject[] extra_key;
    [SerializeField]
    Text[] playerName;
    [SerializeField]
    GameObject[] keyCursor;
    [SerializeField]
    GameObject[] keySet1, keySet2, keySet3;
    [SerializeField]
    Text[] textSet1, textSet2, textSet3;
    public bool[] checkFlag = { false, false };
    int[] verticalNum = { 0, 0 };
    int[] sideNum = { 0, 0 };
    int[] extraNum = { 0, 0 };
    Text[][] textboard = new Text[3][];
    GameObject[][] keyboard = new GameObject[3][];
    private bool nameInput = false;
    public bool isNameInput
    { get { return nameInput; } set { nameInput = value; } }

    void OnEnable()
    {
        playerName[0].text = "";
        playerName[1].text = "";
        keyboard[0] = keySet1; keyboard[1] = keySet2; keyboard[2] = keySet3;
        textboard[0] = textSet1; textboard[1] = textSet2; textboard[2] = textSet3;
        keyCursor[0].transform.position = keyboard[0][0].transform.position;
        keyCursor[1].transform.position = keyCursor[1].transform.position;

    }

    public string[] NameGet()
    {
        return new string[] { playerName[0].text, playerName[1].text };
    }

    void Update()
    {
        Nameinput();
    }
    void Nameinput()
    {
        if (!nameInput) return;
        if (!checkFlag[(int)Hand.Left])
        {
            if (Input.GetKeyDown(KeyCode.A) || joyCon.ButtonGet(NpadButton.Left, Style.Down) ||
                joyCon.AnalogStickGet(Hand.Left).x <= -0.8f)
                LeftCursor((int)Hand.Left);

            if (Input.GetKeyDown(KeyCode.D) || joyCon.ButtonGet(NpadButton.Right, Style.Down) ||
                joyCon.AnalogStickGet(Hand.Left).x >= 0.8f)
                RightCursor((int)Hand.Left);

            if (Input.GetKeyDown(KeyCode.W) || joyCon.ButtonGet(NpadButton.Up, Style.Down) ||
                joyCon.AnalogStickGet(Hand.Left).y <= -0.8f)
                UpCursor((int)Hand.Left);

            if (Input.GetKeyDown(KeyCode.S) || joyCon.ButtonGet(NpadButton.Down, Style.Down) ||
                joyCon.AnalogStickGet(Hand.Left).y >= 0.8f)
                DownCursor((int)Hand.Left);
        }

        if (!checkFlag[(int)Hand.Right])
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || joyCon.ButtonGet(NpadButton.Y, Style.Down))
                LeftCursor((int)Hand.Right);
            if (Input.GetKeyDown(KeyCode.RightArrow) || joyCon.ButtonGet(NpadButton.A, Style.Down))
                RightCursor((int)Hand.Right);
            if (Input.GetKeyDown(KeyCode.UpArrow) || joyCon.ButtonGet(NpadButton.X, Style.Down))
                UpCursor((int)Hand.Right);
            if (Input.GetKeyDown(KeyCode.DownArrow) || joyCon.ButtonGet(NpadButton.B, Style.Down))
                DownCursor((int)Hand.Right);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || joyCon.ButtonGet(NpadButton.ZL, Style.Down))
            GetInput((int)Hand.Left);
        if (Input.GetKeyDown(KeyCode.RightControl) || joyCon.ButtonGet(NpadButton.ZR, Style.Down))
            GetInput((int)Hand.Right);
        if (checkFlag[0] && checkFlag[1])
        { isNameInput = false; }
    }

    void LeftCursor(int num)
    {
        if (verticalNum[num] != 3)
        {

            sideNum[num]--;
            if (sideNum[num] < 0)
            {
                if (verticalNum[num] == 2) sideNum[num] = 7;
                else sideNum[num] = 8;
                if (sideNum[num] >= 4) extraNum[num] = 0;
            }
        }
        if (extraNum[num] == 0) extraNum[num] = 1;
        else extraNum[num] = 0;
        CursorMove(num);

    }

    void RightCursor(int num)
    {
        if (verticalNum[num] != 3)
        {
            sideNum[num]++;
            if (verticalNum[num] == 2)
            { if (sideNum[num] > 7) sideNum[num] = 0; }
            else if (sideNum[num] > 8) sideNum[num] = 0;
            if (sideNum[num] <= 4) extraNum[num] = 1;
        }
        if (extraNum[num] == 0) extraNum[num] = 1;
        else extraNum[num] = 0;
        CursorMove(num);
    }

    void UpCursor(int num)
    {

        if (sideNum[num] == 8 && verticalNum[num] == 3)
        {
            verticalNum[num] -= 2;
        }
        else
        { verticalNum[num]--; }
        if (verticalNum[num] < 0) verticalNum[num] = 3;
        CursorMove(num);
    }

    void DownCursor(int num)
    {
        if (sideNum[num] == 8 && verticalNum[num] == 1)
        {
            verticalNum[num] += 2;
        }
        else
        { verticalNum[num]++; }
        if (verticalNum[num] > 3) verticalNum[num] = 0;
        CursorMove(num);
    }

    void CursorMove(int num)
    {
        Debug.Log("verticalNum" + verticalNum[0] + "sideNum" + sideNum[0]);
        Vector3[] pos = new Vector3[2];
        if (verticalNum[num] == 3)
        {
            if (extraNum[num] == 0) { pos[num] = extra_key[0].transform.position; }
            else { pos[num] = extra_key[1].transform.position; }
        }
        else
        { pos[num] = keyboard[verticalNum[num]][sideNum[num]].transform.position; }
        keyCursor[num].transform.position = pos[num];
    }

    void GetInput(int num)
    {
        if (verticalNum[num] == 3)
        {
            if (extraNum[num] == 0)
            { NameBack(num); }
            else if (extraNum[num] == 1)
            {
                if (playerName[num].text.Length > 0)
                { checkFlag[num] = !checkFlag[num]; }
            }

        }
        else
        {
            if (playerName[num].text.Length >= 5) NameBack(num);
            playerName[num].text += textboard[verticalNum[num]][sideNum[num]].text;
        }
    }

    void NameBack(int num)
    {
        if (!(playerName[num].text.Length <= 0))
            playerName[num].text = playerName[num].text.Remove(playerName[num].text.Length - 1, 1);
    }
}


