using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour {
    [SerializeField]
    Transform homingTransform;
    [SerializeField]
    private bool isMove;
    [SerializeField]
    private Rigidbody thisRigidbody;
    [SerializeField]
    private int playerID;
    [SerializeField]
    private float distance;
    [SerializeField]
    private GameObject effect;

    private const float speed = 50;
    float firstPosZ;
	Vector3 centerPos;


    private void Start()
    {
        firstPosZ = transform.position.z;
    }
    void Update () {
        BulletMove();
        if (DistanceCheck()) DestroyEvent();
	}

    void BulletMove()
    {
        if (!isMove) return;
        thisRigidbody.velocity = transform.forward * speed;
}

bool DistanceCheck()
    {
        if (firstPosZ + distance > transform.position.z) return false;
        return true;
    }

    void DestroyEvent()
    {
        
        Destroy(gameObject); 
    }
    void Effect()
    {
        const float m_effectdestroyTime = 1.3f;
        if (effect)
        {
            GameObject m_obj = Instantiate(effect);
            m_obj.transform.position = transform.position;
            Destroy(m_obj, m_effectdestroyTime);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        const string m_targetTagName = "Target";
        if (collision.transform.tag != m_targetTagName) return;
        Effect();
        if (!collision.transform.GetComponent<TargetObject>().Damage(playerID))
        {
            DestroyEvent();
            return;
        }
        foreach (ContactPoint point in collision.contacts)
        {
            MeshExplosion.meshExplosion.Explode(collision.transform, point.point, thisRigidbody.velocity);
            DestroyEvent();
            break;
        }
    }

    public void TargetTransformSet(Transform targetTransform)
    {
        homingTransform = targetTransform;
        isMove = true;
    }

    public bool IsMove
    {
        set
        {
            isMove = value;
        }
        get
        {
            return isMove;
        }
    }
}
