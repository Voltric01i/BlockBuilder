using UnityEngine;
using System;
using System.Collections;

public class AppControllerForLoadMap : AppControllerBase
{
    [SerializeField]
    protected SmartARController smartARController;

    private const string SCENE_MAP_FILE_NAME = "/scenemap.dat";

    private string _SceneMapFilePath;
    private bool _IsFix;
    private bool _IsSceneMapLoaded = false;

    private bool _DoSaveSceneMap = false;
    private bool _DoLoadSceneMap = false;
    private bool _DoReset = false;
    private bool _DoClearMap = false;
    private bool _DoSwitchFixSceneMap = false;

    void Start()
    {
        _SceneMapFilePath = Application.persistentDataPath + SCENE_MAP_FILE_NAME;
    }

    void Update()
    {
        // Escape key is application quit.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (smartARController.smart_.isConstructorFailed()) { return; }
        if (smartARController.targets_ == null) { return; }

        // For Frame
        foreach (var target in smartARController.targets_)
        {
            foreach (var effector in effectors_)
            {
                if (effector.result_.isRecognized_)
                {
                    if (effector.targetID == null)
                    {
                        var child = effector.transform.Find("Frame");
                        if (child == null) { return; }
                        child.localScale = new Vector3(300f, 150f, 300f);
                    }
                    else if (effector.result_.target_ == target.self_ && !smartARController.isLoadSceneMap_)
                    {
                        var size = new smartar.Vector2();
                        target.GetPhysicalSize(out size);
                        var child = effector.transform.Find("Frame");
                        var nearClipPlane = Camera.main.nearClipPlane * 1000;
                        if (child == null) { return; }
                        child.localScale = new Vector3(size.x_ * nearClipPlane, 150f, size.y_ * nearClipPlane);
                    }
                }
            }
        }
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        smartCheckError(smartARController);

        if (smartARController.smart_.isConstructorFailed()) { return; }

        // Do action
        if (_DoSaveSceneMap)
        {
            var fileStreamOut = new smartar.FileStreamOut(smartARController.smart_, _SceneMapFilePath);
            smartARController.saveSceneMap(fileStreamOut);
            fileStreamOut.Close();
            fileStreamOut = null;
            _DoSaveSceneMap = false;
            return;
        }
        else if (_DoLoadSceneMap)
        {
            smartARController.loadSceneMap(_SceneMapFilePath);
            _IsFix = true;
            _IsSceneMapLoaded = true;
            _DoLoadSceneMap = false;
            return;
        }
        else if (_DoReset)
        {
            smartARController.resetController();
            _DoReset = false;
            return;
        }
        else if (_DoClearMap)
        {
            smartARController.reCreateController(smartARController.orgRecognizerSettings_.recognitionMode, smartARController.orgRecognizerSettings_.sceneMappingInitMode);
            _IsSceneMapLoaded = false;
            _DoClearMap = false;
            return;
        }
        else if (_DoSwitchFixSceneMap)
        {
            _IsFix = !_IsFix;
            smartARController.recognizer_.FixSceneMap(_IsFix);
            _DoSwitchFixSceneMap = false;
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        mySkin.button.fontSize = 15;
        mySkin.label.fontSize = 15;
#endif

        var buttonLongSide = Math.Max((float)Screen.width * 0.1f, (float)Screen.height * 0.1f);
        var buttonNarrowSide = Math.Min((float)Screen.width * 0.1f, (float)Screen.height * 0.1f);

        if (GUI.Button(new Rect(0, Screen.height - buttonNarrowSide, buttonLongSide, buttonNarrowSide), "Reset"))
        {
            _DoReset = true;
        }

        var width = buttonLongSide * 5 / 2;
        var height = buttonNarrowSide;

		if (_IsSceneMapLoaded)
		{
			if (GUI.Button(new Rect(Screen.width - width, Screen.height - height, width, height), "Clear scene map"))
			{
                _DoClearMap = true;
            }
        }

		if (GUI.Button(new Rect(Screen.width - width, 0, width, height), "SaveSceneMap"))
        {
            _DoSaveSceneMap = true;
        }

        if (GUI.Button(new Rect(Screen.width - width, height, width, height), "LoadSceneMap"))
        {
            _DoLoadSceneMap = true;
        }

        if (GUI.Button(new Rect(Screen.width - width, height * 2, width, height), (_IsFix ? "FixSceneMap to Off" : "FixSceneMap to On")))
        {
            _DoSwitchFixSceneMap = true;
        }
    }
}
