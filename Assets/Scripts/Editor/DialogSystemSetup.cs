using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// 对话系统快速设置工具
/// </summary>
public static class DialogSystemSetup
{
    [MenuItem("Tools/对话系统/创建对话管理器")]
    public static void CreateDialogManager()
    {
        // 检查是否已存在DialogManager
        DialogManager existingManager = Object.FindObjectOfType<DialogManager>();
        if (existingManager != null)
        {
            Selection.activeGameObject = existingManager.gameObject;
            EditorGUIUtility.PingObject(existingManager.gameObject);
            Debug.Log("对话管理器已存在！");
            return;
        }
        
        // 创建对话管理器GameObject
        GameObject dialogManagerObj = new GameObject("DialogManager");
        DialogManager dialogManager = dialogManagerObj.AddComponent<DialogManager>();
        DialogUIManager dialogUIManager = dialogManagerObj.AddComponent<DialogUIManager>();
        
        // 设置输入动作引用
        var inputActions = Resources.Load<InputActionAsset>("InputSystem/InputActions");
        if (inputActions != null)
        {
            var playerMap = inputActions.FindActionMap("Player");
            if (playerMap != null)
            {
                var interactAction = playerMap.FindAction("Interact");
                if (interactAction != null)
                {
                    // 这里需要通过SerializedObject来设置InputActionReference
                    SerializedObject serializedManager = new SerializedObject(dialogManager);
                    SerializedProperty interactProperty = serializedManager.FindProperty("interactAction");
                    
                    if (interactProperty != null)
                    {
                        // 创建InputActionReference
                        string actionPath = $"{inputActions.name}/{playerMap.name}/{interactAction.name}";
                        var actionRef = InputActionReference.Create(interactAction);
                        interactProperty.objectReferenceValue = actionRef;
                        serializedManager.ApplyModifiedProperties();
                    }
                }
            }
        }
        
        Selection.activeGameObject = dialogManagerObj;
        
        Debug.Log("对话管理器创建成功！请设置UI组件引用。");
    }
    
    [MenuItem("Tools/对话系统/创建对话UI")]
    public static void CreateDialogUI()
    {
        // 查找Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("场景中没有找到Canvas！请先创建一个Canvas。");
            return;
        }
        
        // 创建对话面板
        GameObject dialoguePanel = CreateDialogPanel(canvas.transform);
        
        // 查找DialogUIManager并设置引用
        DialogUIManager uiManager = Object.FindObjectOfType<DialogUIManager>();
        if (uiManager != null)
        {
            SerializedObject serializedUI = new SerializedObject(uiManager);
            
            // 设置对话面板引用
            SerializedProperty panelProperty = serializedUI.FindProperty("dialogPanel");
            if (panelProperty != null)
            {
                panelProperty.objectReferenceValue = dialoguePanel;
            }
            
            // 设置其他UI组件引用
            Transform characterNameObj = dialoguePanel.transform.Find("ContentArea/CharacterName");
            if (characterNameObj != null)
            {
                SerializedProperty nameProperty = serializedUI.FindProperty("characterNameText");
                if (nameProperty != null)
                {
                    nameProperty.objectReferenceValue = characterNameObj.GetComponent<TextMeshProUGUI>();
                }
            }
            
            Transform dialogTextObj = dialoguePanel.transform.Find("ContentArea/DialogText");
            if (dialogTextObj != null)
            {
                SerializedProperty textProperty = serializedUI.FindProperty("dialogText");
                if (textProperty != null)
                {
                    textProperty.objectReferenceValue = dialogTextObj.GetComponent<TextMeshProUGUI>();
                }
            }
            
            Transform portraitObj = dialoguePanel.transform.Find("ContentArea/Portrait");
            if (portraitObj != null)
            {
                SerializedProperty portraitProperty = serializedUI.FindProperty("characterPortrait");
                if (portraitProperty != null)
                {
                    portraitProperty.objectReferenceValue = portraitObj.GetComponent<Image>();
                }
            }
            
            Transform continueButtonObj = dialoguePanel.transform.Find("ContentArea/ContinueButton");
            if (continueButtonObj != null)
            {
                SerializedProperty buttonProperty = serializedUI.FindProperty("continueButton");
                if (buttonProperty != null)
                {
                    buttonProperty.objectReferenceValue = continueButtonObj.GetComponent<Button>();
                }
            }
            
            serializedUI.ApplyModifiedProperties();
        }
        
        Selection.activeGameObject = dialoguePanel;
        
