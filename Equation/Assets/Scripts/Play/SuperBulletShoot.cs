using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBulletShoot : MonoBehaviour {
    private EventController ec;

    private void Awake() {
        ec = transform.parent.GetComponent<EventController>();
        ec.Score += 20;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Equation") { // 발사된 Oper 총알이 수식에 만난 경우 Operate
            ec.Score += 200;
            collision.GetComponent<Equation>().DoSolve();
            Destroy(gameObject);
        }
    }
}
