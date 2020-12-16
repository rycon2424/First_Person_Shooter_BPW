using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
public class SimpleTools : Editor
{
    
}

public class PlayerPrefsEditor : EditorWindow
{
	[MenuItem("Simple Tools/Edit PlayerPref")]
	public static void openWindow()
	{
		PlayerPrefsEditor window = (PlayerPrefsEditor)EditorWindow.GetWindow(typeof(PlayerPrefsEditor));
		window.titleContent = new GUIContent("Player Prefs");
		WriteString();
		window.Show();
	}

	public enum FieldType { String, Integer, Float }

	private FieldType fieldType = FieldType.String;
	private string setKey = "";
	private string setVal = "";
	private string error = null;
	private string info = "Information will be displayed here!";
	private string currentSelectedKey = "";

	void OnGUI()
	{
		EditorGUILayout.LabelField("Player Prefs Editor", EditorStyles.boldLabel);
		EditorGUILayout.Separator();

		fieldType = (FieldType)EditorGUILayout.EnumPopup("Key Type", fieldType);
		setKey = EditorGUILayout.TextField("Key Name", setKey);
		setVal = EditorGUILayout.TextField("Value to Set", setVal);

		if (GUILayout.Button("Save Key"))
		{
			if (fieldType == FieldType.Integer)
			{
				int result;
				if (!int.TryParse(setVal, out result))
				{
					error = "Invalid Set Key \"" + setVal + "\" needs to be an integer";
					return;
				}
				if (setVal == "" || setKey == "")
				{
					error = "You can't save 'nothing/empty'";
					return;
				}
				if (AddText("Name:  '" + setKey + "'    Value:  '" + result + "'", setKey) == true)
				{
					PlayerPrefs.SetInt(setKey, result);
					info = "The key " + setKey + " has been saved with the value " + result;
					error = null;
				}
				else
				{
					error = "Simple Tools currently doesnt support duplicate names, we recommend removing and adding the value again.";
				}
			}
			else if (fieldType == FieldType.Float)
			{
				float result;
				if (!float.TryParse(setVal, out result))
				{
					error = "Invalid Set Key \"" + setVal + "\" needs to be a float";
					return;
				}
				if (setVal == "" || setKey == "")
				{
					error = "You can't save 'nothing/empty'";
					return;
				}
				if (AddText("Name:  '" + setKey + "'    Value:  '" + result + "'", setKey) == true)
				{
					PlayerPrefs.SetFloat(setKey, result);
					info = "The key " + setKey + " has been saved with the value " + result;
					error = null;
				}
				else
				{
					error = "Simple Tools currently doesnt support duplicate names, we recommend removing and adding the value again.";
				}
			}
			else
			{
				if (setVal == "" || setKey == "")
				{
					error = "You can't save 'nothing/empty'";
					return;
				}
				if (AddText("Name:  '" + setKey + "'    Value:  '" + setVal + "'", setKey) == true)
				{
					PlayerPrefs.SetString(setKey, setVal);
					info = "The key " + setKey + " has been saved with the value " + setVal;
					error = null;
				}
				else
				{
					error = "Simple Tools currently doesnt support duplicate names, we recommend removing and adding the value again.";
				}
			}
			PlayerPrefs.Save();
		}

		if (GUILayout.Button("Get Key"))
		{
			if (fieldType == FieldType.Integer)
			{
				setVal = PlayerPrefs.GetInt(setKey).ToString();
			}
			else if (fieldType == FieldType.Float)
			{
				setVal = PlayerPrefs.GetFloat(setKey).ToString();
			}
			else
			{
				setVal = PlayerPrefs.GetString(setKey);
			}
			if (!PlayerPrefs.HasKey(setKey) || setKey == "")
			{
				if (setKey != "")
				{
					error = ("No such key exists!");
				}
				else
				{
					error = ("You can't look up nothing/empty!");
				}
				currentSelectedKey = "";
			}
			else
			{
				info = ("Key found! named: " + setKey + " with the value: " + setVal);
				currentSelectedKey = setKey;
				error = null;
			}
		}
		if (error == null)
		{
			EditorGUILayout.HelpBox(info, MessageType.Info);
		}
		if (error != null)
		{
			EditorGUILayout.HelpBox(error, MessageType.Error);
		}

		GUILayout.Space(20);
		ReadString();
		if (ReadString().Length > 0)
		{
			foreach (string s in ReadString())
			{
				EditorGUILayout.LabelField(s, EditorStyles.boldLabel);
			}
		}
		else
		{
			EditorGUILayout.LabelField("PlayerPrefs is Empty", EditorStyles.boldLabel);
		}

		GUILayout.Space(20);
		EditorGUILayout.LabelField("DANGERZONE", EditorStyles.boldLabel);
		GUILayout.Space(5);
		GUI.color = Color.yellow;
		if (currentSelectedKey != "")
		{
			EditorGUILayout.LabelField("Current selected key = " + currentSelectedKey, EditorStyles.boldLabel);
			if (GUILayout.Button("Delete Key"))
			{
				if (!PlayerPrefs.HasKey(setKey))
				{
					error = ("The key never existed nothing has been removed.");
				}
				else
				{
					info = ("The key: " + setKey + " has been removed.");
					currentSelectedKey = "";
					RemoveText(setKey);
					PlayerPrefs.DeleteKey(setKey);
					PlayerPrefs.Save();
				}
			}
		}

		GUI.color = Color.red;
		GUILayout.Space(10);
		if (GUILayout.Button("Delete ALL values from playerpref"))
		{
			ClearPlayerPrefs();
		}
	}

