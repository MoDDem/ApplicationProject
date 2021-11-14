using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class LoadData : MonoBehaviour
{
	public GameObject linePrefab;
	public GameObject mainObject;
	public GameObject lineX, lineY;

	public float speed;
	public Vector3 startPosition;
	public Vector3 zeroPosition;
	public Vector3 lineBasisX, lineBasisY;
	public List<Vector3> positions;

    public void LoadFile()
	{
		var path = EditorUtility.OpenFilePanel(
			"Select a file to load in",
			"",
			"txt"
		);

		if (path.Length < 0)
			return;

		string[] text = File.ReadAllLines(path);
		speed = float.Parse(text[0].Replace('.', ','));

		float[] pos = text[1].Trim().Split().Select(x => float.Parse(x.Replace('.', ','))).ToArray();
		startPosition = new Vector3(pos[0], 1, pos[1]);

		pos = text[2].Trim(new char[2] { '(', ')' }).Split(',').Select(x => float.Parse(x.Replace('.', ','))).ToArray();
		zeroPosition = new Vector3(pos[0], 1, pos[2]);

		pos = text[3].Trim(new char[2] { '(', ')' }).Split(',').Select(x => float.Parse(x.Replace('.', ','))).ToArray();
		lineBasisX = new Vector3(pos[0], 1, pos[1]);

		pos = text[4].Trim(new char[2] { '(', ')' }).Split(',').Select(x => float.Parse(x.Replace('.', ','))).ToArray();
		lineBasisY = new Vector3(pos[0], 1, pos[1]);

		for (int i = 5; i < text.Length; i++)
		{
			pos = text[i].Trim(new char[2] { '(', ')' }).Split(',').Select(x => float.Parse(x.Replace('.', ','))).ToArray();
			positions.Add(new Vector3(pos[0], 1, pos[1]));
		}

		InitializeAxis();
		mainObject.GetComponent<ObjectMovement>().startPos = startPosition;
		mainObject.transform.position = mainObject.GetComponent<ObjectMovement>().CalculateNewObjectPosition(startPosition);
	}

	void InitializeAxis()
	{
		linePrefab.GetComponent<LineRenderer>().positionCount = 2;

		lineX = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
		lineY = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

		lineY.transform.parent = lineX.transform.parent = gameObject.transform;

		lineY.SetActive(true);
		lineX.SetActive(true);

		lineX.GetComponent<LineRenderer>().SetPosition(0, zeroPosition);
		lineX.GetComponent<LineRenderer>().SetPosition(1, lineBasisX);

		lineY.GetComponent<LineRenderer>().SetPosition(0, zeroPosition);
		lineY.GetComponent<LineRenderer>().SetPosition(1, lineBasisY);
	}
}
