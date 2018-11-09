using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equation : MonoBehaviour {
    private EventController ec;

    // Equation Prefabs
    public GameObject[] EqNums = new GameObject[10]; // 0 ~ 9
    public GameObject[] EqOpers = new GameObject[2]; // +, -
    public GameObject EqFracBar;                     // 분수 가운데 줄
    public GameObject X;                             // x

    // Item Prefabs
    public GameObject ItemParent;
    public GameObject[] NumItems = new GameObject[10]; // 0 ~ 9
    public GameObject[] OperItems = new GameObject[4]; // +, -, *, /

    // Display용 Parents
    public GameObject CoefficientPar;
    public GameObject OperPar;
    public GameObject ConstantPar;
    public GameObject ResultPar;

    // 생성된 Prefabs
    private GameObject newCoefficientPar;
    private GameObject newX;
    private GameObject newOperPar;
    private GameObject newConstantPar;
    private GameObject newResultPar;

    // 일차방정식 x계수, 상수항, 우변의 결과(분자, 분모), 우변의 결과가 분수인지 아닌지
    public int Coefficient;        // x의 계수 (1~10)
    private int Coefficient_Deno;   // x의 계수 분모
    private int Constant;           // 좌변의 상수항 (-9~9) (2x+5=0일 때 +값을 가짐)
    private int Constant_Deno;      // 좌변의 상수항 분모
    private int Result;             // 결과의 분자
    private int Result_Deno;        // 결과의 분모

    // 생성할 Item 정보 저장공간
    private int[] ItemNumDatas = new int[6];  // 0 ~ 9
    private int[] ItemOperDatas = new int[6]; // 0 ~ 3: +, -, *, /

    public int id;

    void Awake() {
        ec = transform.parent.parent.parent.GetComponent<EventController>();

        newCoefficientPar = Instantiate(CoefficientPar, transform);
        newX = Instantiate(X, transform);
        newOperPar = Instantiate(OperPar, transform);
        newConstantPar = Instantiate(ConstantPar, transform);
        newResultPar = Instantiate(ResultPar, transform);

        NumbersInit();
        DisplayEquation();
        MakeOperAndNum();
    }

    void MakeOperAndNum()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 NewPosition = transform.position + new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f);
            Quaternion NewQuaternion = new Quaternion(0f, 0f, 0f, 1f);
            GameObject newItemParent = Instantiate(ItemParent, NewPosition, NewQuaternion, transform);
            GameObject newItem_num = Instantiate(NumItems[ItemNumDatas[i]], newItemParent.transform.position - Vector3.forward, NewQuaternion, newItemParent.transform);
            GameObject newItem_oper = Instantiate(OperItems[ItemOperDatas[i]], newItemParent.transform.position + Vector3.forward, NewQuaternion, newItemParent.transform);
            newItemParent.GetComponent<Item>().num = ItemNumDatas[i];
            switch (ItemOperDatas[i]) {
                case 0:
                    newItemParent.GetComponent<Item>().oper = '+';
                    break;
                case 1:
                    newItemParent.GetComponent<Item>().oper = '-';
                    break;
                case 2:
                    newItemParent.GetComponent<Item>().oper = '*';
                    break;
                case 3:
                    newItemParent.GetComponent<Item>().oper = '/';
                    break;
            }
        }
    }

    // 일차방정식 숫자 랜덤 생성 및 생성할 Item 정보
    void NumbersInit()
    {
        System.Random rand = new System.Random(transform.parent.childCount);
        Coefficient = rand.Next(1, 10);
        Coefficient_Deno = 1;
        Constant = rand.Next(-9, 10);
        Constant_Deno = 1;
        Result = 0;
        Result_Deno = 1;

        int i = 0;
        ItemNumDatas[i] = abs(Constant);
        if (Constant < 0) ItemOperDatas[i] = 0;
        else ItemOperDatas[i] = 1;
        ItemNumDatas[++i] = abs(Coefficient);
        ItemOperDatas[i++] = 3;
        if (Constant % Coefficient == 0) {
            ItemNumDatas[i] = Coefficient;
            ItemOperDatas[i++] = 3;
        }
        while(i<6) {
            ItemNumDatas[i] = rand.Next(1, 10);
            ItemOperDatas[i] = rand.Next(0, 4);
            if (ItemNumDatas[i] == 0 && ItemOperDatas[i] == 3) i--; // 0으로 나누는 연산 제거
            i++;
        }
    }

    // 계산
    public void Operate(int num, char oper) {
        switch (oper) {
            case '+':
                Constant += Constant_Deno * num;
                Result += Result_Deno * num;
                break;
            case '-':
                Constant -= Constant_Deno * num;
                Result -= Result_Deno * num;
                break;
            case '*':
                Coefficient *= num;
                Constant *= num;
                Result *= num;
                break;
            case '/':
                Coefficient_Deno *= num;
                Constant_Deno *= num;
                Result_Deno *= num;
                break;
        }
        // 기약분수로 만들기
        int CoefGCD = gcd(Coefficient, Coefficient_Deno);
        int ConstGCD = gcd(Constant, Constant_Deno);
        int ResGCD = gcd(Result, Result_Deno);
        Coefficient /= CoefGCD;
        Coefficient_Deno /= CoefGCD;
        Constant /= ConstGCD;
        Constant_Deno /= ConstGCD;
        Result /= ResGCD;
        Result_Deno /= ResGCD;

        RemoveEquation();
        DisplayEquation();
        if (Coefficient == Coefficient_Deno && Constant == 0) StartCoroutine("RemakeAfterSeconds");
    }

    IEnumerator RemakeAfterSeconds() {
        yield return new WaitForSeconds(3f);
        ec.RemakeProblem(gameObject);
    }

    // 절댓값 계산
    int abs(int a) {
        if (a < 0) return -a;
        return a;
    }

    // 최대공약수 계산
    int gcd(int a, int b) {
        if (a == 0) return b;
        a = abs(a);
        b = abs(b);

        if (a < b) {
            int tmp = a;
            a = b;
            b = tmp;
        }
        if (a % b == 0) return b;
        return gcd(b, a % b);
    }

    //---------------------------------------------------------------------------------
    void DisplayEquation()  // 상태에 맞게 Display 변환 (Main Camera는 z<0, Sub Camera는 z>0 인 것들 보여줌)
    {
        if (Coefficient != Coefficient_Deno && Constant != 0)
        { // 계수, 상수항 모두 있는 경우
            DisplayNumbers(Coefficient, Coefficient_Deno, newCoefficientPar, new Vector3(-0.9f, 0f, -1f), false);
            newX.transform.localPosition = new Vector3(-0.22f, 0f, -1f);
            DisplayOperator(Constant < 0, newOperPar, new Vector3(0.25f, 0f, -1f));
            DisplayNumbers(abs(Constant), Constant_Deno, newConstantPar, new Vector3(0.94f, 0f, -1f), false);
        }
        else if (Coefficient != Coefficient_Deno && Constant == 0)
        { // 계수만 있는 경우
            DisplayNumbers(Coefficient, Coefficient_Deno, newCoefficientPar, new Vector3(-0.24f, 0f, -1f), false);
            newX.transform.localPosition = new Vector3(0.44f, 0f, -1f);
        }
        else if (Coefficient == Coefficient_Deno && Constant != 0)
        { // 상수항만 있는 경우
            newX.transform.localPosition = new Vector3(-0.47f, 0f, -1f);
            DisplayOperator(Constant < 0, newOperPar, new Vector3(0.04f, 0f, -1f));
            DisplayNumbers(abs(Constant), Constant_Deno, newConstantPar, new Vector3(0.73f, 0f, -1f), false);
        }
        if (Result < 0) DisplayOperator(Result < 0, newResultPar, new Vector3(-0.58f, 0f, 1f));
        DisplayNumbers(abs(Result), Result_Deno, newResultPar, Vector3.forward, true);
    }

    void DisplayNumbers(int num, int deno, GameObject par, Vector3 pos, bool isResult) { // 모든 숫자들 Display 컨트롤, 분수도 처리
        // 분수 아닌경우
        if (deno == 1) Instantiate(EqNums[num], transform.position + pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
        else { // 분수인 경우
            Vector3 numerator = new Vector3(0f, 0.2f);
            Vector3 denominator = new Vector3(0f, -0.33f);
            Instantiate(EqNums[num], transform.position + pos + numerator, new Quaternion(0f, 0f, 0f, 1f), par.transform);
            if(!isResult) Instantiate(EqFracBar, pos - Vector3.forward, new Quaternion(0f, 0f, 0f, 1f), par.transform);
            else Instantiate(EqFracBar, pos + Vector3.forward, new Quaternion(0f, 0f, 0f, 1f), par.transform);
            Instantiate(EqNums[deno], transform.position + pos + denominator, new Quaternion(0f, 0f, 0f, 1f), par.transform);
        }
    }

    void DisplayOperator(bool isMinus, GameObject par, Vector3 pos) { // 연산자 Display, + or -
        if (!isMinus) Instantiate(EqOpers[0], transform.position + pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
        else Instantiate(EqOpers[1], transform.position + pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
    }

    void RemoveEquation() { // 일차방정식 제거 - 계수, 연산자, 상수항, 결과식 Prefab Destroy
        foreach (Transform child in newCoefficientPar.transform) Destroy(child.gameObject);
        foreach (Transform child in newConstantPar.transform) Destroy(child.gameObject);
        foreach (Transform child in newOperPar.transform) Destroy(child.gameObject);
        foreach (Transform child in newResultPar.transform) Destroy(child.gameObject);
    }
}