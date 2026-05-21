using TMPro;
using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem instance;

    public int money = 0;

    public TextMeshProUGUI moneyText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;

        UpdateUI();
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;

        UpdateUI();
    }

    void UpdateUI()
    {
        moneyText.text = "$" + money.ToString();
    }
}