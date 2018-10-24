using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    public GameObject EventController;

    private EventController ec;

    void Awake()
    {
        ec = EventController.GetComponent<EventController>();
    }

    public int GetItem()
    {
        // if(collider 부딫히면) 태그보고 처리
        // 아이템을 줍는 경우와 Out에 갖다놓는 경우 처리
        return -1;
    }
}
