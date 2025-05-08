using UnityEngine;

public class OopsieScript : MonoBehaviour
{
    public GameObject fusion;
    public GameObject mon;
    public GameObject buttons;

    void Start()
    {
        fusion.SetActive(false);
        mon.SetActive(false);
        buttons.SetActive(false);
    }

    public void PlayThump(){
        FindObjectOfType<AudioManager>().Play("titleBump");
    }

    public void PlayComplete(){
        FindObjectOfType<AudioManager>().Play("titleDone");
    }

    public void SetMenuItemsInactive(){
        fusion.SetActive(false);
        mon.SetActive(false);
        buttons.SetActive(false);
    }

    public void SetFusionLocation(){
        fusion.SetActive(true);
        // fusion.transform.position = new Vector3(0, 300, 0);
    }

    public void SetMonLocation(){
        mon.SetActive(true);
        // mon.transform.position = new Vector3(0, 185, 0);
    }

    public void SetButtonLocation(){
        buttons.SetActive(true);
        // buttons.transform.position = new Vector3(); 
    }
}
