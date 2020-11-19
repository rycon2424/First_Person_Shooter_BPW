using UnityEngine.SceneManagement;

public static class EnterLoading
{
    public static string sceneToLoad;

    public static void NextScene(string sceneName)
    {
        sceneToLoad = sceneName;
        SceneManager.LoadScene("LoadingScreen");
    }
}