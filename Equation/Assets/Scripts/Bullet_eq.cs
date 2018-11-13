using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_eq : MonoBehaviour {
    // 정보 저장, num >= 0, oper = +, -, *, /
    public int num;
    public char oper;

    void Awake() {
        StartCoroutine("DestroyAfter30s");
    }

    IEnumerator DestroyAfter30s() {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    }
}
