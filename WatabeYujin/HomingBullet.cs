using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour {
    [SerializeField]
    Transform homingTransform;
    [SerializeField]
    private bool isMove;

    private const float speed = 10;
    Rigidbody thisRigidbody;
	[SerializeField]
    private float targetDistance;
	Vector3 centerPos;

	void Start () {
        thisRigidbody = GetComponent<Rigidbody>();
		TargetTransformSet (homingTransform);
	}
	
	// Update is called once per frame
	void Update () {
        BulletMove();
	}

    void BulletMove()
    {
        if (!isMove) return;
        thisRigidbody.velocity = transform.forward *speed;
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
