using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Units : MonoBehaviour
{
    //Общий абстрактный класс юнитов
    public int salary;
    public int invPrice;
    public int mergePrice;
    public int spawnTime;
    public bool isDrag;
    public bool isOpen;
    //Методы перемещения работают идентично для всех наследуемых классов! Подробности о них прописаны в классе Employee!
    //Методы мержа работают идентично для всех наследуемых классов! Подробности о них прописаны в классе Entrepreneur!
}
