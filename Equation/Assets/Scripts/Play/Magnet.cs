using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {
    public EventController ec;
    public Vector3 CharacterPos;
    private float ComingVelocity;

    private void Awake() {
        ComingVelocity = 2f;
    }

    private void Update() {
        CharacterPos = ec.Character.transform.position;
        if (ec.magnetActive && ec.haveBullet == 0 && Vector3.Distance(transform.position, CharacterPos) <= 2f) {
            transform.position -= (transform.position - CharacterPos) * ComingVelocity * Time.deltaTime;
        }
    }
}
