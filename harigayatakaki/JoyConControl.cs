using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nn.hid;
using UnityEngine.UI;

public enum Style
{ Up, Down, Push }
public enum Hand
{ Left, Right }

public class JoyConControl : MonoBehaviour
{
    public GameObject[] controllers = new GameObject[2];

    [SerializeField]
    private Text textComponent;
    private System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

    private NpadId npadId = NpadId.Invalid;
    private NpadStyle npadStyle = NpadStyle.Invalid;
    private NpadState npadState = new NpadState();

    private NpadJoyDualState npadDualState = new NpadJoyDualState();

    private SixAxisSensorHandle[] handle = new SixAxisSensorHandle[2];
    private SixAxisSensorState state = new SixAxisSensorState();
    private int handleCount = 0;

    private nn.util.Float4 npadQuaternion = new nn.util.Float4();

    class InputSet { public bool Push, Keep, Back = false; }
    Dictionary<NpadButton, InputSet> ButtonKeep = new Dictionary<NpadButton, InputSet>();
    float AnalogStickMax = 32767;
    int[] HandShake = new int[] { 0, 0 };
    private Quaternion quaternion = new Quaternion();
    private Vector3[] JoyConStance = new Vector3[] { Vector3.zero, Vector3.zero };
    private Vector3[] JoyConAcceleration = new Vector3[] { Vector3.zero, Vector3.zero };
    Vector2[] AnalogStick = new Vector2[] { Vector2.zero, Vector2.zero };
    bool[] ShakeCheck = new bool[] { false, false };
    void Start()
    {
        textComponent = GameObject.Find("/Canvas/Text").GetComponent<UnityEngine.UI.Text>();

        Npad.Initialize();
        Npad.SetSupportedStyleSet(NpadStyle.Handheld | NpadStyle.JoyDual | NpadStyle.FullKey);
        NpadId[] npadIds = { NpadId.Handheld, NpadId.No1 };
        Npad.SetSupportedIdType(npadIds);
    }
   
    void Update()
    {
        stringBuilder.Length = 0;
        NowStyle();
        if (UpdatePadState())
        {
            for (int i = 0; i < handleCount; i++)
            {
                SixAxisSensor.GetState(ref state, handle[i]);

                JoyConShake(i);
                SetAnalogStick(i);
                ControllerStance(i);
                JoyConAcceleration[i] = Float3toVector3(state.acceleration);
                stringBuilder.AppendFormat(
                "{0}[{1}]:\nShakeCheck({2})\nAnalogStick({3})\nVector3\n(X:{4})\n(Y:{5})\n(Z:{6})\nJoyConAcceleration\n(X:{7})\n(Y:{8})\n(Z:{9})",
                npadStyle.ToString(), i + 1, ShakeCheck[i], AnalogStick[i],
                JoyConStance[i].x, JoyConStance[i].y, JoyConStance[i].z,
                JoyConAcceleration[i].x, JoyConAcceleration[i].y, JoyConAcceleration[i].z);

                textComponent.text = stringBuilder.ToString();
            }
        }
    }

    //振ったかどうかの判定
    private void JoyConShake(int i)
    {
        SixAxisSensor.GetState(ref state, handle[i]);

        float x = Mathf.Abs(state.acceleration.x);
        float y = Mathf.Abs(state.acceleration.y);
        float z = Mathf.Abs(state.acceleration.z);
        if (x >= 5 || y >= 5 || z >= 5)
            ShakeCheck[i] = true;
        else
            ShakeCheck[i] = false;
    }
    /// <summary> Float3をVector3に変換</summary>
    private Vector3 Float3toVector3(nn.util.Float3 Float3)
    {
        return new Vector3
            (Float3.x, Float3.y, Float3.z);
    }
    private void ControllerStance(int i)
    {
        SixAxisSensor.GetState(ref state, handle[i]);
        state.GetQuaternion(ref npadQuaternion);
        quaternion.Set(npadQuaternion.x, npadQuaternion.z, npadQuaternion.y, -npadQuaternion.w);
        JoyConStance[i] = quaternion.eulerAngles;
    }
    //アナログスティックの入力
    private void SetAnalogStick(int i)
    {
        if (i == 0)
            AnalogStick[i] = Set(npadState.analogStickL);
        if (i == 1)
            AnalogStick[i] = Set(npadState.analogStickR);
    }
    //Float2をVector2に変換
    private Vector2 Set(AnalogStickState AnalogStick)
    {
        return new Vector2
            (AnalogStick.x / AnalogStickMax, AnalogStick.y / AnalogStickMax);
    }

