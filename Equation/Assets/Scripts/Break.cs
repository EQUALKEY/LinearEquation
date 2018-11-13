using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour {
    private Rigidbody2D r;

    void Awake() {
        r = transform.GetComponent<Rigidbody2D>();
    }
    
	void Update () {
        r.AddForce(-r.velocity * Time.deltaTime * 30f * r.mass);
    }
}
