using System.Data;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    public SOGoblinmon unit;
    private TextMeshProUGUI text;
    private SwitchingManager sm;
    [SerializeField] private ColorBlock colorBlock;

    void Start()
    {
        colorBlock = this.GetComponent<Button>().colors;
        sm = FindObjectOfType<SwitchingManager>();
        text = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        //this is awful, but it's easy and it works
        if (unit == null && text.text != "")
        {
            text.text = "";
        }

        //If attached unit is dead button is red
        if (this.GetComponent<Goblinmon>() != null && gameObject.GetComponent<Goblinmon>().currentHP <= 0)
        {
            //TODO: this isn't changing color 
            if (this.GetComponent<Button>().colors.normalColor != Color.red)
            {
                this.GetComponent<Image>().color = Color.red;
                colorBlock.normalColor = Color.red;
                colorBlock.highlightedColor = new Color(125, 0, 0);
                colorBlock.pressedColor = new Color(75, 0, 0);
                GetComponent<Button>().colors = colorBlock;
            }
        }
    }

    public void SwitchUnitOnPress()
    {
        //Don't switch if no attached unit or attached unit is dead
        if (this.GetComponent<Goblinmon>() == null || this.GetComponent<Goblinmon>().currentHP <= 0)
        {
            FindObjectOfType<AudioManager>().Play("damage");
        }
        else sm.CheckUnitBeforeSwitching(this.GetComponent<Goblinmon>());

    }

}
