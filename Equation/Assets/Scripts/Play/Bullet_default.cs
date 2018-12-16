using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_default : MonoBehaviour {
    private EventController ec;

    private void Awake() {
        // 발사하는 Bullet들은 EventController Object를 부모로 가짐
        ec = transform.parent.GetComponent<EventController>();
        ec.Score += 10;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 발사한 기본 총알이 아이템에 부딫히면
        if (collision.tag == "Item") {
            ec.Score += 50;
            GameObject new_bullet_eq = Instantiate(ec.OperBullet, collision.transform.position, collision.transform.rotation, ec.Bullets.transform);
            new_bullet_eq.GetComponent<Bullet_eq>().num = collision.GetComponent<Item>().num;
            new_bullet_eq.GetComponent<Bullet_eq>().oper = collision.GetComponent<Item>().oper;
            new_bullet_eq.GetComponent<Magnet>().ec = ec;
            Destroy(collision.gameObject);
            Destroy(gameObject);
        } // 발사한 기본 총알이 수식에 부딫히면
        else if (collision.tag == "Equation") {
            ec.Score += 30;
            ec.RemakeEquation(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
