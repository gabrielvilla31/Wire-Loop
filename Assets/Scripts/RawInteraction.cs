
using UnityEngine;

public class RawInteraction : MonoBehaviour
{
 
    public UnityEngine.UI.Text outText;


    public void OnHoverEnter(Transform t)
    {

        GetComponent<Renderer>().material.color = Color.green;

    }

    public void OnHoverExit(Transform t)
    {
        GetComponent<Renderer>().material.color = Color.white;

    }

    public void OnSelected(Transform t)
    {

        Debug.Log("Clicked on " + t.gameObject.name);
        if (outText != null)
        {
            outText.text = "<b>Last Interaction:</b>\nClicked On:" + t.gameObject.name;
        }
    }
    // Update is called once per frame

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Any))
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
    }
}
