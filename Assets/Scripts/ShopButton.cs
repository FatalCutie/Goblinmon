using System.Collections;
using TMPro;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public int playerMoney;
    public TextMeshProUGUI playerMoneyText;
    public int itemValue;
    public TextMeshProUGUI itemValueText;
    public enum ItemValue {IV_CATCHER, IV_FUSER};
    public ItemValue item;
    public TextMeshProUGUI playerAmount;

    void Start()
    {
        playerMoney = FindObjectOfType<PlayerPositionManager>().playerMoney;
        itemValueText.text = $"${itemValue}";
        RefreshPlayerMoneyTotal();
    }

    public void BuyItem(){
        if(FindObjectOfType<PlayerPositionManager>().playerMoney >= itemValue){
            FindObjectOfType<PlayerPositionManager>().playerMoney -= itemValue;
            //play sound
            FindObjectOfType<AudioManager>().Play("press");
            switch(item){
                case ItemValue.IV_CATCHER:
                    FindObjectOfType<PlayerPositionManager>().captureItems++;
                    break;
                case ItemValue.IV_FUSER:
                    FindObjectOfType<PlayerPositionManager>().fusionItems++;
                    break;
            }
            RefreshPlayerMoneyTotal();
        } else StartCoroutine(FailPurchase());
    }

    public void RefreshPlayerMoneyTotal(){
        playerMoneyText.text = $"${FindObjectOfType<PlayerPositionManager>().playerMoney}";
        switch(item){
            case ItemValue.IV_CATCHER:
                playerAmount.text = $"x{FindObjectOfType<PlayerPositionManager>().captureItems}";
                break;
            case ItemValue.IV_FUSER:
                playerAmount.text = $"x{FindObjectOfType<PlayerPositionManager>().fusionItems}";
                break;
        }
    }

    public IEnumerator FailPurchase(){
        //play sound
        FindObjectOfType<AudioManager>().Play("damage");
        playerMoneyText.color = Color.red;
        yield return new WaitForSeconds(.2f);
        playerMoneyText.color = Color.green; //not exact shade. Will fix later :surely:
    }
    

}
