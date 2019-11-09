using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Repository
{
    public static string record = "0";
    public static bool isInfinite = false;
    public static Dictionary<string, string> sData = new Dictionary<string, string>();
    public static bool fLoading = false;

    public static void UpdateData(string key, string value)
    {
        string tmp = "";

        sData[key] = value;
        foreach (KeyValuePair<string, string> target in sData)
        {
            tmp += target.Key + ',' + target.Value + '\n';
        }

        GooglePlayGameServiceManager.SaveToCloud(tmp);
    }

    public static string GetRecord()
    {
        if (!sData.TryGetValue("Record", out record)) record = "0";

        return "최고 " + record + "단계";
    }
}
