using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class SavedData
{
    public List<string> listeObjets = new List<string>();
    public float longueur = 0;
    public static SavedData fromListe(List<string> liste)
    {
        var result = new SavedData();
        result.listeObjets = liste;
        result.longueur = 0;
        return result;
    }
}

[System.Serializable]
public class Essai
{
    public string name;
    public float size;
}

[System.Serializable]
public class SavedExp
{
    public List<Essai> listeEssai = new List<Essai>();
}

public class Torseur
{
    public Vector3 position;
    public Quaternion rotation;

    public Torseur(Vector3 vec, Quaternion quat)
    {
        position = vec;
        rotation = quat;
    }
}

public class NewFilInteraction : MonoBehaviour
{
    Image myImageComponent;
    /*public Sprite SimpleHor; //Drag your first sprite here in inspector.
    public Sprite SimpleVert; //Drag your second sprite here in inspector.
    public Sprite LUp; //Drag your first sprite here in inspector.
    public Sprite LDown; //Drag your second sprite here in inspector.
    public Sprite LRight; //Drag your first sprite here in inspector.
    public Sprite LLeft; //Drag your second sprite here in inspector.*/

    public GameObject GO_Simple;
    public GameObject GO_L;
    private SavedData data = new SavedData();

    public GameObject pipe;

    //Attention: hitsorique se remplit a chaque instanciation, si il n'y a aucune pièce, alors il est vide
    public List<Torseur> historique = new List<Torseur>();


    private float angle;
    private float rayon_L;

    void Start() //Lets start by getting a reference to our image component.
    {//Pas bon, un image comp par bouton, TO DO!!!!!
        historique.Add(new Torseur(transform.position, transform.rotation));
        angle = 0;
        rayon_L = 0.04f;
    }

    void InstanciateSimple()
    {
        historique.Add(new Torseur(transform.position, transform.rotation));

        GameObject GO_Current = Instantiate(GO_Simple);
        GO_Current.transform.SetParent(pipe.transform);
        GO_Current.transform.SetPositionAndRotation(transform.position, transform.rotation);

        transform.Translate(new Vector3(0, 0, 0.08f));

        data.listeObjets.Add("Simple");
        data.longueur += 0.08f;
    }

    void InstanciateLx(float x)
    {
        float rad = x * Mathf.Deg2Rad;

        historique.Add(new Torseur(transform.position, transform.rotation));

        transform.Rotate(0, 0, x);

        GameObject GO_Current = Instantiate(GO_L);
        GO_Current.transform.SetParent(pipe.transform);
        GO_Current.transform.SetPositionAndRotation(transform.position, transform.rotation);

        transform.Translate(new Vector3(0, rayon_L, rayon_L));
        transform.Rotate(-90, 0, 0);

        data.listeObjets.Add("L" + (int)x);
        data.longueur += (rayon_L * 2 * (float)Math.PI) / 4;
    }

    void DestroyLast()
    {
        if (data.listeObjets.Count != 0)
        {
            Destroy(pipe.transform.GetChild(pipe.transform.childCount - 1).gameObject);
            transform.SetPositionAndRotation(historique[historique.Count - 1].position, historique[historique.Count - 1].rotation);
            historique.RemoveAt(historique.Count - 1);
            data.listeObjets.RemoveAt(data.listeObjets.Count - 1);
        }
    }

    void TurnLLastIfLx()
    {
        if (data.listeObjets[data.listeObjets.Count - 1] != "Simple")
        {
            DestroyLast();
            angle += 10;
            InstanciateLx(angle);
        }
    }

    void TurnRLastIfLx()
    {
        if (data.listeObjets[data.listeObjets.Count - 1] != "Simple")
        {
            DestroyLast();
            angle -= 10;
            InstanciateLx(angle);
        }
    }

    void SaveWire(SavedData data)
    {
        string json = JsonUtility.ToJson(data);
        string nom = getname();
        //string filePath = Application.dataPath + "\\" + nom + ".json";
        string filePath = Environment.CurrentDirectory + "\\Saves\\" + nom + ".json";
        File.WriteAllText(filePath, json);
    }

    private string getname()
    {
        int count;
        string dirPath = Environment.CurrentDirectory + "\\Saves\\";
        count = Directory.GetFiles(dirPath, "*.json*", SearchOption.TopDirectoryOnly).Length;
        string name = "Fil n°" + (count+1);
        return name;
    }

    public void OnClickLeft()
    {
        DestroyLast();
       
       // myImageComponent.sprite = mySecondImageLeft;
    }
    public void OnClickCenter()
    {
        InstanciateSimple();
        //myImageComponent.sprite = mySecondImageCenter;
    }
    public void OnClickRight()
    {
        InstanciateLx(angle);
        //myImageComponent.sprite = mySecondImageCenter;
    }

    public void OnClickPlus()
    {
        TurnRLastIfLx();
    }

    public void OnClickMoins()
    {
        TurnLLastIfLx();
    }

    public void OnClickSave()
    {
        SaveWire(data);
        SceneManager.LoadScene("NewExp", LoadSceneMode.Single);

    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("NewExp", LoadSceneMode.Single);
    } 



}
