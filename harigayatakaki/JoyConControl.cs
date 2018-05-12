using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nn.hid;
using UnityEngine.UI;

public enum Style
{ Up, Down, Push }

public class JoyConControl : MonoBehaviour
{
    
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
                GetAnalogStick(i);
                ControllerStance(i);
                stringBuilder.AppendFormat(
                "{0}[{1}]:\nShakeCheck({2})\nAnalogStick({3})\nVector3\n(X:{4})\n(Y:{5})\n(Z:{6})\n",
                npadStyle.ToString(), i + 1, ShakeCheck[i], AnalogStick[i],
                JoyConStance[i].x, JoyConStance[i].y, JoyConStance[i].z);

                textComponent.text = stringBuilder.ToString();
            }
        }
    }

    /// <summary>Buttonの入力</summary>
    /// <param name="input">入力するButton</param>
    /// <param name="Style">入力の種類（引数無しの時は押しっぱなし）</param>
    public bool ButtonGet(NpadButton input, Style Push = Style.Push)
    {
        switch (Push)
        {
            case Style.Up: return npadState.GetButtonUp(input);
            case Style.Down: return npadState.GetButtonDown(input);
            case Style.Push: return npadState.GetButton(input);
        }
        return false;
    }
    //コントローラーの加速度とその値の取得
    private void JoyConShake(int i)
    {
        SixAxisSensor.GetState(ref state, handle[i]);

        float x = Mathf.Abs(state.acceleration.x);
        float y = Mathf.Abs(state.acceleration.y);
        float z = Mathf.Abs(state.acceleration.z);
        JoyConAcceleration[i] = AccelerationSst(state.acceleration);
        if (x >= 5 || y >= 5 || z >= 5)
            ShakeCheck[i] = true;
        else
            ShakeCheck[i] = false;
    }
    //加速度の値が特殊なのでVector3に変換
    private Vector3 AccelerationSst(nn.util.Float3 acceleration)
    {
        return new Vector3
            (acceleration.x, acceleration.y, acceleration.z);
    }
    //コントローラーの傾きの取得
    private void ControllerStance(int i)
    {
        SixAxisSensor.GetState(ref state, handle[i]);
        state.GetQuaternion(ref npadQuaternion);
        quaternion.Set(npadQuaternion.x, npadQuaternion.z, npadQuaternion.y, -npadQuaternion.w);
        JoyConStance[i] = quaternion.eulerAngles;
    }
    //アナログスティックの入力の取得
    private void GetAnalogStick(int i)
    {
        if (i == 0)
            AnalogStick[i] = Set(npadState.analogStickL);
        if (i == 1)
            AnalogStick[i] = Set(npadState.analogStickR);
    }
    //アナログスティックの入力が特殊なのでVector2に変換
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
    public class JoyConInput
    {
        /// <summary>左のアナログスティックの入力</summary>
        public Vector2 LeftAnalogStick;
        /// <summary>右のアナログスティックの入力</summary>
        public Vector2 RightAnalogStick;
        /// <summary>左の振った判定</summary>
        public bool LeftShakeCheck;
        /// <summary>右の振った判定</summary>
        public bool RightShakeCheck;
        /// <summary>左のJoyConのジャイロのVector3</summary>
        public Vector3 LeftJoyConStance;
        /// <summary>右のJoyConのジャイロのVector3</summary>
        public Vector3 RightJoyConStance;
        /// <summary>左のJoyConのAccelerationのVector3</summary>
        public Vector3 LeftJoyConAcceleration;
        /// <summary>右のJoyConのAccelerationのVector3</summary>
        public Vector3 RightJoyConAcceleration;
    }
    public JoyConInput InputGet()
    {
        JoyConInput Get_Input = new JoyConInput();
        Get_Input.LeftAnalogStick = AnalogStick[0];
        Get_Input.LeftJoyConStance = JoyConStance[0];
        Get_Input.LeftShakeCheck = ShakeCheck[0];
        Get_Input.LeftJoyConAcceleration = JoyConAcceleration[0];
        Get_Input.RightAnalogStick = AnalogStick[1];
        Get_Input.RightJoyConStance = JoyConStance[1];
        Get_Input.RightShakeCheck = ShakeCheck[1];
        Get_Input.RightJoyConAcceleration= JoyConAcceleration[1];
        return Get_Input;
    }
}