    private void NowStyle()
    {

        NpadId npadId = NpadId.Handheld;
        NpadStyle npadStyle = NpadStyle.None;

        npadStyle = Npad.GetStyleSet(npadId);

        if (npadStyle != NpadStyle.Handheld)
        {
            npadStyle = Npad.GetStyleSet(npadId);
        }
    }
    private bool UpdatePadState()
    {
        NpadStyle handheldStyle = Npad.GetStyleSet(NpadId.Handheld);
        NpadState handheldState = new NpadState();
        if (handheldStyle != NpadStyle.None)
        {
            Npad.GetState(ref handheldState, NpadId.Handheld, handheldStyle);
            if (handheldState.buttons != NpadButton.None)
            {
                if ((npadId != NpadId.Handheld) || (npadStyle != handheldStyle))
                {
                    this.GetSixAxisSensor(NpadId.Handheld, handheldStyle);
                }
                npadId = NpadId.Handheld;
                npadStyle = handheldStyle;
                npadState = handheldState;
                return true;
            }
        }
        NpadStyle no1Style = Npad.GetStyleSet(NpadId.No1);
        NpadState no1State = new NpadState();
        if (no1Style != NpadStyle.None)
        {
            Npad.GetState(ref no1State, NpadId.No1, no1Style);
            if (no1State.buttons != NpadButton.None)
            {
                if ((npadId != NpadId.No1) || (npadStyle != no1Style))
                {
                    this.GetSixAxisSensor(NpadId.No1, no1Style);
                }
                npadId = NpadId.No1;
                npadStyle = no1Style;
                npadState = no1State;
                return true;
            }
        }

        if ((npadId == NpadId.Handheld) && (handheldStyle != NpadStyle.None))
        {
            npadId = NpadId.Handheld;
            npadStyle = handheldStyle;
            npadState = handheldState;
        }
        else if ((npadId == NpadId.No1) && (no1Style != NpadStyle.None))
        {
            npadId = NpadId.No1;
            npadStyle = no1Style;
            npadState = no1State;
        }
        else
        {
            npadId = NpadId.Invalid;
            npadStyle = NpadStyle.Invalid;
            npadState.Clear();
            return false;
        }
        return true;
    }

    private void GetSixAxisSensor(NpadId id, NpadStyle style)
    {
        for (int i = 0; i < handleCount; i++)
        {
            SixAxisSensor.Stop(handle[i]);
        }

        handleCount = SixAxisSensor.GetHandles(handle, 2, id, style);

        for (int i = 0; i < handleCount; i++)
        {
            SixAxisSensor.Start(handle[i]);
        }
    }
    /// <summary>Buttonの入力</summary>
    /// <param name="input">入力するButton</param>
    /// <param name="Push">入力の種類（引数無しの時は押しっぱなし）</param>
    public bool ButtonGet(NpadButton input, Style style = Style.Push)
    {
        try
        {
            return GetButtonSet(ButtonKeep[input], input, style);
        }
        catch (KeyNotFoundException)
        {
            ButtonKeep[input] = new InputSet();
            return GetButtonSet(ButtonKeep[input], input, style);
        }
    }
    private bool GetButtonSet(InputSet inputSet, NpadButton input, Style style)
    {
        inputSet.Push = npadState.GetButtonDown(input);
        if (style == Style.Down) inputSet.Back = inputSet.Push && !inputSet.Keep;
        if (style == Style.Up) inputSet.Back = !inputSet.Push && inputSet.Keep;
        inputSet.Keep = inputSet.Push;
        if (style == Style.Push) return inputSet.Keep;
        return inputSet.Back;

    }
    /// <summary>アナログスティックの入力を取得</summary>
    /// <param name="hand">持ち手</param>
    /// <returns>アナログスティックで入力されたVector2</returns>
    public Vector2 AnalogStickGet(Hand hand)
    {
        switch (hand)
        {
            case Hand.Left:
                return AnalogStick[0];
            case Hand.Right:
                return AnalogStick[1];
        }
        return Vector2.zero;
    }
    /// <summary>JoyConの傾きの取得</summary>
    /// <param name="hand">持ち手</param>
    /// <returns>JoyConStanceのVector3</returns>
    public Vector3 JoyConStanceGet(Hand hand)
    {
        switch (hand)
        {
            case Hand.Left:
                return JoyConStance[0];
            case Hand.Right:
                return JoyConStance[1];
        }
        return Vector3.zero;
    }
    /// <summary>JoyConの加速度の取得</summary>
    /// <param name="hand">持ち手</param>
    /// <returns>AccelerationのVector3</returns>
    public Vector3 AccelerationGet(Hand hand)
    {
        Vector3 m_leftHandAcc = new Vector3(0, 1, 0);
        Vector3 m_rightHandAcc = new Vector3(0, -1, 0);
        switch (hand)
        {
            case Hand.Left:
                return JoyConAcceleration[0];
            case Hand.Right:
                return JoyConAcceleration[1] ;
        }
        return Vector3.zero;
    }
}
