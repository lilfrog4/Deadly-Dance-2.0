using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using SFB;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class CBmenuScript : MonoBehaviour
{
    public string SelectedBattleName;
    public string SelectedBattleSongName;
    public GameObject ScrollWindows;
    private RectTransform SWrectTransf;
    private float ScrollViewSize;
    public GameObject AddBattleWindow;
    public GameObject ConfirmDeletionWindow;
    public GameObject NoBattlesText;
    public GameObject LoadingScreen;
    private int MenuPage;
    private int PageCount;
    public GameObject PageText;
    private GameObject[] BattleFrames = new GameObject[10];
    public Dictionary<string, Battle> AllBattles = new Dictionary<string, Battle>();

    public class Battle
    {
        public string battleName;
        public string songName;
    }

    public void BackToMM()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GetBattles()
    {
        AllBattles = new Dictionary<string, Battle>();

        string CBpath = Path.Combine(Application.persistentDataPath, "Battles");

        if (!Directory.Exists(CBpath))
        {
            Directory.CreateDirectory(CBpath);
        }

        string[] battleNames = Directory.GetDirectories(CBpath).Select(Path.GetFileName).ToArray();

        foreach (string name in battleNames)
        {
            string songnamepath = Path.Combine(CBpath, name, "SongName.txt");
            string songname = File.ReadAllText(songnamepath);
            Battle battle = new Battle();
            battle.battleName = name;
            battle.songName = songname;

            AllBattles[name] = battle;
        }
        // Debug.Log(AllBattles.Count);
        if (AllBattles.Count == 0)
        {
            NoBattlesText.SetActive(true);
        }
        else
        {
            NoBattlesText.SetActive(false);
        }

        PageCount = (int)Math.Ceiling(AllBattles.Count / 10f);
        if (PageCount == 0)
        {
            PageCount = 1;
            MenuPage = 1;
        }
    }

    public void PlayBattle(GameObject battleName)
    {
        StartCoroutine(PrepareBattle(battleName));
    }

    public IEnumerator PrepareBattle(GameObject battleName)
    {
        SelectedBattleName = battleName.GetComponent<TMP_Text>().text;
        SelectedBattleSongName = battleName.transform.parent.Find("SongName_text").GetComponent<TMP_Text>().text;
        BattleManagerScript.battleName = battleName.GetComponent<TMP_Text>().text;

        string songPath = Path.Combine(Application.persistentDataPath, "Battles", SelectedBattleName, "Song.mp3");
        string FsongPath = "file://" + songPath;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(FsongPath, AudioType.MPEG))
        {
            LoadingScreen.SetActive(true);
            yield return www.SendWebRequest();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            BattleManagerScript.battleSong = clip;
            BattleManagerScript.songName = SelectedBattleSongName;
        }

        SceneManager.LoadScene("CB_Scene");
    }

    public void DisplayBattles()
    {
        Dictionary<string, Battle> BattleBatch = new Dictionary<string, Battle>();
        BattleBatch = AllBattles.Skip((MenuPage - 1) * 10).Take(MenuPage * 10).ToDictionary(x => x.Key, x => x.Value);
        Debug.Log(BattleBatch.Count);

        foreach (GameObject frame in BattleFrames)
        {
            frame.SetActive(false);
        }

        int count = 0;
        foreach (KeyValuePair<string, Battle> pair in BattleBatch)
        {
            BattleFrames[count].SetActive(true);
            BattleFrames[count].transform.Find("BattleName_text").GetComponent<TMP_Text>().text = pair.Value.battleName;
            BattleFrames[count].transform.Find("SongName_text").GetComponent<TMP_Text>().text = pair.Value.songName.Replace(".mp3", "");

            count += 1;
        }

        SWrectTransf.sizeDelta = new Vector2(SWrectTransf.sizeDelta.x, ScrollViewSize * BattleBatch.Count / 10f);
    }

    public void ToggleAddBattleMenu(bool active)
    {
        AddBattleWindow.SetActive(active);
    }

    public void ToggleDeleteConfirmation(bool active)
    {
        ConfirmDeletionWindow.SetActive(active);
    }

    public void SetSelectedBattle(GameObject battleName)
    {
        SelectedBattleName = battleName.GetComponent<TMP_Text>().text;
    }

    public void DeleteBattle()
    {
        string pathToDelete = Path.Combine(Application.persistentDataPath, "Battles", SelectedBattleName);

        if (!Directory.Exists(pathToDelete))
        {
            GetBattles();
            DisplayBattles();
            return;
        }

        Directory.Delete(pathToDelete, true);
        GetBattles();
        
        if (MenuPage > PageCount)
        {
            MenuPage = PageCount;
            PageText.GetComponent<TMP_Text>().text = "Page " + MenuPage.ToString();
        }

        DisplayBattles();
        ConfirmDeletionWindow.SetActive(false);
    }

    public void AddSong()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open file", "", "mp3", false);

        if (paths == null || paths.Length == 0)
        {
            return;
        }

        string sourceFilePath = paths[0];

        if (!File.Exists(sourceFilePath))
        {
            return;
        }

        string fileName = Path.GetFileName(sourceFilePath);

        AddBattleWindow.transform.Find("SongNameText").GetComponent<TMP_Text>().text = fileName;
        AddBattleWindow.transform.Find("SongPathText").GetComponent<TMP_Text>().text = sourceFilePath;
    }

    public void SwitchBattleMenuPage(int pageDelta)
    {
        if (!(MenuPage == 1 & pageDelta < 0) & !(MenuPage == PageCount & pageDelta > 0))
        {
            MenuPage += pageDelta;
            PageText.GetComponent<TMP_Text>().text = "Page " + MenuPage.ToString();
            DisplayBattles();
        }
    }

    public void AddBattle()
    {
        string battleName = AddBattleWindow.transform.Find("BattleName").GetChild(0).Find("Text").GetComponent<TMP_Text>().text.Replace("\u200b", "").Trim();
        string songPath = AddBattleWindow.transform.Find("SongPathText").GetComponent<TMP_Text>().text;
        string songName = AddBattleWindow.transform.Find("SongNameText").GetComponent<TMP_Text>().text;

        if (battleName == "" || songPath == "" || !File.Exists(songPath))
        {
            return;
        }

        string DirPath = Path.Combine(Application.persistentDataPath, "Battles", battleName, "Sprites");
        string DirPath1 = Path.Combine(Application.persistentDataPath, "Battles", battleName);
        Directory.CreateDirectory(DirPath);

        File.WriteAllText(DirPath1 + "/Chart.json", "");
        File.WriteAllText(DirPath1 + "/Objects.json", "");
        File.WriteAllText(DirPath1 + "/SongName.txt", songName);

        File.Copy(songPath, DirPath1 + "/Song.mp3", true);

        GetBattles();
        DisplayBattles();
        AddBattleWindow.SetActive(false);
    }

    public void OpenChartEditor(GameObject BattleName)
    {
        BattleManagerScript.battleName = BattleName.GetComponent<TMP_Text>().text;
        string songName = BattleName.transform.parent.Find("SongName_text").GetComponent<TMP_Text>().text;
        BattleManagerScript.songName = songName;
        SceneManager.LoadScene("Chart_Editor");
    }

    private void GetBattleFrames()
    {
        BattleFrames[0] = ScrollWindows.transform.Find("BattleContainer1").gameObject;
        BattleFrames[1] = ScrollWindows.transform.Find("BattleContainer2").gameObject;
        BattleFrames[2] = ScrollWindows.transform.Find("BattleContainer3").gameObject;
        BattleFrames[3] = ScrollWindows.transform.Find("BattleContainer4").gameObject;
        BattleFrames[4] = ScrollWindows.transform.Find("BattleContainer5").gameObject;
        BattleFrames[5] = ScrollWindows.transform.Find("BattleContainer6").gameObject;
        BattleFrames[6] = ScrollWindows.transform.Find("BattleContainer7").gameObject;
        BattleFrames[7] = ScrollWindows.transform.Find("BattleContainer8").gameObject;
        BattleFrames[8] = ScrollWindows.transform.Find("BattleContainer9").gameObject;
        BattleFrames[9] = ScrollWindows.transform.Find("BattleContainer10").gameObject;
    }

    void Start()
    {
        Time.timeScale = 1f;
        SWrectTransf = ScrollWindows.GetComponent<RectTransform>();
        ScrollViewSize = SWrectTransf.sizeDelta.y;
        
        MenuPage = 1;
        LoadingScreen.SetActive(false);
        AddBattleWindow.SetActive(false);
        ConfirmDeletionWindow.SetActive(false);
        GetBattleFrames();
        GetBattles();
        DisplayBattles();
    }


    void Update()
    {
        
    }
}