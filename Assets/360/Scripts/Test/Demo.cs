using Ardez.Model;
using Quest;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Config.GetInstance().ReadStuffs(ReadStuffsFinish);
    }

    private void ReadStuffsFinish(List<HotStuff> list) 
    {
        foreach (var data in list)
        {
            Quest.LogManager.instance.DebugLog("读取配置表" + data.galleryId);
        }
    }
}
