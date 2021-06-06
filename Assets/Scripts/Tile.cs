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
    }

    public void UpdatePosition(int x, int y)
    {
        transform.position = new Vector3(x, y);
    }

}
