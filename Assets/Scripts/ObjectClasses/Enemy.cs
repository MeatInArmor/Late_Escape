using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   ////
    // характеристики врага
    public int maxHP;                                                // максимальное здоровье
    public int currentHP;                                            // текущее здоровье       
    public int damage;                                               // урон (средний)
    public float magicDammage;                                       // магический урон      
    public int deffence;                                             // блокируемый урон (в единицах)
    public float attackDistance;                                     // дальность атаки
    public bool isDefeated;                                          // персонаж побеждён
    public float moveSpeedMultiplier;                                // множитель скорости передвижения

    //время выполнения способностей
    public  float normalAttackTimeout;                                // перезарядка обычной атаки
    public  float magicCastTimeout;                                   // перезарядка магии
    ////

    public  Enemy enemy;                                              // Объект - враг
    private Marker marker;                                                  // объект - выделение при наведении
    private Selection selection;                                            // объект - выделение цели атаки
    private Selection currentTarget;                                        // для текущей цели атаки

    private void Awake()
    {
        //Выполняется для каждого враго по отдельности
        enemy = this.GetComponent<Enemy>();
        marker = enemy.GetComponentInChildren<Marker>();
        selection = enemy.GetComponentInChildren<Selection>();
        marker.gameObject.SetActive(false);
        selection.gameObject.SetActive(false);
        SetCharacteristics();
    }
    private void Update()
    {
        

    }

    private void SetCharacteristics()
    {   
        // установка характеристик при появлении
        if(gameObject.GetComponent<DevilFarmer>())                          // если враг - красный фермер
        {
            maxHP = 10;
            currentHP = 10; 
        }
    }
    private void OnMouseOver()
    {
        marker.gameObject.SetActive(true);
        if(Input.GetMouseButtonDown(1))
            RightClick();
    }

    private void OnMouseExit()
    { 
        marker.gameObject.SetActive(false);
    }

    private void RightClick()
    {
        enemy = this.GetComponent<Enemy>();
        if(CharacterValues.enemyCurrentTarget == null)
        {
            selection.gameObject.SetActive(true);
            CharacterValues.enemyCurrentTarget = enemy;
            currentTarget = selection;
        }
        else
        {
            currentTarget = CharacterValues.enemyCurrentTarget.GetComponentInChildren<Selection>();
            currentTarget.gameObject.SetActive(false); 
            CharacterValues.enemyCurrentTarget = this.GetComponent<Enemy>();
            selection.gameObject.SetActive(true);
            currentTarget = selection;
        }
    } 
}
