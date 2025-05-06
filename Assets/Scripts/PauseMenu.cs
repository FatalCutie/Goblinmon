using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool restarting = false;
    private bool lockedButtons = false;
    public bool menuOpen = false;
    public bool canOpenMenu = false;
    public GameObject helpMenu;
    public GameObject typeChart;
    void Start()
    {
        if(pauseMenu) pauseMenu.SetActive(false);
        if (helpMenu) helpMenu.SetActive(false);
    }

    void Update()
    {
        if(canOpenMenu && !FindObjectOfType<PlayerTileMovement>().movementLocked || menuOpen){
            if(Input.GetKeyDown(KeyCode.Escape)){ 
                if(!menuOpen) OpenPauseMenu();
                else ClosePauseMenu();
            }
        }
    }

    public void OpenPauseMenu(){
        pauseMenu.SetActive(true);
        FindObjectOfType<PlayerTileMovement>().movementLocked = true;
        menuOpen = true;
        FindObjectOfType<PlayerTileMovement>().idleTimer = 0;
        FindObjectOfType<PlayerTileMovement>().playerIsIdle = false;
        FindObjectOfType<PlayerTileMovement>().idleTimerPaused = true;
    }

    public void ClosePauseMenu(){
        if(restarting) return; //Can't close menu when restarting
        if(lockedButtons) return;
        pauseMenu.SetActive(false);
        FindObjectOfType<PlayerTileMovement>().movementLocked = false;
        menuOpen = false;
        FindObjectOfType<PlayerTileMovement>().playerIsIdle = true;
        FindObjectOfType<PlayerTileMovement>().idleTimerPaused = true;
        FindObjectOfType<AudioManager>().Play("press");
    }

    public void RestartGame(){
        if(lockedButtons) return;
        FindObjectOfType<AudioManager>().Play("press");
        Destroy(FindObjectOfType<PlayerPositionManager>().gameObject); //Delete PPM
        Destroy(FindObjectOfType<PartyStorage>().gameObject); //Delete party
        restarting = true;
        lockedButtons = true;
        FindObjectOfType<SceneController>().TransitionScene("Title Screen");
    }

    public void OpenHelpMenu(){
        helpMenu.SetActive(true);
        FindObjectOfType<AudioManager>().Play("press");
    }

    public void CloseHelpMenu()
    {
        helpMenu.SetActive(false);
        if (typeChart.activeSelf) typeChart.SetActive(false);
        FindObjectOfType<AudioManager>().Play("press");
    }

    public static void QuitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

}
