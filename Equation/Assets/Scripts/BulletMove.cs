using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour {

    float velocity;
    public Vector3 dir;

    private void Awake()
    {
        velocity = 10f;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(dir * Time.deltaTime * velocity);
	}
}
