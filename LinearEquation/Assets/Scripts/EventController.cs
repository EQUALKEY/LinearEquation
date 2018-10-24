using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {
    // 케릭터들
    public GameObject LeftCharacter;
    public GameObject RightCharacter;

    // 일차방정식에 쓸 숫자들 및 연산자
    public GameObject[] Numbers = new GameObject[10];
    public GameObject[] Opers = new GameObject[2];      // +, -
    public GameObject FractionBar;                      // 분수 사이의 줄
    public GameObject x;                                // x

    // 주워쓸 아이템
    public GameObject[] NumberItems = new GameObject[10];
    public GameObject[] OperItems = new GameObject[3]; // +, - , /

    // Prefab 부모 Objects
    public GameObject CoefficientPar;
    public GameObject ConstantPar;
    public GameObject ResultPar;
    public GameObject ItemsPar;
    public GameObject OperBoxPar;
    public GameObject NumBoxPar;

    // 일차방정식 x계수, 상수항, 우변의 결과(분자, 분모), 우변의 결과가 분수인지 아닌지
    private int Coefficient;        // x의 계수 (1~20)
    private int Coefficient_Deno;   // x의 계수 분모\
    private int Constant;           // 좌변의 상수항 (-100~100) (2x+5=0일 때 +값을 가짐)
    private int Constant_Deno;      // 좌변의 상수항 분모\
    private int Result;             // 결과의 분자
    private int Result_Deno;           // 결과의 분모\

    // 주운 연산자, 숫자 저장
    private int GetOper;    // -1, 0, 1, 2: 안주움, +, -, /
    private int GetNum;     // -1: 안주움
    
///////////////////////////////////////////////////////////////////////////// Init
    void Awake()
    {
        Restart();
    }

    void Restart()
    {
        // 캐릭터 위치 초기화
        LeftCharacter.transform.localPosition = new Vector3(-4.0f, -4.0f);
        RightCharacter.transform.localPosition = new Vector3(4.0f, -4.0f);

        NumbersInit();  // 일차방정식 숫자 세팅 초기화
        ItemsInit();    // 떨어져있는 아이템 세팅 초기화
    }

    void NumbersInit()  // 일차방정식 숫자 랜덤 생성 및 기타 숫자들 초기화
    {
        System.Random rand = new System.Random();
        Coefficient = rand.Next(1, 21);
        Coefficient_Deno = 1;
        Constant = rand.Next(-100, 101);
        Constant_Deno = 1;
        Result = 0;
        Result_Deno = 1;
        GetOper = -1;
        GetNum = -1;
    }

    void ItemsInit()    // 아이템 초기화, 생성된 숫자에 알맞게 + 위치 랜덤으로 아이템 나와야하는데 아직 구현X
    {

    }

///////////////////////////////////////////////////////////////////////////// Update
    void Update () {
        // 캐릭터 움직임 (왼, 오, 위, 아래 순)
		if (Input.GetKey("left"))
            Move(LeftCharacter, RightCharacter, 0);
        if (Input.GetKey("right"))
            Move(LeftCharacter, RightCharacter, 1);
        if (Input.GetKey("up"))
            Move(LeftCharacter, RightCharacter, 2);
        if (Input.GetKey("down"))
            Move(LeftCharacter, RightCharacter, 3);

        // Space바 눌렀을 때
        if (Input.GetKey("space"))
            SpaceInput();
    }

    // 캐릭터 움직임
    void Move(GameObject LeftCharacter, GameObject RightCharacter, int direction) // x: -7.7 ~ -0.5, y: -4.6 ~ 0.5
    {
        Vector3 pos = LeftCharacter.transform.localPosition;
        Vector3 delta = new Vector3(8.0f, 0.0f);

        switch (direction)
        {
            case 0: // Left
                if (-7.5f < pos.x && pos.x < -0.3f)
                    LeftCharacter.transform.Translate(new Vector3(-0.1f, 0.0f));
                break;
            case 1: // Right
                if (-7.7f < pos.x && pos.x < -0.5f)
                    LeftCharacter.transform.Translate(new Vector3(0.1f, 0.0f));
                break;
            case 2: // Up
                if (-4.7f < pos.y && pos.y < 0.3f)
                    LeftCharacter.transform.Translate(new Vector3(0.0f, 0.1f));
                break;
            case 3: // Down
                if (-4.5f < pos.y && pos.y < 0.5f)
                    LeftCharacter.transform.Translate(new Vector3(0.0f, -0.1f));
                break;
        }

        RightCharacter.transform.localPosition = LeftCharacter.transform.localPosition + delta;
    }

    // SpaceBar 눌렀을 때
    void SpaceInput()
    {
        KeepItem();                         // 아이템 줍고
        if (GetOper != -1 && GetNum != -1)  // Operation 필요시 Operate();
            Operate();                      // 여기서 많은걸 컨트롤
    }

    void KeepItem() // 아이템 줍기
    {   // 주운 아이템 뭔지 저장
        GetOper = LeftCharacter.GetComponent<Character>().GetItem();
        GetNum = RightCharacter.GetComponent<Character>().GetItem();
        DisplayNumbers(NumBoxPar, false);
        // Instanciate로 OperBox 안에 Operator 생성
    }

    void Operate()  // 연산 및 Display하는 함수
    {               // Display는 계수, 상수항, 결과값의 숫자 Display는 통일
        switch(GetOper)
        {
            case 0: // + 덧셈
                Constant += Constant_Deno * GetNum;
                Result += Result_Deno * GetNum;
                break;
            case 1: // - 뺄셈
                Constant -= Constant_Deno * GetNum;
                Result -= Result_Deno * GetNum;
                break;
            case 2: // / 나눗셈
                Coefficient_Deno *= GetNum;
                Constant_Deno *= GetNum;
                Result_Deno *= GetNum;
                int CoefGCD = gcd(Coefficient, Coefficient_Deno);
                int ConstGCD = gcd(Constant, Constant_Deno);
                int ResGCD = gcd(Result, Result_Deno);
                Coefficient /= CoefGCD;
                Coefficient_Deno /= CoefGCD;
                Constant /= ConstGCD;
                Constant_Deno /= ConstGCD;
                Result /= ResGCD;
                Result_Deno = ResGCD;
                break;
        }

        if (Coefficient == Coefficient_Deno && Constant == 0)  // 연산 결과로 일차방정식 해를 구했으면
            Restart();                                         // 재시작
        else
            DisplayEquation();
    }

    void DisplayEquation()  // 상태에 맞게 Display 변환
    {

    }

    void DisplayNumbers(GameObject Par, bool isFraction)  // 모든 숫자들 Display 컨트롤, 분수도 처리
    {
        if (!isFraction)    // 분수 아닌경우
        {

        }
        else                // 분수인 경우
        {
                    
        }
    }

    int gcd(int a, int b)
    {
        if (a < b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }
        if (b == 1) return 1;
        return gcd(b, a % b);
    }
}
