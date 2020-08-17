using NodeTreeExportor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
public class NodeTreeExporterPanel : EditorWindow
{
    [MenuItem("Tools/NodeTreeExporter")]
    public static void ShowWindow()
    {
        var window = GetWindow<NodeTreeExporterPanel>();
        window.Show();
    }

    private Boolean checkExport()
    {
        Boolean flag = false;
        if (Parame.exportPath != null && Parame.exportPath.Length > 0)
        {

            if (Directory.Exists(Parame.exportPath))
            {
                flag = true;
            }

        }

        return flag;
    }

    private void doExport()
    {
        try
        {
            NodeTreeExportorExport.doExport();

            Debug.Log("Export Success!!!!!!!!");
        }
        catch
        {
            Debug.LogError("Export Error");
        }
    }

    private void OnGUI()
    {
        // Export path
        GUILayout.BeginHorizontal();
        Parame.exportPath = EditorGUILayout.TextField(Parame.exportPath);
        if (GUILayout.Button("...", GUILayout.Width(48), GUILayout.Height(16)))
        {
            Parame.exportPath = EditorUtility.OpenFolderPanel("Export Path", Parame.exportPath, "");
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("导出文件名:", EditorStyles.centeredGreyMiniLabel);
        Parame.exportFileName = EditorGUILayout.TextField(Parame.exportFileName);

        if (GUILayout.Button(" 导出节点树 ", GUILayout.Width(180), GUILayout.Height(24)))
        {
            if (checkExport())
            {
                doExport();
            }
        }
    }
}
