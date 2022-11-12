using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Color OriginMatColor;
    Color HighlightColor = new Color(0, 255, 0);
    bool isClosed = true;

    void Start()
    {
        // запоминаем оригинальные настройки материала объекта
        OriginMatColor = this.GetComponent<Renderer>().material.color;
    }

    void OnMouseOver()
    {
        this.GetComponent<Renderer>().material.color = HighlightColor;
    }

    void OnMouseExit()
    {
        this.GetComponent<Renderer>().material.color = OriginMatColor;
    }

    void OnMouseDown()
    {
        isClosed = !isClosed;
        if (isClosed)
            this.transform.Translate(0, 4.2f, 0, Space.Self);
        else
            this.transform.Translate(0, -4.2f, 0, Space.Self);
    } 
}
