using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Marker marker;
    Selection selection;
    Enemy enemy;
    //Color objectSelected = new Color(255, 0, 0, 255);
    //Color objectNormal = new Color(255, 0, 0, 0);
    bool isClosed = false;

    void Start()
    {
        // запоминаем оригинальные настройки материала объекта
        enemy = this.GetComponent<Enemy>();
        marker = enemy.GetComponentInChildren<Marker>();
        selection = enemy.GetComponentInChildren<Selection>();
        marker.gameObject.SetActive(false);
        selection.gameObject.SetActive(false);



        //Debug.Log(marker);
        //Debug.Log(selection);
    }

    void OnMouseOver()
    {
        //marker.GetComponent<Renderer>().material.color = objectSelected;
        marker.gameObject.SetActive(true);
    }

    void OnMouseExit()
    {
        //marker.GetComponent<Renderer>().material.color = objectNormal;
        marker.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        isClosed = !isClosed;
        if (isClosed)
            //selection.GetComponent<Renderer>().material.color = objectSelected;
            selection.gameObject.SetActive(true);
        else
            //selection.GetComponent<Renderer>().material.color = objectNormal;
            selection.gameObject.SetActive(false);
    } 
}
