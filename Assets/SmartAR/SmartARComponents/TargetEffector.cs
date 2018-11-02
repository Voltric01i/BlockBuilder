using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class TargetEffector : TargetEffectorBase
{
    private SmartARController smartARController_;

    protected override void Awake()
    {
        smartARController_ = FindObjectsOfType<SmartARController>()[0];
        base.Awake();
    }

    protected override void Start()
    {
        if (smartARController_.smart_.isConstructorFailed()) {
            showOrHideChildrens(false);
            return;
        }
        bool active = false;
        foreach (var target in smartARController_.recognizerSettings_.targets)
        {
            if (targetID == target.id)
            {
                active = true;
            }
        }
        gameObject.SetActive(active);

        // For scene mapping mode which not use targets
        bool isRecognittionModeSceneMapping = 
            smartARController_.recognizerSettings_.recognitionMode == smartar.RecognitionMode.RECOGNITION_MODE_SCENE_MAPPING;
        bool isSceneMappingInitModeTarget =
            smartARController_.recognizerSettings_.sceneMappingInitMode == smartar.SceneMappingInitMode.SCENE_MAPPING_INIT_MODE_TARGET;
        if (isRecognittionModeSceneMapping && !isSceneMappingInitModeTarget)
        {
            targetID = null;
        }

        base.Start();
    }

    protected override void Update()
    {
        if (smartARController_ == null) { return; }
        if (!smartARController_.enabled_) { return; }
        if (smartARController_.smart_.isConstructorFailed()) { return; }

        // For load scene mapping mode.
        if (smartARController_.isLoadSceneMap_ && targetID != null)
        {
            if (targetID != smartARController_.recognizerSettings_.targets[0].id) {
                gameObject.SetActive(false);
                return;
            }
            targetID = null;
        }
		base.Update();
    }

}