	public static void ClearPlayerPrefs()
	{
		bool confirm = EditorUtility.DisplayDialog(
			"Delete All Player Prefs",
			"Are you sure you want to delete all PlayerPrefs?",
			"Confirm/Yes",
			"Cancel");
		if (confirm)
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
			File.Delete(path);
			WriteString();
			EditorUtility.DisplayDialog(
				"Delete All Player Prefs",
				"The PlayerPref values have been deleted. \n \n State: Succesfull",
				"Close");
		}
	}

	private static string path;
	static void WriteString()
	{
		var folder = Directory.CreateDirectory(Application.dataPath + "/Editor");
		path = Application.dataPath + "/Editor/playerprefinfo.txt";

		if (!File.Exists(path))
		{
			File.WriteAllText(path, "PlayerPref info \n \n");
			Debug.Log("Added info at " + path);
			AssetDatabase.Refresh();
		}
	}

	static bool AddText(string infoToAdd, string keyName)
	{
		List<string> savedPrefs = File.ReadAllLines(path).ToList();
		for (int s = 0; s < savedPrefs.Count; s++)
		{
			if (savedPrefs[s].Contains(keyName))
			{
				return false;
			}
		}
		string content = infoToAdd + "\n";
		File.AppendAllText(path, content);
		return true;
	}

	static void RemoveText(string key)
	{
		List<string> savedPrefs = File.ReadAllLines(path).ToList();
		for (int s = 0; s < savedPrefs.Count; s++)
		{
			if (savedPrefs[s].Contains(key))
			{
				savedPrefs.RemoveAt(s);
			}
		}
		File.WriteAllLines(path, savedPrefs.ToArray());
	}

	static void RefreshDataBase()
	{
		AssetDatabase.Refresh();
	}

	static string[] ReadString()
	{
		string path = "Assets/Editor/playerprefinfo.txt";

		string[] allLines = File.ReadAllLines(path);
		return allLines;
	}

}


public class AddScenesToBuild : EditorWindow
{
	List<SceneAsset> m_SceneAssets = new List<SceneAsset>();

	// Add menu item named "Example Window" to the Window menu
	[MenuItem("Simple Tools/Add Scene(s) To Build")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(AddScenesToBuild));
	}

	void OnGUI()
	{
		GUILayout.Label("Scenes to include in build:", EditorStyles.boldLabel);
		GUILayout.Space(15);
		for (int i = 0; i < m_SceneAssets.Count; ++i)
		{
			m_SceneAssets[i] = (SceneAsset)EditorGUILayout.ObjectField(m_SceneAssets[i], typeof(SceneAsset), false);
		}
		GUILayout.Space(20);
		GUI.color = Color.green;
		if (GUILayout.Button("Add"))
		{
			m_SceneAssets.Add(null);
		}
		if (m_SceneAssets.Count > 0)
		{
			GUILayout.Space(20);
			GUI.color = Color.red;
			if (GUILayout.Button("Remove"))
			{
				m_SceneAssets.Remove(m_SceneAssets[m_SceneAssets.Count - 1]);
			}
		}
		GUI.color = Color.white;

		GUILayout.Space(40);
		if (m_SceneAssets.Count > 0)
		{
			if (GUILayout.Button("Apply To Build Settings"))
			{
				Debug.Log("De build scenes have been changed.");
				SetEditorBuildSettingsScenes();
			}
		}
		GUILayout.Space(25);
		GUILayout.Label("New scenes in the build settings after apply:", EditorStyles.label);
		string currentScenes = "";

		for (int i = 0; i < m_SceneAssets.Count; ++i)
		{
			if (m_SceneAssets[i] != null)
			{
				currentScenes += m_SceneAssets[i].name + "\n";
			}
		}
		GUILayout.Space(10);
		GUILayout.Label(currentScenes, EditorStyles.boldLabel);
	}

	public void SetEditorBuildSettingsScenes()
	{
		// Find valid Scene paths and make a list of EditorBuildSettingsScene
		List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
		foreach (var sceneAsset in m_SceneAssets)
		{
			string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
			if (!string.IsNullOrEmpty(scenePath))
				editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
		}

		// Set the Build Settings window Scene list
		EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
	}
}