        Debug.Log("对话UI创建成功！");
    }
    
    [MenuItem("Tools/对话系统/创建对话触发器")]
    public static void CreateDialogTrigger()
    {
        GameObject triggerObj = new GameObject("DialogTrigger");
        
        // 添加组件
        DialogTrigger trigger = triggerObj.AddComponent<DialogTrigger>();
        BoxCollider collider = triggerObj.AddComponent<BoxCollider>();
        
        // 设置碰撞器为触发器
        collider.isTrigger = true;
        collider.size = new Vector3(2f, 2f, 2f);
        
        Selection.activeGameObject = triggerObj;
        
        Debug.Log("对话触发器创建成功！请设置对话序列和触发方式。");
    }
    
    [MenuItem("Assets/Create/对话系统/对话序列")]
    public static void CreateDialogSequence()
    {
        DialogSequence asset = ScriptableObject.CreateInstance<DialogSequence>();
        
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (System.IO.Path.GetExtension(path) != "")
        {
            path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New DialogSequence.asset");
        
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    
    private static GameObject CreateDialogPanel(Transform parent)
    {
        // 创建主面板
        GameObject panel = new GameObject("DialogPanel");
        panel.transform.SetParent(parent, false);
        
        // 添加RectTransform和Image
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        Image panelImage = panel.AddComponent<Image>();
        CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
        
        // 设置面板大小和位置
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(1f, 0.3f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // 设置面板样式
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);
        
        // 创建内容区域
        GameObject contentArea = new GameObject("ContentArea");
        contentArea.transform.SetParent(panel.transform, false);
        
        RectTransform contentRect = contentArea.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(20f, 20f);
        contentRect.offsetMax = new Vector2(-20f, -20f);
        
        // 创建角色头像
        GameObject portrait = new GameObject("Portrait");
        portrait.transform.SetParent(contentArea.transform, false);
        
        RectTransform portraitRect = portrait.AddComponent<RectTransform>();
        Image portraitImage = portrait.AddComponent<Image>();
        
        portraitRect.anchorMin = new Vector2(0f, 0.5f);
        portraitRect.anchorMax = new Vector2(0f, 0.5f);
        portraitRect.sizeDelta = new Vector2(80f, 80f);
        portraitRect.anchoredPosition = new Vector2(50f, 0f);
        
        portraitImage.color = Color.white;
        
        // 创建角色名字
        GameObject characterName = new GameObject("CharacterName");
        characterName.transform.SetParent(contentArea.transform, false);
        
        RectTransform nameRect = characterName.AddComponent<RectTransform>();
        TextMeshProUGUI nameText = characterName.AddComponent<TextMeshProUGUI>();
        
        nameRect.anchorMin = new Vector2(0.15f, 0.7f);
        nameRect.anchorMax = new Vector2(1f, 1f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        nameText.text = "角色名字";
        nameText.fontSize = 18f;
        nameText.color = Color.yellow;
        
        // 创建对话文本
        GameObject dialogueText = new GameObject("DialogText");
        dialogueText.transform.SetParent(contentArea.transform, false);
        
        RectTransform textRect = dialogueText.AddComponent<RectTransform>();
        TextMeshProUGUI dialogueTextComponent = dialogueText.AddComponent<TextMeshProUGUI>();
        
        textRect.anchorMin = new Vector2(0.15f, 0.1f);
        textRect.anchorMax = new Vector2(0.9f, 0.7f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        dialogueTextComponent.text = "对话内容将在这里显示...";
        dialogueTextComponent.fontSize = 16f;
        dialogueTextComponent.color = Color.white;
        
        // 创建继续按钮
        GameObject continueButton = new GameObject("ContinueButton");
        continueButton.transform.SetParent(contentArea.transform, false);
        
        RectTransform buttonRect = continueButton.AddComponent<RectTransform>();
        Image buttonImage = continueButton.AddComponent<Image>();
        Button button = continueButton.AddComponent<Button>();
        
        buttonRect.anchorMin = new Vector2(0.9f, 0.1f);
        buttonRect.anchorMax = new Vector2(0.9f, 0.1f);
        buttonRect.sizeDelta = new Vector2(80f, 30f);
        buttonRect.anchoredPosition = Vector2.zero;
        
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // 按钮文字
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(continueButton.transform, false);
        
        RectTransform buttonTextRect = buttonText.AddComponent<RectTransform>();
        TextMeshProUGUI buttonTextComponent = buttonText.AddComponent<TextMeshProUGUI>();
        
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        buttonTextComponent.text = "继续";
        buttonTextComponent.fontSize = 14f;
        buttonTextComponent.color = Color.white;
        buttonTextComponent.alignment = TextAlignmentOptions.Center;
        
        return panel;
    }
    
    [MenuItem("Tools/对话系统/完整设置")]
    public static void CompleteSetup()
    {
        CreateDialogManager();
        CreateDialogUI();
        
        Debug.Log("对话系统完整设置完成！");
    }
}
