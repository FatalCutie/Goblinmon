using System.Collections;
using UnityEngine;

public class CatchSystem : MonoBehaviour
{
    BattleSystem bs;
    [SerializeField] Animator battleAnimator;
    [SerializeField] Animator enemyAnimator;

    void Start()
    {
        bs = FindObjectOfType<BattleSystem>();
    }
    public IEnumerator AttemptToCatch(Goblinmon g){
        
        StartCoroutine(bs.ScrollText("Player threw a box!"));
        yield return new WaitForSeconds(1);
        battleAnimator.SetBool("BallThrown", true);
        yield return new WaitForSeconds(0.7f);
        // bs.enemyUnit.GetComponent<SpriteRenderer>().enabled = false; //makes enemy unit disappear
        enemyAnimator.SetBool("EnemyBeingCaught", true);
        yield return new WaitForSeconds(1f);
        battleAnimator.SetTrigger("Shake");
        yield return new WaitForSeconds(2f);
        enemyAnimator.SetBool("EnemyBeingCaught", false);
        yield return new WaitForSeconds(0.2f);
        battleAnimator.SetBool("BallThrown", false);
        bs.enemyUnit.GetComponent<SpriteRenderer>().enabled = true;
        //animator.SetBool("BallThrown", false);

        // if(IsCaptureSuccessful(g)) SuccessfulCapture();

        // else FailedCapture();

    }

    private bool IsCaptureSuccessful(Goblinmon g){
        SOGoblinmon gd = g.goblinData;

        //Run calculation to see if catch is successful
        //TODO: Replace 16 with ball modifier. Lower number = better ball
        float number = gd.maxHP * gd.catchRate * 4 / g.currentHP * 16;
        float numberToBeat = bs.rnd.Next(0, 255);

        //Return true or false
        if(number > numberToBeat) return true;
 
        return false;
    }

    private void SuccessfulCapture(){
        //Print success text
        
        //Add goblinmon to player party

        //End battle
    }

    private void FailedCapture(){
        //Print failure text

        //Enemy turn
    }

    //Just making this to have it later down the line so I don't have to look this shit up again
    //IK it will be implemented differently from how it's written but idrc
    private int CalculateShakes(Goblinmon g){
        int shakeValue = g.goblinData.catchRate * 100 / 255;
        //where the value of Ball is 255 for the Poké Ball, 200 for the Great Ball, or 150 for other balls.
        if(shakeValue >= 225) return 3;
        //int forCalc = shakeValue * number (from catch calculations) / 255 (ball)
        int forCalc = 0; //temp
        if(forCalc < 10) return 0;
        else if(forCalc < 30) return 1;
        else if(forCalc < 70) return 2;
        
        return 3;
    }
}
