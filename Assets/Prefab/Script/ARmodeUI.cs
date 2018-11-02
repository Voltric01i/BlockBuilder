using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class ARmodeUI : AppControllerBase
{
    [SerializeField]
    protected SmartARController smartARController;

    private const string SCENE_MAP_FILE_NAME = "/scenemap.dat";

    private string _SceneMapFilePath;
    private bool _IsFix;

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

    public void Reset()
    {

        smartARController.restartController();
    }
    public void Room()
    {
        SceneManager.LoadScene("StartLoad");
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        smartCheckError(smartARController);

        if (smartARController.smart_.isConstructorFailed()) { return; }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        mySkin.button.fontSize = 15;
        mySkin.label.fontSize = 15;
#endif

        var buttonLongSide = Math.Max((float)Screen.width * 0.1f, (float)Screen.height * 0.1f);
        var buttonNarrowSide = Math.Min((float)Screen.width * 0.1f, (float)Screen.height * 0.1f);

        /*
        if (GUI.Button(new Rect(0, Screen.height - buttonNarrowSide, buttonLongSide, buttonNarrowSide), "Reset"))
        {
            smartARController.restartController();
        }

        if ((GUI.Button(new Rect(0, 0, buttonLongSide, buttonNarrowSide), "Back")) || (Input.GetKeyDown(KeyCode.Escape)))

        {
            SceneManager.LoadScene("StartLoad");
        }
        */
        var width = buttonLongSide * 2;
        var height = buttonNarrowSide;
        /*if (GUI.Button(new Rect(Screen.width - width, 0, width, height), "SaveSceneMap"))
        {
            var fileStreamOut = new smartar.FileStreamOut(smartARController.smart_, _SceneMapFilePath);
            smartARController.recognizer_.SaveSceneMap(fileStreamOut);
        }

        if (GUI.Button(new Rect(Screen.width - width, height, width, height), "LoadSceneMap"))
        {
            smartARController.loadSceneMap(_SceneMapFilePath);
            _IsFix = true;
        }

        if (GUI.Button(new Rect(Screen.width - width, height * 2, width, height), "FixSceneMap"))
        {
            _IsFix = !_IsFix;
            smartARController.recognizer_.FixSceneMap(_IsFix);
        }
        */
    }
}