public class AutoSave : EditorWindow
{
	private DateTime lastSaveTimeScene = DateTime.Now;

	public const string menuname = "Simple Tools/AutoSave/Toggle";

	private string projectPath;
	private string scenePath;

	public static int saveTime = 120;

    private static bool enabled_;
    /// Called on load thanks to the InitializeOnLoad attribute
    static AutoSave()
    {
        /*EditorApplication.delayCall += () => {
            PerformAction(!AutoSave.enabled_);
        };*/
    }

    [MenuItem("Simple Tools/AutoSave/Settings")]
    static void ShowWindow()
	{
		EditorWindow window = GetWindow(typeof(AutoSave));
		window.Show();
	}

	[MenuItem(menuname)]
	static void Use()
	{
		PerformAction(!AutoSave.enabled_);
	}

	private void OnEnable()
    {
        AutoSave.enabled_ = EditorPrefs.GetBool(AutoSave.menuname, false);
		projectPath = Application.dataPath;
	}

    void Update()
	{
		if (EditorPrefs.GetBool(AutoSave.menuname, true))
		{
			var difference = DateTime.Now - lastSaveTimeScene;
			if (difference.TotalSeconds > saveTime)
			{
				Debug.ClearDeveloperConsole();
				saveScene();
				lastSaveTimeScene = DateTime.Now;
			}
			if (warning)
			{
				if (difference.TotalSeconds > (saveTime - 10))
				{
					Debug.Log("Autosave incoming in " + Mathf.RoundToInt(saveTime - (float)difference.TotalSeconds/* - saveTime*/));
				}
			}
			if (EditorApplication.isPlayingOrWillChangePlaymode && EditorPrefs.GetBool(AutoSave.menuname, true))
			{
				Debug.Log("Entering Play Mode, Disabling autosave");
				PerformAction(false);
			}
        }
	}

	void saveScene()
	{
		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		if (showDebug)
		{
			Debug.Log("AutoSave saved on : " + lastSaveTimeScene);
		}
		AutoSave repaintSaveWindow = (AutoSave)EditorWindow.GetWindow(typeof(AutoSave));
		repaintSaveWindow.Repaint();
	}

    public static void PerformAction(bool enabled)
    {
        /// Set checkmark on menu item
        Menu.SetChecked(AutoSave.menuname, enabled);
        /// Saving editor state
        EditorPrefs.SetBool(AutoSave.menuname, enabled);

        AutoSave.enabled_ = enabled;

		/// Perform your logic here...
		if (EditorPrefs.GetBool(AutoSave.menuname, true))
		{
			Debug.Log("AutoSave has been enabled!");
			Debug.Log("AutoSave will save every " + AutoSave.saveTime.ToString() + " seconds!");
		}
		else
		{
			Debug.Log("AutoSave has been disabled!");
		}
	}
	static int timeSave;
	static bool showDebug;
	static bool warning;
	string buttonText;
	void OnGUI()
	{
		GUILayout.Label("AutoSave Settings:", EditorStyles.boldLabel);

		GUI.color = Color.cyan;

		timeSave = EditorGUILayout.IntField("Seconds between Save:", timeSave);

		string extraInfo = "(AutoSave is Disabled)";
		if (EditorPrefs.GetBool(AutoSave.menuname, true))
		{
			extraInfo = "(AutoSave is Enabled)";
		}
		if (timeSave < 10)
		{
			GUI.color = Color.red;
			buttonText = "Save time is too short";
		}
		else
		{
			GUI.color = Color.green;
			showDebug = EditorGUILayout.Toggle(new GUIContent("Display debug save:", "Everytime it saves it displays the time when it was saved in console"), showDebug);
			warning = EditorGUILayout.Toggle(new GUIContent("10 second save warning:", "Show a 10 second countdown in the console before the save"), warning);
			buttonText = "Save Settings   -   " + extraInfo;
		}

		if (GUILayout.Button(buttonText))
		{
			if (timeSave < 10)
			{
				Debug.Log("Too low of a number (minimal timer recommended = 10");
				return;
			}
			saveTime = timeSave;
			Debug.Log("Autosave has been configured to save every " + saveTime + " seconds!");
		}
	}
}

