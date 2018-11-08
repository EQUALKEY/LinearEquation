using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    public GameObject EventController;
    private EventController ec;
    
	void Awake() {
        ec = EventController.GetComponent<EventController>();
	}

    // Item이랑 충돌시 먹고 ec에 haveBullet, num, oper 값 업데이트
}
