using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour {
    public GameObject Character;
    private float CharacterAngularVelocity;
    private float Force;
    private float VelocityLimit;
    private Vector2 CharacterVelocity;
    public Text Text;

    void Awake()
    {
        VelocityLimit = 3f;
        CharacterVelocity = new Vector2();
        CharacterAngularVelocity = 500f;
        Force = 100f * Character.GetComponent<Rigidbody2D>().mass;
    }

    void Update() {
        // 캐릭터 이동 및 속도 조절. 건드리지 말자
        CharacterVelocity = Character.GetComponent<Rigidbody2D>().velocity;
        Text.text = CharacterVelocity.magnitude.ToString();
        float Cross = CharacterVelocity.y * Character.transform.up.y + CharacterVelocity.x * Character.transform.up.x;
        if (Input.GetKey(KeyCode.LeftArrow)) Character.transform.rotation *= Quaternion.Euler(Vector3.forward * CharacterAngularVelocity * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow)) Character.transform.rotation *= Quaternion.Euler(-Vector3.forward * CharacterAngularVelocity * Time.deltaTime);
        if (Input.GetKey(KeyCode.UpArrow) && (CharacterVelocity.magnitude <= VelocityLimit || Cross < 0)) Character.GetComponent<Rigidbody2D>().AddForce(Character.transform.up * Force * Time.deltaTime);
        if (Input.GetKey(KeyCode.DownArrow) && (CharacterVelocity.magnitude <= VelocityLimit || Cross > 0)) Character.GetComponent<Rigidbody2D>().AddForce(-Character.transform.up * Force * Time.deltaTime);
    }
}