public class ObjExporterScript
{
	private static int StartIndex = 0;

	public static void Start()
	{
		StartIndex = 0;
	}
	public static void End()
	{
		StartIndex = 0;
	}

	public static string MeshToString(MeshFilter mf, Transform t)
	{
		Vector3 s = t.localScale;
		Vector3 p = t.localPosition;
		Quaternion r = t.localRotation;

		int numVertices = 0;
		Mesh m = mf.sharedMesh;
		if (!m)
		{
			return "####Error####";
		}
		Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

		StringBuilder sb = new StringBuilder();

		foreach (Vector3 vv in m.vertices)
		{
			Vector3 v = t.TransformPoint(vv);
			numVertices++;
			sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
		}
		sb.Append("\n");
		foreach (Vector3 nn in m.normals)
		{
			Vector3 v = r * nn;
			sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
		}
		sb.Append("\n");
		foreach (Vector3 v in m.uv)
		{
			sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
		}
		for (int material = 0; material < m.subMeshCount; material++)
		{
			sb.Append("\n");
			sb.Append("usemtl ").Append(mats[material].name).Append("\n");
			sb.Append("usemap ").Append(mats[material].name).Append("\n");

			int[] triangles = m.GetTriangles(material);
			for (int i = 0; i < triangles.Length; i += 3)
			{
				sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
					triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
			}
		}

		StartIndex += numVertices;
		return sb.ToString();
	}
}
public class ObjExporter : ScriptableObject
{
	[MenuItem("Simple Tools/Export/OBJ")]
	static void DoExportWSubmeshes()
	{
		DoExport(true);
	}

	[MenuItem("Simple Tools/Export/OBJ (No Submeshes)")]
	static void DoExportWOSubmeshes()
	{
		DoExport(false);
	}

	static void DoExport(bool makeSubmeshes)
	{
		if (Selection.gameObjects.Length == 0)
		{
			Debug.Log("Didn't Export Any Meshes; Nothing was selected!");
			return;
		}

		string meshName = Selection.gameObjects[0].name;
		string fileName = EditorUtility.SaveFilePanel("Export .obj file", "", meshName, "obj");

		ObjExporterScript.Start();

		StringBuilder meshString = new StringBuilder();

		meshString.Append("#" + meshName + ".obj"
							+ "\n#" + System.DateTime.Now.ToLongDateString()
							+ "\n#" + System.DateTime.Now.ToLongTimeString()
							+ "\n#-------"
							+ "\n\n");

		Transform t = Selection.gameObjects[0].transform;

		Vector3 originalPosition = t.position;
		t.position = Vector3.zero;

		if (!makeSubmeshes)
		{
			meshString.Append("g ").Append(t.name).Append("\n");
		}
		meshString.Append(processTransform(t, makeSubmeshes));

		WriteToFile(meshString.ToString(), fileName);

		t.position = originalPosition;

		ObjExporterScript.End();
		Debug.Log("Exported Mesh: " + fileName);
	}

	static string processTransform(Transform t, bool makeSubmeshes)
	{
		StringBuilder meshString = new StringBuilder();

		meshString.Append("#" + t.name
						+ "\n#-------"
						+ "\n");

		if (makeSubmeshes)
		{
			meshString.Append("g ").Append(t.name).Append("\n");
		}

		MeshFilter mf = t.GetComponent<MeshFilter>();
		if (mf)
		{
			meshString.Append(ObjExporterScript.MeshToString(mf, t));
		}

		for (int i = 0; i < t.childCount; i++)
		{
			meshString.Append(processTransform(t.GetChild(i), makeSubmeshes));
		}

		return meshString.ToString();
	}

