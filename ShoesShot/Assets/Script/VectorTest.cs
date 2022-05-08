using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VectorTest : MonoBehaviour
{
    [SerializeField]
    GameObject Line;
    [SerializeField]
    Material[] materials;
    [SerializeField]
    Text text;

    LineRenderer[] lines = new LineRenderer[4];

    Vector2[] lines_vec = new Vector2[4] { Vector2.one, Vector2.one, Vector2.one, Vector2.one };
    Vector2[] lines_start = new Vector2[4] { new Vector2(2, 0) , new Vector2(-2, 0), new Vector2(2, 0) , new Vector2(-2, 0) };

    float[] lines_angle = new float[4] { 45, 45, 135, 135 };

    void Start()
    {
        for(int i = 0;  i < 4; i++)
		{
            GameObject obj = Instantiate(Line, Vector3.zero, Quaternion.identity) as GameObject;
            lines[i] = obj.GetComponent<LineRenderer>();
            lines[i].positionCount = 2;
            lines[i].startWidth = 0.05f;
            lines[i].endWidth = 0.2f;
            lines[i].material = materials[i];
            lines[i].SetPosition(0, lines_start[i]);
            lines[i].SetPosition(1, lines_vec[i]);

        }
    }
    void Update()
    {
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");

        if(x > 0)
		{
            lines_angle[0] += 15f * Time.deltaTime;
            if(lines_angle[0] > 360f)
                lines_angle[0] = 0f;
		}
        else if(x < 0)
		{
            lines_angle[0] -= 15f * Time.deltaTime;
            if(lines_angle[0] < 0f)
                lines_angle[0] = 360f;
		}
        if(y > 0)
		{
            lines_angle[1] += 15f * Time.deltaTime;
            if(lines_angle[1] > 360f)
                lines_angle[1] = 0f;
		}
        else if(y < 0)
		{
            lines_angle[1] -= 15f * Time.deltaTime;
            if(lines_angle[1] < 0f)
                lines_angle[1] = 360f;
		}

        lines_vec[0] = GetVectorFromAngle(lines_angle[0]);
        lines_vec[1] = GetVectorFromAngle(lines_angle[1]);
        lines_vec[2] = GetVectorFromAngle(lines_angle[0] + 90f);
        lines_vec[3] = GetVectorFromAngle(lines_angle[1] - 90f);

        lines[0].SetPosition(1, lines_start[0] + lines_vec[0]);
        lines[1].SetPosition(1, lines_start[1] + lines_vec[1]);
        lines[2].SetPosition(1, lines_start[2] + lines_vec[2]);
        lines[3].SetPosition(1, lines_start[3] + lines_vec[3]);

        float dot1 = Vector2.Dot(lines_vec[0], lines_vec[1]);
        float dot2 = Vector2.Dot(lines_vec[2], lines_vec[3]);
        float dot3 = Vector2.Dot(lines_vec[0], lines_vec[3]);
        float dot4 = Vector2.Dot(lines_vec[1], lines_vec[2]);
        text.text =  $"Dot1:{dot1}\n";
        text.text += $"Dot2:{dot2}\n";
        text.text += $"Dot3:{dot3}\n";
        text.text += $"Dot4:{dot4}\n";
    }

    Vector2 GetVectorFromAngle(float angleDeg)
	{
        return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
	}
}
