using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Employee : Units
{
    public int InvestmentsPrice
    {
        get { return invPrice; }
    }

    public int MergePrice
    {
        get { return mergePrice; }
    }

    public int SpawnTime
    {
        get { return spawnTime; }
    }

    public int Salary
    {
        get { return salary; }
        set { salary = value; }
    }

    public bool IsDrag { get { return isDrag; } }

    // Use this for initialization
    void Start()
    {
        isDrag = false;
    }

    Transform startPos;
    Vector3 dist;
    float x;
    float y;
    float z;

    //Действие юнита в зависимости от юнита.
    void ActionByUnit(bool isEmployee, GameObject otherUnit)
    {
        //Если это сотрудник, то юнит просто возвращается на место
        if (isEmployee)
        {
            transform.localPosition = new Vector3(0f, 0.8f, 0f);
        }
        //Иначе юнит уничтожается,т.к. смержился с другим юнитом
        else
        {
            Destroy(gameObject);
        }
    }

    //Событие при нажатии на юнита. Берутся стратовые позиции для перемещения юнита
    private void OnMouseDown()
    {
        startPos = transform;
        dist = Camera.main.WorldToScreenPoint(transform.position);
        x = Input.mousePosition.x - dist.x;
        y = Input.mousePosition.y - dist.y;
        z = Input.mousePosition.z - dist.z;

        isDrag = true;
    }

    //Событие при перемещении юнита. Юнит перемещается по X и Z, используя оси X и Y
    private void OnMouseDrag()
    {
        if (isDrag)
        {
            float disX = Input.mousePosition.x - x;
            float disY = Input.mousePosition.y - y;
            float disZ = Input.mousePosition.z - z;

            Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(disX, disY, disZ));

            transform.position = new Vector3(world.x, world.y, world.y - 1f);
        }
    }

    //Событие при отпускании юнита.
    private void OnMouseUp()
    {
        //Берутся все колайдеры, которые соприкасаются с юнитом, затем разделяются на блоки и других юнитов
        List<Collider> colliders = Physics.OverlapSphere(gameObject.transform.position, GetComponent<SphereCollider>().radius).ToList();
        List<Collider> platforms = colliders.FindAll(o => o.GetComponent<Platform>() != null);
        Collider unit = colliders.Find(o => o.GetComponent<Units>() != null && o.gameObject != gameObject);

        //Если есть юнит, то происходит проверка на то, какой это юнит и что делать в зависимости от этого юнита
        if (unit)
        {
            GameObject otherUnit = unit.gameObject;

            ActionByUnit(UnitsManager.uManager.CheckEmployee(gameObject, otherUnit), otherUnit);
        }
        //Если юнитов нету, то просиходит проверка блока. Если блок свободен, юнит переместится на него
        else if (platforms.Count > 0)
        {
            GameObject otherUnit = platforms.Count == 1 ? platforms[0].gameObject : null; //Небольшой костыль, т.к. радиус объекта умудряется хватать лишний блок и соответственно перемещается не туда.
                                                                                          //При этом визуально видно, что юнит не касается нескольких блоков.

            if (otherUnit && otherUnit.GetComponent<Platform>().IsFree)
            {
                //Если этот блок не блок, с которого был перемещен юнит, то он меняет родителя и положение.
                if (otherUnit.transform != transform.parent)
                {
                    transform.parent = otherUnit.transform;
                    transform.localPosition = new Vector3(0f, 0.8f, 0f);
                    transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
                }
                //Иначе он просто вернется на свое место
                else
                {
                    transform.localPosition = new Vector3(0f, 0.8f, 0f);
                    transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
                }
            }
            //Иначе он просто вернется на свое место
            else
            {
                transform.localPosition = new Vector3(0f, 0.8f, 0f);
                transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
            }
        }
        //Если его просто увести куда-то, то он вернется на место
        else
        {
            transform.localPosition = new Vector3(0f, 0.8f, 0f);
            transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
        }

        isDrag = false;
    }
}
