using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using static OVRInput;



public class LoadExpInteraction : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject ChoiceCanvas;
    // Start is called before the first frame update
    void Start()
    {
        StaticClass.filename = null;
        try
        {
            // Set a variable to the My Documents path.
            string docPath = Environment.CurrentDirectory+ "\\Saves\\Exp\\";

            var files = from file in Directory.EnumerateFiles(docPath, "*.json")
                        select new
                        {
                            File = file,
                        };
           
            foreach (var f in files)
            {
                int found = 0;
                found = f.File.LastIndexOf("\\");
                string name = f.File.Substring(found+1);
                GameObject button = (GameObject)Instantiate(buttonPrefab);
                button.transform.SetParent(ChoiceCanvas.transform,false);//Setting button parent
               
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnClick(name));//Setting what button does when clicked
                                                                            //Next line assumes button has child with text as first gameobject like button created from GameObject->UI->Button
                button.transform.GetChild(0).GetComponent<Text>().text = name;//Changing text   
}
        }

        
        catch (UnauthorizedAccessException UAEx)
        {
        }
        catch (PathTooLongException PathEx)
        {
        }
    }
    
    
    public void OnClick(string file)
    {
        StaticClass.filename = file;
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    public void OnClickStart()
    {
        if (StaticClass.filename != null)
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }

}
