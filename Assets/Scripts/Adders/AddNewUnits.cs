using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNewUnits : MonoBehaviour
{
    public static AddNewUnits adder;

    public List<GameObject> prefabs;

    private void Awake()
    {
        adder = this;
    }

    //Запуск таймера спавна юнитов, если их время не равно нулю и создание стартового юнита
    void Start()
    {
        foreach (GameObject obj in prefabs)
        {
            StartCoroutine(SpawnTimer(obj.GetComponent<Units>().spawnTime, obj));
        }

        SpawnObject(prefabs[1]);
    }

    //Покупка (Инвесциция) нового юнита, если соблюдены все условия
    public void InvestToObject(GameObject obj)
    {
        //Получаем количество свободных игровых блоков
        int emptyCount = AddNewBlock.adder.PlatformList.FindAll(p => p.GetComponent<Platform>().IsFree).Count - 1;

        //Проверка на доступность юнита(мержил ли игрок его до покупки)
        if (!obj.GetComponent<Units>().isOpen)
        {
            UIManager.GetUIManager.OpenNode(3);
            return;
        }
        //Проверка на наличие свободных игровых блоков
        else if (emptyCount < 0)
        {
            UIManager.GetUIManager.OpenNode(2);
            return;
        }
        //Проверка на наличие необходимой суммы для покупки
        else if (Global.GetGlobal.getMoney < obj.GetComponent<Units>().invPrice)
        {
            UIManager.GetUIManager.OpenNode(1);
            return;
        }

        //Если все условия соблюдены, происходит создания нового юнита и списывания суммы у игрока.
        GameObject platform = AddNewBlock.adder.PlatformList.FindAll(p => p.GetComponent<Platform>().IsFree)[Random.Range(0, emptyCount)];

        Instantiate(obj, platform.transform, false);
        platform.GetComponent<Platform>().IsFree = false;

        Global.GetGlobal.UpdateMoney(-obj.GetComponent<Units>().invPrice);
    }

    //Создание юнита. Известен индекс и на каком конкретном блоке создается юинт. Используется при повышении уровня юнита
    public void SpawnObject(int index, Transform parent)
    {
        Instantiate(prefabs[index], parent, false);

        if (!prefabs[index].GetComponent<Units>().isOpen)
            prefabs[index].GetComponent<Units>().isOpen = true;
    }

    //Создание юнита. Известен юнит, который должен быть создан. Берется рандомный блок из свободных(если имеются) и на нем создается юнит.
    //Используется для создания юнита по таймеру.
    public void SpawnObject(GameObject obj)
    {
        int emptyCount = AddNewBlock.adder.PlatformList.FindAll(p => p.GetComponent<Platform>().IsFree).Count - 1;

        if (emptyCount < 0)
            return;

        GameObject platform = AddNewBlock.adder.PlatformList.FindAll(p => p.GetComponent<Platform>().IsFree)[Random.Range(0, emptyCount)];

        Instantiate(obj, platform.transform, false);

        if (!obj.GetComponent<Units>().isOpen)
            obj.GetComponent<Units>().isOpen = true;

        platform.GetComponent<Platform>().IsFree = false;
    }

    //Таймер создания юнита.
    IEnumerator SpawnTimer(int time, GameObject prefab)
    {
        if (time == 0)
            yield break;

        float t = time;

        while (t > 0f)
        {
            t /= Global.GetGlobal.getGameTime;
            t--;

            yield return new WaitForSeconds(1f);
        }

        SpawnObject(prefab);

        StartCoroutine(SpawnTimer(time, prefab));

        yield return null;
    }
}
