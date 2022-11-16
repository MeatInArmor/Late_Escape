using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Marker marker;
    private Selection selection;
    private Enemy enemy;
    private bool isClosed = false;

    private void Start()
    {
        enemy = this.GetComponent<Enemy>();
        marker = enemy.GetComponentInChildren<Marker>();
        selection = enemy.GetComponentInChildren<Selection>();
        marker.gameObject.SetActive(false);
        selection.gameObject.SetActive(false);
        //Debug.Log(marker);
        //Debug.Log(selection);
    }

    private void OnMouseOver()
    {
        //marker.GetComponent<Renderer>().material.color = objectSelected;
        marker.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        //marker.GetComponent<Renderer>().material.color = objectNormal;
        marker.gameObject.SetActive(false);
    }

    private void OnMouseDown()
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
