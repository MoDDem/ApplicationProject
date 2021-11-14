using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AxisDrawer : MonoBehaviour
{
    public RectTransform canvasRect;
    public GameObject linePrefab;
    public GameObject dot;

    public bool axisFlag = false;

    public Vector2 axisBasisX = Vector2.zero;
    public Vector2 axisBasisY = Vector2.zero;
    public Vector3 startPosition = Vector2.zero;

    Vector3 mousePos;

    GameObject axis;
    GameObject lineY;
    GameObject lineX;

    Plane plane = new Plane(Vector3.up, 0);

    // Start is called before the first frame update
    void Awake()
    {
        axis = this.gameObject;
        linePrefab.SetActive(false);
        linePrefab.GetComponent<LineRenderer>().positionCount = 2;

        dot.GetComponent<DotMovement>().lineX = lineX = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        dot.GetComponent<DotMovement>().lineY = lineY = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

        lineY.transform.parent = lineX.transform.parent = axis.transform;
    }

    // Update is called once per frame
    void Update()
    {
        DrawAxis();
    }

    public void HideAxis()
	{
        lineY.SetActive(false);
        lineX.SetActive(false);

        GetComponent<UIWorker>().xText.SetActive(false);
        GetComponent<UIWorker>().yText.SetActive(false);

        startPosition = Vector3.zero;
	}

    private void DrawAxis()
	{
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
                mousePos = ray.GetPoint(distance);

            if (!axisFlag)
            {
                if (startPosition == Vector3.zero)
                    startPosition = mousePos;
                lineX.SetActive(true);
            }
            else
                lineY.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector2 textPos = Vector2.zero;
            if (plane.Raycast(ray, out float distance))
			{
                mousePos = ray.GetPoint(distance);
                textPos = RectTransformUtility.WorldToScreenPoint(Camera.main, mousePos) - canvasRect.sizeDelta / 2.4f;
            }

            if (!axisFlag)
            {
                lineX.GetComponent<LineRenderer>().SetPosition(0, new Vector3(startPosition.x, 0, startPosition.z));
                lineX.GetComponent<LineRenderer>().SetPosition(1, mousePos);
                axisBasisX = new Vector2(mousePos.x, mousePos.z);

                GetComponent<UIWorker>().xText.GetComponent<RectTransform>().anchoredPosition = textPos;
                GetComponent<UIWorker>().xText.SetActive(true);
            }
            else
            {
                lineY.GetComponent<LineRenderer>().SetPosition(0, new Vector3(startPosition.x, 0, startPosition.z));
                lineY.GetComponent<LineRenderer>().SetPosition(1, mousePos);
                axisBasisY = new Vector2(mousePos.x, mousePos.z);

                GetComponent<UIWorker>().yText.GetComponent<RectTransform>().anchoredPosition = textPos;
                GetComponent<UIWorker>().yText.SetActive(true);
            }
            dot.transform.position = startPosition;
            dot.GetComponent<DotMovement>().zeroCords = startPosition;
            dot.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
            axisFlag = true;
    }
}
