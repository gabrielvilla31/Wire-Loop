using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInit : MonoBehaviour
{

    public GameObject GO_Simple;
    public GameObject GO_L;
    public GameObject Pipe;
    public GameObject StartPipeZone;
    public GameObject EndPipeZone;
    public GameObject PipeZone;
    private Vector3 origine_position;
    private Quaternion origine_rotation;

    public GameObject Anneau;

    public GameObject CanvasReussite;
    public GameObject CanvasFin;
    public GameObject SelectionVisualizer;

    public Text timer;
    public Text resultat;

    private string file;
    private int count;
    private SavedExp exp;
    private int nbrEssai;


    private String name;
    private List<String> result = new List<string>();
    public List<Torseur> historique = new List<Torseur>();
    private float rayon_L;
    //EN COURS, SW ligne 227 228 




    //Fonction d'instantiation de prefabs

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
    private void InstantiateStartZone()
    {
        GameObject StartZone = Instantiate(StartPipeZone);
        StartZone.transform.SetParent(PipeZone.transform);
        StartZone.transform.SetPositionAndRotation(new Vector3(
                                                transform.position.x,
                                                transform.position.y,
                                                transform.position.z
                                                ), new Quaternion(
                                                transform.rotation.x,
                                                transform.rotation.y,
                                                transform.rotation.z,
                                                transform.rotation.w
                                                ));

    }

    private void InstantiateEndZone()
    {
        GameObject EndZone = Instantiate(EndPipeZone);
        EndZone.transform.SetParent(PipeZone.transform);
        EndZone.transform.SetPositionAndRotation(new Vector3(
                                                transform.position.x,
                                                transform.position.y,
                                                transform.position.z
                                                ), new Quaternion(
                                                transform.rotation.x,
                                                transform.rotation.y,
                                                transform.rotation.z,
                                                transform.rotation.w
                                                ));

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

    public void Essaisuiv()
    {
        Essai essai = exp.listeEssai[count];
        build(essai.name, essai.size);
        count++;
    }

    private void Destroy()
    {
        foreach (Transform child in Pipe.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in PipeZone.transform)
        {
            Destroy(child.gameObject);
        }
        transform.SetPositionAndRotation(origine_position, origine_rotation);
    }

    private void SetAnneauSize(float sizeAnneau)
    {
        Anneau.transform.localScale = (new Vector3(sizeAnneau, sizeAnneau, sizeAnneau));
    }

    private void build(string file, float sizeAnneau)
    {
        InstantiateStartZone();
        InstanciateFromData(LoadWire(file));
        InstantiateEndZone();
        SetAnneauSize(sizeAnneau);
    }

    SavedData LoadWire(string nom)
    {
        string filePath = Environment.CurrentDirectory + "\\Saves\\" + nom;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<SavedData>(json);
        }
        else
        {
            return new SavedData();
        }
    }

    private void SaveResultTemp()
    {
        result.Add(timer.text);
    }

    private void SaveResult()
    {
        string FilePath = Environment.CurrentDirectory + "\\Saves\\Result\\" + getNameFileCSV();
        int num=0;
        foreach (string line in File.ReadLines(FilePath))
        {
            if (line != null)
            {
                num++;
            }
        }


        string resultCSV = "Sujet n°"+num+";";

        int nbr =result.Count;
        for (int i=0; i < nbr; i++)
        {
            if (i != nbr-1)
            {
                resultCSV += result[i] + ";";
            }
            else
            {
                resultCSV += result[i]+"\n";
            }
        }
       
       
        File.AppendAllText(FilePath, resultCSV);

        timer.text = "";
        

    }

    private String getNameFileCSV()
    {
        String result;
        int index = StaticClass.filename.LastIndexOf(".");
        result = StaticClass.filename.Substring(0,index);
        result += ".cvs";
        return result;
    }

    //OnClicks

    public void OnClickRetry()
    {
        CanvasReussite.SetActive(false);
        SelectionVisualizer.SetActive(false);

    }

    public void OnClickSuivant()
    {
        Destroy();
        CanvasReussite.SetActive(false);
        if (count<=nbrEssai)
        {
 
            Essaisuiv();
            SelectionVisualizer.SetActive(false);
            SaveResultTemp();
            timer.text = "";
        }
        else
        {
            SaveResultTemp();
            CanvasFin.SetActive(true);
        }
    }

    public void OnClickSave()
    {
        SaveResult();
        SceneManager.LoadScene("Main", LoadSceneMode.Single);

    }

    public void OnClickRetryAll()
    {
        count = 0;
        Essaisuiv();
        CanvasFin.SetActive(false);
        SelectionVisualizer.SetActive(false);

    }

    public void OnClickQuit()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }


    //Start and Update

    void Start()
    {
        rayon_L = 0.04f;
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

        string filePath = Environment.CurrentDirectory + "\\Saves\\Exp\\" + StaticClass.filename;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            exp = JsonUtility.FromJson<SavedExp>(json);
            nbrEssai = exp.listeEssai.Count-1;
        }       
        CanvasReussite.SetActive(false);
        CanvasFin.SetActive(false);
        SelectionVisualizer.SetActive(false);

        count = 0;
        Essaisuiv();

    }

    // Update is called once per frame
    void Update()
    {
        resultat.text = "Vous avez reussi";
    }
    
  
    
}
