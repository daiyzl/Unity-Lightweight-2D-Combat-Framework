using UnityEngine;

public class GhostController : MonoBehaviour
{
    private SpriteRenderer sr;//获得物体上的SpriteRenderer组件

    [Header("残影参数")]
    [Tooltip("残影彻底消失需要多少秒")]
    [SerializeField] private float fadeTime = 0.5f;

    [Tooltip("残影的初始颜色和透明度")]
    // 默认设置为纯白色，初始透明度为 0.8，避免和本体混淆
    [SerializeField] private Color initialColor = new Color(1f, 1f, 1f, 0.8f);

    private float currentAlpha;//记录现在的Alpha透明度

    // Awake 只在物体第一次生成时执行一次，用于获取组件引用
    private void Awake()
    {
       sr=GetComponent<SpriteRenderer>();//初始化
    }
    private void OnEnable()
    {
        if(sr==null)sr=GetComponent<SpriteRenderer>();

        //重置透明度
        currentAlpha=initialColor.a;
        //重置颜色
        sr.color=initialColor;
    }
    private void Update()
    {
        currentAlpha -= (initialColor.a / fadeTime) * Time.deltaTime;
        //  将新的透明度应用给 SpriteRenderer
        Color newColor = sr.color;
        newColor.a = currentAlpha;
        sr.color = newColor;
        /*
        不能给作为属性（Property）返回的临时结构体里面的变量赋值 sr.color.a = currentAlpha;
        public Color color { get; set; }这个 { get; set; }说明 color 是一个属性 (Property)，而不是普通字段
        */

        //透明度为0回收失活
        if (currentAlpha <= 0f)
        {
            Debug.Log("残影回收");
            gameObject.SetActive(false);
        }
    }
}
