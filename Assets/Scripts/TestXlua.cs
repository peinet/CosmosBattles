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
        TextAsset ta = Resources.Load<TextAsset>("main.lua");
        print(ta);
        luaenv.DoString(ta.text);
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
