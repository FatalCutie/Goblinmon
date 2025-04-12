using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CatchSystem : MonoBehaviour
{
    BattleSystem bs;
    PartyStorage ps;
    [SerializeField] Animator battleAnimator;
    [SerializeField] Animator enemyAnimator;
    public Goblinmon tempHoler;
    float captureScore;

    void Start()
    {
        ps = FindObjectOfType<PartyStorage>();
        bs = FindObjectOfType<BattleSystem>();
    }
    public IEnumerator AttemptToCatch(Goblinmon g){

        StartCoroutine(bs.ScrollText("Player threw a ball!"));
        yield return new WaitForSeconds(1);
        battleAnimator.SetBool("BallThrown", true); //Throw ball animation
        yield return new WaitForSeconds(0.7f);
        enemyAnimator.SetBool("EnemyBeingCaught", true); //Enemy goes into ball

        //Run catch calculations
        if (IsCaptureSuccessful(g))
        {
            StartCoroutine(SuccessfulCapture());
        }

        else
        {
            StartCoroutine(FailedCapture());
        }
    }

    private bool IsCaptureSuccessful(Goblinmon g){
        SOGoblinmon gd = g.goblinData;

        //Run calculation to see if catch is successful
        //TODO: Replace 16 with ball modifier. Lower number = better ball
        float number = gd.maxHP * gd.catchRate * 4 / (g.currentHP * 10);
        //Debug.Log($"{gd.maxHP * gd.catchRate * 4} / {g.currentHP * 255}");
        float numberToBeat = bs.rnd.Next(0, 255);
        //Debug.Log($"Using {number} to beat {numberToBeat}!");
        captureScore = numberToBeat - number;
        //Return true or false
        if (number >= numberToBeat) return true;

        return false;
    }

    private IEnumerator SuccessfulCapture()
    {
        //Shake ball three times
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++)
        {
            battleAnimator.SetTrigger("Shake");
            FindObjectOfType<AudioManager>().Play("shake");
            yield return new WaitForSeconds(1);
        }

        //Successful capture text
        FindObjectOfType<AudioManager>().Play("catch");
        yield return new WaitForSeconds(1);

        //Add goblinmon to player party
        if (ps.goblinmon.Count + 1 <= 6)
        {
            Goblinmon g = GoblinToAdd(bs.enemyUnit);
            ps.goblinmon.Add(g);

            //End battle
            bs.state = BattleState.WON;
            bs.EndBattle();
        }
        else
        {
            StartCoroutine(bs.ScrollText("Please select a party member to release!"));
            FindObjectOfType<SwitchingManager>().FlipUnitButtonFunctionality(); //Pressed button will now release unit, not switch it
            FindObjectOfType<ButtonManager>().enableSwitchingMenu();
            //Standby waiting for a press
        }
    }
    //Dummy void so we can disable the switching menu during victory
    public void BeginReleaseUnit(int i)
    {
        StartCoroutine(ReleaseUnit(i));
    }
    public IEnumerator ReleaseUnit(int i)
    {
        Goblinmon g = GoblinToAdd(bs.enemyUnit);
        StartCoroutine(bs.ScrollText($"Goodbye {ps.goblinmon[i].goblinData.gName}, hello {g.goblinData.gName}!"));
        Destroy(ps.goblinmon[i]); //Remove Goblinmon before overriding to prevent memory leak
        ps.goblinmon[i] = g;
        ps.goblinmonSO[i] = g.goblinData;
        FindObjectOfType<ButtonManager>().HideSwitchingMenuAfterCapture();
        yield return new WaitForSeconds(1.5f);

        //End battle
        bs.state = BattleState.WON;
        bs.EndBattle();
    }

    //Clone data from old unit to a new unit
    private Goblinmon GoblinToAdd(Goblinmon g)
    {
        Goblinmon toReturn = FindObjectOfType<PartyStorage>().AddComponent<Goblinmon>();
        toReturn.currentHP = g.currentHP;
        toReturn.goblinData = g.goblinData;
        toReturn.CloneIdFrom(g);
        tempHoler = toReturn;
        return toReturn;
    }

    private IEnumerator FailedCapture()
    {
        //Calculate # of shakes before break
        int j = CalculateShakes();
        yield return new WaitForSeconds(1);
        for (int i = 0; i < j; i++)
        {
            battleAnimator.SetTrigger("Shake");
            FindObjectOfType<AudioManager>().Play("shake");
            yield return new WaitForSeconds(1);
        }
        //Enemy breaks out of ball
        enemyAnimator.SetBool("EnemyBeingCaught", false);
        yield return new WaitForSeconds(0.2f);
        battleAnimator.SetBool("BallThrown", false);

        yield return new WaitForSeconds(1);
        //Print failure text
        switch (j)
        {
            case 0:
                StartCoroutine(bs.ScrollText("Argh! Not even close!"));
                break;
            case 1:
                StartCoroutine(bs.ScrollText("Darn! This is a strong one!"));
                break;
            case 2:
                StartCoroutine(bs.ScrollText("Shoot! It's not as weak as it seemed!"));
                break;
            case 3:
                StartCoroutine(bs.ScrollText("So close! Nearly had it!"));
                break;
        }

        yield return new WaitForSeconds(2);

        //Enemy turn
        bs.state = BattleState.ENEMYTURN;
        FindObjectOfType<EnemyAI>().FindOptimalOption();
    }

    //Calculate the # of times ball shakes before breaking
    private int CalculateShakes()
    {
        if (captureScore < 10) return 3;
        else if (captureScore < 30) return 2;
        else if (captureScore < 70) return 1;

        return 0;
    }
}
