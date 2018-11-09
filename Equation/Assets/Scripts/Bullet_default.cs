using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_default : MonoBehaviour {
    private EventController ec;

    private void Awake() {
        ec = transform.parent.GetComponent<EventController>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Item") {
            GameObject new_bullet_eq = Instantiate(ec.OperBullet, collision.transform.position, collision.transform.rotation);
            new_bullet_eq.GetComponent<Bullet_eq>().num = collision.GetComponent<Item>().num;
            new_bullet_eq.GetComponent<Bullet_eq>().oper = collision.GetComponent<Item>().oper;
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.tag == "Equation") {
            ec.RemakeProblem(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
