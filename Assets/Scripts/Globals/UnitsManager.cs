using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager uManager;

    private void Awake()
    {
        uManager = this;
    }

    //Проверка юнита, с которым пытаются смержить выбранного юнита
    public bool CheckEmployee(GameObject current, GameObject other)
    {
        var unit = other.GetComponent<Units>();
        var cur = current.GetComponent<Units>();

        //Если юнит - сотрудник, то просто возвращается true
        if (unit.GetType() == typeof(Employee))
        {
            return true;
        }
        //Если выбранный юнит - сотрудник, то идет проверка на мерж. Если юнит для мержа - фирма, то ничего не происходит, иначе происходит мерж
        else if (cur.GetType() == typeof(Employee))
        {
            if (unit.GetType() != typeof(Firm))
            {
                MergeUnits(current, other);
                return false;
            }
            else
                return true;
        }
        //Если юнит для мержа и выбранный юнит - не сотрудники и не фирмы и одни и они одинаковы, то происходит мерж
        else if (cur.GetType() == unit.GetType())
        {
            if (cur.GetType() == typeof(Firm))
                return true;
            else
            {
                MergeUnits(current, other);
                return false;
            }
        }

        return true;
    }

    //Улучшения юнита, если набрана достаточно сотрудников или были смержаны 2 одинаковых юнита - не сотрудника
    public void UpgradeUnit(GameObject obj)
    {
        var unit = obj.GetComponent<Units>();

        if (unit.GetType() == typeof(Entrepreneur))
        {
            AddNewUnits.adder.SpawnObject(2, obj.transform.parent);
        }
        else if (unit.GetType() == typeof(Startup))
        {
            AddNewUnits.adder.SpawnObject(3, obj.transform.parent);
        }
        else if (unit.GetType() == typeof(SmallFirm))
        {
            AddNewUnits.adder.SpawnObject(4, obj.transform.parent);
        }

        //Расширение игрового поля после появления новой компании
        AddNewBlock.adder.Initialize();
    }

    //Мерж юнита, с которым хотят смержить выбранного юнита
    void MergeUnits(GameObject current, GameObject other)
    {
        var unit = other.GetComponent<Units>();
        var cur = current.GetComponent<Units>();

        if (unit.GetType() == typeof(Entrepreneur))
        {
            other.GetComponent<Entrepreneur>().Merge(cur.gameObject);
        }
        else if (unit.GetType() == typeof(Startup))
        {
            other.GetComponent<Startup>().Merge(cur.gameObject);
        }
        else if (unit.GetType() == typeof(SmallFirm))
        {
            other.GetComponent<SmallFirm>().Merge(cur.gameObject);
        }
        else if (unit.GetType() == typeof(Firm))
        {
            other.GetComponent<Firm>().Merge(cur.gameObject);
        }
    }
}
