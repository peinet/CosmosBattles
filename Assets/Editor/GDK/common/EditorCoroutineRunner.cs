﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Assets.Editor.GDK.common
{
    public static class EditorCoroutineRunner
    {
        private class EditorCoroutine : IEnumerator
        {
            private Stack<IEnumerator> executionStack;
            public EditorCoroutine(IEnumerator iterator)
            {
                this.executionStack = new Stack<IEnumerator>();
                this.executionStack.Push(iterator);
            }
            public object Current
            {
                get
                {
                    return this.executionStack.Peek().Current;
                }
            }

            public bool MoveNext()
            {
                IEnumerator i = this.executionStack.Peek();
                if (i.MoveNext())
                {
                    object result = i.Current;
                    if (result != null && result is IEnumerator)
                    {
                        this.executionStack.Push((IEnumerator)result);
                    }
                    return true;
                }
                else
                {
                    if (this.executionStack.Count > 1)
                    {
                        this.executionStack.Pop();
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
            public bool Find(IEnumerator iterator)
            {
                return this.executionStack.Contains(iterator);
            }

        }
        private static List<EditorCoroutine> editorCoroutineList;
        private static List<IEnumerator> buffer;
        public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
        {
            if(editorCoroutineList == null)
            {
                editorCoroutineList = new List<EditorCoroutine>();
            }
            if(buffer == null)
            {
                buffer = new List<IEnumerator>();
            }
            if(editorCoroutineList.Count == 0)
            {
                EditorApplication.update += Update;
            }
            buffer.Add(iterator);
            return iterator;
        }

        private static bool Find(IEnumerator iterator)
        {
            foreach (var editorCoroutine in editorCoroutineList)
            {
                if(editorCoroutine.Find(iterator))
                {
                    return true;
                }
            }return false;
        }

        private static void Update()
        {
            editorCoroutineList.RemoveAll(
                coroutine => {return  coroutine.MoveNext() == false; }
                );
            if(buffer.Count > 0 )
            {
                foreach (var iterator in buffer)
                {
                    if(Find(iterator) == false)
                    {
                        editorCoroutineList.Add(new EditorCoroutine(iterator));
                    }
                }
                buffer.Clear();
            }
            if(editorCoroutineList.Count == 0)
            {
                EditorApplication.update -= Update;
            }
        }

    }
    

}
