using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotNotScratch.Scripts.Editor
{
    public class MenuFunctions : MonoBehaviour
    {
        [MenuItem("GameThing/Reference")]
        static void OpenReferencePage() {
            Application.OpenURL("http://matthewpoindexter.com/gamething");
        }
    }
}
