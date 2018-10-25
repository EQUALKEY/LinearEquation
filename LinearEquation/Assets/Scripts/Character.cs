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
    {   // 기본적인 초기화
        ec = EventController.GetComponent<EventController>();
        isOut = true;
        ChildCount = 0;
    }

    // Trigger Enter, Exit으로 Out과 만나고 있는지, [Collision = 부딫히고 있는 GameObject] 가 되도록 제어
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

    public void SpaceInput()    // 스페이스바 눌렀을 때
    {
        ChildCount = transform.childCount;  // 캐릭터 자식 수
        // Out에 갖다놓는 경우 (인자 2개 다 주웠을 때만 실행)
        if (isOut && ChildCount == 2)
        {
            // 주운 인자들 반환
            if (transform.GetChild(0).tag == "num")
            {
                ec.GetNum = (int)System.Char.GetNumericValue(transform.GetChild(0).name[0]);
                ec.GetOper = transform.GetChild(1).tag;
            }
            else
            {
                ec.GetNum = (int)System.Char.GetNumericValue(transform.GetChild(1).name[0]);
                ec.GetOper = transform.GetChild(0).tag;
            }
            // 자식들 제거
            for (int i = 1; i >=0; i--) Destroy(transform.GetChild(i).gameObject);
            // EventController.Operate() 중복실행 방지
            if (!ec.isOperated)
            {
                ec.Operate();
                ec.isOperated = true;
            }
            else ec.isOperated = false;
        }
        // Item 버리는 경우
        else if (Collision == null)
        {
            if (ChildCount != 0)
                transform.GetChild(ChildCount - 1).transform.parent = ec.ItemsPar.transform;
        }
        // Item 줍는 경우
        else if (Collision.tag != "out")
        {
            // 자식이 없거나 새로운 인자를 주우면 캐릭터에 붙임
            if (ChildCount == 0 || (ChildCount == 1 && transform.GetChild(0).tag != Collision.transform.tag)) {
                Collision.transform.parent = transform;
            }
            // 똑같은 종류의 인자를 주우면 바꿈 (숫자 가지고 있는데 숫자 또 주우면 바꿈)
            else if (ChildCount == 1 && transform.GetChild(0).transform.tag == Collision.transform.tag)
            {
                transform.GetChild(0).parent = ec.ItemsPar.transform;
                Collision.transform.parent = transform;
            }
            // 2개 가지고 있는데 주우면 있는거 중에 같은 종류의 인자를 바꿈. ex)(숫자, 연산자)가지고 있는데 숫자를 주우면 가지고 있던 숫자가 바뀜
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
