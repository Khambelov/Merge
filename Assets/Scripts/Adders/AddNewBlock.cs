using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNewBlock : MonoBehaviour
{
    public static AddNewBlock adder;

    [SerializeField]
    List<GameObject> platforms;

    [SerializeField]
    GameObject prefab;

    [SerializeField]
    int YLength;

    [SerializeField]
    int XLength;

    public List<GameObject> PlatformList { get { return platforms; } }

    List<GameObject> tempPlatforms;

    int initCount;
    int cameraStep;

    private void Awake()
    {
        adder = this;
        tempPlatforms = new List<GameObject>();
        initCount = 0;
        cameraStep = 1;
    }

    //Запускает процесс инициализации полей выбора для расширения карты
    public void Initialize()
    {
        initCount++;
        InitializeBlocks();
    }

    //Уствновка игрового блока на место выбранного временного блока
    public void AddBlock(GameObject platform)
    {
        platforms.Add(platform);

        ClearAllTemps();
    }

    //Инициализация полей выбора для расширения карты
    void InitializeBlocks()
    {
        //Проверка на наличие полей выбора
        if (tempPlatforms.Count > 0)
            return;

        Vector3 v3 = Vector3.zero;

        //Вычисление краев по оси X и Y
        float? maxY = platforms.Max(o => o.transform.position.y);
        float? maxX = platforms.Max(o => o.transform.position.x);
        float? minY = platforms.Min(o => o.transform.position.y);
        float? minX = platforms.Min(o => o.transform.position.x);

        //Проверка на необходимость отдаления карты
        if (cameraStep == 1 && (maxY == 2 && minY == -2) && (maxX == XLength && minX == -XLength))
            NextCameraStep();
        else if (cameraStep == 2 && (maxY == 3 && minY == -3) && (maxX == XLength && minX == -XLength))
            NextCameraStep();

        //Проход по краям карты по оси Y
        for (int y = (int)maxY; y >= minY; y--)
        {
            //Проход по краям карты по оси X
            for (int x = (int)minX; x <= maxX; x++)
            {
                //Если по координатам нету блока, происходит проверка на блоки вокруг
                if (platforms.Find(c => c.transform.position.y == y && c.transform.position.x == x) == null)
                {
                    //Если есть рядом блок, создается временное поле выбора
                    if (CheckAroundX(x, y))
                    {
                        v3 = new Vector3(x, y, y);
                        tempPlatforms.Add(Instantiate(prefab, v3, prefab.transform.rotation));
                    }
                }
                //Если блок есть, то проверяется его позиция с границами карты
                else
                {
                    //Если X равен границе слева и граница не равна длине карты, то создается временный блок слева от блока
                    if (x == minX && minX > -XLength)
                    {
                        v3 = new Vector3(x - 1, y, y);
                        tempPlatforms.Add(Instantiate(prefab, v3, prefab.transform.rotation));
                    }
                    //Если X равен границе справа и граница не равна длине карты, то создается временный блок справа от блока
                    else if (x == maxX && maxX < XLength)
                    {
                        v3 = new Vector3(x + 1, y, y);
                        tempPlatforms.Add(Instantiate(prefab, v3, prefab.transform.rotation));
                    }

                    //Та же ситуация и с Y
                    if (y == maxY && maxY < YLength)
                    {
                        v3 = new Vector3(x, y + 1, y + 1);
                        tempPlatforms.Add(Instantiate(prefab, v3, prefab.transform.rotation));
                    }
                    else if (y == minY && minY > -YLength)
                    {
                        v3 = new Vector3(x, y - 1, y - 1);
                        tempPlatforms.Add(Instantiate(prefab, v3, prefab.transform.rotation));
                    }
                }
            }

            ClearByStep();
        }
    }

    //Проверка поля вокруг координат X и Y по оси X
    bool CheckAroundX(float x, float y)
    {
        bool isBorder = false;

        if (x > 0)
        {
            if (x <= XLength)
            {
                isBorder = HasBorder(x - 1, y);
                isBorder = !isBorder ? HasBorder(x + 1, y) : isBorder;
            }
            else
            {
                isBorder = false;
            }
        }
        else if (x < 0)
        {
            if (x >= -XLength)
            {
                isBorder = HasBorder(x + 1, y);
                isBorder = !isBorder ? HasBorder(x - 1, y) : isBorder;
            }
            else
            {
                isBorder = false;
            }
        }
        else
        {
            isBorder = HasBorder(x, y - 1);
            isBorder = !isBorder ? HasBorder(x + 1, y) : isBorder;
        }

        if (isBorder)
            return isBorder;
        else
            return CheckAroundY(x, y);
    }

    //Проверка поля вокруг координат X и Y по оси Y
    bool CheckAroundY(float x, float y)
    {
        bool isBorder = false;

        if (y > 0)
        {
            if (y <= YLength)
            {
                isBorder = HasBorder(x, y - 1);
            }
            else
            {
                isBorder = false;
            }
        }
        else if (y < 0)
        {
            if (y >= -YLength)
            {
                isBorder = HasBorder(x, y + 1);
            }
            else
            {
                isBorder = false;
            }
        }

        return isBorder;
    }

    //Поиск блока по заданным координатам
    bool HasBorder(float x, float y)
    {
        if (platforms.Find(c => c.transform.position.y == y && c.transform.position.x == x) == null)
            return false;
        else
            return true;
    }

    //Переход на следующее расстояние камеры от поля
    void NextCameraStep()
    {
        cameraStep++;

        switch (cameraStep)
        {
            case 2: StartCoroutine(BackCamera(1.5f)); break;
            case 3: StartCoroutine(BackCamera(2f)); break;
        }
    }

    //Очистка лишних блоков в зависимости от этапа камеры
    void ClearByStep()
    {
        switch (cameraStep)
        {
            case 1:
                {
                    foreach (GameObject tempPlatform in tempPlatforms.FindAll(p => p.transform.position.y > 2 || p.transform.position.y < -2))
                    {
                        Destroy(tempPlatform);
                    }

                    tempPlatforms.RemoveAll(p => p.transform.position.y > 2 || p.transform.position.y < -2);
                    break;
                }
            case 2:
                {
                    foreach (GameObject tempPlatform in tempPlatforms.FindAll(p => p.transform.position.y > 3 || p.transform.position.y < -3))
                    {
                        Destroy(tempPlatform);
                    }

                    tempPlatforms.RemoveAll(p => p.transform.position.y > 3 || p.transform.position.y < -3);
                    break;
                }
        }
    }

    //Плавное отдаление камеры
    IEnumerator BackCamera(float size)
    {
        float s = 0f;

        while (s < size)
        {
            Camera.main.orthographicSize += 0.25f;

            s += 0.25f;

            yield return new WaitForSeconds(0.01f);
        }
    }

    //Удаление всех временных блоков после установки нового блока
    void ClearAllTemps()
    {
        initCount--;

        foreach (GameObject temp in tempPlatforms)
        {
            Destroy(temp);
        }

        tempPlatforms = new List<GameObject>();

        if (initCount > 0)
            InitializeBlocks();
    }
}