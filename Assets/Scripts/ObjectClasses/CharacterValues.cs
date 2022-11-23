using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterValues : MonoBehaviour
{
	public AnimatorController[] _characterController; 
    public GameObject[] _characterModel;
    

 // Структура персоанжей с характеристиками
    public static Character[] character = new Character[4]; // 0 - фермер; 1 - дочь; 2 - урядник; 3 - воин
    public struct Character
    {   
        //основные характеристики
        //public int currentLevel;                                      // уровень персонажа
        //public int levelPoints;                                       // опыт персонажа для увеличения уровня
        public int maxHP;                                               // максимальное здоровье
        public int currentHP;                                           // текущее здоровье
        //public int maxStamina;                                        // максимальная выносливость
        //public int currentStamina;                                    // текущая выносливость
        //public int maxMana;                                           // максимальная мана
        //public int currentMana;                                       // текущая мана
        public int damage;                                              // урон (средний)
        public float magicDammage;                                      // магический урон
        //public float chargeAttackDammageMultiplier;                   // множитель урона сильной атаки
        public int deffence;                                            // блокируемый урон (в единицах)
        public float attackDistance;                                    // дальность атаки
        public bool isDefeated;                                         // персонаж побеждён
        public float moveSpeed;                                         // скорость ходьбы   // потом изменим на скорость передвижения в бою / вне боя
        public float sprintSpeed;                                       // скорость бега     //

        //время выполнения способностей
        public float normalAttackTimeout;                               // перезарядка обычной атаки
        //public float magicCastTimeout;                                  // перезарядка магии
        //public float dodgeDurationTimeout;                            // длительность уклонения (неуязвимость)
        //public float dodgeCastingTimeout;                             // время до конца возможности уклонения (нажатие кнопок)
        //public float dodgeReloadingTimeout;                           // перезарядка уклонения
        //public float dodgeModeReloadingTimeout;                        // перезарядка переключения режимов уклонения
        public  float deadTimeout;                                      // время до смены персонажа после поражения (или проигрыша, если их не осталось)

       

    }  

    // игровые аспекты
    public static bool canMove;                                        // становится false во время атаки, сильной атаки, магии, переключения персонажа, поражения 
    public bool isCameraRotationBlocked;                               // остановка поворота камеры при движении мышью при true

    // общие сведения команды
    public static int teamMembers;                                      // всего членов команды в текущий момент
    public static int currentTeamMember;                                // номер текущего члена отряда
    public static float teamMemberChangeCD;                             // время перезарядки смены персонажа
    public static bool inFightMode;                                     // находится ли отряд в состоянии боя
    
    // Взаимодействия со врагами
    public static List<Enemy> enemyInFight = new List<Enemy>();         // список врагов, с которыми сражаемся в данный момент 
    public static Enemy enemyCurrentTarget;                             // текущая цель атаки
    public static float distanceToEnemyCurrentTarget;                   //дистанция до цели атаки
    public float distance = 0;


    private void Awake()
    {   // установка значений
        canMove = true;                             // ходить можно
        isCameraRotationBlocked = false;            // глазеть по сторонам можно

        teamMembers = 1;                            // доступно персонажей на дамнный момент
        currentTeamMember = 0;                      // текущий персонаж (пока что временный, пото будет девушка, потом заменим на фермера)
        //teamMemberChangeCD = 5;                   // кд смены персонажа
        //inFightMode = false;                      // потом сделаем отдельно режим в бою и режим вне боя

        // настраиваем характеристики отдельно для каждого персонажа
        character[0].maxHP = 40;
        character[0].currentHP = 40;
        character[0].damage = 8;
        character[0].magicDammage = 12;    
        character[0].deffence = 2;
        character[0].isDefeated = false;
        character[0].attackDistance = 3;
        //character[0].moveSpeed = 2.0f;        // потом будем использовать вместо таких же переменных в ThirdPersonController
        //character[0].moveSpeed = 5.335f;      //

        character[0].normalAttackTimeout = 1.5f;
        character[0].deadTimeout = 5;

        // Debug.Log(character[0].maxHP);
        // Debug.Log(character[0].currentHP);
        // Debug.Log(character[0].damage);
        // Debug.Log(character[0].magicDammage);
        // Debug.Log(character[0].deffence);
        // Debug.Log(character[0].isDefeated);
        // Debug.Log(character[0].moveSpeedMultiplier);
        // Debug.Log(character[0].magicCastTimeout);
        


    }

}

   