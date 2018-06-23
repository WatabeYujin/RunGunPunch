using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccSmartPhoneTest : MonoBehaviour {
    [SerializeField]
    Text txt;
    [SerializeField]
    Text txt2;
    [SerializeField]
    RobotControl roboCon;

    Vector3 acc;
    Vector3 nowAcceleration;
    Vector3 oldAcceleration;
    bool samplingFlag;
    Vector3[] AccelerationKeep = new Vector3[10];
    int listCount;
    float[] max = new float[3] {0,0,0 };
	void Update () {
        acc = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
        txt2.text = "左：" + max[0] + "　中央：" + max[1] + "　右：" + max[2];
        Attack();

    }
    void Attack()
    {
        const float l_controlValue_x = 4f;
        const float l_controlValue_y = 4f;


        Vector3 m_acceleration = new Vector3();

        oldAcceleration = nowAcceleration;
        Vector3 m_baseAcc = new Vector3(0, -1, 0);
        nowAcceleration = acc - m_baseAcc;
        txt.text = "" + nowAcceleration;

        if (!samplingFlag)
            samplingFlag = Mathf.Abs(nowAcceleration.z) >= l_controlValue_x ||  Mathf.Abs(nowAcceleration.y) >= l_controlValue_y;
        else
        {
            AccelerationKeep[listCount] = nowAcceleration;
            listCount++;

            if (listCount >= AccelerationKeep.Length)
            {
                Vector3 Vec3 = Vector3.zero;

                for (int i = 0; i < listCount; i++)
                    Vec3 += AccelerationKeep[i];

                for (int i = 0; i < 10; i++)
                    AccelerationKeep[i] = Vector3.zero;

                m_acceleration = Vec3;
                listCount = 0;
                samplingFlag = false;
            }
        }
        if (Mathf.Abs(m_acceleration.y) >= l_controlValue_y &&
            Mathf.Abs(m_acceleration.z) < l_controlValue_x)
        {
            if (max[1] < Mathf.Abs(m_acceleration.y)) max[1] = Mathf.Abs(m_acceleration.y);
            roboCon.Attack(0, RobotControl.Lane.Center);
        }
        if (Mathf.Abs(m_acceleration.z) >= l_controlValue_x)
        {
            if (m_acceleration.z >=0)
            {
                if(max[2]<m_acceleration.z)max[2] = m_acceleration.z;
                roboCon.Attack(0, RobotControl.Lane.Right);
            }
            else
            {
                if (max[0] > m_acceleration.z) max[0] = m_acceleration.z;
                roboCon.Attack(0, RobotControl.Lane.Left);
            }
        }
    }
    public void MaxReset()
    {
        for(int i = 0; i < 3; i++)
        {
            max[i] = 0;
        }
    }
}
