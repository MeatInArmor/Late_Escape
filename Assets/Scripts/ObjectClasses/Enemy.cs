using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy enemy; // Объект - враг

    private Marker marker; // объект - выделение при наведении
    private Selection selection; // объект - выделение цели атаки
    private Selection currentTarget; // текущая цель атаки
    //private bool isClosed = false;

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
        enemy = this.GetComponent<Enemy>();
        //Debug.Log(enemy);
        //Debug.Log(selection);
        //Debug.Log(currentTarget);
        //Debug.Log(CharacterValues.enemyCurrentTarget);
        if(CharacterValues.enemyCurrentTarget == null)
        {
            selection.gameObject.SetActive(true);
            CharacterValues.enemyCurrentTarget = enemy;
            currentTarget = selection;
            //Debug.Log(currentTarget);
            //Debug.Log(CharacterValues.enemyCurrentTarget);
        }
        else
        {
            currentTarget = CharacterValues.enemyCurrentTarget.GetComponentInChildren<Selection>();
            currentTarget.gameObject.SetActive(false); 
            CharacterValues.enemyCurrentTarget = this.GetComponent<Enemy>();
            selection.gameObject.SetActive(true);
            currentTarget = selection;
            //Debug.Log(currentTarget);
            //Debug.Log(CharacterValues.enemyCurrentTarget);
        }
    } 
}
