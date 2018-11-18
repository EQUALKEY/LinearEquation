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
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "bullet_default" && ec.haveBullet==0) { // 캐릭터와 기본 총알 충돌 시
            ec.haveBullet = 1;
            ec.MakeBullet();
            Destroy(collision.gameObject);
        }
        else if(collision.tag == "bullet_eq" && ec.haveBullet == 0) { // 캐릭터와 Oper 총알 충돌 시
            ec.haveBullet = 2;
            ec.num = collision.GetComponent<Bullet_eq>().num;
            ec.oper = collision.GetComponent<Bullet_eq>().oper;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "MapEdge") {
            Vector2 VelocityBeforeCollision = collision.relativeVelocity;
            if (collision.transform.name == "Left" && collision.relativeVelocity.x > 0) transform.position = new Vector3(19.2f, transform.position.y);
            else if (collision.transform.name == "Right" && collision.relativeVelocity.x < 0) transform.position = new Vector3(-19.2f, transform.position.y);
            else if (collision.transform.name == "Top" && collision.relativeVelocity.y < 0) transform.position = new Vector3(transform.position.x, -19.2f);
            else if (collision.transform.name == "Bottom" && collision.relativeVelocity.y > 0) transform.position = new Vector3(transform.position.x, 19.2f);
            transform.GetComponent<Rigidbody2D>().velocity = -VelocityBeforeCollision;
            ec.SetCameraWithCharacterPos();
        }
    }
}
