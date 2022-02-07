using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that takes care of main menu functionality .
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Field representing network manager in menu.
    /// </summary>
    [SerializeField] public static NetworkManagerHearts networkManager = null;
    /// <summary>
    /// Field representing prefab for network manager.
    /// </summary>
    [SerializeField] public GameObject networkManagerPrefab;
    /// <summary>
    /// Field representing prefab for sound manager.
    /// </summary>
    [SerializeField] public GameObject soundManagerPrefab;
    

    [Header("UI")]
    [SerializeField] private GameObject panel_play = null;
    [SerializeField] private GameObject background = null;
    [SerializeField] private GameObject panel_menu = null;
    [SerializeField] private GameObject panel_scoreHolder = null;
    [SerializeField] private GameObject panel_results = null;
    [SerializeField] private GameObject button_retunFromScoreBoard = null;

    private bool gotScore = false;

    
    private void Awake()
    {
        if (networkManager == null)
        {
            networkManager = Instantiate(networkManagerPrefab.GetComponent<NetworkManagerHearts>());
        }
        CheckNetworkManager();

        if (!GameObject.Find("SoundManager(Clone)")) {
            Instantiate(soundManagerPrefab);
        }
    }

    private void Update()
    {
        if (FindObjectOfType<ScoreBoard>() && !gotScore && !panel_scoreHolder.activeSelf)
        {
            SetUpScoreBoard();
            gotScore = true;
        }
    }
    /// <summary>
    /// Method used for hosting lobby.
    /// </summary>
    public void HostLobby() {
        CheckNetworkManager();
        networkManager.StartHost();
        panel_play.SetActive(false);
        background.SetActive(false);
    }
    /// <summary>
    /// Method used for starting game vs bots(offline game).
    /// </summary>
    public void PlayOffline()
    {
        CheckNetworkManager();
        panel_play.SetActive(false);
        background.SetActive(false);
        SceneManager.LoadScene("Scene_Offline");
        
    }
    /// <summary>
    /// Method used for showing players statistics and results of his games.
    /// </summary>
    public async void ViewResults() {
        panel_scoreHolder.SetActive(true);
        background.SetActive(false);
        button_retunFromScoreBoard.SetActive(true);
        ReadGameResults read = GetComponent<ReadGameResults>();
        await read.GetPlayerGameHistory();
        await read.GetPlayerStats();
    }

    private void SetUpScoreBoard()
    {
        GameObject scoreBoard = FindObjectOfType<ScoreBoard>().gameObject;
        GameObject canvasMenu = GameObject.Find("Canvas_Menu");
        scoreBoard.transform.SetParent(canvasMenu.transform, false);
        scoreBoard.GetComponent<RectTransform>().anchorMin = new Vector2(0.3f, 0.3f);
        scoreBoard.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f, 0.7f);
        scoreBoard.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        scoreBoard.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        scoreBoard.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        scoreBoard.transform.rotation = canvasMenu.transform.rotation;
        panel_menu.SetActive(false);
        background.SetActive(false);
        button_retunFromScoreBoard.SetActive(true);
    }
    /// <summary>
    /// Method used for hiding score board when leaving window with player history.
    /// </summary>
    public void HideScoreBoard() {
        if (FindObjectOfType<ScoreBoard>()) {
            GameObject scoreBoard = FindObjectOfType<ScoreBoard>().gameObject;
            Destroy(GameObject.Find("Panel_ScoreBoard"));
            Destroy(GameObject.Find("Panel_ScoreBoard"));
            scoreBoard.SetActive(false);
            
        }
        if (panel_scoreHolder.activeSelf) {
            foreach (Transform transform in panel_results.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }
            panel_scoreHolder.SetActive(false);
        }
    }

    private void CheckNetworkManager()
    {
        if (networkManager == null)
        {
            networkManager = Instantiate(networkManagerPrefab.GetComponent<NetworkManagerHearts>());
        }
    }

}