	static void WriteToFile(string s, string filename)
	{
		using (StreamWriter sw = new StreamWriter(filename))
		{
			sw.Write(s);
		}
	}
}

public class Recompile : EditorWindow
{

	[MenuItem("Simple Tools/Recompile all scripts (fix miscellaneous files)")]
	static void Init()
	{
		Debug.Log("Recompile function called!, Recompiling...");
		RecompileUnityEditor();
	}

	public static void RecompileUnityEditor()
	{

		BuildTargetGroup target = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
		string rawSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

		PlayerSettings.SetScriptingDefineSymbolsForGroup(target, rawSymbols + "a");
		PlayerSettings.SetScriptingDefineSymbolsForGroup(target, rawSymbols);
	}

}

public class QuickWindows : EditorWindow
{

	[MenuItem("Simple Tools/QuickWindows/Open Animator+Animation")]
	static void OpenAnimations()
	{
		EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
		EditorApplication.ExecuteMenuItem("Window/Animation/Animator");
	}
}

public class WorkWithSelection : EditorWindow
{

	/// <summary>
	/// Checks if a transform is selected, if not then returns false
	/// </summary>
	[MenuItem("Simple Tools/Selection/GameObject Look at Camera", true)]
	[MenuItem("Simple Tools/Selection/Remove All Components", true)]
	static bool ValidateSelection()
	{
		return Selection.transforms.Length != 0;
	}

    #region look to camera

    [MenuItem("Simple Tools/Selection/GameObject Look at Camera")]
	static void Look()
	{
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		var camera = Camera.main;

		if (camera)
		{
			foreach (Transform transform in Selection.transforms)
			{
				Undo.RegisterCompleteObjectUndo(transform, transform.name);

				transform.LookAt(camera.transform);
			}
		}
	}

	[MenuItem("Simple Tools/Selection/Remove All Components")]
	static void RemoveAllComponents()
	{
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		foreach (Transform transform in Selection.transforms)
		{
			foreach (var comp in transform.gameObject.GetComponents<Component>())
			{
				if (!(comp is Transform))
				{
					Undo.DestroyObjectImmediate(comp);
					DestroyImmediate(comp);
				}
			}
		}
	}

    #endregion

    #region paste mult components
    [MenuItem("Simple Tools/Paste Multiple Components (NO UNDO)")]
	static void PasteNew()
	{
		EditorWindow window = GetWindow(typeof(WorkWithSelection));
		window.Show();
	}

	List<GameObject> gameobjectsList = new List<GameObject>();
	private GameObject objectToCopyFrom;

	public List<Component> components;

	private void OnGUI()
	{
		GUILayout.Space(15);
		GUILayout.Label("Object To Copy From:", EditorStyles.boldLabel);
		objectToCopyFrom = (GameObject)EditorGUILayout.ObjectField(objectToCopyFrom, typeof(GameObject), true);
		if (objectToCopyFrom == null)
		{
			return;
		}
		GUILayout.Label("Objects to receive all components from above:", EditorStyles.boldLabel);
		GUILayout.Space(15);
		for (int i = 0; i < gameobjectsList.Count; ++i)
		{
			gameobjectsList[i] = (GameObject)EditorGUILayout.ObjectField(gameobjectsList[i], typeof(GameObject), true);
		}
		GUILayout.Space(20);
		GUI.color = Color.green;
		if (GUILayout.Button("Add"))
		{
			gameobjectsList.Add(null);
		}
		if (gameobjectsList.Count > 0)
		{
			GUILayout.Space(10);
			GUI.color = Color.red;
			if (GUILayout.Button("Remove"))
			{
				gameobjectsList.Remove(gameobjectsList[gameobjectsList.Count - 1]);
				return;
			}
			if (gameobjectsList[0] != null)
			{
				GUILayout.Space(30);
				GUI.color = Color.white;
				if (GUILayout.Button("Paste Components as new"))
				{
					components = new List<Component>();
					components.AddRange(objectToCopyFrom.GetComponents<Component>());
					for (int i = 0; i < gameobjectsList.Count; ++i)
					{
						EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
						foreach (Component c in components)
						{
							if (c.GetType() != typeof(Transform))
							{
								gameobjectsList[i].AddComponent(c.GetType());
							}
						}
						Debug.Log("Components have succesfully been added to " + gameobjectsList[i].name);
					}
					components = new List<Component>();
				}
			}
		}
	}
	#endregion

