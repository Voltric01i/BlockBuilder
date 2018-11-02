using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenu : MonoBehaviour {

    public GameObject menuTint;
    public GameObject menuList;
    public GameObject SettingList;
    public GameObject Touch;

    public GameObject SmartARTargetFrame;
    public GameObject SmartARTargetLine;

    GameObject robot;

    public void Start()
    {
        //robot = GameObject.Find("MainRobot");
        //RobotRl = robot.GetComponent<RoomLiving>();
    }


    public void ItemMenuEnabler()
    {
        menuTint.SetActive(true);
        menuList.SetActive(true);
        if (Touch != null)
        {
            Touch.SetActive(false);
        }
    }

    public void ItemMenuDisabler()
    {
        menuTint.SetActive(false);
        menuList.SetActive(false);
        if (Touch != null)
        {
            Touch.SetActive(true);
        }
    }



    public void  SettingMenuEnabler()
    {
        menuTint.SetActive(true);
        SettingList.SetActive(true);
    }

    public void SettingMenuDisabler()
    {
        menuTint.SetActive(false);
        SettingList.SetActive(false);

    }

    public void ShowTargetPointButton(bool value)
    {
        if (value)
        {
            SmartARTargetLine.SetActive(true);
            SmartARTargetFrame.SetActive(true);
            PlayerPrefs.SetInt("showTargetPoint", 1);
        }
        else
        {
            SmartARTargetLine.SetActive(false);
            SmartARTargetFrame.SetActive(false);
            PlayerPrefs.SetInt("showTargetPoint", 1);
        }
    }

}
