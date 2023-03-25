using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public GameObject postProccess;
    public TMP_Text text;
    public bool disabled = true;

    public static Settings Instance;

    void AsignVars(){
        postProccess = GameObject.Find("Post proccess");
    }
    
    void Awake(){
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
        AsignVars();
    }
    public void Disable(){
        if (disabled == false){
            postProccess.SetActive(false);
            Debug.Log("disabled");
            disabled = true;
        } else {
            postProccess.SetActive(true);
            disabled = false;
        }
    }

    public void Title(){
        MenuManager.Instance.OpenMenu("title");
    }

    public void SceneChanged(){        
        AsignVars();
        if (disabled == true){
            postProccess.SetActive(false);
        } else {
            postProccess.SetActive(true);
        }
    }
}




