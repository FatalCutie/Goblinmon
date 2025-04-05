using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FusionCalculator : MonoBehaviour
{
    #region Units
    #region Declare SO's
    [Header("Base Units")]
    public SOGoblinmon fire1;
    public SOGoblinmon fire2;
    public SOGoblinmon grass1;
    public SOGoblinmon grass2;
    public SOGoblinmon ice1;
    public SOGoblinmon ice2;
    public SOGoblinmon magic1;
    public SOGoblinmon water1;
    public SOGoblinmon water2;
    #endregion

    #region Declare Fire1 Fusions
    [Header("Fire1 Fusions")]
    public SOGoblinmon fire1Fire1;
    public SOGoblinmon fire1Fire2;
    public SOGoblinmon fire1Grass1;
    public SOGoblinmon fire1Grass2;
    public SOGoblinmon fire1Ice1;
    public SOGoblinmon fire1Ice2;
    public SOGoblinmon fire1Magic1;
    public SOGoblinmon fire1Water1;
    public SOGoblinmon fire1Water2;
    #endregion

    #region Declare Fire2 Fusions
    [Header("Fire2 Fusions")]
    public SOGoblinmon fire2Fire2;
    public SOGoblinmon fire2Grass1;
    public SOGoblinmon fire2Grass2;
    public SOGoblinmon fire2Ice1;
    public SOGoblinmon fire2Ice2;
    public SOGoblinmon fire2Magic1;
    public SOGoblinmon fire2Water1;
    public SOGoblinmon fire2Water2;
    #endregion

    #region Declare Grass1 Fusions
    [Header("Grass1 Fusions")]
    public SOGoblinmon grass1Grass1;
    public SOGoblinmon grass1Grass2;
    public SOGoblinmon grass1Ice1;
    public SOGoblinmon grass1Ice2;
    public SOGoblinmon grass1Magic1;
    public SOGoblinmon grass1Water1;
    public SOGoblinmon grass1Water2;
    #endregion

    #region Declare Grass2 Fusions
    [Header("Grass2 Fusions")]
    public SOGoblinmon grass2Grass2;
    public SOGoblinmon grass2Ice1;
    public SOGoblinmon grass2Ice2;
    public SOGoblinmon grass2Magic1;
    public SOGoblinmon grass2Water1;
    public SOGoblinmon grass2Water2;
    #endregion

    #region Declare Ice1 Fusions
    [Header("Ice1 Fusions")]
    public SOGoblinmon ice1Ice1;
    public SOGoblinmon ice1Ice2;
    public SOGoblinmon ice1Magic1;
    public SOGoblinmon ice1Water1;
    public SOGoblinmon ice1Water2;
    #endregion

    #region Declare Ice2 Fusions
    [Header("Ice2 Fusions")]
    public SOGoblinmon ice2Ice2;
    public SOGoblinmon ice2Magic1;
    public SOGoblinmon ice2Water1;
    public SOGoblinmon ice2Water2;
    #endregion

    #region Declare Magic1 Fusions
    [Header("Magic1 Fusions")]
    public SOGoblinmon magic1Magic1;
    public SOGoblinmon magic1Water1;
    public SOGoblinmon magic1Water2;
    #endregion

    #region Declare Water Fusions
    [Header("Water Fusions")]
    public SOGoblinmon water1Water1;
    public SOGoblinmon water1Water2;
    public SOGoblinmon water2Water2;
    #endregion
    #endregion
    

    
    //Apparently you can't write this with switch statements so
    //prepare to read some downright CRIMINAL code
    public SOGoblinmon CalculateFusionUnit(SOGoblinmon unitOne, SOGoblinmon unitTwo){
        if(unitOne.isFusion || unitTwo.isFusion){
            //Temp buffer
            Debug.LogWarning("You cannot fuse a fusion! Terminating!");
            return null;
        } 
        //Debug.Log($"Trying to find: {unitTwo.gName}, or {unitTwo.name}");
        if(unitOne == fire1){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){
                return fire1Grass1;
            } else if(unitTwo == grass2){

            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }
        } else if(unitOne == fire2){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        } else if(unitOne == grass1){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        } else if(unitOne == grass2){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        } else if(unitOne == ice1){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        } else if(unitOne == ice2){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        } else if(unitOne == magic1){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        } else if(unitOne == water1){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        } else if(unitOne == water2){
            if(unitTwo == fire1){

            } else if(unitTwo == fire2){

            } else if(unitTwo == grass1){

            } else if(unitTwo == grass2){
                
            } else if(unitTwo == ice1){

            } else if(unitTwo == ice2){

            } else if(unitTwo == magic1){

            } else if(unitTwo == water1){

            } else if(unitTwo == water2){

            }
            else{
                Debug.LogWarning("Could not find Unit2 in FusionCalculator. Did you declare it properly?");
                return null;
            }

        }
        else{
            Debug.LogWarning("Could not find Unit1 in FusionCalculator. Did you declare it properly?");
            return null;
        }
        Debug.LogWarning("Something in FusionCalculator has gone horribly wrong.");
        Debug.LogWarning($"Failed to fuse these units: {unitOne.gName}, {unitTwo.gName}");
        return null;
    }

    
}
