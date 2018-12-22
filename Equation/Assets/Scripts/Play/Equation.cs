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
    private int[] ItemNumDatas = new int[10];  // 0 ~ 9
    private int[] ItemOperDatas = new int[10]; // 0 ~ 3: +, -, *, /
    private float ItemRange;
    public int id;

    void Awake() {
        ItemRange = 5.9f;
        // 각 수식 Object는 EventController - Prefab - Equations - Equation 순으로 존재한다.
        ec = transform.parent.parent.parent.GetComponent<EventController>();

        // 각 수식은 Display를 위해 위에서부터 계수, x, 연산자, 상수, 결과값이 들어갈 부모 Object를 가진다.
        // 사실 계수, 상수, 결과는 부모 Object가 있고 x, 연산자는 직접적으로 Equation 밑으로 들어간다.
        newCoefficientPar = Instantiate(CoefficientPar, transform);
        newX = Instantiate(X, transform);
        newOperPar = Instantiate(OperPar, transform);
        newConstantPar = Instantiate(ConstantPar, transform);
        newResultPar = Instantiate(ResultPar, transform);

        NumbersInit();
        DisplayEquation();
        MakeOperAndNum();
    }

    // 수식 정보에 알맞은 연산자-숫자 Item 생성
    void MakeOperAndNum() {
        for (int i = 0; i < 10; i++) {
            float x = 0f, y = 0f;
            while (-2f < x && x < 2f) x = Random.Range(-ItemRange, ItemRange);
            while (-2f < y && y < 2f) y = Random.Range(-ItemRange, ItemRange);
            Vector3 NewPosition = transform.position + new Vector3(x, y, 0f);
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

    // 방정식 숫자 랜덤 생성 및 생성할 Item 정보
    void NumbersInit() {
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
        while(i<10) {
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

        // 계산 결과 방정식이 풀렸으면 1초 뒤 Remake(제거 후 새로운 수식 생성)
        if (Coefficient == Coefficient_Deno && Constant == 0) DoSolve();
    }

    public void DoSolve() {
        Result = (Result * Coefficient_Deno * Constant_Deno) - Constant * Coefficient_Deno * Result_Deno;
        Result_Deno = Coefficient * Constant_Deno * Result_Deno;
        int ResGCD = gcd(Result, Result_Deno);
        Coefficient = 1;
        Coefficient_Deno = 1;
        Constant = 0;
        Constant_Deno = 1;
        Result /= ResGCD;
        Result_Deno /= ResGCD;
        RemoveEquation();
        DisplayEquation();

        ec.LeftTime += 20f;
        ec.Score += 2000;
        StartCoroutine("RemakeAfterSeconds");
    }

    // 1초 후 Remake
    IEnumerator RemakeAfterSeconds() {
        yield return new WaitForSeconds(1f);
        ec.RemakeEquation(gameObject);
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
    void DisplayEquation() { // 상태에 맞게 Display 변환 (Main Camera는 z<0, Sub Camera는 z>0 인 것들 보여줌)
        // 3자리 수 이상되면 Remake
        if (Coefficient >= 100 || Coefficient_Deno >= 100 || Constant >= 100 || Constant_Deno >= 100 || Result >= 100 || Result_Deno >= 100) ec.RemakeEquation(gameObject);
        if (Coefficient != Coefficient_Deno && Constant != 0) { // 계수, 상수항 모두 있는 경우
            DisplayNumbers(Coefficient, Coefficient_Deno, newCoefficientPar, new Vector3(-0.9f, 0f, -1f), false);
            newX.transform.localPosition = new Vector3(-0.22f, 0f, -1f);
            DisplayOperator(Constant < 0, newOperPar, new Vector3(0.25f, 0f, -1f));
            DisplayNumbers(abs(Constant), Constant_Deno, newConstantPar, new Vector3(0.94f, 0f, -1f), false);
        }
        else if (Coefficient != Coefficient_Deno && Constant == 0) { // 계수만 있는 경우
            DisplayNumbers(Coefficient, Coefficient_Deno, newCoefficientPar, new Vector3(-0.24f, 0f, -1f), false);
            newX.transform.localPosition = new Vector3(0.44f, 0f, -1f);
        }
        else if (Coefficient == Coefficient_Deno && Constant != 0) { // 상수항만 있는 경우
            newX.transform.localPosition = new Vector3(-0.47f, 0f, -1f);
            DisplayOperator(Constant < 0, newOperPar, new Vector3(0.04f, 0f, -1f));
            DisplayNumbers(abs(Constant), Constant_Deno, newConstantPar, new Vector3(0.73f, 0f, -1f), false);
        } else if (Coefficient == Coefficient_Deno && Constant == 0) {
            newX.transform.localPosition = new Vector3(0f, 0f, -1f);
        }

        // 결과(방정식의 우변) Display
        if (Result < 0) DisplayOperator(Result < 0, newResultPar, new Vector3(0.58f, 0f, 1f));
        DisplayNumbers(abs(Result), Result_Deno, newResultPar, Vector3.forward, true);
    }

    // 두 자리 or 한 자리 수 Display. 주의할건 결과식의 경우는 반대로 보여야하므로 (Mirror Image가 제대로 보이려면 반전시켜야됨) localScale의 x값을 -1로 바꿔준다.
    void DisplayNumber(int num, GameObject par, Vector3 pos, bool isResult) {
        // 한 자리 수
        if (num < 10) Instantiate(EqNums[num], pos, new Quaternion(0f, 0f, 0f, 1f), par.transform).transform.localScale = new Vector3(isResult ? -1f : 1f, 1f, 0);
        else { // 두 자리 수
            Vector3 delta = new Vector3(0.155f, 0f, 0f);
            Instantiate(EqNums[num / 10], pos + (isResult ? delta : -delta), new Quaternion(0f, 0f, 0f, 1f), par.transform).transform.localScale = new Vector3(isResult ? -1f : 1f, 1f, 0);
            Instantiate(EqNums[num % 10], pos + (isResult ? -delta : delta), new Quaternion(0f, 0f, 0f, 1f), par.transform).transform.localScale = new Vector3(isResult ? -1f : 1f, 1f, 0);
        }
    }

    void DisplayNumbers(int num, int deno, GameObject par, Vector3 pos, bool isResult) { // 모든 숫자들 Display 컨트롤, 분수도 처리
        // 기준 위치 조정, 결과(방정식의 우변)의 경우 z축이 -, 이외의 경우 +이다.
        pos += transform.position;
        if (!isResult) pos -= Vector3.forward;
        else pos += Vector3.forward;

        // 분수 아닌경우
        if (deno == 1) DisplayNumber(num, par, pos, isResult);
        else { // 분수인 경우
            Vector3 numerator = new Vector3(0f, 0.2f);
            Vector3 denominator = new Vector3(0f, -0.33f);
            DisplayNumber(num, par, pos + numerator, isResult);    // 분자
            DisplayNumber(deno, par, pos + denominator, isResult); // 분모

            // 분수 표시를 위한 분자-분모 사이 막대 Display. 분모나 분자가 두 자리 수인 경우 좀 더 길다
            if(num < 10 && deno < 10) Instantiate(EqFracBar, pos, new Quaternion(0f, 0f, 0f, 1f), par.transform);
            else {
                Vector3 FracBarDelta = new Vector3(0.167f, 0f, 0f);
                Instantiate(EqFracBar, pos + FracBarDelta, new Quaternion(0f, 0f, 0f, 1f), par.transform);
                Instantiate(EqFracBar, pos - FracBarDelta, new Quaternion(0f, 0f, 0f, 1f), par.transform);
            }
        }
    }

    void DisplayOperator(bool isMinus, GameObject par, Vector3 pos) { // 연산자 Display. + or -
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