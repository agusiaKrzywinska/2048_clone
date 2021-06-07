using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private int value;
    public int Value => value;
    [SerializeField]
    private SpriteRenderer spr;
    [SerializeField]
    private TMPro.TextMeshPro text;
    [HideInInspector]
    public bool isMoving = false;
    
    public void SetupTile(int value)
    {
        this.value = value;
        text.text = value.ToString();
        spr.color = GameManager.Instance.tileColors[(int)Mathf.Log(value, 2)];
    }

    public void UpdatePosition(int x, int y)
    {
        StartCoroutine(LerpPosition(new Vector3(x, y)));
    }

    public IEnumerator LerpPosition(Vector3 newPosition)
    {
        isMoving = true;

        /*
        while(transform.position != newPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, GameManager.Instance.tileSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        */
        Vector3 startPos = transform.position;
        float currentTime = 0;
        float totalTime = Vector3.Distance(startPos, newPosition) / GameManager.Instance.tileSpeed;
        while(currentTime < totalTime)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, newPosition, currentTime / totalTime);
            yield return new WaitForEndOfFrame();
        }

        isMoving = false;
    }

}
