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

	void Update () {
        transform.Translate(dir * Time.deltaTime * velocity);
        if (transform.position.x < -20f || transform.position.x > 20f || transform.position.y < -20f || transform.position.y > 20f) Destroy(gameObject);
	}
}
