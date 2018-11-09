using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    // 저장된 숫자, 연산자 정보 저장 num >= 0
    public int num;
    public char oper;

    private bool isDying;

    void Awake() {
        isDying = false;
    }

    void Update () {
        // 부모 Equation 없어지면 IEnumerator 시작
        if (transform.parent.name == "Equations" && !isDying) {
            isDying = true;
            StartCoroutine("DieAfterSeconds");
        }
    }

    IEnumerator DieAfterSeconds() {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    }
}
