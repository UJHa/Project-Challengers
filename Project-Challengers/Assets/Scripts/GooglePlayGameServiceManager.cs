using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GooglePlayGameServiceManager
{
    private static string sContent;
    private static readonly string sfilename = "Challengers";

    public static void Init()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .EnableSavedGames()
            .RequestServerAuthCode(false)
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public static bool CheckLogin()
    {
        return Social.localUser.authenticated;
    }

    //저장
    public static void SaveToCloud(string content)
    {
        if (!CheckLogin())
        {
            return;
        }
        sContent = content;
        OpenSavedGame(sfilename, true);
    }

    static void OpenSavedGame(string filename, bool bSave)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (bSave)
        {
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave);
        }
        else
        {
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead);
        }
    }

    //콜백함수
    static void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // 파일 준비 완료
            SaveGame(game, Encoding.UTF8.GetBytes(sContent), DateTime.Now.TimeOfDay);
        }
        else
        {
            Debug.Log("파일 열기 실패");
        }
    }

    static void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("데이터 저장 완료");
        } 
        else
        {
            Debug.Log("데이터 저장 실패");
        }
    }

    //파일 읽기
    public static void LoadFromCloud()
    {
        if (!CheckLogin())
        {
            return;
        }
        OpenSavedGame(sfilename, false);
    }

    static void OnSavedGameOpenedToRead(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            LoadGameData(game);
        }
        else
        {
            Debug.Log("파일 열기 실패");
        }
    }

    static void LoadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    static void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("TMPBYTES : " + data);
            Debug.Log("TMPSTRING : " + Encoding.Default.GetString(data));
            string tmpData = Encoding.Default.GetString(data);
            foreach (string saved in tmpData.Split('\n'))
            {
                string[] tmp = saved.Split(',');
                Repository.sData.Add(tmp[0], tmp[1]);
            }

            Repository.fLoading = true;
            Debug.Log("읽기 성공");
        }
        else
        {
            Debug.Log("읽기 실패");
        }
    }
}
