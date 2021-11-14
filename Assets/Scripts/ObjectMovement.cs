using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMovement : MonoBehaviour
{
    public GameObject axis;
    public GameObject textOnObject;
    public GameObject xText, yText;
    public GameObject projectAxisX, projectAxisY;

    public Text errorText;

    public Vector3 startPos;

    GameObject linePrefab;
    bool canMove = false;
    int position = 0;

	private void Start()
	{
        linePrefab = axis.GetComponent<LoadData>().linePrefab;
        linePrefab.GetComponent<LineRenderer>().positionCount = 2;

        projectAxisX = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        projectAxisY = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

        projectAxisX.transform.parent = projectAxisY.transform.parent = gameObject.transform;

        projectAxisX.SetActive(true);
        projectAxisY.SetActive(true);
    }

    private Vector3 projectObjectX, projectObjectY;
    private float dist = -1f;
	// Update is called once per frame
	void FixedUpdate()
    {
		if (canMove)
		{
            textOnObject.GetComponent<TextMesh>().text = position.ToString();
            float step = axis.GetComponent<LoadData>().speed * Time.fixedDeltaTime;

            Vector3 dotPos = CalculateNewObjectPosition(axis.GetComponent<LoadData>().positions[position]);

            if (Vector3.Distance(transform.position, dotPos) > 0.001f)
			{
                TextAnimation(dotPos);
                DrawProjections(step);

                transform.position = Vector3.MoveTowards(transform.position, dotPos, step);
            }
			else
			{
                if(position + 1 < axis.GetComponent<LoadData>().positions.Count)
				{
                    position++;
                    dist = -1f;
                    transform.position = dotPos;
                }
			}
        }
    }

	private void DrawProjections(float step)
	{
        projectAxisX.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        projectAxisY.GetComponent<LineRenderer>().SetPosition(0, transform.position);

        if (projectObjectX == Vector3.zero && projectObjectY == Vector3.zero)
		{
            projectObjectX = CalculateNewObjectPosition(new Vector3(startPos.x, 1, 0));
            projectObjectY = CalculateNewObjectPosition(new Vector3(0, 1, startPos.z));
        }

        var projectionX = CalculateNewObjectPosition(new Vector3(axis.GetComponent<LoadData>().positions[position].x, 1, 0));
        var projectionY = CalculateNewObjectPosition(new Vector3(0, 1, axis.GetComponent<LoadData>().positions[position].z));

        projectObjectX = Vector3.MoveTowards(projectObjectX, projectionX, step);
        projectObjectY = Vector3.MoveTowards(projectObjectY, projectionY, step);

        projectAxisX.GetComponent<LineRenderer>().SetPosition(1, projectObjectX);
        projectAxisY.GetComponent<LineRenderer>().SetPosition(1, projectObjectY);

        xText.transform.position = projectObjectX;
        yText.transform.position = projectObjectY;

        xText.GetComponent<TextMesh>().text = projectObjectX.ToString();
        yText.GetComponent<TextMesh>().text = projectObjectY.ToString();
    }

	private void TextAnimation(Vector3 dotPos)
	{
        if (dist == -1f)
            dist = Vector3.Distance(transform.position, dotPos);

        var material = textOnObject.GetComponent<Renderer>().material;
        var color = material.color;

        Vector3 textPos;
        
        if (Vector3.Distance(transform.position, dotPos) > dist / 2)
        {
            float fade = Mathf.Clamp01(Vector3.Distance(transform.position, dotPos) - dist / 2);
            textPos = Vector3.Lerp(
                transform.position,
                new Vector3(transform.position.x + 1, transform.position.y + 3, transform.position.z),
                1 - fade
            );
            material.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(1 - fade));
        }
        else
        {
            float fade = Mathf.Clamp01(dist / 2 - Vector3.Distance(transform.position, dotPos));
            textPos = Vector3.Lerp(
                new Vector3(transform.position.x + 1, transform.position.y + 3, transform.position.z),
                transform.position,
                fade
            );
            material.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(1 - fade));
        }

        textOnObject.transform.position = textPos;
    }

	public Vector3 CalculateNewObjectPosition(Vector3 pos)
    {
        Vector3 xPos = (axis.GetComponent<LoadData>().lineX.GetComponent<LineRenderer>().GetPosition(1) - axis.GetComponent<LoadData>().zeroPosition) * pos.x;
        Vector3 zPos = (axis.GetComponent<LoadData>().lineY.GetComponent<LineRenderer>().GetPosition(1) - axis.GetComponent<LoadData>().zeroPosition) * pos.z;

        return axis.GetComponent<LoadData>().zeroPosition + (zPos + xPos);
    }

	private void OnTriggerEnter(Collider other)
	{
        errorText.text = "Ошибка! Столкновение с " + other.name;
	}

	void OnMouseDown()
    {
        if (axis.GetComponent<LoadData>().lineY == null)
		{
            Debug.LogError("No data found, load file first");
            return;
		}

        canMove = !canMove;
    }
}
