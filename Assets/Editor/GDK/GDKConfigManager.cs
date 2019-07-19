using Assets.Editor.GDK.common;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.GDK
{
    class GDKConfigManager
    {
        private static GDKConfigManager _instance;
        private string twProjectPath;
        private string krProjectPath;
        private string cnProjectPath;
        private string worldProjectPath;
        private string worldServerPath;
        private Vector2 v2;
        private CommonWindow window;
        int browseBtnWid = 200;
        int groupSpace = 20;

        internal static GDKConfigManager getInstance()
        {
            if(_instance == null)
            {
                _instance = new GDKConfigManager();
            }
            return _instance;
        }


        public static string BrowseDir(string title, string extension = "", string directory = "")
        {
            return EditorUtility.OpenFolderPanel(title, directory, extension);
        }


        public void show()
        {
            window = CommonWindow.show(() =>
            {

                v2 = EditorGUILayout.BeginScrollView(v2);
                GUILayout.Label("配置各客户端的路径(选择SUNClient目录)，统一在此处进行配置，工具集中的其他工具可能会使用。");


                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("国服客户端目录");
                cnProjectPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_CN);
                if (GUILayout.Button("浏览", GUILayout.Width(browseBtnWid)))
                {
                    cnProjectPath = BrowseDir("!!!!!!!!!!!!!找到你国服客户端目录!!!!!!!!!!!!!!!!!", "*", cnProjectPath);
                    GDKApplication.setProjectPath(GDKApplication.PROJECT_CN, cnProjectPath);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.TextField(cnProjectPath);
                GUILayout.Space(groupSpace);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("台湾客户端目录");
                twProjectPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_TW);
                if (GUILayout.Button("浏览", GUILayout.Width(browseBtnWid)))
                {
                    twProjectPath = BrowseDir("!!!!!!!!!!!!!找到你台湾客户端目录!!!!!!!!!!!!!!!!!", "", twProjectPath);
                    GDKApplication.setProjectPath(GDKApplication.PROJECT_TW, twProjectPath);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.TextField(twProjectPath);
                GUILayout.Space(groupSpace);


                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("世界客户端目录");
                worldProjectPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_WORLD);
                if (GUILayout.Button("浏览", GUILayout.Width(browseBtnWid)))
                {
                    worldProjectPath = BrowseDir("!!!!!!!!!!!!!找到你世界客户端目录!!!!!!!!!!!!!!!!!", "", worldProjectPath);
                    GDKApplication.setProjectPath(GDKApplication.PROJECT_WORLD, worldProjectPath);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.TextField(worldProjectPath);
                GUILayout.Space(groupSpace);


                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("韩国客户端目录");
                krProjectPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_KR);
                if (GUILayout.Button("浏览", GUILayout.Width(browseBtnWid)))
                {
                    krProjectPath = BrowseDir("!!!!!!!!!!!!!找到你韩国客户端目录!!!!!!!!!!!!!!!!!", "*", krProjectPath);
                    GDKApplication.setProjectPath(GDKApplication.PROJECT_KR, krProjectPath);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.TextField(krProjectPath);
                GUILayout.Space(groupSpace);



                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("世界服务端目录(后端使用的工具依赖此目录)");
                worldServerPath = GDKApplication.getProjectPath(GDKApplication.PROJECT_WORLD_SERVER);
                if (GUILayout.Button("浏览", GUILayout.Width(browseBtnWid)))
                {
                    worldServerPath = BrowseDir("!!!!!!!!!!!!!找到你世界服务端目录!!!!!!!!!!!!!!!!!", "*", worldServerPath);
                    GDKApplication.setProjectPath(GDKApplication.PROJECT_WORLD_SERVER, worldServerPath);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.TextField(worldServerPath);
                GUILayout.Space(groupSpace);


                EditorGUILayout.EndScrollView();
            });
        }
    }
}
