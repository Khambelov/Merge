using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{

    public static Global GetGlobal;

    long money;
    int gameTime;

    bool start;

    public long getMoney { get { return money; } }

    public int getGameTime { get { return gameTime; } }

    DateTime beforePause;

    private void Awake()
    {
        GetGlobal = this;
        gameTime = 1;
        start = false;
    }

    //Обработка во время паузы приложения (сворачивания)
    private void OnApplicationPause(bool pause)
    {
        //Проверка на подготовку игры
        if (start)
        {
            //Если пауза, то запоминаем время ухода из приложения
            if (pause)
            {
                beforePause = DateTime.UtcNow;
            }
            //Если мы отжали паузу, то выполняем события, которые должны были произойти за прошедшее время.
            else
            {
                //Берем время паузы в секундах
                int seconds = (DateTime.UtcNow - beforePause).Seconds;

                //Проходимся по юнитам на карте, которые должны были приносить прибыль и суммируем их прибыль
                foreach (GameObject platform in AddNewBlock.adder.PlatformList.FindAll(p => p.transform.childCount > 0))
                {
                    UpdateMoney(platform.GetComponentInChildren<Units>().salary * seconds);
                }

                //Производим создание юнитов, которые должны были создаться за время паузы. Берем время спавна у юнитов и какое кол-во должно было создаться
                int spawnEmpTime = AddNewUnits.adder.prefabs.Find(u => u.GetComponent<Employee>() != null).GetComponent<Employee>().spawnTime;
                int spawnEmpCount = seconds / spawnEmpTime;
                int spawnEntTime = AddNewUnits.adder.prefabs.Find(u => u.GetComponent<Entrepreneur>() != null).GetComponent<Entrepreneur>().spawnTime;
                int spawnEntCount = seconds / spawnEntTime;

                //Создаем юнитов столько, сколько должно было создаться во время паузы
                for (int i = 1; i <= spawnEmpCount; i++)
                {
                    AddNewUnits.adder.SpawnObject(AddNewUnits.adder.prefabs.Find(u => u.GetComponent<Employee>() != null));

                    if (i % (spawnEntTime / spawnEmpTime) == 0 && i / (spawnEntTime / spawnEmpTime) < spawnEntCount)
                        AddNewUnits.adder.SpawnObject(AddNewUnits.adder.prefabs.Find(u => u.GetComponent<Entrepreneur>() != null));
                }
            }
        }
    }

    // Задаем стартовое кол-во денег и обновляем инфу на экране
    void Start()
    {
        money = 0;

        UpdateMoney(0);

        start = true;
    }

    //Обновление кол-ва денег игрока и обновления текста на экране
    public void UpdateMoney(int price)
    {
        money += price;

        UIManager.GetUIManager.UpdateMoney(money);
    }

    //Ускорение игрового времени при нажатии на кнопку ускорения
    public void FastTime(bool fast)
    {
        if (fast)
        {
            gameTime = 5;
        }
        else
        {
            gameTime = 1;
        }
    }
}
