using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraScaller : MonoBehaviour
{
    

    //Скрипт, с помощью которого камера подстраивается под разные разрешения экранов

    Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Start()
    {
        
        float screenRatio = (float)Screen.width / (float)Screen.height;
        //номинальное разрешение 1080х1920
        float myRatio =  10.80f/ 19.20f;

        if (screenRatio >= myRatio)
        {
            camera.orthographicSize = 7;
        }
        else
        {
            float different = myRatio / screenRatio;
            camera.orthographicSize = 7 * different;
        }
               
    }

    /// <summary>
    /// Метод переключения сцены
    /// </summary>
    /// <param name="i">Номер сцены</param>
    public void SetScene(int i) 
    {
        SceneManager.LoadScene(i);
    }
    /// <summary>
    /// Метод выхода из приложения
    /// </summary>
    public void Quit()
    {
        Debug.Log(111);
        Application.Quit();
    }

}
