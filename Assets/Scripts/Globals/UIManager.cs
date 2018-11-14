using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager GetUIManager;

    public Text money;
    public Text perSecond;
    public GameObject shop;
    public List<ShopButton> shopButtons;
    public GameObject node;
    public Text nodeText;

    int moneyPerSecond;

    private void Awake()
    {
        GetUIManager = this;
    }

    //Инициализация кнопок для магазина
    private void Start()
    {
        int i = 2;
        moneyPerSecond = 0;

        foreach (ShopButton button in shopButtons)
        {
            GameObject obj = AddNewUnits.adder.prefabs[i];
            button.btn.onClick.AddListener(() => AddNewUnits.adder.InvestToObject(obj));
            button.title.text = obj.name;
            button.price.text = obj.GetComponent<Units>().invPrice.ToString();
            button.salary.text = obj.GetComponent<Units>().salary.ToString() + "/s";

            i++;
        }
    }

    //Кнопки открытия/закрытия магазина
    public void OpenShop()
    {
        shop.SetActive(true);
    }

    public void CloseShop()
    {
        shop.SetActive(false);
    }

    //Открытие/закрытие магазинного окошка с ошибкой или какими-то сообщением
    public void OpenNode(int nodeCode)
    {
        node.SetActive(true);

        if (nodeCode == 1)
        {
            nodeText.text = "Not enough money!";
        }
        else if (nodeCode == 2)
        {
            nodeText.text = "No have empty platforms!";
        }
        else if (nodeCode == 3)
        {
            nodeText.text = "You still didn't open this company!";
        }
    }

    public void CloseNode()
    {
        node.SetActive(false);
    }

    //Обновление отображения кол-ва денег
    public void UpdateMoney(long money)
    {
        this.money.text = ConvertMoney(money);
    }

    //Обновление отображения скорости заработка денег
    public void UpdatePerSecond(int salary)
    {
        moneyPerSecond += salary;
        this.perSecond.text = ConvertMoney((long)moneyPerSecond) + "/s";
    }

    //Кнопка ускорения игрового времени
    public void FastToward()
    {
        StartCoroutine(DoFast());
    }

    //Ускорение игрового времени
    IEnumerator DoFast()
    {
        Global.GetGlobal.FastTime(true);

        yield return new WaitForSeconds(1f);

        Global.GetGlobal.FastTime(false);
    }

    //Конвертация денег или скорости заработка в сообщение с сокращением цифр (5500 -> 5,5K)
    string ConvertMoney(long money)
    {
        if (money.ToString().Length >= 4 && money.ToString().Length < 7)
        {
            float div = Mathf.Round((((float)money % 1000f) / 1000f) * 10f);
            div = div >= 9.5f ? 9f : div;
            return string.Concat(money / 1000, ",", div, "K");
        }
        else if (money.ToString().Length >= 7 && money.ToString().Length < 10)
        {
            float div = Mathf.Round((((float)money % 1000000f) / 1000000f) * 10f);
            div = div >= 9.5f ? 9f : div;
            return string.Concat(money / 1000000, ",", div, "M");
        }
        else if (money.ToString().Length >= 10)
        {
            float div = Mathf.Round((((float)money % 1000000000f) / 1000000000f) * 10f);
            div = div >= 9.5f ? 9f : div;
            return string.Concat(money / 1000000000, ",", div, "B");
        }
        return money.ToString();
    }
}

[System.Serializable]
public struct ShopButton
{
    public Button btn;
    public Text title;
    public Text price;
    public Text salary;
}
