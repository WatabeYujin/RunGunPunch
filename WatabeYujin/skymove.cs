using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skymove : MonoBehaviour {
    [SerializeField]
    private float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles += Vector3.up * speed;
	}
}
