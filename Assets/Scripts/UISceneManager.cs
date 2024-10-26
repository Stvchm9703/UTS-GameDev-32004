using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneManager : MonoBehaviour
{
    public Scene startScene, level1Scene, level2Scene;
    public float tweenDuration = 0.5f;

    private RectTransform loadingScreen;
    private GameObject loadingScreenObj;
    [SerializeField] GameObject loadingScreenPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (loadingScreenPrefab == null)
        {
            loadingScreenPrefab = Resources.Load<GameObject>("Prefabs/LoadingScreen");
        }
        loadingScreenObj = Instantiate(loadingScreenPrefab, Vector3.zero, Quaternion.identity, this.transform);
        loadingScreen = loadingScreenObj.transform.Find("loadingPanel").GetComponent<RectTransform>();
        loadingScreen.sizeDelta = new Vector2(Screen.width, Screen.height);
        loadingScreen.anchoredPosition = Vector2.down * Screen.height;
        HideLoadingScreen();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public void LoadLevel(string level)
    {
        if (level == "Start") StartCoroutine(LoadSceneAsync("StartScene"));
        else if (level == "level1") StartCoroutine(LoadSceneAsync("level1"));
        else if (level == "level2") StartCoroutine(LoadSceneAsync("level2"));
        return;
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        ShowLoadingScreen();
        yield return new WaitForSeconds(1);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }


    public void ShowLoadingScreen()
    {
        StartCoroutine(TweenLoadingScreen(Vector2.down * Screen.height, Vector2.zero)); // Slide in from bottom
    }

    // Method to hide the loading screen
    public void HideLoadingScreen()
    {
        StartCoroutine(TweenLoadingScreen(Vector2.zero, Vector2.down * Screen.height)); // Slide out to bottom
    }

    IEnumerator TweenLoadingScreen(Vector2 startPos, Vector2 endPos)
    {
        float elapsedTime = 0f;
        while (elapsedTime < tweenDuration)
        {
            elapsedTime += Time.deltaTime;
            loadingScreen.anchoredPosition = Vector2.Lerp(
                startPos,
                endPos,
                elapsedTime / tweenDuration
            );
            yield return null;
        }

        // Ensure the final position is exactly the target position
        loadingScreen.anchoredPosition = endPos;
    }
}