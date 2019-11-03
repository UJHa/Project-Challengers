using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Repository
{
    static int record = 28;
    public static bool isInfinite = false;

    public static string GetRecord()
    {
        return "최고 " + record + "단계";
    }
}
