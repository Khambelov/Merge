﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : Units
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

    int curEmpl;

    // Use this for initialization
    void Start()
    {
        curEmpl = 0;
        isDrag = false;
        StartCoroutine(Profit());
        UIManager.GetUIManager.UpdatePerSecond(salary);
    }

    public void Merge(GameObject unit)
    {
        transform.parent.GetComponent<Platform>().ClearPlatform();

        if (unit.GetComponent<Employee>())
        {
            curEmpl++;
            salary += 50;
            UIManager.GetUIManager.UpdatePerSecond(50);

            if (curEmpl >= mergePrice)
            {
                UnitsManager.uManager.UpgradeUnit(gameObject);
                UIManager.GetUIManager.UpdatePerSecond(-salary);
                Destroy(gameObject);
            }
        }
        else if (unit.GetComponent<Startup>())
        {
            UnitsManager.uManager.UpgradeUnit(gameObject);
            UIManager.GetUIManager.UpdatePerSecond(-salary);
            Destroy(gameObject);
        }
        else
        {
            transform.parent = unit.transform.parent;
            transform.transform.localPosition = unit.transform.localPosition;
        }
    }

    void ActionByUnit(bool isEqual, GameObject otherUnit)
    {
        if (isEqual)
        {
            Transform parent = transform.parent;
            transform.parent = otherUnit.transform.parent;
            transform.localPosition = new Vector3(0f, 1.1f, 0f);

            otherUnit.transform.parent = parent;
            otherUnit.transform.localPosition = otherUnit.GetComponent<Employee>() ? new Vector3(0f, 0.8f, 0f)
                                                                                   : new Vector3(0f, 1.1f, 0f);
        }
        else if (otherUnit.GetComponent<Startup>())
        {
            Destroy(gameObject);
        }
    }

    Transform startPos;
    Vector3 dist;
    float x;
    float y;
    float z;

    private void OnMouseDown()
    {
        startPos = transform;
        dist = Camera.main.WorldToScreenPoint(transform.position);
        x = Input.mousePosition.x - dist.x;
        y = Input.mousePosition.y - dist.y;
        z = Input.mousePosition.z - dist.z;

        isDrag = true;
    }

    private void OnMouseDrag()
    {
        if (isDrag)
        {
            float disX = Input.mousePosition.x - x;
            float disY = Input.mousePosition.y - y;
            float disZ = Input.mousePosition.z - z;

            Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(disX, disY, disZ));

            transform.position = new Vector3(world.x, world.y, world.y - 1.4f);
        }
    }

    private void OnMouseUp()
    {
        List<Collider> colliders = Physics.OverlapCapsule(gameObject.transform.position, gameObject.transform.position, GetComponent<CapsuleCollider>().radius).ToList();
        List<Collider> platforms = colliders.FindAll(o => o.GetComponent<Platform>() != null);
        Collider unit = colliders.Find(o => o.GetComponent<Units>() != null && o.gameObject != gameObject);

        if (unit)
        {
            GameObject otherUnit = unit.gameObject;

            ActionByUnit(UnitsManager.uManager.CheckEmployee(gameObject, otherUnit), otherUnit);
        }
        else if (platforms.Count > 0)
        {
            GameObject otherUnit = platforms.Count == 1 ? platforms[0].gameObject : null;

            if (otherUnit && otherUnit.GetComponent<Platform>().IsFree)
            {
                if (otherUnit.transform != transform.parent)
                {
                    transform.parent = otherUnit.transform;
                    transform.localPosition = new Vector3(0f, 1.1f, 0f);
                    transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
                }
                else
                {
                    transform.localPosition = new Vector3(0f, 1.1f, 0f);
                    transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
                }
            }
            else
            {
                transform.localPosition = new Vector3(0f, 1.1f, 0f);
                transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
            }
        }
        else
        {
            transform.localPosition = new Vector3(0f, 1.1f, 0f);
            transform.parent.gameObject.GetComponent<Platform>().ClearPlatform();
        }

        isDrag = false;
    }

    IEnumerator Profit()
    {
        bool first = true;

        while (true)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                Global.GetGlobal.UpdateMoney(salary);
            }

            yield return new WaitForSeconds(1f / Global.GetGlobal.getGameTime);
        }
    }
}
