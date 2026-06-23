using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("拖入代表血条本体的红条 Image")]
    public Image healthFillImage;

    [Header("拖入玩家身上的 PlayerStats 脚本")]
    public PlayerStats playerStats;

    void Start()
    {
        if (playerStats != null)
        {
            UpdateHealthBar(playerStats.CurrentHealth, playerStats.MaxHealth);
        }
    }

    void OnEnable()
    {
        // 订阅广播：当脚本激活时，把 UpdateHealthBar 函数绑定到玩家的受伤事件上
        if (playerStats != null)
        {
            playerStats.HealthChanged += UpdateHealthBar;
        }
    }

    void OnDisable()
    {
        // 取消订阅，当血条UI被隐藏或销毁时，切断联系，防止报空指针内存泄漏！
        if (playerStats != null)
        {
            playerStats.HealthChanged -= UpdateHealthBar;
        }
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        // 强转 float，算出百分比
        float fillRatio = (float)currentHealth / maxHealth;
        Debug.Log("currentHealth：" + currentHealth);
        Debug.Log("maxHealth：" + maxHealth);
        // 改变 UI 图片的填充长度
        healthFillImage.fillAmount = fillRatio;
        Debug.Log("fillRatio = " + fillRatio);
    }
}
