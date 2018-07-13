using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRotate : MonoBehaviour {
    float speed = 1;
    Vector3 pos = new Vector3(0,0,0);
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        pos.x += Mathf.Cos(Time.time * speed)*0.1f;
        //pos.z += Mathf.Cos(Time.time * speed);
        //pos.x += Mathf.Cos(Time.time * speed);
        transform.localPosition = pos;
    }
}
