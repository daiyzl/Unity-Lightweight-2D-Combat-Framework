using System;
using System.Collections.Generic;
using XLua;

public static class XLuaConfig
{
    // 告诉 xLua，下面列表里的 C# 委托，都需要生成“Lua翻译官”
    [CSharpCallLua]
    public static List<Type> csharpCallLuaList = new List<Type>()
    {
        // 把报错的那个委托类型写在这里
        typeof(Func<int, int, int>),
        
    };
}
