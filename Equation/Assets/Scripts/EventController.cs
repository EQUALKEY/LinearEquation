using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour
{
    // Prefab 부모
    public GameObject Equations;
    public GameObject Bullets;

    // 캐릭터 이동
    public GameObject Character;
    private float CharacterAngularVelocity;
    private float Force;
    private float VelocityLimit;
    private Vector2 CharacterVelocity;
    public Text Text;

    // Bullet
    public GameObject DefaultBullet;
    public GameObject OperBullet;

    //----------------------------------------------------------------------------------------------------------------
    private Vector3[] ProblemPosition = { new Vector3(-20f, -20f, 0f), new Vector3(0f, -20f, 0f), new Vector3(20f, -20f, 0f), new Vector3(-20f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(20f, 0f, 0f), new Vector3(-20f, 20f, 0f), new Vector3(0f, 20f, 0f), new Vector3(20f, 20f, 0f) };
    private int[] NumOfPosition = new int[9];
    public GameObject Problem;

    public bool haveBullet;
    public int num;
    public char oper;

    void Awake()
    {
        VelocityLimit = 3f;
        CharacterVelocity = new Vector2();
        CharacterAngularVelocity = 500f;
        Force = 100f * Character.GetComponent<Rigidbody2D>().mass;
        
        while (Equations.transform.childCount < 5) MakeProblem();
        StartCoroutine(BulletManage());
    }

    void Update()
    {
        // 캐릭터 이동 및 속도 조절. 건드리지 말자
        CharacterVelocity = Character.GetComponent<Rigidbody2D>().velocity;
        Text.text = CharacterVelocity.magnitude.ToString();
        float Cross = CharacterVelocity.y * Character.transform.up.y + CharacterVelocity.x * Character.transform.up.x;
        if (Input.GetKey(KeyCode.LeftArrow)) Character.transform.rotation *= Quaternion.Euler(Vector3.forward * CharacterAngularVelocity * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow)) Character.transform.rotation *= Quaternion.Euler(-Vector3.forward * CharacterAngularVelocity * Time.deltaTime);
        if (Input.GetKey(KeyCode.UpArrow) && (CharacterVelocity.magnitude <= VelocityLimit || Cross < 0)) Character.GetComponent<Rigidbody2D>().AddForce(Character.transform.up * Force * Time.deltaTime);
        if (Input.GetKey(KeyCode.DownArrow) && (CharacterVelocity.magnitude <= VelocityLimit || Cross > 0)) Character.GetComponent<Rigidbody2D>().AddForce(-Character.transform.up * Force * Time.deltaTime);
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------
    void MakeProblem()
    {
        int RandomNum;
        do
        {
            RandomNum = Random.Range(0, 9);
        } while (NumOfPosition[RandomNum] != 0);
        NumOfPosition[RandomNum]++;
        Vector3 Offset = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
        Vector3 NewPosition = ProblemPosition[RandomNum] + Offset;
        Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);

        GameObject newProblem = Instantiate(Problem, NewPosition, NewQuaternion, Equations.transform);
    }
    
    IEnumerator BulletManage()
    {
        while (Bullets.transform.childCount < 100) MakeBullet();

        yield return new WaitForSeconds(1f);
    }

    void MakeBullet()
    {
        Vector3 NewPosition = new Vector3(Random.Range(-23f, 23f), Random.Range(-23f, 23f), 0f);
        Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);
        GameObject newBullet = Instantiate(DefaultBullet, NewPosition, NewQuaternion);
        newBullet.transform.parent = Bullets.transform;
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------
}
