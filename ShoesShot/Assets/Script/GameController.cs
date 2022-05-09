using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject TargetShoes;
    [SerializeField]
    GameObject ShotShoes;
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    Material lineMaterial;
    [SerializeField]
    Slider gaugeSlider;
    [SerializeField]
    GameObject main_cam;
    [SerializeField]
    GameObject resultPanel;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    GameObject clickPanel;

    GameObject targetShoes;
    GameObject shotShoes;
    GameObject targetShoesInner;
    GameObject shotShoesInner;

    Transform targetShoesInnerTransform;
    Transform shotShoesInnerTransform;

    Rigidbody shotShoesRigidbody;

    Vector3 lineVector;
    Vector3 targetShoesInnerVector;
    Vector3 shotShoesInnerVector;
    Vector3 TSVector; //target->shot
    Vector3 STVector; //shot->target

    int stopCount = 0;

    float angle = -90;
    float score = 0;

    bool isShotGauge = false;
    bool isShot = false;
    bool isUp = false;
    bool isAngle = true;
    bool isGaugeUp = true;
    bool isCheck = false;
    bool CheckOK = false;
    bool isResult = false;

    void Start()
    {
        Application.targetFrameRate = 60;

        targetShoes = Instantiate(TargetShoes, new Vector3(2 + Random.value * 1 - 0.5f, 1, -28 + Random.value * 1 - 0.5f), Quaternion.Euler(0, Random.value * 360, 0));
        
        shotShoes = Instantiate(ShotShoes);
        shotShoesRigidbody = shotShoes.GetComponent<Rigidbody>();

        targetShoesInner = targetShoes.transform.GetChild(1).gameObject;
        shotShoesInner = shotShoes.transform.GetChild(1).gameObject;

        targetShoesInnerVector = targetShoesInner.transform.position - targetShoes.transform.position;
        shotShoesInnerVector = shotShoesInner.transform.position - shotShoes.transform.position;

        main_cam.transform.parent = shotShoes.transform;

        isAngle = true;

        lineRenderer.positionCount = 5;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.material = lineMaterial;

        DrawArrow(shotShoes.transform.position, angle, 5f);

        gaugeSlider.gameObject.SetActive(false);
        resultPanel.gameObject.SetActive(false);
        clickPanel.gameObject.SetActive(true);
    }

    void Update()
    {
        if(isResult)
		{
            return;
		}

        if(isAngle && isUp)
		{
            angle += 25f * Time.deltaTime;
            if(angle > -60f)
                isUp = false;
		}
        else if(isAngle)
		{
            angle -= 25f * Time.deltaTime;
            if(angle < -120f)
                isUp = true;
		}

        if(isShotGauge && isGaugeUp)
		{
            gaugeSlider.value += 0.5f * Time.deltaTime;
            if(gaugeSlider.value >= 1.5f)
                isGaugeUp = false;
		}
        else if(isShotGauge)
		{
            gaugeSlider.value -= 0.5f * Time.deltaTime;
            if (gaugeSlider.value <= 0.5f)
                isGaugeUp = true;
		}

        if(!isShot)
        {
            lineRenderer.enabled = true;
            DrawArrow(shotShoes.transform.position, angle, 5f);
        }
        else
        {
            if(isCheck)
            {
                targetShoesInnerVector = targetShoesInner.transform.position - targetShoes.transform.position;
                shotShoesInnerVector = shotShoesInner.transform.position - shotShoes.transform.position;

                TSVector = (shotShoes.transform.position - targetShoes.transform.position).normalized;
                STVector = (targetShoes.transform.position - shotShoes.transform.position).normalized;

                float dotShot = Vector3.Dot(STVector, shotShoesInnerVector);
                float dotTarget = Vector3.Dot(TSVector, targetShoesInnerVector);

                float add_score = 0f;
                float tShoes = 1f;
                if((Mathf.Abs(dotShot) < 0.5f && Mathf.Abs(dotTarget) > 0.85f))
				{
                    if(dotTarget > 0)
                    {
                        tShoes = 2.555555555f;
                        add_score = 5f;
                    }
                    else
                    {
                        tShoes = 1.888888888f;
                        add_score = 10f;
                    }
                }
                else if(Mathf.Abs(dotTarget) < 0.5f && Mathf.Abs(dotShot) > 0.85f)
				{
                    if(dotShot > 0)
                    {
                        tShoes = 3.141592653f;
                        add_score = 10f;
                    }
                    else
                    {
                        tShoes = 2.236067977f;
                        add_score = 20f;
                    }
				}
                float score_base = (15 - (shotShoes.transform.position - targetShoes.transform.position).magnitude) * (dotShot + 1) * (dotTarget + 1);
                score = (score_base + add_score) * tShoes;
#if UNITY_EDITOR
                Debug.Log($"({score_base} + {add_score}) * {tShoes} = {score}");
#endif
                isResult = true;
                resultPanel.gameObject.SetActive(true);
                scoreText.text = "SCORE: " + score.ToString("G5");
            }

            if(CheckOK && shotShoesRigidbody.velocity.sqrMagnitude < 0.2f)
			{
                stopCount++;
                if(stopCount > 15)
                    isCheck = true;
			}
		}

        if(Input.GetKeyDown(KeyCode.Space) && !isShot)
		{
            Shot();
        }
    }
    public void Shot()
    {
        if(isShotGauge)
        {
            shotShoesRigidbody.AddForce(lineVector * 400 * gaugeSlider.value);
            isShot = true;
            Invoke("ShotInvoke", 0.2f);
            gaugeSlider.gameObject.SetActive(false);
            clickPanel.gameObject.SetActive(false);
            lineRenderer.enabled = false;
        }
        else
        {
            gaugeSlider.gameObject.SetActive(true);
            isAngle = false;
        }
        isShotGauge = !isShotGauge;
	}
    void ShotInvoke() => CheckOK = true;

    void ReStart()
    {
        stopCount = 0;

        float angle = -90;

        isShotGauge = false;
        isShot = false;
        isUp = false;
        isAngle = true;
        isGaugeUp = true;
        isCheck = false;
        CheckOK = false;
        isResult = false;

        gaugeSlider.value = gaugeSlider.minValue;

        main_cam.transform.parent = null;

        Destroy(shotShoes);
        Destroy(targetShoes);

        targetShoes = Instantiate(TargetShoes, new Vector3(2 + Random.value * 1 - 0.5f, 1, -28 + Random.value * 1 - 0.5f) , Quaternion.Euler(0, Random.value * 360, 0));
        shotShoes = Instantiate(ShotShoes);
        shotShoesRigidbody = shotShoes.GetComponent<Rigidbody>();

        targetShoesInner = targetShoes.transform.GetChild(1).gameObject;
        shotShoesInner = shotShoes.transform.GetChild(1).gameObject;

        targetShoesInnerVector = targetShoesInner.transform.position - targetShoes.transform.position;
        shotShoesInnerVector = shotShoesInner.transform.position - shotShoes.transform.position;

        main_cam.transform.position = new Vector3(0, 10, 50);
        main_cam.transform.rotation = Quaternion.Euler(30, 180, 0);
        main_cam.transform.parent = shotShoes.transform;

        isAngle = true;

        lineRenderer.positionCount = 5;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.material = lineMaterial;

        DrawArrow(shotShoes.transform.position, angle, 5f);

        gaugeSlider.gameObject.SetActive(false);
        resultPanel.gameObject.SetActive(false);
        clickPanel.gameObject.SetActive(true);
    }

    public void ClickReTry()
	{
        resultPanel.gameObject.SetActive(false);
        ReStart();
	}
    public void ClickTweet()
	{
        string text = "ShoesShotをプレイ！\n結果は、" + score.ToString("G5") + "でした。\nhttps://unityroom.com/games/shoesshot\n";
        string esctext = UnityWebRequest.EscapeURL(text);
        string esctag = UnityWebRequest.EscapeURL("unity1week,shoesshot");
        string url = "https://twitter.com/intent/tweet?text=" + esctext + "&hashtags=" + esctag;

        Application.OpenURL(url);
	}
    void DrawArrow(Vector3 from, float angle, float dif)
    {
        lineVector = VectorFromAngle(angle, 0) * 5f;

        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, from + lineVector);
        lineRenderer.SetPosition(2, from + lineVector + VectorFromAngle(angle + dif * 180, 0));
        lineRenderer.SetPosition(3, from + lineVector);
        lineRenderer.SetPosition(4, from + lineVector + VectorFromAngle(angle - dif + 180, 0));
    }

    Vector3 VectorFromAngle(float angle, float posY)
	{
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), posY, Mathf.Sin(angle * Mathf.Deg2Rad));
	}
}
