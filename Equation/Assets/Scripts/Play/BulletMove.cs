using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour {

    float velocity;

    private void Awake() {
        velocity = 10f; // 발사한 총알 속도
    }

	void Update () {
        transform.Translate(Vector3.up * Time.deltaTime * velocity);
        // 발사한 총알이 밖으로 나가면 제거
        if (transform.position.x < -30f || transform.position.x > 30f || transform.position.y < -30f || transform.position.y > 30f) Destroy(gameObject);
	}
}