	#region material check
	[MenuItem("Simple Tools/Selection/Check Material in Folder")]
	private static void CheckMaterial()
	{
		Material matToCheck = Selection.activeObject as Material;
		int amountChecked = 0;
		int amountInScene = 0;
		bool exists = false;
		foreach (var renderer in FindObjectsOfType<MeshRenderer>())
		{
			amountChecked++;
			if (renderer.sharedMaterials.Contains(matToCheck))
			{
				Debug.Log("Material used by " + renderer.transform.name, renderer.gameObject);
				amountInScene++;
				exists = true;
			}
		}
		if (exists)
		{
			Debug.Log("There are " + amountInScene + " of this material in current scene.");
		}
		else
		{
			Debug.Log("There is no type of this material in current scene.");
		}
		Debug.Log("Checked " + amountChecked + " renderers.");
	}

	[MenuItem("Simple Tools/Selection/Check Material in Folder", true)]
	private static bool CheckMaterialValidation()
	{
		return Selection.activeObject is Material;
	}
    #endregion

}

public class EasyDuplicate : EditorWindow
{
	static int amountToDuplicate;
	static GameObject gameToDupe;
	[MenuItem("Simple Tools/Selection/DuplicateWizard (NO UNDO)", false)]
	private static void PasteXTimes()
	{
		gameToDupe = Selection.activeObject as GameObject;
		EditorWindow window = GetWindow(typeof(EasyDuplicate));
		window.Show();
	}
	[MenuItem("Simple Tools/Selection/DuplicateWizard (NO UNDO)", true)]
	private static bool CheckGameObjectValidation()
	{
		return Selection.transforms.Length != 0;
	}

	private void OnGUI()
	{
		if (CheckGameObjectValidation())
		{
			GUILayout.Label("How many times u want to paste the selection:", EditorStyles.boldLabel);
		}
		else
		{
			GUILayout.Label("Select a GameObject!", EditorStyles.boldLabel);
		}
		GUILayout.Space(10);
		if (CheckGameObjectValidation())
		{
			amountToDuplicate = EditorGUILayout.IntField("Amount to Dupe:", amountToDuplicate);
			GUILayout.Space(15);
			if (amountToDuplicate > 0 && amountToDuplicate < 10000)
			{
				GUI.color = Color.green;
				if (GUILayout.Button("Paste " + amountToDuplicate + " times"))
				{
					PasteAmount();
				}
			}
			else if (amountToDuplicate > 9999)
			{
				GUI.color = Color.yellow;
				if (GUILayout.Button(amountToDuplicate + " is a high number and MAY cause CRASHES!"))
				{
					PasteAmount();
				}
			}
			else
			{
				GUI.color = Color.red;
				if (GUILayout.Button("Cannot paste 0 times"))
				{
					Debug.Log("Amount to dupe must be higher then 0!");
				}
			}
		}
	}

	private void PasteAmount()
	{
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		foreach (Transform transform in Selection.transforms)
		{
			//Undo.RegisterCompleteObjectUndo(transform, transform.name);
			UnityEngine.Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(transform);
			for (int i = 0; i < amountToDuplicate; i++)
			{
				if (prefabRoot != null)
				{
					PrefabUtility.InstantiatePrefab(prefabRoot);
				}
				else
				{
					Instantiate(transform);
				}
			}
		}
	}
}

public class SingleTonTool : Editor
{
	public static GameObject currentSelectedObject;
	[MenuItem("Simple Tools/Singleton/Add SingleTon to Object")]
	private static void AddSingleton()
	{
		currentSelectedObject = Selection.activeObject as GameObject;

		pathSingleton = Application.dataPath + "/SingleTon.cs";
		if (!File.Exists(pathSingleton))
		{
			Error();
			return;
		}
		currentSelectedObject.AddComponentExt("SingleTon");
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
	}


	private static void Error()
	{
		Debug.Log("Create a SingleTon before adding one!");
		Debug.Log("Creating SingleTon...");
		CreateSingleTon();
	}

	[MenuItem("Simple Tools/Singleton/Add SingleTon to Object", true)]
	private static bool CheckGameObject()
	{
		return Selection.activeObject is GameObject;
	}

