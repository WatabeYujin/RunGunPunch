using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yazirusimove : MonoBehaviour {
    [SerializeField]
    float speed = 1;
    [SerializeField]
    RectTransform rect;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void yazimove()
    {
        StartCoroutine(move());
    }
    IEnumerator move()
    {
        for (;;)
        {
            rect.position += Vector3.up * speed;
            yield return null;
            if (speed >= 0 && rect.position.y >= Screen.height / 2)
                break;
            if (speed <= 0 && rect.position.y <= Screen.height / 2)
                break;
        }
        yield return new WaitForSeconds(2f);
        Destroy(gameObject, 2);
        for (;;)
        {
            rect.position += Vector3.up * speed;
            yield return null;
        }
    }
}
