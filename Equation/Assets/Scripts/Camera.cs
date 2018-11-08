using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    public GameObject Character;
    private Vector3 Direction;
    private float Velocity;

    void Awake()
    {
        Velocity = 5f;
    }

	void Update () {
        Direction = transform.position - Character.transform.position;
        if(transform.position.z < 0) transform.Translate(new Vector3(-Direction.x * Velocity, -Direction.y * Velocity, 0f) * Time.deltaTime);
        else  transform.Translate(new Vector3(Direction.x * Velocity, -Direction.y * Velocity, 0f) * Time.deltaTime);
    }
}
