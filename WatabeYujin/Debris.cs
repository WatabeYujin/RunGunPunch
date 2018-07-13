using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour {
    Vector3 spawnscale;
    float time;
    void Start()
    {
        spawnscale = transform.localScale;
    }

	void Update () {
        time += Time.deltaTime;
        transform.localScale = spawnscale - Vector3.one * time*2;
        if (transform.localScale.x > 0 &&
            transform.localScale.y > 0 &&
            transform.localScale.z > 0) return;
        Destroy(gameObject);
	}

}
