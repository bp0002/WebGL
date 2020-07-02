using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using ModelTextureDetail;
using UnityEngine;

/// 将 Unity 的场景或者资源导出为 glTF 2.0 标准格式
public class AnalyPanel : EditorWindow
{
    const string TitleName = "ModelsTextureDetailAnaly";

    [MenuItem("Tools/" + TitleName)]
    static void Init()
    {
        #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX // edit: added Platform Dependent Compilation - win or osx standalone
        AnalyPanel window = (AnalyPanel)EditorWindow.GetWindow(typeof(AnalyPanel));
                window.titleContent.text = TitleName;
                window.Show();
        #else // and error dialog if not standalone
		        EditorUtility.DisplayDialog("Error", "Your build target must be set to standalone", "Okay");
        #endif
    }

    AnalyComponent analy;

    GameObject panel;


    void OnEnable()
    {
        this.minSize = new Vector2(400, 250);
        if (panel == null)
        {
            panel = new GameObject("Exporter");
            analy = panel.AddComponent<AnalyComponent>();
            panel.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnDisable()
    {
        if (panel != null)
        {
            GameObject.DestroyImmediate(panel);
            panel = null;
        }
    }

    private void OnGUI()
    {

        if (GUILayout.Button(TitleName, GUILayout.Width(180), GUILayout.Height(24)))
        {
            doAnaly();
        }
    }
    private void doAnaly()
    {
        try
        {
            analy.analy();
        }
        finally
        {

        }
    }
}
