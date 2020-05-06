using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// 主程序启动类
public class AppMain : MonoBehaviour
{
    public static AppMain instance;

    public static WaitForEndOfFrame oneFrame = new WaitForEndOfFrame();

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        GameObject tempSingleObj = new GameObject(typeof(AppMain).Name);
        //tempSingleObj.hideFlags = HideFlags.HideInHierarchy;
        AppMain tempScript = tempSingleObj.AddComponent<AppMain>();
        GameObject.DontDestroyOnLoad(tempSingleObj);
    }

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator ReadFile(string path, Action<string> response)
    {
        UnityWebRequest tempRequest = UnityWebRequest.Get(path);
        yield return tempRequest.SendWebRequest();

        if (response != null) 
        {
            response(tempRequest.downloadHandler.text);
        }

        if (tempRequest != null && tempRequest.isDone) 
        {
            tempRequest.Dispose();
        }
    }
}