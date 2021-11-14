using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotMovement : MonoBehaviour
{
    public GameObject axis;
    public GameObject lineX;
    public GameObject lineY;
    public float speed = 1;
    public Vector3 zeroCords = Vector3.zero;
    public float timeLeft;
    Vector3 _zeroCords = Vector3.zero;

    public Queue<IEnumerator> coroutineStack = new Queue<IEnumerator>();
    public bool coroutineStackEnd = true;

    private void FixedUpdate()
	{
        float startX, startY = 0;
        float.TryParse(axis.GetComponent<UIWorker>().startX.text.Replace('.', ','), out startX);
        float.TryParse(axis.GetComponent<UIWorker>().startY.text.Replace('.', ','), out startY);

        if (coroutineStack.Count != 0 && coroutineStackEnd)
        {
            coroutineStackEnd = false;
            StartCoroutine(CoroutineStack());
        }

        if (coroutineStackEnd && _zeroCords != CalculateNewDotPosition(new Vector2(startX, startY)))
        {
            transform.position = CalculateNewDotPosition(new Vector2(startX, startY));
            _zeroCords = transform.position;
        }
	}

    public IEnumerator CoroutineStack()
    {
        while (coroutineStack.Count > 0)
        {
            yield return StartCoroutine(coroutineStack.Dequeue());
        }

        coroutineStackEnd = true;
        coroutineStack.Clear();
    }

    public IEnumerator MoveDot(Vector2 pos)
	{
        float step = speed * Time.fixedDeltaTime;
        
        Vector3 dotPos = CalculateNewDotPosition(pos);

        while (Vector3.Distance(transform.position, dotPos) > 0.001f)
		{
            timeLeft += Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, dotPos, step);
            yield return new WaitForFixedUpdate();
		}

        transform.position = dotPos;
    }

    public Vector3 CalculateNewDotPosition(Vector2 pos)
	{
        Vector3 xPos = (lineX.GetComponent<LineRenderer>().GetPosition(1) - zeroCords) * pos.x;
        Vector3 yPos = (lineY.GetComponent<LineRenderer>().GetPosition(1) - zeroCords) * pos.y;

        return zeroCords + (yPos + xPos);
    }
}
