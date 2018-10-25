﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {
    public void Debugging()
    {
        Debug.Log(Coefficient.ToString() + " " + Constant.ToString() + " " + Result.ToString());
        Debug.Log(Coefficient_Deno.ToString() + " " + Constant_Deno.ToString() + " " + Result_Deno.ToString());
    }
    // 케릭터들
    public GameObject LeftCharacter;
    public GameObject RightCharacter;

    // 일차방정식에 쓸 숫자들 및 연산자
    public GameObject[] Numbers = new GameObject[10];
    public GameObject[] Opers = new GameObject[2];      // +, -
    public GameObject FractionBar;                      // 분수 사이의 줄
    public GameObject x;                                // x

    // 주워쓸 아이템
    public GameObject[] Items = new GameObject[15]; // 0 ~ 9: 숫자, 10, 11, 12, 13: +, -, /, *,   14: Empty
    private static int max_itemKind = 15;

    // Prefab 부모 Objects
    public GameObject EquationPar;
    public GameObject CoefficientPar;
    public GameObject ConstantPar;
    public GameObject ResultPar;
    public GameObject ItemsPar;

    // Item 정보
    private int[] ItemsData = new int[10]; // -1: 빔,   0 ~ 9: 숫자,   10, 11, 12, 13: +, -, /, *
    private Vector3[] ItemsPos = new Vector3[10];
    private static int max_itemNum = 10;

    // Random Class
    private System.Random rand;

    // 일차방정식 x계수, 상수항, 우변의 결과(분자, 분모), 우변의 결과가 분수인지 아닌지
    private int Coefficient;        // x의 계수 (1~10)
    private int Coefficient_Deno;   // x의 계수 분모\
    private int Constant;           // 좌변의 상수항 (-9~9) (2x+5=0일 때 +값을 가짐)
    private int Constant_Deno;      // 좌변의 상수항 분모\
    private int Result;             // 결과의 분자
    private int Result_Deno;        // 결과의 분모\

    // 주운 연산자, 숫자 저장
    public string GetOper; // "", "plus", "minus", "div", "mul": 안주움, +, -, /, *
    public int GetNum;     // -1: 안주움

    // LeftCharacter, RightCharacter 중복으로 Operate 방지
    public bool isOperated;
    
///////////////////////////////////////////////////////////////////////////// Init
    void Awake()
    {
        rand = new System.Random();
        ItemsPos[0] = new Vector3(-6.81f, -3.82f);
        ItemsPos[1] = new Vector3(-7.48f, 0.0f);
        ItemsPos[2] = new Vector3(-0.5f, -4.38f);
        ItemsPos[3] = new Vector3(-0.76f, 0.0f);
        ItemsPos[4] = new Vector3(-1.8f, -0.79f);
        ItemsPos[5] = new Vector3(-2.33f, -2.3f);
        ItemsPos[6] = new Vector3(-2.94f, -1.2f);
        ItemsPos[7] = new Vector3(-7.33f, -1.83f);
        ItemsPos[8] = new Vector3(-4.89f, -1.89f);
        ItemsPos[9] = new Vector3(-0.35f, -1.7f);
        Restart();
    }

    void Restart()
    {
        // 캐릭터 위치 초기화
        LeftCharacter.transform.localPosition = new Vector3(-4.0f, -4.0f);
        RightCharacter.transform.localPosition = new Vector3(4.0f, -4.0f);

        NumbersInit();  // 일차방정식 숫자 세팅 초기화
        ItemsInit();    // 떨어져있는 아이템 세팅 초기화
        DisplayEquation();
    }

    void NumbersInit()  // 일차방정식 숫자 랜덤 생성 및 기타 숫자들 초기화
    {
        Coefficient = rand.Next(1, 10);
        Coefficient_Deno = 1;
        Constant = rand.Next(-9, 10);
        Constant_Deno = 1;
        Result = 0;
        Result_Deno = 1;
        GetOper = "";
        GetNum = -1;
        isOperated = false;
    }

    void ItemsInit()    // 아이템 초기화, 생성된 숫자, 연산자에 알맞게 + 위치 랜덤
    {
        RemoveItems();
        for (int i = 0; i < max_itemNum; i++) ItemsData[i] = -1;
        int rndCoe = rand.Next(0, 10);
        ItemsData[rndCoe] = Coefficient;
        int rndCon = rand.Next(0, 9);
        for (int i = rndCon;; i++)
        {
            if (i == max_itemNum) i = 0;
            if (ItemsData[i] == -1)
            {
                if (Constant < 0) ItemsData[i] = -Constant;
                else ItemsData[i] = Constant;
                break;
            }
        }
        if (Coefficient_Deno != 1)
        {
            int rndCoe_Deno = rand.Next(0, 8);
            for (int i = rndCoe_Deno;; i++)
            {
                if (i == max_itemNum) i = 0;
                if (ItemsData[i] == -1)
                {
                    ItemsData[i] = Coefficient_Deno;
                    break;
                }
            }
        }
        if(Constant != 0 && Constant_Deno != 1)
        {
            int rndCon_Deno = rand.Next(0, 7);
            for (int i = rndCon_Deno;; i++)
            {
                if (i == max_itemNum) i = 0;
                if (ItemsData[i] == -1)
                {
                    ItemsData[i] = Constant_Deno;
                    break;
                }
            }
        }
        if (Constant < 0)
        {
            int rndPlus = rand.Next(0, 6);
            for (int i = rndPlus;; i++)
            {
                if (i == max_itemNum) i = 0;
                if (ItemsData[i] == -1)
                {
                    ItemsData[i] = 10;  // Plus
                    break;
                }
            }
        } else
        {
            int rndMinus = rand.Next(0, 5);
            for (int i = rndMinus;; i++)
            {
                if (i == max_itemNum) i = 0;
                if (ItemsData[i] == -1)
                {
                    ItemsData[i] = 11;  // Minus
                    break;
                }
            }
        }
        if (Coefficient > Coefficient_Deno)
        {
            int rndDiv = rand.Next(0, 4);
            for (int i = rndDiv;; i++)
            {
                if (i == max_itemNum) i = 0;
                if (ItemsData[i] == -1)
                {
                    ItemsData[i] = 12;  // Div
                    break;
                }
            }
        }
        if (Coefficient < Coefficient_Deno)
        {
            int rndMul = rand.Next(0, 3);
            for (int i = rndMul;; i++)
            {
                if (i == max_itemNum) i = 0;
                if (ItemsData[i] == -1)
                {
                    ItemsData[i] = 13;  // Mul
                    break;
                }
            }
        }
        for (int i = 0; i < max_itemNum; i++)
        {
            if (ItemsData[i] == -1)
                ItemsData[i] = rand.Next(0, max_itemKind-1);
        }
        for (int i = 0; i < max_itemNum; i++)
        {
            Vector3 delta = new Vector3(8f, 0f);
            Instantiate(Items[ItemsData[i]], ItemsPos[i], new Quaternion(0f, 0f, 0f, 1f), ItemsPar.transform);
            Instantiate(Items[ItemsData[i]], ItemsPos[i]+delta, new Quaternion(0f, 0f, 0f, 1f), ItemsPar.transform);
        }
    }

    void RemoveItems()
    {
        int ItemsNum = ItemsPar.transform.childCount;
        for (int i = ItemsNum - 1; i >= 0; i--) Destroy(ItemsPar.transform.GetChild(i).gameObject);
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
        if (Input.GetKeyDown("space"))
        {
            LeftCharacter.GetComponent<Character>().SpaceInput();
            RightCharacter.GetComponent<Character>().SpaceInput();
        }
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

    public void Operate()  // 연산 및 Display하는 함수
    {               // Display는 계수, 상수항, 결과값의 숫자 Display는 통일
        int CoefGCD;
        int ConstGCD;
        int ResGCD;
        switch (GetOper)
        {
            case "plus": // + 덧셈
                Constant += Constant_Deno * GetNum;
                Result += Result_Deno * GetNum;
                break;
            case "minus": // - 뺄셈
                Constant -= Constant_Deno * GetNum;
                Result -= Result_Deno * GetNum;
                break;
            case "div": // / 나눗셈
                Coefficient_Deno *= GetNum;
                Constant_Deno *= GetNum;
                Result_Deno *= GetNum;
                CoefGCD = gcd(Coefficient, Coefficient_Deno);
                ConstGCD = gcd(Constant, Constant_Deno);
                ResGCD = gcd(Result, Result_Deno);
                Coefficient /= CoefGCD;
                Coefficient_Deno /= CoefGCD;
                Constant /= ConstGCD;
                Constant_Deno /= ConstGCD;
                Result /= ResGCD;
                Result_Deno /= ResGCD;
                break;
            case "mul": // * 곱셈
                Coefficient *= GetNum;
                Constant *= GetNum;
                Result *= GetNum;
                CoefGCD = gcd(Coefficient, Coefficient_Deno);
                ConstGCD = gcd(Constant, Constant_Deno);
                ResGCD = gcd(Result, Result_Deno);
                Coefficient /= CoefGCD;
                Coefficient_Deno /= CoefGCD;
                Constant /= ConstGCD;
                Constant_Deno /= ConstGCD;
                Result /= ResGCD;
                Result_Deno /= ResGCD;
                break;
        }

        RemoveEquation();

        if (Coefficient == Coefficient_Deno && Constant == 0)  // 연산 결과로 일차방정식 해를 구했으면
            Restart();                                         // 재시작
        else
            DisplayEquation();
    }

    void RemoveEquation()   // 계수, 연산자, 상수항, 결과식 Prefab Destroy
    {
        int CoefNum = CoefficientPar.transform.childCount;
        int ConstNum = ConstantPar.transform.childCount;
        int ResNum = ResultPar.transform.childCount;

        if (EquationPar.transform.childCount == 4) Destroy(EquationPar.transform.GetChild(3).gameObject);
        for (int i = CoefNum - 1; i >= 0; i--) Destroy(CoefficientPar.transform.GetChild(i).gameObject);
        for (int i = ConstNum - 1; i >= 0; i--) Destroy(ConstantPar.transform.GetChild(i).gameObject);
        for (int i = ResNum - 1; i >= 0; i--) Destroy(ResultPar.transform.GetChild(i).gameObject);
    }

    void DisplayEquation()  // 상태에 맞게 Display 변환
    {
        if (Coefficient != Coefficient_Deno && Constant != 0)       // 계수, 상수항 모두 있는 경우
        {
            DisplayNumbers(Coefficient, Coefficient_Deno, CoefficientPar, new Vector3(-6.24f, 2.84f));
            x.transform.localPosition = new Vector3(-5.05f, 2.84f);
            DisplayOperator(Constant < 0, EquationPar, new Vector3(-3.49f, 2.84f));
            DisplayNumbers(abs(Constant), Constant_Deno, ConstantPar, new Vector3(-1.8f, 2.84f));
        } else if(Coefficient != Coefficient_Deno && Constant == 0) // 계수만 있는 경우
        {
            DisplayNumbers(Coefficient, Coefficient_Deno, CoefficientPar, new Vector3(-5.01f, 2.84f));
            x.transform.localPosition = new Vector3(-3.44f, 2.84f);
        } else if(Coefficient == Coefficient_Deno && Constant != 0) // 상수항만 있는 경우
        {
            x.transform.localPosition = new Vector3(-6.15f, 2.84f);
            DisplayOperator(Constant < 0, EquationPar, new Vector3(-4.12f, 2.84f));
            DisplayNumbers(abs(Constant), Constant_Deno, ConstantPar, new Vector3(-2.27f, 2.84f));
        }
        if (Result < 0)
        {
            DisplayOperator(Result < 0, EquationPar, new Vector3(3.78f, 2.84f));
            DisplayNumbers(-Result, Result_Deno, ResultPar, new Vector3(4.8f, 2.84f));
        }
        else DisplayNumbers(Result, Result_Deno, ResultPar, new Vector3(4.03f, 2.84f));
    }

    void DisplayNumbers(int num, int deno, GameObject par, Vector3 pos)  // 모든 숫자들 Display 컨트롤, 분수도 처리
    {
        // Debug.Log(num.ToString() + " " + deno.ToString());
        if (deno == 1)    // 분수 아닌경우
            Instantiate(Numbers[num], pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
        else                // 분수인 경우
        {
            Vector3 numerator = new Vector3(0f, 0.76f);
            Vector3 denominator = new Vector3(0f, -1.26f);
            Instantiate(Numbers[num], pos + numerator, new Quaternion(0f, 0f, 0f, 1f), par.transform);
            Instantiate(FractionBar, pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
            Instantiate(Numbers[deno], pos + denominator, new Quaternion(0f, 0f, 0f, 1f), par.transform);
        }
    }

    void DisplayOperator(bool isMinus, GameObject par, Vector3 pos)
    {
        if (isMinus) Instantiate(Opers[1], pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
        else Instantiate(Opers[0], pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
    }

    int gcd(int a, int b)
    {
        a = abs(a);
        b = abs(b);
        if (a == 0) return b;
        if (a < b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }
        if (a % b == 0) return b;
        return gcd(b, a % b);
    }

    int abs(int a)
    {
        if (a < 0) return -a;
        return a;
    }
}
