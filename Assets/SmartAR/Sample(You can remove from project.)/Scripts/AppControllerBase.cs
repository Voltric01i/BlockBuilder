using UnityEngine;
using System.Collections;

public class AppControllerBase : MonoBehaviour
{
    [SerializeField]
    protected GUISkin mySkin;

    protected TargetEffector[] effectors_ = { };

    protected virtual void Awake()
    {
        effectors_ = FindObjectsOfType<TargetEffector>();

        mySkin.label.fontSize = 30;
        mySkin.label.alignment = TextAnchor.UpperLeft;
        mySkin.label.normal.textColor = Color.green;
    }

    protected virtual void OnGUI()
    {
        GUI.skin = mySkin;
    }

    protected void smartCheckError(SmartARController smartARController)
    {
        if (!smartARController.smart_.isConstructorFailed()) { return; }

        mySkin.label.fontSize = Screen.width / 20;
        mySkin.label.alignment = TextAnchor.MiddleCenter;
        mySkin.label.normal.textColor = Color.red;

        var message = "";
        switch (smartARController.smart_.getInitResultCode())
        {
            case smartar.Error.ERROR_EXPIRED_LICENSE:
                message = "SmartAR SDK expired license.";
                break;
            default:
                message = "Smart initialized error.";
                break;
        }

        message += "\nPlease exit the application.";
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), message);
    }
}
