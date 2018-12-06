using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapEquation : MonoBehaviour {
    private GameObject Equation;
    private Vector3 EqPosition;
    private float MinimapRate;

    // 미니맵에 수식 생성하면서 원래 수식을 세팅해주려고 만든 함수
    public void SetEquation(GameObject eq) {
        Equation = eq;
        MinimapRate = 2.9f;
    }
    
    void Update() {
        // 부모 수식 없어지면 미니맵 수식도 제거
        if (Equation == null) Destroy(gameObject);
        else { // 부모 수식 따라 미니맵에서 움직임. 비율은 아래 식대로
            EqPosition = Equation.transform.position;
            transform.localPosition = new Vector3(EqPosition.x * MinimapRate, EqPosition.y * MinimapRate - 190f, 0f);
        }
    }
}
