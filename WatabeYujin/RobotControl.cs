using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotControl : MonoBehaviour
{
    [SerializeField]
    private Transform[] centerShotTransform =
        new Transform[2];                       //�����̍U������̈ʒu�i�z��0��1P,�z��1��2P�j
    [SerializeField]
    private Transform[] rightShotTransform =
        new Transform[2];                       //�E�̍U������̈ʒu�i�z��0��1P,�z��1��2P�j
    [SerializeField]
    private Transform[] leftShotTransform =
        new Transform[2];                       //���̍U������̈ʒu�i�z��0��1P,�z��1��2P�j
    [SerializeField]
    private LineRenderer lineRenderer;          //�U�����ɕ\�����郉�C����LineRenderer
    [SerializeField]
    private Material[] lineMaterials =
        new Material[2];                        //�U�����\������LineRenderer��Material
    [SerializeField]
    private float attackDistance = 15;          //�ő�˒�

    IEnumerator lineDeleteIEnumerator;

    public enum Lane                            //�U�����[���̈ʒu
    {
        Left = 1,
        Center = 2,
        Right = 3
    }

    ///////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        lineDeleteIEnumerator = LineRendererDelete();
    }

    void Update()
    {
        PcJoyConControl();
    }

    /// <summary>
    /// PC�ł�JoyCon��U����͂��s���B
    /// 1P�FA=���U�� , S=�c�U�� , D=�E�U��
    /// 2P�F�����L�[��=���U�� , �����L�[��=�c�U�� , �����L�[��=�E�U��
    /// </summary>
    void PcJoyConControl()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Attack(0, Lane.Left);
        if (Input.GetKeyDown(KeyCode.S))
            Attack(0, Lane.Center);
        if (Input.GetKeyDown(KeyCode.D))
            Attack(0, Lane.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Attack(1, Lane.Left);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            Attack(1, Lane.Center);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Attack(1, Lane.Right);
    }

    /// <summary>
    /// �ݒ肵���U������̏ꏊ����Ray���΂��Ė���������s��
    /// </summary>
    /// <param name="shotTransform">Ray���΂�Transform</param>
    /// <returns><para>��������Target�^�O��GameObject</para><para>�i�������Ȃ������A���邢�͕ʂ̃^�O�������ꍇnull��Ԃ��j</para></returns>
    private GameObject AttackRayCast(Transform shotTransform)
    {
        Ray m_attackRay = new Ray(shotTransform.position, shotTransform.forward);
        RaycastHit m_hit;                                 //���C�̋���
        const string m_targetTagName = "Target";
        if (!Physics.Raycast(m_attackRay, out m_hit, attackDistance))
            return null;
        if (m_hit.collider.tag != m_targetTagName)
            return null;
        return m_hit.collider.gameObject;
    }

    /// <summary>
    /// ���̓^�[�Q�b�g���ݎ��̍U������
    /// </summary>
    /// <param name="playerID">�v���C���[��ID</param>
    /// <param name="attackLane">�U�����郌�[���̈ʒu(enum)</param>
    void CompositeAttack(int playerID, Lane attackLane)
    {
        Transform m_shotTransform = centerShotTransform[playerID];
        GameObject m_hitTargetObject = AttackRayCast(m_shotTransform);                     //Ray�ɂ�閽��������s��        
        if (m_hitTargetObject != null)
        {
            m_hitTargetObject.GetComponent<TargetObject>().CompositeDamage(playerID, (int)attackLane);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_hitTargetObject.transform.position);
        }
        else
        {
            Vector3 m_endPosition = m_shotTransform.position + (m_shotTransform.transform.forward * attackDistance);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_endPosition);
        }
    }

    private void RayView(Material lineMaterial, Vector3 shotPosition, Vector3 endPosition)
    {
        lineRenderer.enabled = true;
        lineRenderer.material = lineMaterial;
        lineRenderer.SetPosition(0, shotPosition);
        lineRenderer.SetPosition(1, endPosition);
        StopCoroutine(lineDeleteIEnumerator);
        StartCoroutine(lineDeleteIEnumerator);
    }

    private IEnumerator LineRendererDelete()
    {
        const float m_deleteTime = 0.5f;
        yield return new WaitForSeconds(m_deleteTime);
        lineRenderer.enabled = false;
    }

    ///////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// �v���C���[�̍U���̏������s��
    /// </summary>
    /// <param name="playerID">�v���C���[��ID</param>
    /// <param name="attackLane">�U�����郌�[���̈ʒu(enum)</param>
    public void Attack(int playerID, Lane attackLane)
    {
        if (PlaySceneManager.SceneManager.GetSetNowCondition ==
            PlaySceneManager.Condition.Reverse)
            playerID = playerID == 0 ? 1 : 0;
        if (PlaySceneManager.SceneManager.GetSetNowCondition ==
            PlaySceneManager.Condition.Composite)
        {
            CompositeAttack(playerID, attackLane);
            return;
        }
        GameObject m_hitTargetObject = null;
        Transform m_shotTransform = null;
        if (attackLane == Lane.Center)
            m_shotTransform = centerShotTransform[playerID];
        if (attackLane == Lane.Right)
            m_shotTransform = rightShotTransform[playerID];
        if (attackLane == Lane.Left)
            m_shotTransform = leftShotTransform[playerID];

        m_hitTargetObject = AttackRayCast(m_shotTransform);                     //Ray�ɂ�閽��������s��

        if (m_hitTargetObject != null)
        {
            m_hitTargetObject.GetComponent<TargetObject>().Damage(playerID);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_hitTargetObject.transform.position);
        }
        else
        {
            Vector3 m_endPosition = m_shotTransform.position + (m_shotTransform.transform.forward * attackDistance);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_endPosition);
        }
    }

    /// <summary>
    /// ��ɏ���Ă�p�C���b�g�����𓮂�������
    /// </summary>
    /// <param name="playerID">�v���C���[��id</param>
    /// <param name="axis">�X�e�B�b�N�̓���</param>
    public void Pilot(int playerID, Vector2 axis)
    {

    }


}