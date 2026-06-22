using System.Data.Common;
using UnityEngine;
using XLua;
using System;
using System.IO;
public class LuaConfigLoader : MonoBehaviour
{
    [Header("拖入Lua 配置文件")]
    public TextAsset luaConfigFile;

    [Header("拖入青蛙的参数黑板")]
    public FSM targetFSM;

    // 声明一个 Lua 虚拟机对象
    private LuaEnv luaEnv;

    // 这里的 Func<int, int, int> 表示：接收两个 int 参数，返回一个 int 结果
    private Func<int, int, int> luaDamageCalculator;
    // 记录 Lua 文件的真实硬盘路径
    private string luaFilePath;

    void Awake()
    {
        luaEnv = new LuaEnv();
        luaFilePath = Path.Combine(Application.dataPath, "XLuaFiles", "FrogConfig.lua.txt");
        Debug.Log("<color=yellow>我要找的绝对路径是：</color>" + luaFilePath);
        ReloadLua();
    }
    void Update()
    {
        //随时监听 F5 键，按下就重新加载 Lua！
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ReloadLua();
        }
    }
    // 专门封装一个用来“热重载”的函数
    private void ReloadLua()
    {
        Debug.Log("数据重新加载");
        if (targetFSM != null)
        {
            // 绕开 Unity 缓存，直接去硬盘上读取你刚刚 Ctrl+S 保存的最新文本！
            string latestLuaCode = File.ReadAllText(luaFilePath);

            //  让虚拟机执行最新的代码，覆盖掉旧的变量和函数
            luaEnv.DoString(latestLuaCode);

            // 重新提取数据，注入档案袋
            targetFSM.paramenter.health = luaEnv.Global.Get<int>("hp");
            targetFSM.paramenter.dashForce = luaEnv.Global.Get<float>("dashForce");
            targetFSM.paramenter.luaDamageCalculator = luaEnv.Global.Get<Func<int, int, int>>("CalculateRealDamage");

            Debug.Log("<color=cyan>[GM命令] F5 热重载成功！敌人数据和伤害公式已更新为最新版本！</color>");
        }
    }

    void OnDestroy()
    {
        //无视 Unity 的 GameObject 销毁状态，使用底层 ReferenceEquals 强行拿到 C# 内存对象
        if (!object.ReferenceEquals(targetFSM, null) && targetFSM.paramenter != null)
        {
            // 彻底切断 C# 对 Lua 函数的引用
            targetFSM.paramenter.luaDamageCalculator = null;
        }

        // 强制 C# 的垃圾回收器立刻开始工作
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();

        // 此时已经绝对干净，可以安全销毁 Lua 宇宙了
        if (luaEnv != null)
        {
            luaEnv.Dispose();
        }
    }
}
