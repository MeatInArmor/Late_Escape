using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   ////
    // характеристики врага
    public int maxHP;                                                   // максимальное здоровье
    public int currentHP;                                               // текущее здоровье       
    public int damage;                                                  // урон (средний)
    public float magicDammage;                                          // магический урон      
    public int deffence;                                                // блокируемый урон (в единицах)
    public float attackDistance;                                        // дальность атаки
    public bool isDefeated;                                             // персонаж побеждён
    public float moveSpeedMultiplier;                                   // множитель скорости передвижения

    //время выполнения способностей
    public  float normalAttackTimeout;                                  // перезарядка обычной атаки
    public  float magicCastTimeout;                                     // перезарядка магии
    ////

    public  Enemy enemy;                                                // Объект - враг
    private Marker marker;                                              // объект - выделение при наведении
    private Selection selection;                                        // объект - выделение цели атаки
    private Selection currentTarget;                                    // для текущей цели атаки (selection выбранный в данный момент)

    private void Awake()
    {
        //Выполняется для каждого враго по отдельности
        enemy = this.GetComponent<Enemy>();                             // запихиваем нашего врага впеременную
        marker = enemy.GetComponentInChildren<Marker>();                // круг наводки мышью
        selection = enemy.GetComponentInChildren<Selection>();          // круг выбора цели
        marker.gameObject.SetActive(false);                             // 
        selection.gameObject.SetActive(false);                          // делаем эти две штуки неактивными
        SetCharacteristics();
    }

    private void SetCharacteristics()
    {   
////////// установка характеристик при появлении
        // если враг - красный фермер он получает свои характеристики
        if(gameObject.GetComponent<DevilFarmer>())                          
        {
            maxHP = 10;
            currentHP = 10; 
            // сюда добавлять остальные характеристики
        }
    }
    private void OnMouseOver()                          // когда курсор вошёл в область колайдера (физической части) объекта 
    {   
        marker.gameObject.SetActive(true);              // сделать круг наводки активным
        if(Input.GetMouseButtonDown(1))                 // если при этом щёлкнуть ПКМ...
            RightClick();
    }

    private void OnMouseExit()                          // когда курсор вышел из области колайдера (физической части) объекта 
    { 
        marker.gameObject.SetActive(false);             // сделать круг наводки неактивным
    }

    private void RightClick()
    {
        enemy = this.GetComponent<Enemy>();             // снова запихиваем этот объект в переменную ибо она сбросилась
        if(CharacterValues.enemyCurrentTarget == null)  // если игрок не выбрал цель
        {
            selection.gameObject.SetActive(true);       // сделать активным круг выбора врага
            CharacterValues.enemyCurrentTarget = enemy; // сделать этого врага целью игрока             
        }
        else                                            // если игрок уже выбирал цель и она жива (когда умирает сама сбрасывается)
        {
            currentTarget = CharacterValues.enemyCurrentTarget.GetComponentInChildren<Selection>(); // выбрать круг выбора цели игрока (великий и могучий Русский язык)
            currentTarget.gameObject.SetActive(false);                                              // сделать его неактивным
            CharacterValues.enemyCurrentTarget = enemy;                                             // сделать этого врага целью игрока  
            selection.gameObject.SetActive(true);                                                   // сделать активным круг выбора врага
        }
    } 
}
