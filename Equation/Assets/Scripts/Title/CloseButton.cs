using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour {
    public GameObject target;

    void OnMouseDown() {
        target.SetActive(false);
    }
}
