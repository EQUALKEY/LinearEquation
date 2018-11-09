using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_eq_shoot : MonoBehaviour {
    // 정보 저장, num >= 0
    public int num;
    public char oper;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Equation") {
            collision.GetComponent<Equation>().Operate(num, oper);
            Destroy(gameObject);
        }
    }
}