	private static string pathSingleton;
	[MenuItem("Simple Tools/Singleton/Create SingleTon Script")]
	private static void CreateSingleTon()
	{
		pathSingleton = Application.dataPath + "/SingleTon.cs";

		if (!File.Exists(pathSingleton))
		{
			File.WriteAllText(pathSingleton, "using UnityEngine;public class SingleTon : MonoBehaviour{public static SingleTon instance;private void Awake(){if (instance != null){Destroy(this);}instance = this; DontDestroyOnLoad(this.gameObject);}}");
			Debug.Log("Added singleton script at " + pathSingleton);
			AssetDatabase.Refresh();
		}
	}

}

public class Credits : Editor
{
	[MenuItem("Simple Tools/ ", false)]
	private static void Space()
	{

	}

	[MenuItem("\n Simple Tools/Made By Bosko Ivkovic", true)]
	[MenuItem("Simple Tools/ ", true)]
	private static bool DisableInteraction()
	{
		return false;
	}

	[MenuItem("Simple Tools/Made By Bosko Ivkovic", false)]
	private static void MadeBy()
	{

	}
}

public static class ExtensionMethod
{
	public static Component AddComponentExt(this GameObject obj, string scriptName)
	{
		Component cmpnt = null;


		for (int i = 0; i < 10; i++)
		{
			//If call is null, make another call
			cmpnt = _AddComponentExt(obj, scriptName, i);

			//Exit if we are successful
			if (cmpnt != null)
			{
				break;
			}
		}


		//If still null then let user know an exception
		if (cmpnt == null)
		{
			Debug.LogError("Failed to Add Component");
			return null;
		}
		return cmpnt;
	}

	private static Component _AddComponentExt(GameObject obj, string className, int trials)
	{
		//Any script created by user(you)
		const string userMadeScript = "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
		//Any script/component that comes with Unity such as "Rigidbody"
		const string builtInScript = "UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

		//Any script/component that comes with Unity such as "Image"
		const string builtInScriptUI = "UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

		//Any script/component that comes with Unity such as "Networking"
		const string builtInScriptNetwork = "UnityEngine.Networking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

		//Any script/component that comes with Unity such as "AnalyticsTracker"
		const string builtInScriptAnalytics = "UnityEngine.Analytics, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

		//Any script/component that comes with Unity such as "AnalyticsTracker"
		const string builtInScriptHoloLens = "UnityEngine.HoloLens, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

		Assembly asm = null;

		try
		{
			//Decide if to get user script or built-in component
			switch (trials)
			{
				case 0:

					asm = Assembly.Load(userMadeScript);
					break;

				case 1:
					//Get UnityEngine.Component Typical component format
					className = "UnityEngine." + className;
					asm = Assembly.Load(builtInScript);
					break;
				case 2:
					//Get UnityEngine.Component UI format
					className = "UnityEngine.UI." + className;
					asm = Assembly.Load(builtInScriptUI);
					break;

				case 3:
					//Get UnityEngine.Component Video format
					className = "UnityEngine.Video." + className;
					asm = Assembly.Load(builtInScript);
					break;

				case 4:
					//Get UnityEngine.Component Networking format
					className = "UnityEngine.Networking." + className;
					asm = Assembly.Load(builtInScriptNetwork);
					break;
				case 5:
					//Get UnityEngine.Component Analytics format
					className = "UnityEngine.Analytics." + className;
					asm = Assembly.Load(builtInScriptAnalytics);
					break;

				case 6:
					//Get UnityEngine.Component EventSystems format
					className = "UnityEngine.EventSystems." + className;
					asm = Assembly.Load(builtInScriptUI);
					break;

				case 7:
					//Get UnityEngine.Component Audio format
					className = "UnityEngine.Audio." + className;
					asm = Assembly.Load(builtInScriptHoloLens);
					break;

				case 8:
					//Get UnityEngine.Component SpatialMapping format
					className = "UnityEngine.VR.WSA." + className;
					asm = Assembly.Load(builtInScriptHoloLens);
					break;

				case 9:
					//Get UnityEngine.Component AI format
					className = "UnityEngine.AI." + className;
					asm = Assembly.Load(builtInScript);
					break;
			}
		}
		catch (Exception e)
		{
			Debug.Log("Failed to Load Assembly" + e.Message);
		}

		//Return if Assembly is null
		if (asm == null)
		{
			return null;
		}

		//Get type then return if it is null
		Type type = asm.GetType(className);
		if (type == null)
			return null;

		//Finally Add component since nothing is null
		Component cmpnt = obj.AddComponent(type);
		return cmpnt;
	}
}