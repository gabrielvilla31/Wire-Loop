using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollideEffects : MonoBehaviour
{
    private bool gameOn;
    public Text timer;
    private DateTime DateDebutJeu;
    public GameObject Pipe;
    public GameObject CanvasReussite;
    public GameObject SelectionVisualizer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOn)
        {
            timer.text = (DateTime.Now-DateDebutJeu).ToString();
        }
       
         
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "StartPipeZone(Clone)")
        {
            gameOn = true;
            GetComponent<Renderer>().material.color = Color.white;
            CanvasReussite.SetActive(false);
            SelectionVisualizer.SetActive(false);

            DateDebutJeu = DateTime.Now;
        }
        if (other.gameObject.name == "EndPipeZone(Clone)" && gameOn)
        {
            gameOn = false;
            CanvasReussite.SetActive(true);
            SelectionVisualizer.SetActive(true);

        }
        foreach (Transform child in Pipe.transform)
        {
            if (other.gameObject.name == child.name)
            {
                GetComponent<Renderer>().material.color = Color.red;
                gameOn = false;
            }
        }
    }
}
