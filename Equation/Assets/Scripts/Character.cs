﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    public GameObject EventController;
    private EventController ec;
    
	void Awake() {
        ec = EventController.GetComponent<EventController>();
	}

    // Item이랑 충돌시 먹고 ec에 haveBullet, num, oper 값 업데이트
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "bullet_default" && ec.haveBullet==0) {
            ec.haveBullet = 1;
            Destroy(collision.gameObject);
        }
        else if(collision.tag == "bullet_eq" && ec.haveBullet == 0) {
            ec.haveBullet = 2;
            ec.num = collision.GetComponent<Bullet_eq>().num;
            ec.oper = collision.GetComponent<Bullet_eq>().oper;
            Destroy(collision.gameObject);
        }
    }
}
