using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIWorker : MonoBehaviour
{
    public GameObject xText, yText;
    public ScrollRect scrollView;
    public Text timeLeft;

    public InputField inputSpeed;
    public InputField startX, startY;
    InputField newPosX, newPosY;

    List<Vector2> positions = new List<Vector2>();
    int currentPos = -1;

    DotMovement dotMovement;
    
    void Awake()
    {
        newPosX = scrollView.content.GetChild(0).GetComponent<InputField>();
        newPosY = scrollView.content.GetChild(1).GetComponent<InputField>();

        startX.text = startY.text = "0";

        dotMovement = GetComponent<AxisDrawer>().dot.GetComponent<DotMovement>();
        inputSpeed.text = dotMovement.speed.ToString();
    }

	private void FixedUpdate()
	{
        timeLeft.text = "Time left: " + dotMovement.timeLeft.ToString("0.00");
	}

    public void SavePositions()
	{
        var saveList = new List<string>
        {
            inputSpeed.text,
            startX.text + " " + startY.text,
            dotMovement.zeroCords.ToString(),
            GetComponent<AxisDrawer>().axisBasisX.ToString(),
            GetComponent<AxisDrawer>().axisBasisY.ToString(),
        };
        saveList.AddRange(positions.ConvertAll(x => x.ToString()));

        var path = EditorUtility.SaveFilePanel(
            "Select folder to save file",
            "",
            "positions.txt",
            "txt"
        );

        if(path.Length > 0)
            File.WriteAllLines(path, saveList);
	}

	public void ResetAxis()
    {
        GetComponent<AxisDrawer>().axisFlag = false;
        GetComponent<AxisDrawer>().HideAxis();
        dotMovement.gameObject.SetActive(false);
    }
    public void UpdateSpeed() => dotMovement.speed = float.Parse(inputSpeed.text);

    public void PrevButton() 
    {
        if(currentPos > 0)
		{
            currentPos--;
            dotMovement.timeLeft = 0;
            dotMovement.coroutineStack.Enqueue(dotMovement.MoveDot(new Vector2(positions[currentPos].x, positions[currentPos].y)));
		}
    }

    public void PlayButton() 
    {
        dotMovement.timeLeft = 0;
		for (int i = 0; i < positions.Count; i++)
		{
            currentPos = i;
            dotMovement.coroutineStack.Enqueue(dotMovement.MoveDot(new Vector2(positions[i].x, positions[i].y)));
		}
    }

    public void NextButton() 
    {
        if (currentPos + 1 < positions.Count)
		{
            currentPos++;
            dotMovement.timeLeft = 0;
            dotMovement.coroutineStack.Enqueue(dotMovement.MoveDot(new Vector2(positions[currentPos].x, positions[currentPos].y)));
		}
    }

    public void AddNewPosition() 
    {
		if (!string.IsNullOrEmpty(newPosX.text) && !string.IsNullOrEmpty(newPosY.text))
		{
            positions.Add(new Vector2(float.Parse(newPosX.text.Replace('.', ',')), float.Parse(newPosY.text.Replace('.', ','))));

            Vector3 posX = newPosX.GetComponent<RectTransform>().anchoredPosition3D;
            Vector3 posY = newPosY.GetComponent<RectTransform>().anchoredPosition3D;

            posX.y -= 30;
            posY.y -= 30;

            newPosX = Instantiate(newPosX);
            newPosY = Instantiate(newPosY);

            newPosX.text = newPosY.text = string.Empty;

            newPosX.transform.SetParent(scrollView.content.transform);
            newPosY.transform.SetParent(scrollView.content.transform);

            newPosX.GetComponent<RectTransform>().anchoredPosition3D = posX;
            newPosY.GetComponent<RectTransform>().anchoredPosition3D = posY;
        }
    }
}
