using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    public GameObject EventController;

    private GameObject Collision;   // 부딫히는 중인 GameObject
    private EventController ec;
    private bool isOut;             // Out과 충돌 중인지 확인
    private int ChildCount;         // Children 수, 0 ~ 2

    void Awake()
    {
        ec = EventController.GetComponent<EventController>();
        isOut = true;
        ChildCount = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "out")
            isOut = true;
        Collision = collision.transform.gameObject;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "out")
            isOut = false;
        if (Collision != null && 
            Collision.transform.name == collision.transform.name) Collision = null;
    }

    public void SpaceInput()
    {
        ChildCount = transform.childCount;
        if (isOut && ChildCount == 2) // Out에 갖다놓는 경우
        {
            if (transform.GetChild(0).tag == "num")
            {
                ec.GetNum = (int)System.Char.GetNumericValue(transform.GetChild(0).name[0]);
                ec.GetOper = transform.GetChild(1).tag;
            } else
            {
                ec.GetNum = (int)System.Char.GetNumericValue(transform.GetChild(1).name[0]);
                ec.GetOper = transform.GetChild(0).tag;
            }
            for (int i = 1; i >=0; i--) Destroy(transform.GetChild(i).gameObject);
            if (!ec.isOperated)
            {
                ec.Operate();
                ec.isOperated = true;
            }
            else ec.isOperated = false;
        }
        else if (Collision == null)   // Item 버리는 경우
        {
            if (ChildCount != 0)
                transform.GetChild(ChildCount - 1).transform.parent = ec.ItemsPar.transform;
        }
        else if (Collision.tag != "out") // Item 줍는 경우
        {
            if (ChildCount == 0 || (ChildCount == 1 && transform.GetChild(0).tag != Collision.transform.tag)) {
                Collision.transform.parent = transform;
            }
            else if (ChildCount == 1 && transform.GetChild(0).transform.tag == Collision.transform.tag)
            {
                transform.GetChild(0).parent = ec.ItemsPar.transform;
                Collision.transform.parent = transform;
            }
            else if (ChildCount == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (transform.GetChild(i).tag == Collision.transform.tag)
                    {
                        transform.GetChild(i).parent = ec.ItemsPar.transform;
                        Collision.transform.parent = transform;
                    }
                }
            }
        }
    }
}
