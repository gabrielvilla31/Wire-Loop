using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainInteraction : MonoBehaviour
{
    public void OnLoadNewExp()
    {
        SceneManager.LoadScene("NewExp", LoadSceneMode.Single);
    }
    public void OnLoadLoadExp()
    {
        SceneManager.LoadScene("LoadExp", LoadSceneMode.Single);
    }
}