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
        while(transform.position != newPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, GameManager.Instance.tileSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

}
