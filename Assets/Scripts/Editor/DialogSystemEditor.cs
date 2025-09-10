using UnityEngine;
using UnityEditor;

/// <summary>
/// 对话系统编辑器工具
/// </summary>
[CustomEditor(typeof(DialogTrigger))]
public class DialogTriggerEditor : Editor
{
    private DialogTrigger trigger;
    
    void OnEnable()
    {
        trigger = (DialogTrigger)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("工具", EditorStyles.boldLabel);
        
        if (GUILayout.Button("重置触发器"))
        {
            trigger.ResetTrigger();
            EditorUtility.SetDirty(trigger);
        }
        
        if (GUILayout.Button("测试触发对话"))
        {
            if (Application.isPlaying)
            {
                trigger.TriggerDialog();
            }
            else
            {
                EditorGUILayout.HelpBox("需要在运行时测试", MessageType.Info);
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("状态信息", EditorStyles.boldLabel);
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("已触发", trigger.HasTriggered());
        
        if (trigger.GetDialogSequence() != null)
        {
            EditorGUILayout.ObjectField("当前对话序列", trigger.GetDialogSequence(), typeof(DialogSequence), false);
        }
        EditorGUI.EndDisabledGroup();
    }
}

/// <summary>
/// 对话序列编辑器
/// </summary>
[CustomEditor(typeof(DialogSequence))]
public class DialogSequenceEditor : Editor
{
    private DialogSequence sequence;
    private Vector2 scrollPosition;
    
    void OnEnable()
    {
        sequence = (DialogSequence)target;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.LabelField("对话序列设置", EditorStyles.boldLabel);
        
        SerializedProperty loopProp = serializedObject.FindProperty("loopSequence");
        SerializedProperty pauseProp = serializedObject.FindProperty("pauseGameDuringDialog");
        SerializedProperty dialogsProp = serializedObject.FindProperty("dialogs");
        
        EditorGUILayout.PropertyField(loopProp);
        EditorGUILayout.PropertyField(pauseProp);
        
        EditorGUILayout.Space();
        
        // 对话列表
        EditorGUILayout.LabelField($"对话列表 ({dialogsProp.arraySize})", EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        for (int i = 0; i < dialogsProp.arraySize; i++)
        {
            SerializedProperty dialogProp = dialogsProp.GetArrayElementAtIndex(i);
            
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField($"对话 {i + 1}", EditorStyles.boldLabel);
            
            SerializedProperty characterNameProp = dialogProp.FindPropertyRelative("characterName");
            SerializedProperty dialogTextProp = dialogProp.FindPropertyRelative("dialogText");
            SerializedProperty voiceClipProp = dialogProp.FindPropertyRelative("voiceClip");
            SerializedProperty textDisplaySpeedProp = dialogProp.FindPropertyRelative("textDisplaySpeed");
            SerializedProperty waitForInputProp = dialogProp.FindPropertyRelative("waitForInput");
            SerializedProperty autoAdvanceDelayProp = dialogProp.FindPropertyRelative("autoAdvanceDelay");
            SerializedProperty characterPortraitProp = dialogProp.FindPropertyRelative("characterPortrait");
            SerializedProperty textColorProp = dialogProp.FindPropertyRelative("textColor");
            
            EditorGUILayout.PropertyField(characterNameProp);
            
            EditorGUILayout.LabelField("对话内容:");
            dialogTextProp.stringValue = EditorGUILayout.TextArea(dialogTextProp.stringValue, GUILayout.MinHeight(60));
            
            EditorGUILayout.PropertyField(voiceClipProp);
            EditorGUILayout.PropertyField(characterPortraitProp);
            EditorGUILayout.PropertyField(textColorProp);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("显示设置", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(textDisplaySpeedProp);
            EditorGUILayout.PropertyField(waitForInputProp);
            
            if (!waitForInputProp.boolValue)
            {
                EditorGUILayout.PropertyField(autoAdvanceDelayProp);
            }
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("向上移动", GUILayout.Width(80)))
            {
                if (i > 0)
                {
                    dialogsProp.MoveArrayElement(i, i - 1);
                }
            }
            
            if (GUILayout.Button("向下移动", GUILayout.Width(80)))
            {
                if (i < dialogsProp.arraySize - 1)
                {
                    dialogsProp.MoveArrayElement(i, i + 1);
                }
            }
            
            if (GUILayout.Button("复制", GUILayout.Width(60)))
            {
                dialogsProp.InsertArrayElementAtIndex(i);
            }
            
            if (GUILayout.Button("删除", GUILayout.Width(60)))
            {
                dialogsProp.DeleteArrayElementAtIndex(i);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加对话"))
        {
            dialogsProp.arraySize++;
            SerializedProperty newDialog = dialogsProp.GetArrayElementAtIndex(dialogsProp.arraySize - 1);
            
            // 设置默认值
            newDialog.FindPropertyRelative("characterName").stringValue = "";
            newDialog.FindPropertyRelative("dialogText").stringValue = "在这里输入对话内容...";
            newDialog.FindPropertyRelative("textDisplaySpeed").floatValue = 0.05f;
            newDialog.FindPropertyRelative("waitForInput").boolValue = true;
            newDialog.FindPropertyRelative("autoAdvanceDelay").floatValue = 3f;
            newDialog.FindPropertyRelative("textColor").colorValue = Color.white;
        }
        
        if (GUILayout.Button("清空所有对话"))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要删除所有对话吗？此操作无法撤销。", "确定", "取消"))
            {
                dialogsProp.arraySize = 0;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
        
        // 保存提示
        if (GUI.changed)
        {
            EditorUtility.SetDirty(sequence);
        }
    }
}

/// <summary>
/// 对话管理器编辑器
/// </summary>
[CustomEditor(typeof(DialogManager))]
public class DialogManagerEditor : Editor
{
    private DialogManager dialogManager;
    
    void OnEnable()
    {
        dialogManager = (DialogManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (!Application.isPlaying)
        {
            return;
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("运行时控制", EditorStyles.boldLabel);
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("对话活跃", dialogManager.IsDialogActive());
        
        var progress = dialogManager.GetDialogProgress();
        EditorGUILayout.LabelField($"对话进度", $"{progress.current}/{progress.total}");
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("前进对话"))
        {
            dialogManager.AdvanceDialog();
        }
        
        if (GUILayout.Button("跳过对话"))
        {
            dialogManager.SkipDialog();
        }
        
        if (GUILayout.Button("结束对话"))
        {
            dialogManager.EndDialog();
        }
        
        EditorGUILayout.EndHorizontal();
    }
}
