using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Editor.GDK
{
    class ExternButtonManager : ScriptableObject
    {
        private static ExternButtonManager _instance;
        public static ExternButtonManager getInstance()
        {
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<ExternButtonManager>().FirstOrDefault();
            }
            if (_instance == null)
            {
                _instance = CreateInstance<ExternButtonManager>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
            }
            return _instance;
        }

        public void showExternView()
        {
            CommonWindow.show(() =>
            {




            });
        }
    }
}