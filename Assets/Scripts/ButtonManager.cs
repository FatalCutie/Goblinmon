using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonsBasic;
    public GameObject buttonsAttack;

    void Start()
    {
        if (buttonsAttack.activeSelf) buttonsAttack.SetActive(false);
        if (!buttonsBasic.activeSelf) buttonsBasic.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void enableAttackButtonsOnPress()
    {
        buttonsBasic.SetActive(false);
        buttonsAttack.SetActive(true);
    }

    public void enableBasicButtonsOnPress()
    {
        buttonsAttack.SetActive(false);
        buttonsBasic.SetActive(true);
    }

    public void unimplementedButtonError()
    {
        Debug.LogWarning("Pressed button is not yet Implemented! If this error is unexpected please check your code!");
    }
}
