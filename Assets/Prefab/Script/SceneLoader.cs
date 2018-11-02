using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour {

    private AsyncOperation async;
    public GameObject LoadingUi;
    public Slider Slider;

    public void Start ()
    {
        LoadingUi.SetActive(true);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        int dateHour = System.DateTime.Now.Hour;
        if ((dateHour>=6) && (dateHour<=17) )
        {
            async = SceneManager.LoadSceneAsync("MainRoomDay");
        }
        else
        {
            async = SceneManager.LoadSceneAsync("MainRoomNight");
        }


        while (!async.isDone)
        {
            Slider.value = async.progress;
            yield return null;
        }
    }
}
