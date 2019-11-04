using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Repository
{
    public static string record = "0";
    public static bool isInfinite = false;
    public static Dictionary<string, string> sData;

    public static void UpdateData(string key, string value)
    {
        string tmp = "";

        sData[key] = value;
        foreach (KeyValuePair<string, string> target in sData)
        {
            tmp += target.Key + ',' + target.Value + '\n';
        }

        GooglePlayGameServiceManager.SaveToCloud(tmp);
        GooglePlayGameServiceManager.LoadFromCloud();
    }

    public static string GetRecord()
    {
        sData.TryGetValue("Record", out record);

        return "최고 " + record + "단계";
    }
}
