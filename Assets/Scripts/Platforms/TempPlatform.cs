using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlatform : MonoBehaviour
{
    public GameObject platform;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(FlashCube());
    }

    //Анимация мигания временного блока
    IEnumerator FlashCube()
    {
        while (GetComponent<Renderer>().material.color.g < 0.5f)
        {
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r,
                                                                GetComponent<Renderer>().material.color.g + 0.01f,
                                                                GetComponent<Renderer>().material.color.b);
            yield return new WaitForSeconds(0.01f);
        }

        while (GetComponent<Renderer>().material.color.g > 0f)
        {
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r,
                                                                GetComponent<Renderer>().material.color.g - 0.01f,
                                                                GetComponent<Renderer>().material.color.b);
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(FlashCube());

        yield return null;
    }

    //Выбор временного кубика, на месте которого строится игровой блок
    private void OnMouseDown()
    {
        GameObject platform = Instantiate(this.platform, transform.position, this.platform.transform.rotation);
        AddNewBlock.adder.AddBlock(platform);
    }
}
