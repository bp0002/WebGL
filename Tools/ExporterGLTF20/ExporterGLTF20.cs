#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;
using Assets.ExporterGLTF20;

/// 将 Unity 的场景或者资源导出为 glTF 2.0 标准格式
public class ExporterGLTF20 : EditorWindow {
    // Fields limits
    const int NAME_LIMIT    = 48;
    const int DESC_LIMIT    = 1024;
    const int TAGS_LIMIT    = 50;
    const int SPACE_SIZE    = 5;

    Vector2 DESC_SIZE       = new Vector2(512, 64);

    GameObject mExporterGo;
    SceneToGlTFWiz mExporter;

    string mExportPath  = "";
    string mStatus      = "";
    string mResult      = "";

    ExportOption exportOption = new ExportOption();

    GUIStyle mTextAreaStyle;
    GUIStyle mStatusStyle;

    //private bool mExportAnimation   = true;
    //private bool mExportPBR         = true;
	//private bool mCombineMesh       = false;
    //private bool mBuildZip        = false;
	//private bool mConvertImage      = false;
    //private bool mExportNormal      = false;
    private string mParamName       = "";
    private string mParamTags       = "";
    private string mParamDescription    = "";

    void OnEnable() {
        this.minSize = new Vector2(400, 250);
        if (mExporterGo == null) {
            mExporterGo = new GameObject("Exporter");
            mExporter = mExporterGo.AddComponent<SceneToGlTFWiz>();
            mExporterGo.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnDisable() {
        if (mExporterGo != null) {
            GameObject.DestroyImmediate(mExporterGo);
            mExporterGo = null;
        }
    }

    void OnSelectionChange() {
        updateExporterStatus();
        Repaint();
    }

    void OnGUI() {
        if (mTextAreaStyle == null) {
            mTextAreaStyle = new GUIStyle(GUI.skin.textArea);
            mTextAreaStyle.fixedWidth = DESC_SIZE.x;
            mTextAreaStyle.fixedHeight = DESC_SIZE.y;
        }

        if (mStatusStyle == null) {
            mStatusStyle = new GUIStyle(EditorStyles.label);
            mStatusStyle.richText = true;
        }

        // Export path
        GUILayout.Label("Export Path", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        //mExportPath = EditorGUILayout.TextField(mExportPath);
        exportOption.mPrePath = EditorGUILayout.TextField(exportOption.mPrePath);

        if (GUILayout.Button("...", GUILayout.Width(48), GUILayout.Height(16))) {
            //mExportPath = EditorUtility.OpenFolderPanel("Export Path", mExportPath, "");
            exportOption.mPrePath = EditorUtility.OpenFolderPanel("Export Path", exportOption.mPrePath, "");
        }

        GUILayout.EndHorizontal();

        // Model settings
        GUILayout.Label("Model properties", EditorStyles.boldLabel);
        exportOption.useGLTFNameByModelName = EditorGUILayout.ToggleLeft("是否使用选择的节点名称命名导出文件(当只选中一个节点时才生效)", exportOption.useGLTFNameByModelName);

        // Model name
        GUILayout.Label("Name");
        if (exportOption.useGLTFNameByModelName)
        {
            Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel);
            if (transforms.Length == 1)
            {
                mParamName = GlTF_Writer.cleanNonAlphanumeric(transforms[0].name);
                if (mParamName.IndexOf(" ") >=0)
                {
                    Debug.LogWarning("节点名称不规范(可能有空格) " + mParamName);
                    mParamName = "";
                }
            }
        }
        mParamName = EditorGUILayout.TextField(mParamName);
        GUILayout.Label("(" + mParamName.Length + "/" + NAME_LIMIT + ")", EditorStyles.centeredGreyMiniLabel);
        EditorStyles.textField.wordWrap = true;
        GUILayout.Space(SPACE_SIZE);

        /*GUILayout.Label("Description");
        mParamDescription = EditorGUILayout.TextArea(mParamDescription, mTextAreaStyle);
        GUILayout.Label("(" + mParamDescription.Length + " / 1024)", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(SPACE_SIZE);*/

        /*GUILayout.Label("Tags (separated by spaces)");
        mParamTags = EditorGUILayout.TextField(mParamTags);
        GUILayout.Label("'unity' and 'unity3D' added automatically (" + mParamTags.Length + "/50)", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(SPACE_SIZE);*/

        GUILayout.Label("Options", EditorStyles.largeLabel, GUILayout.MinWidth(300.0F), GUILayout.Width(300.0F));
        GUILayout.BeginVertical(GUILayout.MinWidth(300.0F));

        exportOption.creatDirectoryByModelName = EditorGUILayout.ToggleLeft("是否自动创建文件夹(存放位置:路径/模型名/模型名.gltf)", exportOption.creatDirectoryByModelName);

        exportOption.convertRightHanded = EditorGUILayout.ToggleLeft("是否转换为右手坐标系（默认否）", exportOption.convertRightHanded, GUILayout.ExpandWidth(true));

        //mExportPBR = EditorGUILayout.Toggle("Export PBR Material", mExportPBR);
        //mExportAnimation = EditorGUILayout.Toggle("Export animation (beta)", mExportAnimation);
        //mConvertImage = EditorGUILayout.Toggle("Convert Images", mConvertImage);
        //mBuildZip = EditorGUILayout.Toggle("Build Zip", mBuildZip);
        //mConvertImage = EditorGUILayout.Toggle("是否网格合并（默认不合）", mConvertImage);
        exportOption.mCombineMesh = EditorGUILayout.ToggleLeft("是否将网格数据合并为一个文件（默认合并）", exportOption.mCombineMesh);

        //mExportPBR = EditorGUILayout.Toggle("Export PBR Material", mExportPBR);
        //mExportAnimation = EditorGUILayout.Toggle("Export animation (beta)", mExportAnimation);
        //mConvertImage = EditorGUILayout.Toggle("Convert Images", mConvertImage);
        //mBuildZip = EditorGUILayout.Toggle("Build Zip", mBuildZip);
        //mExportNormal = EditorGUILayout.Toggle("是否导出法线（默认不导）", mExportNormal);
        exportOption.mNormal = EditorGUILayout.ToggleLeft("是否导出法线（默认不导）", exportOption.mNormal);

        exportOption.mMaxLightmapMultiple = EditorGUILayout.FloatField("光照图可导出的最大亮度倍数", exportOption.mMaxLightmapMultiple, EditorStyles.label, null);

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        GUILayout.Space(SPACE_SIZE);

        bool enable = updateExporterStatus();

        GUILayout.Label("Status", EditorStyles.boldLabel);

        if (enable)
            GUILayout.Label(string.Format("<color=#0F0F0FFF>{0}</color>", mStatus), mStatusStyle);
        else
            GUILayout.Label(string.Format("<color=#F00F0FFF>{0}</color>", mStatus), mStatusStyle);

        if (mResult.Length > 0)
            GUILayout.Label(string.Format("<color=#0F0F0FFF>{0}</color>", mResult), mStatusStyle);


        GUILayout.Space(SPACE_SIZE);
        GUI.enabled = enable;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Export Selection", GUILayout.Width(180), GUILayout.Height(24))) {
            if (!enable) {
                EditorUtility.DisplayDialog("Error", mStatus, "Ok");
            }
            else {
                exportOption.mPath = exportOption.mPrePath;
                if (!Directory.Exists(exportOption.mPath))
                {
                    Directory.CreateDirectory(exportOption.mPath);
                }

                if (exportOption.creatDirectoryByModelName)
                {
                    exportOption.mPath = exportOption.mPrePath + "/" + mParamName;
                    if (!Directory.Exists(exportOption.mPath))
                    {
                        Directory.CreateDirectory(exportOption.mPath);
                    }
                }

				string exportFileName = Path.Combine(exportOption.mPath, mParamName + ".gltf");
                exportOption.mFileName = Path.Combine(exportOption.mPath, mParamName + ".gltf");

                Debug.ClearDeveloperConsole();
                mExporter.ExportCoroutine(exportOption);

                mResult = string.Format("Exort Finished: {0}", exportFileName);
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Banner
        GUILayout.Space(SPACE_SIZE*2);
        GUI.enabled = true;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
		string url = "http://192.168.31.241:8181/docs/game_render/babylonjs_build";
        if (GUILayout.Button("Document: "+ url, EditorStyles.helpBox)) {
            Application.OpenURL(url);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(SPACE_SIZE * 2);
    }

    private bool updateExporterStatus() {
        mStatus = "";

        int nbSelectedObjects = Selection.GetTransforms(SelectionMode.Deep).Length;
        if (nbSelectedObjects == 0) {
            mStatus = "No object selected to export";
            return false;
        }

        if (exportOption.mPrePath.Length == 0) {
            mStatus = "Please set export path";
            return false;
        }

        if (mParamName.Length > NAME_LIMIT) {
            mStatus = "Model name is too long";
            return false;
        }


        if (mParamName.Length == 0) {
            mStatus = "Please give a name to your model";
            return false;
        }


        if (mParamDescription.Length > DESC_LIMIT) {
            mStatus = "Model description is too long";
            return false;
        }


        if (mParamTags.Length > TAGS_LIMIT) {
            mStatus = "Model tags are too long";
            return false;
        }

        mStatus = "Export " + nbSelectedObjects + " object" + (nbSelectedObjects != 1 ? "s" : "");
        return true;
    }

    [MenuItem("Tools/Export to glTF 2.0")]
    static void Init() {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX // edit: added Platform Dependent Compilation - win or osx standalone
        ExporterGLTF20 window = (ExporterGLTF20)EditorWindow.GetWindow(typeof(ExporterGLTF20));
        window.titleContent.text = "GLTF 2.0 Exporter - GC";
        window.Show();
#else // and error dialog if not standalone
		EditorUtility.DisplayDialog("Error", "Your build target must be set to standalone", "Okay");
#endif
    }
}

#endif