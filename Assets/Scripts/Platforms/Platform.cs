using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    bool isEmpty;

    public bool IsFree { get { return isEmpty; } set { isEmpty = false; } }

    private void Awake()
    {
        isEmpty = true;
    }

    //Проверка на наличие юнитов на блоке
    private void Update()
    {
        if (transform.childCount > 0)
            isEmpty = false;
        else
            isEmpty = true;
    }

    //Очистка цвета блока
    public void ClearPlatform()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    //Выделения блока, на который нацелил игрок во время перетаскивания юнита
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Units>() != null && collision.gameObject.GetComponent<Units>().isDrag)
        {
            GetComponent<Renderer>().material.color = Color.grey;
        }
    }

    //Очистка выделения блока
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Units>() != null)
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
