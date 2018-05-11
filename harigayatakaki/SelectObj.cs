using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObj : MonoBehaviour
{
    [SerializeField]
    List<GameObject> TargetObj = new List<GameObject>();
    [SerializeField]
    JoyConControl JoyCon_Input;
    List<Material> objmt = new List<Material>();
    [SerializeField]
    List<GameObject> Setobj = new List<GameObject>();
    bool AnalogStickInput = true;
    void Start()
    {
        Setobj.Add(TargetObj[0]);
        Setobj.Add(TargetObj[0]);
        StartCoroutine(SelfUpdate());

    }
    IEnumerator SelfUpdate()
    {
        for (;;)
        {

            if (JoyCon_Input.InputGet().JoyConButton_ZL && JoyCon_Input.InputGet().JoyConButton_ZR)
                AnalogStickInput = ! AnalogStickInput;
            for (int i = 0; i <= 1; i++)
                Set(i);
            yield return null;
        }

    }
    void Set(int i)
    {
        float AnalogStick = 0;
        float JoyConStance = 0;

        switch (AnalogStickInput)
        {
            case true:
                if (i == 0)
                    AnalogStick = JoyCon_Input.InputGet().LeftAnalogStick;
                else if (i == 1)
                    AnalogStick = JoyCon_Input.InputGet().RightAnalogStick;
                break;
            case false:
                if (i == 0)
                    JoyConStance = JoyCon_Input.InputGet().LeftJoyConStance.y;
                if (JoyConStance >= 180) JoyConStance -= 360;
                else if (i == 1)
                    JoyConStance = JoyCon_Input.InputGet().RightJoyConStance.y;
                if (JoyConStance >= 180) JoyConStance -= 360;
                break;
        }
        int x = 0;
        if (i == 1)
            x += 3;
        if (AnalogStick >= 0.7f || JoyConStance >= 60)
        { Mark(0 + x); Clear(2 + x); Clear(1 + x); }
        else if (AnalogStick <= -0.7f || JoyConStance <= -60)
        { Mark(2 + x); Clear(0 + x); Clear(1 + x); }
        else
        { Mark(1 + x); Clear(2 + x); Clear(0 + x); }

    }
    void Clear(int i)
    {
        TargetObj[i].GetComponent<Renderer>().material.color = Color.white;
    }
    void Mark(int i)
    {
        TargetObj[i].GetComponent<Renderer>().material.color = Color.yellow;

        if (i <= 2)
            Setobj[0] = TargetObj[i];
        else
            Setobj[1] = TargetObj[i];
    }
}

