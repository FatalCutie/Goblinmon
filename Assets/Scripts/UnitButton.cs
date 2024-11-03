using System.Data;
using TMPro;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
    public SOGoblinmon unit;
    private TextMeshProUGUI text;
    private SwitchingManager sm;

    void Start()
    {
        sm = FindObjectOfType<SwitchingManager>();
        text = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); //this can be cleaned up(?)
    }

    void FixedUpdate()
    {
        //this is awful, but it's easy and it works
        if (unit == null && text.text != "")
        {
            text.text = "";
        }
    }

    public void SwitchUnitOnPress()
    {
        if (this.GetComponent<Goblinmon>() == null)
        {
            FindObjectOfType<AudioManager>().Play("damage");
        }
        else sm.CheckUnitBeforeSwitching(this.GetComponent<Goblinmon>());

    }

}
