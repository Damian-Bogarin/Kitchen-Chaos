using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Loader 
{
    private static Scene targetSceneIndex;
    
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene
    }

    public static void Load(Scene targetSceneName)
    {
        Loader.targetSceneIndex = targetSceneName;



        SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetSceneIndex.ToString());
    }

}
