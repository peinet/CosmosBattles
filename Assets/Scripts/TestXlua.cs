using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
public class TestXlua : MonoBehaviour
{
    LuaEnv luaenv;
    // Start is called before the first frame update
    void Start()
    {
        luaenv = new LuaEnv();
        luaenv.AddLoader((ref string filename) =>
        {
            if (filename == "Luas/XXX/XXX")
            {
                UnityEngine.Debug.Log("test decode");
                //加载文件
                //解密文件
                //luaenv.DoString("文件内容");
                //return System.Text.Encoding.UTF8.GetBytes("文件内容");
            }
            return null;
        });
        TextAsset ta = Resources.Load<TextAsset>("Luas/main.lua");
        print(ta);
        luaenv.DoString(ta.text, "main.lua");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDestroy()
    {
        luaenv.Dispose();
    }
}
