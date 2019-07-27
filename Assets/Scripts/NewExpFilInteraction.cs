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



public class NewExpFilInteraction : MonoBehaviour
{
    //listeFil
    public GameObject buttonPrefab;
    public GameObject ChoiceFilCanvas;

    //Generation du fil
    public GameObject GO_Simple;
    public GameObject GO_L;
    public GameObject Pipe;
    private Vector3 origine_position;
    private Quaternion origine_rotation;

    //Gestion de l'anneau
    public Text TextAnneau;
    public GameObject Anneau;
    private float sizeAnneau;
    private string filename;

    //Enregistrement de l'exp
    private SavedExp exp = new SavedExp();

    //liste Essai
    public GameObject textPrefab;
    public GameObject EssaiCanvas;

    public List<Torseur> historique = new List<Torseur>();
   
    private float rayon_L;

    // Start is called before the first frame update
    void Start()
    {
        rayon_L = 0.04f;
        sizeAnneau = 10;
        TextAnneau.text = "Rayon = " + sizeAnneau;
        origine_position = new Vector3(
                                                   transform.position.x,
                                                   transform.position.y,
                                                   transform.position.z
                                                   );

        origine_rotation = new Quaternion(
                                               transform.rotation.x,
                                               transform.rotation.y,
                                               transform.rotation.z,
                                               transform.rotation.w
                                               );
        try
        {
            // Set a variable to the My Documents path.
            string docPath = Environment.CurrentDirectory+"\\Saves\\";

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
                button.transform.SetParent(ChoiceFilCanvas.transform,false);//Setting button parent
               
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnClickFil(name));//Setting what button does when clicked
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

        if (StaticClass.savetemp != null)
        {
            StaticClass.savetemp.listeEssai.ForEach(delegate (Essai essaicour)
            {
                
                GameObject text = (GameObject)Instantiate(textPrefab);
                text.transform.SetParent(EssaiCanvas.transform, false);//Setting button parent
                                                                       //Next line assumes button has child with text as first gameobject like button created from GameObject->UI->Button
                text.transform.GetComponent<Text>().text = essaicour.name + "  " + essaicour.size;//Changing text   
                exp.listeEssai.Add(essaicour);
            }
       );
            
        }
    }

    void InstanciateSimple()
    {
        historique.Add(new Torseur(transform.position, transform.rotation));

        GameObject GO_Current = Instantiate(GO_Simple);
        GO_Current.transform.SetParent(Pipe.transform);
        GO_Current.transform.SetPositionAndRotation(transform.position, transform.rotation);

        transform.Translate(new Vector3(0, 0, 0.08f));

    }

    void InstanciateLx(float x)
    {
        float rad = x * Mathf.Deg2Rad;

        historique.Add(new Torseur(transform.position, transform.rotation));

        transform.Rotate(0, 0, x);

        GameObject GO_Current = Instantiate(GO_L);
        GO_Current.transform.SetParent(Pipe.transform);
        GO_Current.transform.SetPositionAndRotation(transform.position, transform.rotation);

        transform.Translate(new Vector3(0, rayon_L, rayon_L));
        transform.Rotate(-90, 0, 0);

    }
    
    SavedData LoadWire(string nom)
    {
        string filePath = Environment.CurrentDirectory + "\\Saves\\" + nom;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            //print("Load " + nom + ":  " + json);
            return JsonUtility.FromJson<SavedData>(json);
        }
        else
        {
            return new SavedData();
        }
    }

    void InstanciateFromData(SavedData data)
    {
        data.listeObjets.ForEach(delegate (String name)
        {
            if (name == "Simple")
                InstanciateSimple();
            else if (name[0] == 'L')
                InstanciateLx(float.Parse(name.Substring(1, name.Length - 1)));
        }
        );
    }

    private void Detroy()
    {
        foreach (Transform child in Pipe.transform)
        {
            Destroy(child.gameObject);
        }
        transform.SetPositionAndRotation(origine_position, origine_rotation);
    }

    private string getname()
    {
        int count;
        string dirPath = Environment.CurrentDirectory + "\\Saves\\Exp\\";
        count = Directory.GetFiles(dirPath, "*.json*", SearchOption.TopDirectoryOnly).Length;
        string name = "Experience n°" + (count + 1);
        return name;
    }

    private float getLongueur(string nameFile)
    {
        string filePath = Environment.CurrentDirectory + "\\Saves\\" + nameFile;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            //print("Load " + nom + ":  " + json);
            SavedData fil= JsonUtility.FromJson<SavedData>(json);
            return fil.longueur;
        }
        else
        {
            return-1;
        }
    }
    private string getnameExp()
    {
        int count;
        string dirPath = Environment.CurrentDirectory + "\\Saves\\Exp\\";
        count = Directory.GetFiles(dirPath, "*.json*", SearchOption.TopDirectoryOnly).Length;
        string name = "Experience n°" + (count);
        return name;
    }
    private string getCVSLine()
    {
        string text=getnameExp()+";";
        exp.listeEssai.ForEach(delegate (Essai essaicour)
        {
            text+=getLongueur(essaicour.name)+"   "+essaicour.size+";";
        });


        return text.Substring(0, text.LastIndexOf(";"))+"\n" ;
    }
    void SaveExp(SavedExp exp)
    {
        string json = JsonUtility.ToJson(exp);
        string nom = getname();
        string filePath = Environment.CurrentDirectory + "\\Saves\\Exp\\" + nom + ".json";
        string filePath2 = Environment.CurrentDirectory + "\\Saves\\Result\\" + nom + ".cvs";

        File.WriteAllText(filePath, json);

        string cvs= getCVSLine();
        File.WriteAllText(filePath2, cvs);

    }

    public void OnClickFil(string file)
    {

        Detroy();
        InstanciateFromData(LoadWire(file));
        filename = file;
    }

    public void OnClickMoins()
    {
        if (sizeAnneau > 3)
        {
            sizeAnneau -= (float)1;
            TextAnneau.text = "Rayon = " + sizeAnneau;
            Anneau.transform.localScale = (new Vector3(sizeAnneau, sizeAnneau, sizeAnneau));
        }
    }

    public void OnClickPlus()
    {
        if (sizeAnneau < 20)
        {
            sizeAnneau += (float)1;
            TextAnneau.text = "Rayon = " + sizeAnneau;
            Anneau.transform.localScale = (new Vector3(sizeAnneau, sizeAnneau, sizeAnneau));
        }
    }
       
 
    public void OnClickAdd()
    {
        if (filename != null && sizeAnneau > 0)
        {
            Essai essai = new Essai();

            essai.name = filename;
            essai.size = sizeAnneau;
            exp.listeEssai.Add(essai);

            GameObject text = (GameObject)Instantiate(textPrefab);
            text.transform.SetParent(EssaiCanvas.transform, false);//Setting button parent
                                                                   //Next line assumes button has child with text as first gameobject like button created from GameObject->UI->Button
            text.transform.GetComponent<Text>().text = filename + "  " + sizeAnneau;//Changing text   
        }
    }

    public void OnClickNewFil()
    {
        StaticClass.savetemp = exp;
        SceneManager.LoadScene("NewFil", LoadSceneMode.Single);
    }
    public void OnClickBack()
    {
        StaticClass.savetemp = null;
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void OnClickSave()
    {
            SaveExp(exp);
            StaticClass.savetemp = null;
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
       
    }

   
}
