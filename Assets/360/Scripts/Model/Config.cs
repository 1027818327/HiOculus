using Ardez.Model;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Config
{
    private static Config instance;

    private List<HotStuff> hotStuffs;


    public static Config GetInstance() 
    {
        if (instance == null) 
        {
            instance = new Config();
        }
        return instance;
    }

    private Config() 
    {
        
    }

    public void ReadStuffs(Action<List<HotStuff>> response) 
    {
        if (hotStuffs != null) 
        {
            if (response != null)
            {
                response(hotStuffs);
            }
            return;
        }

        string path = Path.Combine(Application.streamingAssetsPath, "HotStuff.json");
        AppMain.instance.StartCoroutine(AppMain.instance.ReadFile(path, (json)=> 
        {
            hotStuffs = new List<HotStuff>();

            SimpleJSON.JSONNode tempNode = SimpleJSON.JSON.Parse(json);
            for (int i = 0; i < tempNode.Count; i++)
            {
                HotStuff tempHs = new HotStuff();
                tempHs.galleryId = tempNode[i]["galleryId"];
                tempHs.mainPlate = tempNode[i]["mainPlate"];
                tempHs.markGraph = tempNode[i]["markGraph"];
                tempHs.effectPicId = tempNode[i]["effectPicId"];
                tempHs.configFile = tempNode[i]["configFile"];

                hotStuffs.Add(tempHs);
            }

            if (response != null) 
            {
                response(hotStuffs);
            }
        }));
    }
}
