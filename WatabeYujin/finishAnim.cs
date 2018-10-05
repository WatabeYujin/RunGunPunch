using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finishAnim : MonoBehaviour {
    [SerializeField]
    private Animator[] anim = new Animator[2];
    [SerializeField]
    private int finishAnimInt = 0;
	// Use this for initialization
	void Awake () {
        anim[0].SetInteger("losemotion", finishAnimInt);
        anim[1].SetInteger("losemotion", finishAnimInt);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
