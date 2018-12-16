using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour {
    // Prefab 부모. 미니맵에 있는 수식, 그냥 맵에 수식, 맵에 기본총알들 부모가 되는 GameObject
    public GameObject MinimapEquations;
    public GameObject Equations;
    public GameObject Bullets;
    private float MinimapRate;

    // 캐릭터 이동. 위에서부터 캐릭터 오브젝트, 회전속도, 힘, 속도제한, 캐릭터속도, 속도 출력이고 Awake에서 초기화
    public GameObject Character;
    private float NormalCharacterAngularVelocity;
    private float BoosterCharacterAngularVelocity;
    private float CharacterAngularVelocity;
    private float NormalForce;
    private float BoosterForce;
    private float Force;
    private float VelocityLimit;
    private Vector2 CharacterVelocity;

    // 게임종료화면
    public GameObject GameFinish;

    // 점수 및 시간
    public GameObject TimeBarGauge;
    public Text ScoreText;
    public Text TimeText;
    public int Score;
    private float CurrentTime;
    public float LeftTime;

    // Bullet. 위에서부터 캐릭터가 줍는 기본총알, Oper총알, 캐릭터가 쏘는 기본총알, Oper총알
    public GameObject DefaultBullet;
    public GameObject OperBullet;
    public GameObject DefaultBullettoshoot;
    public GameObject OperBullettoshoot;

    // Minimap. 위에서부터 메인카메라, 미니맵에 카메라 사각형 (그냥 Object), 미니맵에 수식 (Prefab), 메인카메라 위치벡터
    public GameObject MainCamera;
    public GameObject MinimapCamera;
    public GameObject MinimapEquation;
    private Vector3 MainCameraPosition;

    // BulletBox. BulletBox안에 들어갈 숫자, 연산자, 기본총알. (UI Object) On/Off로 조절 - 위치가 바뀔일없어서
    public GameObject[] NumbersInBox = new GameObject[10];
    public GameObject[] OpersInBox = new GameObject[4];
    public GameObject DefaultBulletInBox;
    public GameObject SuperBulletInBox;
    public GameObject MagnetInBox;

    // Item. super, magnet, time
    public GameObject ItemPar;
    public GameObject[] Items = new GameObject[3];
    public GameObject SuperBulletToShoot;

    // 우측 카메라
    public GameObject MirrorCamera;

    //----------------------------------------------------------------------------------------------------------------
    private const float EdgeLength = 10f;
    private Vector3[] EquationPosition = { new Vector3(-EdgeLength, -EdgeLength, 0f), new Vector3(0f, -EdgeLength, 0f), new Vector3(EdgeLength, -EdgeLength, 0f), new Vector3(-EdgeLength, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(EdgeLength, 0f, 0f), new Vector3(-EdgeLength, EdgeLength, 0f), new Vector3(0f, EdgeLength, 0f), new Vector3(EdgeLength, EdgeLength, 0f) };
    private int[] NumOfPosition = new int[9];
    public GameObject Equation;

    public int haveBullet; // 0: nothing, 1: default, 2: OperAndNum, 3: super, 4: magnet
    public int num;
    public char oper;
    public bool magnetActive;
    private static float MaxLife = 60f;

    private bool isPlaying;

    void Awake() { // 초기화
        isPlaying = true;
        MinimapRate = 2.9f;
        VelocityLimit = 10f;
        CharacterVelocity = new Vector2();
        CharacterAngularVelocity = 200f;
        NormalCharacterAngularVelocity = 200f;
        BoosterCharacterAngularVelocity = 600f;
        Force = 100f * Character.GetComponent<Rigidbody2D>().mass;
        NormalForce = 100f * Character.GetComponent<Rigidbody2D>().mass;
        BoosterForce = 300f * Character.GetComponent<Rigidbody2D>().mass;
        Score = 0;
        LeftTime = MaxLife;
        magnetActive = false;
        
        while (Equations.transform.childCount < 3) MakeEquation();
        StartCoroutine(BulletManage());
        StartCoroutine(ItemManage());

        StartCoroutine("Timer");

        haveBullet = 0;
    }

    IEnumerator Timer() {
        yield return new WaitForSeconds(0.01f);
        if (LeftTime > MaxLife) LeftTime = MaxLife;
        LeftTime -= 0.01f * Mathf.Pow(1.2f, CurrentTime / 20);
        TimeBarGauge.GetComponent<Image>().fillAmount = LeftTime / MaxLife;
        CurrentTime += 0.01f;
        if (LeftTime < 0f) GameOver();
        else StartCoroutine("Timer");
    }

    void Update() {
        // 미니맵 카메라 컨트롤
        MainCameraPosition = MainCamera.transform.position;
        MinimapCamera.transform.localPosition = new Vector3(MainCameraPosition.x * MinimapRate, MainCameraPosition.y * MinimapRate - 190f, 0f);

        // BulletBox 컨트롤 (저장된 num, oper따라서 출력만 함
        BulletBoxDelete(); // BulletBox 내 모든거 끔
        if (haveBullet == 1) DefaultBulletInBox.SetActive(true);
        else if (haveBullet == 2) {
            NumbersInBox[num].SetActive(true);
            switch (oper) {
                case '+':
                    OpersInBox[0].SetActive(true);
                    break;
                case '-':
                    OpersInBox[1].SetActive(true);
                    break;
                case '*':
                    OpersInBox[2].SetActive(true);
                    break;
                case '/':
                    OpersInBox[3].SetActive(true);
                    break;
            }
        } else if (haveBullet == 3) {
            SuperBulletInBox.SetActive(true);
        } else if (haveBullet == 4) {
            MagnetInBox.SetActive(true);
        }

        // 캐릭터 이동 및 속도 조절.
        CharacterVelocity = Character.GetComponent<Rigidbody2D>().velocity;
        float Cross = CharacterVelocity.y * Character.transform.up.y + CharacterVelocity.x * Character.transform.up.x;
        if (Input.GetKey(KeyCode.LeftShift)) {
            Force = BoosterForce;
            CharacterAngularVelocity = BoosterCharacterAngularVelocity;
        } else {
            Force = NormalForce;
            CharacterAngularVelocity = NormalCharacterAngularVelocity;
        }
        if (isPlaying && Input.GetKey(KeyCode.LeftArrow)) Character.transform.rotation *= Quaternion.Euler(Vector3.forward * CharacterAngularVelocity * Time.deltaTime);
        if (isPlaying && Input.GetKey(KeyCode.RightArrow)) Character.transform.rotation *= Quaternion.Euler(-Vector3.forward * CharacterAngularVelocity * Time.deltaTime);
        if (isPlaying && Input.GetKey(KeyCode.UpArrow) && (CharacterVelocity.magnitude <= VelocityLimit || Cross < 0)) {
            Character.GetComponent<Rigidbody2D>().AddForce(Character.transform.up * Force * Time.deltaTime);
            Character.GetComponent<Character>().FireOn();
        }
        if (isPlaying && Input.GetKey(KeyCode.DownArrow)) Character.GetComponent<Rigidbody2D>().AddForce(-CharacterVelocity * Force * Time.deltaTime);

        // 총알 발사
        if(isPlaying && Input.GetKey(KeyCode.Space)) {
            if (haveBullet == 1) { // 기본 총알 가지고 있는 경우
                haveBullet = 0;
                Vector3 NewPosition = Character.transform.position;
                Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);
                GameObject ShootBullet = Instantiate(DefaultBullettoshoot, NewPosition, NewQuaternion, transform);
                
                ShootBullet.transform.rotation = Character.transform.rotation;
            }
            else if (haveBullet == 2) { // Oper 총알 가지고 있는 경우
                haveBullet = 0;
                Vector3 NewPosition = Character.transform.position;
                Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);
                GameObject ShootBullet = Instantiate(OperBullettoshoot, NewPosition, NewQuaternion, transform);
                ShootBullet.GetComponent<Bullet_eq_shoot>().num = num;
                ShootBullet.GetComponent<Bullet_eq_shoot>().oper = oper;

                ShootBullet.transform.rotation = Character.transform.rotation;
            } else if (haveBullet == 3) {
                haveBullet = 0;
                Vector3 NewPosition = Character.transform.position;
                Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);
                GameObject ShootBullet = Instantiate(SuperBulletToShoot, NewPosition, NewQuaternion, transform);

                ShootBullet.transform.rotation = Character.transform.rotation;
            } else if (haveBullet == 4) {
                haveBullet = 0;
                StartCoroutine("StartMagnet");
            }
        }

        // 점수 세팅
        ScoreText.text = Score.ToString() + "점";
        TimeText.text = CurrentTime.ToString("#00.00") + "초";
    }

    IEnumerator StartMagnet() {
        magnetActive = true;
        yield return new WaitForSeconds(10f);
        magnetActive = false;
    }

    void BulletBoxDelete() { // BulletBox 내의 모든거 끔
        for (int i = 0; i < 10; i++) NumbersInBox[i].SetActive(false);
        for (int i = 0; i < 4; i++) OpersInBox[i].SetActive(false);
        DefaultBulletInBox.SetActive(false);
        SuperBulletInBox.SetActive(false);
        MagnetInBox.SetActive(false);
    }

    // 캐릭터가 맵 가장자리 충돌해서 반대편으로 갈 때 카메라도 순간이동
    public void SetCameraWithCharacterPos() {
        Vector3 CharacterPos = Character.transform.position;
        MainCamera.transform.position = new Vector3(CharacterPos.x, CharacterPos.y, -10f);
        MirrorCamera.transform.position = new Vector3(CharacterPos.x, CharacterPos.y, 10f);
    }

    void GameOver() {
        GameFinish.SetActive(true);
        Character.GetComponent<Rigidbody2D>().velocity = new Vector2();
        isPlaying = false;
    }

    public void SoundOn() {
        GetComponent<AudioSource>().mute = false;
    }

    public void SoundOff() {
        GetComponent<AudioSource>().mute = true;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------
    // 수식 만들기 (위치만 잡아주고 Instantiate. 수식 내부 내용은 Equation Script에서 다룸)
    void MakeEquation() {
        int RandomNum;
        do {
            RandomNum = Random.Range(0, 9);
        } while (NumOfPosition[RandomNum] != 0);
        NumOfPosition[RandomNum]++;
        Vector3 Offset = new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0f);
        Vector3 NewPosition = EquationPosition[RandomNum] + Offset;
        Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);

        GameObject newEquation = Instantiate(Equation, NewPosition, NewQuaternion, Equations.transform);
        newEquation.GetComponent<Equation>().id = RandomNum;

        GameObject newMinimapEquation = Instantiate(MinimapEquation, NewPosition, NewQuaternion, MinimapEquations.transform);
        newMinimapEquation.GetComponent<MinimapEquation>().SetEquation(newEquation);
    }

    // 인자로 전달된 eq Object를 제거하고 (제거 전에 아이템들은 Equations(수식 부모 Object)를 부모로 바꾼다) 새로운 수식을 Instantiate. 
    public void RemakeEquation(GameObject eq) {
        for(int i = eq.transform.childCount - 1; i >= 0; i--)
            if (eq.transform.GetChild(i).tag == "Item") eq.transform.GetChild(i).parent = eq.transform.parent;
        NumOfPosition[eq.GetComponent<Equation>().id]--;
        Destroy(eq);
        MakeEquation();
    }
    
    IEnumerator BulletManage() { // Bullet 100개 만듦
        while (Bullets.transform.childCount < 100) MakeBullet();
        yield return new WaitForSeconds(1f);
    }
    
    public void MakeBullet() { // Bullet 1개 만듦
        Vector3 NewPosition = new Vector3(Random.Range(-18f, 18f), Random.Range(-18f, 18f), 0f);
        Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);
        GameObject newBullet = Instantiate(DefaultBullet, NewPosition, NewQuaternion);
        newBullet.GetComponent<Magnet>().ec = transform.GetComponent<EventController>();
        newBullet.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-180, 180));
        newBullet.transform.parent = Bullets.transform;
    }

    IEnumerator ItemManage() {
        while (ItemPar.transform.childCount < Mathf.Min((int)(4.0f * Mathf.Pow(1.2f, CurrentTime / 10)), 16)) MakeItem();
        yield return new WaitForSeconds(1f);
    }

    public void MakeItem() {    // Item 1개 만듦
        Vector3 NewPosition = new Vector3(Random.Range(-18f, 18f), Random.Range(-18f, 18f), 0f);
        Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);
        GameObject newItem = Instantiate(Items[(int)Random.Range(0f, 3f)], NewPosition, NewQuaternion);
        newItem.transform.parent = ItemPar.transform;
        newItem.transform.GetComponent<Magnet>().ec = transform.GetComponent<EventController>();
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------
}
