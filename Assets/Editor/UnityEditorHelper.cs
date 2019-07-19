using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace EditorExtension
{
    #region AssetDatabase

    public static class AssetDatabaseHelper
    {
        const int AssetsStrLength = 7;  //AssetsStrLength = "Assets/".Length;
        public static string GetAssetAbsolutePath(UObject obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            return Path.Combine(Application.dataPath, path.Substring(AssetsStrLength));
        }

        public static string GetAssetPath(string absolutePath)
        {
            return absolutePath.Substring(Application.dataPath.Length - AssetsStrLength + 1);
        }

        public static IEnumerable<string> GetPaths(string filter)
        {
            var guids = AssetDatabase.FindAssets(filter);

            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (EditorUtility.DisplayCancelableProgressBar(filter, path, i / (float)guids.Length))
                    break;

                yield return path;
            }

            EditorUtility.ClearProgressBar();
        }
    }

    #endregion

    #region HierarchyHelper

    public static class HierarchyHelper
    {
        public static string GetHierarchyPath(this Transform transform)
        {
            if (transform == null)
                return null;

            var sb = new StringBuilder();

            var stack = new Stack<Transform>();

            do
            {
                stack.Push(transform);
                transform = transform.parent;

            } while (transform);

            sb.Append(stack.Pop().name);

            while (stack.Count > 0)
            {
                sb.Append("/");
                sb.Append(stack.Pop().name);
            }

            return sb.ToString();
        }
    }

    #endregion

    #region Editor coroutine

    public abstract class IAsyncTask : IEnumerator
    {
        public enum Result
        {
            Running, Completed
        }

        bool m_running;

        object IEnumerator.Current
        {
            get { return null; }
        }

        bool IEnumerator.MoveNext()
        {
            if (!m_running)
            {
                m_running = true;
                OnStart();
            }

            var r = OnUpdate();

            if (r == Result.Completed)
            {
                m_running = false;
                OnEnd();
            }

            return r == Result.Running;
        }

        protected abstract void OnStart();

        protected abstract Result OnUpdate();

        protected abstract void OnEnd();

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        public bool UpdateBeginThisFrame
        {
            get;
            protected set;
        }
    }

    public class WaitEditorTime : IAsyncTask
    {
        DateTime m_timer;

        public WaitEditorTime(float seconds)
        {
            if (seconds <= 0)
                throw new ArgumentException("seconds <=0");

            m_timer = DateTime.Now + TimeSpan.FromSeconds(seconds);
            UpdateBeginThisFrame = true;
        }

        protected override void OnStart()
        {
        }

        protected override Result OnUpdate()
        {
            if (m_timer < DateTime.Now)
                return Result.Completed;
            else
                return Result.Running;
        }

        protected override void OnEnd()
        {
        }
    }

    public class WaitEditorFrames : IAsyncTask
    {
        int m_count;

        public WaitEditorFrames(int count)
        {
            if (count <= 0)
                throw new ArgumentException("count <= 0");

            m_count = count;
            UpdateBeginThisFrame = true;
        }

        protected override void OnStart()
        {
        }

        protected override Result OnUpdate()
        {
            if (--m_count <= 0)
                return Result.Completed;
            else
                return Result.Running;
        }

        protected override void OnEnd()
        {
        }
    }

    public class AsyncProgressTask : IAsyncTask
    {
        string m_title;
        IEnumerator<Content> m_task;
        Action<Content> m_onCancel;

        public class Content
        {
            public float Percent;
            public string Info;

            public Content(float percent, string info)
            {
                Percent = percent;
                Info = info;
            }
        }

        public AsyncProgressTask(string title, IEnumerator<Content> task, Action<Content> onCancel = null)
        {
            m_title = title;
            m_task = task;
            m_onCancel = onCancel;
        }

        protected override void OnEnd()
        {
            EditorUtility.ClearProgressBar();
        }

        protected override void OnStart()
        {
        }

        protected override Result OnUpdate()
        {
            if (m_task.MoveNext())
            {
                if (m_task.Current != null)
                {
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("{0}({1:D}%)", m_title, (int)(m_task.Current.Percent * 100)), m_task.Current.Info, m_task.Current.Percent))
                    {
                        if (m_onCancel != null)
                            m_onCancel(m_task.Current);
                        return Result.Completed;
                    }
                }
                return Result.Running;
            }

            return Result.Completed;
        }
    }

    public class WaitAsyncOperation : IAsyncTask
    {
        AsyncOperation m_asyncOperation;

        public WaitAsyncOperation(AsyncOperation asyncOperation)
        {
            m_asyncOperation = asyncOperation;

            UpdateBeginThisFrame = true;
        }

        protected override void OnStart()
        {
        }

        protected override Result OnUpdate()
        {
            if (m_asyncOperation.isDone)
                return Result.Completed;
            else
                return Result.Running;
        }

        protected override void OnEnd()
        {
        }

        public AsyncOperation AsyncOperation
        {
            get { return m_asyncOperation; }
        }
    }

    public static class EditorCoroutineHelper
    {
        static List<StackableCoroutine> s_coroutines = new List<StackableCoroutine>();

        // 对IEnumerator进行包装
        // 扩展Unity协程功能，递归展开IEnumerator中返回的IEnumerator
        class StackableCoroutine : IEnumerator
        {
            Stack<object> m_stack = new Stack<object>();

            public StackableCoroutine(IEnumerator coroutine)
            {
                m_stack.Push(coroutine);
            }

            object IEnumerator.Current
            {
                get { return m_stack.Count > 0 ? m_stack.Peek() : null; }
            }

            public bool MoveNext()
            {
                if (m_stack.Count == 0)
                    return false;

                var e = m_stack.Peek() as IEnumerator;
                if (e != null && e.MoveNext())
                {
                    if (e.Current != null)
                    {
                        m_stack.Push(e.Current);
                        var t = e.Current as IAsyncTask;
                        if (t != null && t.UpdateBeginThisFrame)
                            MoveNext();
                    }
                }
                else
                    m_stack.Pop();

                return m_stack.Count > 0;
            }

            void IEnumerator.Reset()
            {
                throw new InvalidOperationException("Can't reset a stackable coroutine");
            }
        }

        // 以此方法启动协程，将会递归展开IEnumerator中返回的IEnumerator
        public static void StartEditorCoroutine(IEnumerator coroutine)
        {
            if (coroutine == null)
                throw new ArgumentNullException("coroutine");

            if (s_coroutines.Count == 0)
                EditorApplication.update += Update;

            s_coroutines.Add(new StackableCoroutine(coroutine));
        }

        static void Update()
        {
            s_coroutines.RemoveAll(e => !e.MoveNext());

            if (s_coroutines.Count == 0)
                EditorApplication.update -= Update;
        }

        #region Test

        //[MenuItem( "Mu/EditorCoroutineTest" )]
        static void Test()
        {
            //UnityEditorHelper.ClearEditorConsoleLog();
            Console.Clear();
            Debug.Log("StartCoroutine");
            StartEditorCoroutine(EditorCoroutineTest());
            Debug.Log("Keep going");
        }

        static IEnumerator EditorCoroutineTest()
        {
            yield return WaitTimeTest();

            for (int i = 0; i < 100; i++)
            {
                yield return new WaitEditorFrames(1);
                Debug.Log(i);
            }
        }

        static IEnumerator WaitTimeTest()
        {
            var start = DateTime.Now;
            yield return new WaitEditorTime(2.5f);
            Debug.Log((DateTime.Now - start).TotalSeconds);
        }

        #endregion
    }

    #endregion

    #region TextureImporterPlatformSettings

    public static class TextureImporterPlatformSettingsHelper
    {
        public static bool Equal(this TextureImporterPlatformSettings self, TextureImporterPlatformSettings other)
        {
            return self.allowsAlphaSplitting == other.allowsAlphaSplitting
                && self.compressionQuality == other.compressionQuality
                && self.crunchedCompression == other.crunchedCompression
                && self.format == other.format
                && self.maxTextureSize == other.maxTextureSize
                && self.overridden == other.overridden
                && self.textureCompression == other.textureCompression;
        }
    }

    #endregion
}