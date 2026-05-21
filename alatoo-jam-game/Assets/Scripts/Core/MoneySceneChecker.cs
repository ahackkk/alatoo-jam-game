using UnityEngine;
using UnityEngine.SceneManagement;

public class MoneySceneChecker : MonoBehaviour
{
    [Header("Money Check")]
    public int requiredMoney = 1000;

    [Header("Scenes")]
    public string enoughMoneyScene;
    public string notEnoughMoneyScene;

    void OnEnable()
    {
        CheckMoney();
    }

    void CheckMoney()
    {
        if (MoneySystem.instance == null)
            return;

        if (MoneySystem.instance.money >= requiredMoney)
        {
            SceneManager.LoadScene(enoughMoneyScene);
        }
        else
        {
            SceneManager.LoadScene(notEnoughMoneyScene);
        }
    }
}