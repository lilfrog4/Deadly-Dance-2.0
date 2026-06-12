using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class EditorScript : MonoBehaviour
{


    public Sprite OneWide;
    public Sprite TwoWide;
    public Sprite ThreeWide;
    public Sprite FourWide;
    public Sprite FiveWide;

    public Sprite OneWideS;
    public Sprite TwoWideS;
    public Sprite ThreeWideS;
    public Sprite FourWideS;
    public Sprite FiveWideS;

    public Sprite WallShield;
    public Sprite Wall;

    public Sprite OneWideOutline;
    public Sprite TwoWideOutline;
    public Sprite ThreeWideOutline;
    public Sprite FourWideOutline;
    public Sprite FiveWideOutline;

    public Sprite OneWideSOutline;
    public Sprite TwoWideSOutline;
    public Sprite ThreeWideSOutline;
    public Sprite FourWideSOutline;
    public Sprite FiveWideSOutline;

    public Sprite RegularWallOutline;
    public Sprite ShieldWallOutline;
    public Sprite CrackerNoteOutline;

    private Sprite[] SimpleNoteSprites;
    private Sprite[] SimpleShieldNoteSprites;
    private Sprite[] WallNoteSprites;

    private Sprite[] SimpleNoteOutlines;
    private Sprite[] SimpleShieldNoteOutlines;
    private Sprite[] WallOutlines;



    public string BattleName;
    public GameObject AudioSource;

    private GameObject WidthButtonsContainer;

    public GameObject SpriteWindow;
    private GameObject[] SpriteWindows;
    public GameObject ObjectWindow;
    private GameObject[] ObjectWindows;
    private int ObjectPageNumber;
    public GameObject ObjectPageSwitcher;
    private bool ObjectEditMode;
    public GameObject PageToggleButton;
    public GameObject AddObjectButton;
    public GameObject ObjectEditPage;
    public GameObject PauseMenu;
    private int SpriteWindowPage;
    private int SpritePageCount;
    private int ObjectPageCount;
    public GameObject SpritePageText;
    public GameObject ObjectPageText;
    public Texture2D MissingTexturePic;

    private float scaleFactor;
    private float baseNoteScaling = 40f;
    private float NotesPerScreen;
    public TextAsset ImportedChart;
    private bool Importing;
    private bool Paused = false;

    public GameObject ImportScreen;
    public GameObject TextFlash;

    public Canvas MainCanvas;
    public GameObject Parameter_Panel;
    public Button SimpleNoteButton;
    public Button WallNoteButton;
    public Button CrackerNoteButton;

    public Button AlphaTriggerButton;
    public Button SpriteTriggerButton;

    public Button SelectButton;
    public Button DefaultColorButton;
    public Button DefaultSpeedButton;

    public GameObject Selected_Note;
    public GameObject Selected_Trigger;

    public Button Selected_Tool_Button;
    public Button SelectedColorButton;
    public Button SelectedSpeedButton;
    public bool SelectedPunchable;
    public GameObject PunchableToggle;
    public GameObject SpeedValueObject;
    public GameObject DelayValueObject;
    private string DelayValueText;
    public string SpeedValueText;
    public GameObject Cursor;       // Курсор, показывающий куда ставится нота
    public Button SimpleNote;
    public Button CrackerNote;
    public Button WallNote;

    public Button AlphaTrigger;
    public Button SpriteTrigger;

    public GameObject Chart_Container;
    public GameObject Note_Container;       // Контейнер для нот - когда скроллишь, он смещается на 20f по своим локальным координатам
    private Vector3 NCInitialPos;
    // public GameObject MusicGuideline;
    private bool PlaybackActive = false;        // Активен плейбек, или нет. Когда активен - должна идти музыка, когда нет - останавливаться.
    private List<float> Lane_X_positions = new List<float>() {0.80f, 0.89f, 0.98f, 1.07f, 1.17f, 1.27f, 1.34f, 1.41f, 1.48f};       // В MousePosition X-координата всегда идет от 0 до 2, независимо от размера экрана.
    // private List<float> Trigger_Lane_X_positions = new List<float>() {};
    private List<float> Lane_X;

    
    private int Closest_Lane;
    private float Closest_Y;        // Ближайшая Y-координата точки на экране, кратная 20.
    private float Local_Y;          // Closest_Y, переведенная в координаты контейнера нот.
    private float YParallax = 0f;       // Смещение по вертикали, кратно 20 (В САМОМ НАЧАЛЕ ТРЕКА РАВЕН -300, НАДО УЧЕСТЬ)
    // private float GlobalNCStep;
    private Vector3 MousePosition;
    public List<float> Occupied_Coords = new List<float>();         // Занятые координаты, чтоб ноты не ставились друг на друга.
    public Dictionary<int, List<int>> OCdict = new Dictionary<int, List<int>>();

    private Coroutine PlayBackCoroutine;

    private float YParallaxMultiplier = 4f;

    public AudioSource sound;

    public GameObject timestamp1;
    public GameObject timestamp2;
    public GameObject timestamp3;

    private float TimeSinceCall = 0f;

    private float TimeOnTrack;
    private float timestamp_value;


    public float YPvalue1;
    public float YPvalue2;

    public int LastNoteID = 0;
    public int[] FreeNoteIDs = new int[0];
    private int CurrentChunk;

    // private static AudioClip Song;
    private static string SongName;

    private Coroutine TextFadeCoroutine;


    [System.Serializable]
    public class Battle_Sprite
    {
	    public string spriteName;
    }

    [System.Serializable]
    public class BattleSpriteList
    {
        public Battle_Sprite[] battle_sprite = new Battle_Sprite[0];
    }
    public BattleSpriteList AllBattleSprites = new BattleSpriteList();
    
    [System.Serializable]
    public class SceneObject
    {
        public int editorID;            // Уникальное id для редактора
        public List<int> objectIDs;            // Неуникальные id для триггеров
        public string spriteName;       // Спрайт, с которым объект начинает
        public float alpha;             // Альфа, с которым начинает
        public float Xpos;              // Стартовая X позиция
        public float Ypos;              // Стартовая Y позиция
        public float scale;             // Стартовый множитель масштаба
        public int zOrder;              // Порядок по Z
    }

    int LastObjectID = 0;

    public Dictionary<int, SceneObject> SceneObjects = new Dictionary<int, SceneObject>();      // int - editor ID
    private List<int> AllObjectGroups = new List<int>();

    public class SceneObjectList
    {
        public SceneObject[] scene_object = new SceneObject[0];
    }
    public SceneObjectList AllSceneObjects = new SceneObjectList();

    private Func<int, bool> NoteDrawInput = Input.GetMouseButtonDown;
    private bool DrawModeSwitch = true;
    

    [System.Serializable]
    public class Simple_Note
    {
        public int editorID;
        public List<float> color;
        public List<int> rand_lanes;
        public float delay;
        public int speed;
        public int width;
        public bool absorbable = false;
        public bool punchable = true;
    }

    [System.Serializable]
    public class SimpleNoteList
    {
        public Simple_Note[] simple_note = new Simple_Note[0];
    }
    public SimpleNoteList AllSimpleNotes = new SimpleNoteList();

    public class SNchunk            // чанк нот
    {
        public Simple_Note[] SNbatch = new Simple_Note[0];
    }
    // public SNchunk DrawnSN = new SNchunk();


    [System.Serializable]
    public class Wall_Note
    {
        public int editorID;
        public List<float> color;
        public List<int> rand_lanes;
        public float delay;
        public int speed;
        public bool absorbable = false;
        public bool punchable = true;
    }

    [System.Serializable]
    public class WallNoteList
    {
        public Wall_Note[] wall_note = new Wall_Note[0];
    }
    public WallNoteList AllWallNotes = new WallNoteList();

    public class WNchunk            // чанк нот
    {
        public Wall_Note[] WNbatch = new Wall_Note[0];
    }
    // public WNchunk DrawnWN = new WNchunk();


    [System.Serializable]
    public class Cracker_Note
    {
        public int editorID;
        public List<float> color;
        public List<int> rand_lanes;
        public float delay;
        public int speed;
        public bool absorbable = false;
        public bool punchable = true;
    }

    [System.Serializable]
    public class CrackerNoteList
    {
        public Cracker_Note[] cracker_note = new Cracker_Note[0];
    }
    public CrackerNoteList AllCrackerNotes = new CrackerNoteList();

    public class CNchunk            // чанк нот
    {
        public Cracker_Note[] CNbatch = new Cracker_Note[0];
    }
    // public CNchunk DrawnCN = new CNchunk();


    [System.Serializable]
    public class AllNoteList
    {
        public SimpleNoteList SimpleNotes = new SimpleNoteList();
        public WallNoteList WallNotes = new WallNoteList();
        public CrackerNoteList CrackerNotes = new CrackerNoteList();
    }
    public AllNoteList AllNotes = new AllNoteList();


    public Dictionary<int, SNchunk> SNdict = new Dictionary<int, SNchunk>();
    public Dictionary<int, WNchunk> WNdict = new Dictionary<int, WNchunk>();
    public Dictionary<int, CNchunk> CNdict = new Dictionary<int, CNchunk>();


    [System.Serializable]
    public class Alpha_Trigger
    {
        public int editorLane;
        public int editorID;
        public int usedGroup;
        public float delay;
        public float duration;
        public float opacity;
    }

    [System.Serializable]
    public class AlphaTriggerList
    {
        public Alpha_Trigger[] alpha_trigger = new Alpha_Trigger[0];
    }
    public AlphaTriggerList AllAlphaTriggers = new AlphaTriggerList();

    public class ATchunk
    {
        public Alpha_Trigger[] ATbatch = new Alpha_Trigger[0];
    }


    [System.Serializable]
    public class Sprite_Trigger
    {
        public int editorLane;
        public int editorID;
        public int usedGroup;
        public float delay;
        public string spritename;
    }

    [System.Serializable]
    public class SpriteTriggerList
    {
        public Sprite_Trigger[] sprite_trigger = new Sprite_Trigger[0];
    }
     public SpriteTriggerList AllSpriteTriggers = new SpriteTriggerList();

    public class STchunk
    {
        public Sprite_Trigger[] STbatch = new Sprite_Trigger[0];
    }


    [System.Serializable]
    public class AllTriggerList
    {
        public AlphaTriggerList AlphaTriggers = new AlphaTriggerList();
        public SpriteTriggerList SpriteTriggers = new SpriteTriggerList();
    }
    public AllTriggerList AllTriggers = new AllTriggerList();

    public Dictionary<int, ATchunk> ATdict = new Dictionary<int, ATchunk>();
    public Dictionary<int, STchunk> STdict = new Dictionary<int, STchunk>();
    

    private int[] DrawnChunks = new int[3];


    private IEnumerator LoadSong()
    {
        Debug.Log(BattleManagerScript.songName);
        Debug.Log(BattleManagerScript.battleSong);
        if (BattleManagerScript.battleSong != null & BattleManagerScript.songName == SongName)
        {
            AudioSource.GetComponent<AudioSource>().clip = BattleManagerScript.battleSong;

            yield return null;
            ImportScreen.SetActive(false);

            yield break;
        }

        string songPath = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Song.mp3");
        string FsongPath = "file://" + songPath;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(FsongPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            AudioSource.GetComponent<AudioSource>().clip = clip;

            ImportScreen.SetActive(false);

            // Song = clip;
            SongName = BattleManagerScript.songName;

            BattleManagerScript.battleSong = clip;
        }
    }

    public void PlayBattle()
    {
        ExportChart();
        BattleManagerScript.battleName = BattleName;

        SceneManager.LoadScene("CB_Scene");
    }

    void SetSelectedNote(Button Note)           // Задает выбранную ноту
    {
        if (Selected_Trigger != null)
        {
            Selected_Trigger.GetComponent<Image>.color = new Color (1f, 1f, 1f, 1f);
        }

        Selected_Trigger = null;

        if (Selected_Note != null)
        {
            Color NoteColor = Selected_Note.GetComponent<Image>().color;
            Selected_Note.GetComponent<Image>().color = new Color32(System.Convert.ToByte(NoteColor.r * 255), System.Convert.ToByte(NoteColor.g * 255), System.Convert.ToByte(NoteColor.b * 255), System.Convert.ToByte(255f));
            Selected_Note.transform.GetChild(0).GetComponent<Image>().color = new Color32(System.Convert.ToByte(255), System.Convert.ToByte(255), System.Convert.ToByte(255), System.Convert.ToByte(255f));
        }
        Selected_Note = Note.gameObject;
        Color Note_Color = Selected_Note.GetComponent<Image>().color;
        // Selected_Note.GetComponent<Image>().color = new Color(79f / 255f, 79f / 255f, 79f / 255f);
        Selected_Note.GetComponent<Image>().color = new Color32(System.Convert.ToByte(Note_Color.r * 255), System.Convert.ToByte(Note_Color.g * 255), System.Convert.ToByte(Note_Color.b * 255), System.Convert.ToByte(70f));
        Selected_Note.transform.GetChild(0).GetComponent<Image>().color = new Color32(System.Convert.ToByte(255), System.Convert.ToByte(255), System.Convert.ToByte(255), System.Convert.ToByte(50f));

        SpeedValueText = Selected_Note.GetComponent<SimpleNoteStats>().speed.ToString();
        DelayValueText = Selected_Note.GetComponent<SimpleNoteStats>().delay.ToString();

        string noteType = Selected_Note.GetComponent<SimpleNoteStats>().type;
        if (noteType == "wall_note" || noteType == "simple_note")
        {
            PunchableToggle.GetComponent<Toggle>().interactable = true;
        }
        else
        {
            PunchableToggle.GetComponent<Toggle>().interactable = false;
        }

        PunchableToggle.GetComponent<Toggle>().isOn = Selected_Note.GetComponent<SimpleNoteStats>().Punchable;
    }

    void SetSelectedTrigger(Button Trigger)
    {
        if (Selected_Note != null)
        {
            Color NoteColor = Selected_Note.GetComponent<Image>().color;
            Selected_Note.GetComponent<Image>().color = new Color32(System.Convert.ToByte(NoteColor.r * 255), System.Convert.ToByte(NoteColor.g * 255), System.Convert.ToByte(NoteColor.b * 255), System.Convert.ToByte(255f));
            Selected_Note.transform.GetChild(0).GetComponent<Image>().color = new Color32(System.Convert.ToByte(255), System.Convert.ToByte(255), System.Convert.ToByte(255), System.Convert.ToByte(255f));
        }

        if (Selected_Trigger != null)
        {
            Selected_Trigger.GetComponent<Image>.color = new Color (1f, 1f, 1f, 1f);
        }

        Selected_Note = null;
        
        Selected_Trigger = Trigger.gameObject;
        Selected_Trigger.GetComponent<Image>.color = new Color (1f, 1f, 1f, 0.5f);
    }


    public void SetSelectedToolButton(Button button)            // Задает выбранную кнопку вверху
    {
        if (Selected_Tool_Button != null)
        {
            Selected_Tool_Button.GetComponent<Image>().color = Color.white;
        }
        Selected_Tool_Button = button;
        Selected_Tool_Button.GetComponent<Image>().color = new Color(79f / 255f, 79f / 255f, 79f / 255f);
        // WallNoteButton.GetComponent<Image>().color = new Color(79f / 255f, 79f / 255f, 79f / 255f);
    }

    private void GetClosestLane()           // Вычисляет ближайшие координаты для добавления ноты
    {
        float min = 9999;
        foreach (float i in Lane_X_positions)
        {
            float difference = Mathf.Abs(MousePosition.x - i);
            if (difference < min)
            {
                min = difference;
                // Closest_Lane = i;
                Closest_Lane = Lane_X_positions.IndexOf(i);
            }
        }
        Closest_Y = (int)Mathf.Round(Input.mousePosition.y / (baseNoteScaling * scaleFactor)) * baseNoteScaling * scaleFactor;
        Local_Y = Note_Container.transform.InverseTransformPoint(0f, Closest_Y, 0f).y;
    }

    public void SetNoteWidth(int Width)         // Задает ширину ноты
    {
        Selected_Note.gameObject.GetComponent<SimpleNoteStats>().width = Width;
        Selected_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * Width * 1.3f, 1 * 18);                                     // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Selected_Note.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(64 * Width * 1.3f, 1 * 18);

        if (Selected_Note.GetComponent<SimpleNoteStats>().type == "simple_note")
        {
            if (Selected_Note.GetComponent<SimpleNoteStats>().Punchable)
            {
                Selected_Note.GetComponent<Image>().sprite = SimpleNoteSprites[Width - 1];
                Selected_Note.transform.GetChild(0).GetComponent<Image>().sprite = SimpleNoteOutlines[Width - 1];
            }
            else
            {
                Selected_Note.GetComponent<Image>().sprite = SimpleShieldNoteSprites[Width - 1];
                Selected_Note.transform.GetChild(0).GetComponent<Image>().sprite = SimpleShieldNoteOutlines[Width - 1];
            }
        }

        if (Width == 1 || Width == 3 || Width == 5)
        {
            Selected_Note.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        }
        else if (Width == 2)
        {
            Selected_Note.GetComponent<RectTransform>().pivot = new Vector2(0.20f, 0.5f);
        }
        else if (Width == 4)
        {
            Selected_Note.GetComponent<RectTransform>().pivot = new Vector2(0.11f, 0.5f);
        }

        int ChunkID = (int)Math.Ceiling(Selected_Note.GetComponent<SimpleNoteStats>().delay / 0.7f);

        foreach (Simple_Note SN in SNdict[ChunkID].SNbatch)
        {
            if (SN.editorID == Selected_Note.GetComponent<SimpleNoteStats>().editorID)
            {
                SN.width = Width;
            }
        }
    }

    public void SetWidth(GameObject Note, int Width)
    {
        Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * Width * 1.3f, 1 * 18);
        Note.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(64 * Width * 1.3f, 1 * 18);
        if (Note.GetComponent<SimpleNoteStats>().type == "simple_note")
        {
            if (Note.GetComponent<SimpleNoteStats>().Punchable)
            {
                Note.GetComponent<Image>().sprite = SimpleNoteSprites[Width - 1];
                Note.transform.GetChild(0).GetComponent<Image>().sprite = SimpleNoteOutlines[Width - 1];
            }
            else
            {
                Note.GetComponent<Image>().sprite = SimpleShieldNoteSprites[Width - 1];
                Note.transform.GetChild(0).GetComponent<Image>().sprite = SimpleShieldNoteOutlines[Width - 1];
            }
        }
        else if (Note.GetComponent<SimpleNoteStats>().type == "wall_note")
        {
            int num = Note.GetComponent<SimpleNoteStats>().Punchable ? 1 : 0;
            Note.GetComponent<Image>().sprite = WallNoteSprites[num];
            Note.transform.GetChild(0).GetComponent<Image>().sprite = WallOutlines[num];
        }

        if (Width == 1 || Width == 3 || Width == 5)
        {
            Note.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        }
        else if (Width == 2)
        {
            Note.GetComponent<RectTransform>().pivot = new Vector2(0.20f, 0.5f);
        }
        else if (Width == 4)
        {
            Note.GetComponent<RectTransform>().pivot = new Vector2(0.11f, 0.5f);
        }
    }

    public void SetSelectedSpeedButton(Button SpeedButton)
    {
        if (SelectedSpeedButton != null)
        {
            SelectedSpeedButton.transform.parent.Find("Highlight").gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f);
        }
        SelectedSpeedButton = SpeedButton;
        SelectedSpeedButton.transform.parent.Find("Highlight").gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);

        int speed = int.Parse(SpeedButton.transform.Find("Value").gameObject.GetComponent<TMP_Text>().text);

        if (Selected_Note != null)
        {
            Selected_Note.GetComponent<SimpleNoteStats>().speed = speed;
            SpeedValueText = SpeedButton.transform.Find("Value").gameObject.GetComponent<TMP_Text>().text;
        }
        else
        {
            return;
        }

            int NoteID = Selected_Note.GetComponent<SimpleNoteStats>().editorID;
            int ChunkID = (int)Math.Ceiling(Selected_Note.GetComponent<SimpleNoteStats>().delay / 0.7f);

            if (ChunkID == 0)
            {
                ChunkID = 1;
            }

            if (Selected_Note.GetComponent<SimpleNoteStats>().type == "simple_note")
            {
                foreach (Simple_Note SN in SNdict[ChunkID].SNbatch)
                {
                    if (SN.editorID == NoteID)
                    {
                        SN.speed = speed;
                    }
                }
            }
            else if (Selected_Note.GetComponent<SimpleNoteStats>().type == "wall_note")
            {
                foreach (Wall_Note WN in WNdict[ChunkID].WNbatch)
                {
                    if (WN.editorID == NoteID)
                    {
                        WN.speed = speed;
                    }
                }
            }
            else if (Selected_Note.GetComponent<SimpleNoteStats>().type == "cracker_note")
            {
                foreach (Cracker_Note CN in CNdict[ChunkID].CNbatch)
                {
                    if (CN.editorID == NoteID)
                    {
                        CN.speed = speed;
                    }
                }
            }
    }


    // public void SetNoteColor(int color)         // Задает цвет ноты
    // {
    //     // List<float> NoteColor = new List<float>() {color[0], color[1], color[2]};
    //     Selected_Note.GetComponent<Image>().color = new Color(Mathf.Round((float)color / 1000000) / 255, Mathf.Round((float)color / 1000) % 1000 / 255, (float)(color % 1000) / 255);
    //     Selected_Note.GetComponent<SimpleNoteStats>().color = new List<float> { Selected_Note.GetComponent<Image>().color.r * 255, Selected_Note.GetComponent<Image>().color.g * 255, Selected_Note.GetComponent<Image>().color.b * 255 };
    // }

    public void SetSelectedColor(Button ColorButton)
    {
        if (SelectedColorButton != null)
        {
            SelectedColorButton.transform.parent.Find("Highlight").gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f);
        }
        SelectedColorButton = ColorButton;
        SelectedColorButton.transform.parent.Find("Highlight").gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);

        if (Selected_Note != null)
        {
            Selected_Note.GetComponent<Image>().color = SelectedColorButton.colors.normalColor;
            Selected_Note.GetComponent<SimpleNoteStats>().color = new List<float> { SelectedColorButton.colors.normalColor.r * 255, SelectedColorButton.colors.normalColor.g * 255, SelectedColorButton.colors.normalColor.b * 255 };

            int NoteID = Selected_Note.GetComponent<SimpleNoteStats>().editorID;
            int ChunkID = (int)Math.Ceiling(Selected_Note.GetComponent<SimpleNoteStats>().delay / 0.7f);
            // if (ChunkID == 0)
            // {
            //     ChunkID = 1;
            // }

            if (Selected_Note.GetComponent<SimpleNoteStats>().type == "simple_note")
            {
                foreach (Simple_Note SN in SNdict[ChunkID].SNbatch)
                {
                    if (SN.editorID == NoteID)
                    {
                        SN.color = new List<float> { Selected_Note.GetComponent<Image>().color.r * 255, Selected_Note.GetComponent<Image>().color.g * 255, Selected_Note.GetComponent<Image>().color.b * 255 };
                    }
                }
            }
            else if (Selected_Note.GetComponent<SimpleNoteStats>().type == "wall_note")
            {
                foreach (Wall_Note WN in WNdict[ChunkID].WNbatch)
                {
                    if (WN.editorID == NoteID)
                    {
                        WN.color = new List<float> { Selected_Note.GetComponent<Image>().color.r * 255, Selected_Note.GetComponent<Image>().color.g * 255, Selected_Note.GetComponent<Image>().color.b * 255 };
                    }
                }
            }
            else if (Selected_Note.GetComponent<SimpleNoteStats>().type == "cracker_note")
            {
                foreach (Cracker_Note CN in CNdict[ChunkID].CNbatch)
                {
                    if (CN.editorID == NoteID)
                    {
                        CN.color = new List<float> { Selected_Note.GetComponent<Image>().color.r * 255, Selected_Note.GetComponent<Image>().color.g * 255, Selected_Note.GetComponent<Image>().color.b * 255 };
                    }
                }
            }
        }
    }

    public void ActivateSNoutline(bool active)
    {
        if (Selected_Note != null)
        {
            Selected_Note.transform.GetChild(0).gameObject.SetActive(active);
        }
    }

    public void SwapSelectedPunchable()
    {

        SelectedPunchable = !SelectedPunchable;

        if (Selected_Note != null)
        {
            Selected_Note.GetComponent<SimpleNoteStats>().Punchable = SelectedPunchable;
            

            int NoteID = Selected_Note.GetComponent<SimpleNoteStats>().editorID;
            int ChunkID = (int)Math.Ceiling(Selected_Note.GetComponent<SimpleNoteStats>().delay / 0.7f);

            if (Selected_Note.GetComponent<SimpleNoteStats>().type == "simple_note")
            {
                foreach (Simple_Note SN in SNdict[ChunkID].SNbatch)
                {
                    if (SN.editorID == NoteID)
                    {
                        SN.punchable = SelectedPunchable;
                    }
                }
            }
            else if (Selected_Note.GetComponent<SimpleNoteStats>().type == "wall_note")
            {
                foreach (Wall_Note WN in WNdict[ChunkID].WNbatch)
                {
                    if (WN.editorID == NoteID)
                    {
                        WN.punchable = SelectedPunchable;
                    }
                }
            }

            SetWidth(Selected_Note, Selected_Note.GetComponent<SimpleNoteStats>().width);
        }
    }

    public void SetPunchToggleInteractable(bool interatable)
    {
        PunchableToggle.GetComponent<Toggle>().interactable = interatable;
    }

    private void SetNoteAnchors(Button Note)
    {
        Vector3[] NoteCorners = new Vector3[4];

            Note.GetComponent<RectTransform>().GetWorldCorners(NoteCorners);

            RectTransform NoteRT = Note.GetComponent<RectTransform>();
            RectTransform NoteParentRT = Note.GetComponent<RectTransform>().parent as RectTransform;

            Vector2 AnchorMin = new Vector2(0.5f, NoteRT.anchorMin.y + NoteRT.offsetMin.y / NoteParentRT.rect.height);
            Vector2 AnchorMax = new Vector2(0.5f, NoteRT.anchorMax.y + NoteRT.offsetMax.y / NoteParentRT.rect.height);

            Vector3 OrigSize = NoteRT.rect.size;
            Vector3 OrigPos = NoteRT.position;

            NoteRT.anchorMin = (AnchorMin + AnchorMax) / 2;
            NoteRT.anchorMax = (AnchorMin + AnchorMax) / 2;
            NoteRT.offsetMin = NoteRT.offsetMax = new Vector2(0f, 0f);

            NoteRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, OrigSize.x);
            NoteRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, OrigSize.y);
            NoteRT.position = OrigPos;

            // Debug.Log(Closest_Y);
            // Debug.Log(Local_Y);
    }

    public void AddTrigger(Button Trigger)
    {
        if (Closest_Lane < 5)
        {
            return;
        }

        float preCoords = (float)Math.Round(((Closest_Y / Screen.height * NotesPerScreen + YParallax / 20f) * 0.05f - 0.7f) * 400f) + Closest_Lane;
        float delay = (float)Math.Round((Closest_Y / Screen.height * NotesPerScreen + YParallax / 20f) * 0.05f - 0.7f, 2);
        int chunkNum = (int)Math.Ceiling(delay / 0.7f);

        OCdict.TryAdd(chunkNum, new List<int>());

        if (!OCdict[chunkNum].Contains((int)preCoords))
        {
            Button Added_Trigger = Instantiate(Trigger, new Vector3(Lane_X[(int)Closest_Lane] + 2f + Closest_Lane, Local_Y, 1f), Trigger.transform.rotation);

            TriggerValues triggerValues = Added_Trigger.GetComponent<TriggerValues>();

            triggerValues.delay = delay;
            triggerValues.editorLane = Closest_Lane;
            triggerValues.usedGroup = 999999;
            triggerValues.duration = 0f;

            Added_Trigger.transform.SetParent(Note_Container.transform, false);

            Added_Trigger.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 45);

            OCdict[chunkNum].Add((int)preCoords);

            if (FreeNoteIDs.Length != 0)
            {
                triggerValues.editorID = FreeNoteIDs[FreeNoteIDs.Length - 1];
                Array.Resize(ref FreeNoteIDs, FreeNoteIDs.Length - 1);
            }
            else
            {
                Added_Trigger.GetComponent<TriggerValues>().editorID = LastNoteID + 1;
                LastNoteID += 1;
            }

            if (Selected_Tool_Button == AlphaTriggerButton)
            {
                Added_Trigger.GetComponent<TriggerValues>().triggerType = "alpha_trigger";
                triggerValues.opacity = 1f;

                Alpha_Trigger AT = new Alpha_Trigger();
                AT.editorLane = Closest_Lane;
                AT.editorID = triggerValues.editorID;
                AT.usedGroup = 999999;
                AT.delay = triggerValues.delay;
                AT.duration = 0f;
                AT.opacity = 1f;

                ATchunk newATchunk = new ATchunk();
                // int chunkNum = (int)Math.Ceiling(delay / 0.7f);
                ATdict.TryAdd(chunkNum, newATchunk);
                Array.Resize(ref ATdict[chunkNum].ATbatch, ATdict[chunkNum].ATbatch.Length + 1);
                ATdict[chunkNum].ATbatch[ATdict[chunkNum].ATbatch.Length - 1] = AT;
            }
            else if (Selected_Tool_Button == SpriteTriggerButton)
            {
                Added_Trigger.GetComponent<TriggerValues>().triggerType = "sprite_trigger";
                triggerValues.spriteName = "";

                Sprite_Trigger ST = new Sprite_Trigger();
                ST.editorLane = Closest_Lane;
                ST.editorID = triggerValues.editorID;
                ST.usedGroup = 999999;
                ST.delay = triggerValues.delay;
                ST.spritename = "";

                STchunk newSTchunk = new STchunk();
                // int chunkNum = (int)Math.Ceiling(delay / 0.7f);
                STdict.TryAdd(chunkNum, newSTchunk);
                Array.Resize(ref STdict[chunkNum].STbatch, STdict[chunkNum].STbatch.Length + 1);
                STdict[chunkNum].STbatch[STdict[chunkNum].STbatch.Length - 1] = ST;
            }
        }
    }


    public void AddNote(Button Note)            // Ставит ноту
    {
        if (Closest_Lane > 4)
        {
            return;
        }

        float preCoords = (float)Math.Round(((Closest_Y / Screen.height * NotesPerScreen + YParallax / 20f) * 0.05f - 0.7f) * 400f) + Closest_Lane;
        float delay = (float)Math.Round((Closest_Y / Screen.height * NotesPerScreen + YParallax / 20f) * 0.05f - 0.7f, 2);
        int chunkNum = (int)Math.Ceiling(delay / 0.7f);
        
        Debug.Log(chunkNum);
        
        // Debug.Log(Closest_Y);
        // Debug.Log(preCoords);
        // if (Occupied_Coords.Any(element => Mathf.Approximately(preCoords, element)) == false & preCoords >= 0)
        OCdict.TryAdd(chunkNum, new List<int>());
        if (!OCdict[chunkNum].Contains((int)preCoords) & preCoords >= 0)
        {
            // Debug.Log(Closest_Y / Screen.height * NotesPerScreen);                   // дохуя важные вычисления
            // Debug.Log(((preCoords - Closest_Lane - YParallax + 300f) / 20f) -1f);

            // Debug.Log(Closest_Y);
            // Debug.Log((((preCoords - Closest_Lane - YParallax + 300f) / 20f) -1f) * Screen.height / NotesPerScreen);
            // Occupied_Coords.Add(preCoords);

            // Button Added_Note = Instantiate(Note, new Vector3(Lane_X[(int)Closest_Lane], Closest_Y + YParallax, 1f), Note.transform.rotation);
            Button Added_Note = Instantiate(Note, new Vector3(Lane_X[(int)Closest_Lane] + 2f + 1f*Closest_Lane, Local_Y, 1f), Note.transform.rotation);

            // Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 18);
            // Added_Note.GetComponent<SimpleNoteStats>().occupied_space = (int)Closest_Y + Closest_Lane + (int)YParallax;
            Added_Note.GetComponent<Image>().color = SelectedColorButton.colors.normalColor;
            Added_Note.GetComponent<SimpleNoteStats>().color = new List<float> { SelectedColorButton.colors.normalColor.r * 255, SelectedColorButton.colors.normalColor.g * 255, SelectedColorButton.colors.normalColor.b * 255 };
            Added_Note.GetComponent<SimpleNoteStats>().speed = int.Parse(SelectedSpeedButton.transform.Find("Value").gameObject.GetComponent<TMP_Text>().text);
            // Added_Note.GetComponent<SimpleNoteStats>().Punchable = SelectedPunchable;
            
            Added_Note.GetComponent<SimpleNoteStats>().delay = delay;
            Added_Note.GetComponent<SimpleNoteStats>().rand_lanes = new List<int>() {(int)Closest_Lane};

            Added_Note.GetComponent<SimpleNoteStats>().width = 1;

            OCdict[chunkNum].Add((int)preCoords);


            if (FreeNoteIDs.Length != 0)
            {
                Added_Note.GetComponent<SimpleNoteStats>().editorID = FreeNoteIDs[FreeNoteIDs.Length - 1];
                Array.Resize(ref FreeNoteIDs, FreeNoteIDs.Length - 1);
            }
            else
            {
                Added_Note.GetComponent<SimpleNoteStats>().editorID = LastNoteID + 1;
                LastNoteID += 1;
            }


            if (Selected_Tool_Button == SimpleNoteButton)
            {
                Added_Note.GetComponent<SimpleNoteStats>().type = "simple_note"; 

                Added_Note.GetComponent<SimpleNoteStats>().Punchable = SelectedPunchable;

                Simple_Note SN = new Simple_Note();
                SN.editorID = Added_Note.GetComponent<SimpleNoteStats>().editorID;
                SN.color = Added_Note.GetComponent<SimpleNoteStats>().color;
                SN.rand_lanes = Added_Note.GetComponent<SimpleNoteStats>().rand_lanes;
                SN.delay = delay;
                SN.speed = Added_Note.GetComponent<SimpleNoteStats>().speed;
                SN.width = 1;
                SN.punchable = Added_Note.GetComponent<SimpleNoteStats>().Punchable;

                SNchunk newSNchunk = new SNchunk();
                // int chunkNum = (int)Math.Ceiling(delay / 0.7f);
                SNdict.TryAdd(chunkNum, newSNchunk);
                Array.Resize(ref SNdict[chunkNum].SNbatch, SNdict[chunkNum].SNbatch.Length + 1);
                SNdict[chunkNum].SNbatch[SNdict[chunkNum].SNbatch.Length - 1] = SN;

                // Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * 1.3f, 1 * 18);
                SetWidth(Added_Note.gameObject, 1);
            }
            else if (Selected_Tool_Button == WallNoteButton)
            {
                Added_Note.GetComponent<SimpleNoteStats>().type = "wall_note";

                Added_Note.GetComponent<SimpleNoteStats>().Punchable = SelectedPunchable;

                Wall_Note WN = new Wall_Note();
                WN.editorID = Added_Note.GetComponent<SimpleNoteStats>().editorID;
                WN.color = Added_Note.GetComponent<SimpleNoteStats>().color;
                WN.rand_lanes = Added_Note.GetComponent<SimpleNoteStats>().rand_lanes;
                WN.delay = delay;
                WN.speed = Added_Note.GetComponent<SimpleNoteStats>().speed;
                WN.punchable = Added_Note.GetComponent<SimpleNoteStats>().Punchable;


                WNchunk newWNchunk = new WNchunk();
                // int chunkNum = (int)Math.Ceiling(delay / 0.7f);
                WNdict.TryAdd(chunkNum, newWNchunk);
                Array.Resize(ref WNdict[chunkNum].WNbatch, WNdict[chunkNum].WNbatch.Length + 1);
                WNdict[chunkNum].WNbatch[WNdict[chunkNum].WNbatch.Length - 1] = WN;

                // Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * 1.3f, 1 * 18);
                SetWidth(Added_Note.gameObject, 1);
            }
            else if (Selected_Tool_Button == CrackerNoteButton)
            {
                Toggle PunchToggle = PunchableToggle.GetComponent<Toggle>();
                PunchToggle.isOn = true;
                PunchToggle.interactable = false;
                SelectedPunchable = true;
                

                Added_Note.GetComponent<SimpleNoteStats>().type = "cracker_note";

                Added_Note.GetComponent<SimpleNoteStats>().Punchable = true;

                Cracker_Note CN = new Cracker_Note();
                CN.editorID = Added_Note.GetComponent<SimpleNoteStats>().editorID;
                CN.color = Added_Note.GetComponent<SimpleNoteStats>().color;
                CN.rand_lanes = Added_Note.GetComponent<SimpleNoteStats>().rand_lanes;
                CN.delay = delay;
                CN.speed = Added_Note.GetComponent<SimpleNoteStats>().speed;
                CN.punchable = true;

                CNchunk newCNchunk = new CNchunk();
                // int chunkNum = (int)Math.Ceiling(delay / 0.7f);
                CNdict.TryAdd(chunkNum, newCNchunk);
                Array.Resize(ref CNdict[chunkNum].CNbatch, CNdict[chunkNum].CNbatch.Length + 1);
                CNdict[chunkNum].CNbatch[CNdict[chunkNum].CNbatch.Length - 1] = CN;

                Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * 1.3f, 1 * 18);
                Added_Note.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(64 * 1.3f, 1 * 18);
            }


            if (Added_Note.GetComponent<Image>().color.r != 0)
            {
                Added_Note.transform.GetChild(0).gameObject.SetActive(false);                       // Обводка
            }
            
            Added_Note.GetComponent<SimpleNoteStats>().occupied_space = (int)Mathf.Round(delay * 400f) + Closest_Lane;

            // Button Added_Note = Instantiate(Note, Note_Container.transform);
            Added_Note.transform.position = new Vector3(Added_Note.transform.position.x, Added_Note.transform.position.y, 0f);
            // Added_Note.transform.position = new Vector3(Lane_Parent_positions[lane_index], Added_Note.transform.position.y - 140f, 0f);

            Added_Note.transform.SetParent(Note_Container.transform, false);

            // Added_Note.onClick.AddListener(() => SetSelectedNote(Added_Note));
            Added_Note.onClick.AddListener(delegate { SetSelectedNote(Added_Note); });
            SetSelectedNote(Added_Note);
            // SetNoteWidth(1);
            // Debug.Log(Occupied_Coords);

            SetNoteAnchors(Added_Note);
        }
    }

    public void DrawSimpleNote(Simple_Note SN)
    {
        int OC = (int)Math.Round(SN.delay * 400f) + SN.rand_lanes[0];
        float ClosestY = (((OC - (OC % 20f) - YParallax + 300f) / 20f) -1f) * Screen.height / NotesPerScreen;
        float LocalY = Note_Container.transform.InverseTransformPoint(0f, ClosestY, 0f).y;
        int ClosestLane = OC % 20;
        // Debug.Log(Math.Round(SN.delay * 400f, 2));
        // Debug.Log(SN.delay);
        Button Added_Note = Instantiate(SimpleNote, new Vector3(Lane_X[ClosestLane] + 2f + 1f*ClosestLane, LocalY, 1f), SimpleNote.transform.rotation);
        Added_Note.transform.SetParent(Note_Container.transform, false);

        Added_Note.GetComponent<Image>().color = new Color(SN.color[0] / 255f, SN.color[1] / 255f, SN.color[2] / 255f);
        Added_Note.GetComponent<SimpleNoteStats>().color = new List<float> { SN.color[0], SN.color[1], SN.color[2] };
        Added_Note.GetComponent<SimpleNoteStats>().speed = SN.speed;
        Added_Note.GetComponent<SimpleNoteStats>().type = "simple_note"; 
        Added_Note.GetComponent<SimpleNoteStats>().rand_lanes = SN.rand_lanes;
        Added_Note.GetComponent<SimpleNoteStats>().delay = (float)Math.Round(SN.delay, 2);
        Added_Note.GetComponent<SimpleNoteStats>().occupied_space = OC;
        Added_Note.GetComponent<SimpleNoteStats>().editorID = SN.editorID;
        Added_Note.GetComponent<SimpleNoteStats>().Punchable = SN.punchable;

        Added_Note.onClick.AddListener(delegate { SetSelectedNote(Added_Note); });

        // Selected_Note.gameObject.GetComponent<SimpleNoteStats>().width = Width;
        int Width = SN.width;
        // Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * Width * 1.3f, 1 * 18);                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        SetWidth(Added_Note.gameObject, Width);
        
        Added_Note.GetComponent<SimpleNoteStats>().width = SN.width;

        if (Width == 1 || Width == 3 || Width == 5)
        {
            Added_Note.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        }
        else if (Width == 2)
        {
            Added_Note.GetComponent<RectTransform>().pivot = new Vector2(0.20f, 0.5f);
        }
        else if (Width == 4)
        {
            Added_Note.GetComponent<RectTransform>().pivot = new Vector2(0.11f, 0.5f);
        }

        if (SN.color[0] != 0)
        {
            Debug.Log(SN.color[0]);
            Added_Note.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void DrawWallNote(Wall_Note WN)
    {
        int OC = (int)Math.Round(WN.delay * 400f) + WN.rand_lanes[0];
        float ClosestY = (((OC - (OC % 20f) - YParallax + 300f) / 20f) -1f) * Screen.height / NotesPerScreen;
        float LocalY = Note_Container.transform.InverseTransformPoint(0f, ClosestY, 0f).y;
        int ClosestLane = OC % 20;
        Button Added_Note = Instantiate(WallNote, new Vector3(Lane_X[ClosestLane] + 2f + 1f*ClosestLane, LocalY, 1f), SimpleNote.transform.rotation);
        Added_Note.transform.SetParent(Note_Container.transform, false);

        Added_Note.GetComponent<Image>().color = new Color(WN.color[0] / 255f, WN.color[1] / 255f, WN.color[2] / 255f);
        Added_Note.GetComponent<SimpleNoteStats>().color = new List<float> { SelectedColorButton.colors.normalColor.r * 255, SelectedColorButton.colors.normalColor.g * 255, SelectedColorButton.colors.normalColor.b * 255 };
        Added_Note.GetComponent<SimpleNoteStats>().speed = WN.speed;
        Added_Note.GetComponent<SimpleNoteStats>().type = "wall_note"; 
        Added_Note.GetComponent<SimpleNoteStats>().rand_lanes = WN.rand_lanes;
        Added_Note.GetComponent<SimpleNoteStats>().delay = (float)Math.Round(WN.delay, 2);
        Added_Note.GetComponent<SimpleNoteStats>().occupied_space = OC;
        Added_Note.GetComponent<SimpleNoteStats>().editorID = WN.editorID;
        Added_Note.GetComponent<SimpleNoteStats>().width = 1;
        Added_Note.GetComponent<SimpleNoteStats>().Punchable = WN.punchable;

        Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * 1.3f, 1 * 18);

        Added_Note.onClick.AddListener(delegate { SetSelectedNote(Added_Note); });

        SetWidth(Added_Note.gameObject, 1);

        if (WN.color[0] != 0)
        {
            Debug.Log(WN.color[0]);
            Added_Note.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void DrawCrackerNote(Cracker_Note CN)
    {
        int OC = (int)Math.Round(CN.delay * 400f) + CN.rand_lanes[0];
        float ClosestY = (((OC - (OC % 20f) - YParallax + 300f) / 20f) -1f) * Screen.height / NotesPerScreen;
        float LocalY = Note_Container.transform.InverseTransformPoint(0f, ClosestY, 0f).y;
        int ClosestLane = OC % 20;
        Button Added_Note = Instantiate(CrackerNote, new Vector3(Lane_X[ClosestLane] + 2f + 1f*ClosestLane, LocalY, 1f), SimpleNote.transform.rotation);
        Added_Note.transform.SetParent(Note_Container.transform, false);

        Added_Note.GetComponent<Image>().color = new Color(CN.color[0] / 255f, CN.color[1] / 255f, CN.color[2] / 255f);
        Added_Note.GetComponent<SimpleNoteStats>().color = new List<float> { SelectedColorButton.colors.normalColor.r * 255, SelectedColorButton.colors.normalColor.g * 255, SelectedColorButton.colors.normalColor.b * 255 };
        Added_Note.GetComponent<SimpleNoteStats>().speed = CN.speed;
        Added_Note.GetComponent<SimpleNoteStats>().type = "cracker_note";
        Added_Note.GetComponent<SimpleNoteStats>().rand_lanes = CN.rand_lanes;
        Added_Note.GetComponent<SimpleNoteStats>().delay = (float)Math.Round(CN.delay, 2);
        Added_Note.GetComponent<SimpleNoteStats>().occupied_space = OC;
        Added_Note.GetComponent<SimpleNoteStats>().editorID = CN.editorID;
        Added_Note.GetComponent<SimpleNoteStats>().width = 1;
        Added_Note.GetComponent<SimpleNoteStats>().Punchable = true;

        Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(64 * 1.3f, 1 * 18);
        Added_Note.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(64 * 1.3f, 1 * 18);


        Added_Note.onClick.AddListener(delegate { SetSelectedNote(Added_Note); });

        if (CN.color[0] != 0)
        {
            // Debug.Log(CN.color[0]);
            Added_Note.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void DrawAlphaTrigger(Alpha_Trigger AT)
    {
        int OC = (int)Math.Round(AT.delay * 400f) + AT.editorLane;
        float ClosestY = (((OC - (OC % 20f) - YParallax + 300f) / 20f) -1f) * Screen.height / NotesPerScreen;
        float LocalY = Note_Container.transform.InverseTransformPoint(0f, ClosestY, 0f).y;
        int ClosestLane = OC % 20;
        Button Added_Trigger = Instantiate(AlphaTrigger, new Vector3(Lane_X[ClosestLane] + 2f + 1f*ClosestLane, LocalY, 1f), AlphaTrigger.transform.rotation);
        Added_Trigger.transform.SetParent(Note_Container.transform, false);

        Added_Trigger.GetComponent<TriggerValues>().type = "alpha_trigger";
        Added_Trigger.GetComponent<TriggerValues>().duration = 0f;
        Added_Trigger.GetComponent<TriggerValues>().opacity = 1f;
        Added_Trigger.GetComponent<TriggerValues>().editorLane = AT.editorLane;
        Added_Trigger.GetComponent<TriggerValues>().delay = (float)Math.Round(AT.delay, 2);
        // Added_Note.GetComponent<SimpleNoteStats>().occupied_space = OC;
        Added_Trigger.GetComponent<TriggerValues>().editorID = AT.editorID;

        Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 45);

        Added_Note.onClick.AddListener(delegate { SetSelectedTrigger(Added_Trigger); });
    }

    public void DrawSpriteTrigger(Sprite_Trigger ST)
    {
        int OC = (int)Math.Round(ST.delay * 400f) + ST.editorLane;
        float ClosestY = (((OC - (OC % 20f) - YParallax + 300f) / 20f) -1f) * Screen.height / NotesPerScreen;
        float LocalY = Note_Container.transform.InverseTransformPoint(0f, ClosestY, 0f).y;
        int ClosestLane = OC % 20;
        Button Added_Trigger = Instantiate(SpriteTrigger, new Vector3(Lane_X[ClosestLane] + 2f + 1f*ClosestLane, LocalY, 1f), SpriteTrigger.transform.rotation);
        Added_Trigger.transform.SetParent(Note_Container.transform, false);

        Added_Trigger.GetComponent<TriggerValues>().type = "alpha_trigger";
        Added_Trigger.GetComponent<TriggerValues>().spriteName = "";
        Added_Trigger.GetComponent<TriggerValues>().editorLane = AT.editorLane;
        Added_Trigger.GetComponent<TriggerValues>().delay = (float)Math.Round(AT.delay, 2);
        // Added_Note.GetComponent<SimpleNoteStats>().occupied_space = OC;
        Added_Trigger.GetComponent<TriggerValues>().editorID = AT.editorID;

        Added_Note.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 45);

        Added_Note.onClick.AddListener(delegate { SetSelectedTrigger(Added_Trigger); });
    }

    public void DeleteNote()
    {
        // Occupied_Coords.Remove(Selected_Note.GetComponent<SimpleNoteStats>().occupied_space);               // Убрать это позже !!!
        Array.Resize(ref FreeNoteIDs, FreeNoteIDs.Length + 1);
        FreeNoteIDs[FreeNoteIDs.Length - 1] = Selected_Note.GetComponent<SimpleNoteStats>().editorID;

        string type = Selected_Note.GetComponent<SimpleNoteStats>().type;
        int chunkID = (int)Math.Ceiling(Selected_Note.GetComponent<SimpleNoteStats>().delay / 0.7f);
        int noteID = Selected_Note.GetComponent<SimpleNoteStats>().editorID;
        OCdict[chunkID].Remove(Selected_Note.GetComponent<SimpleNoteStats>().occupied_space);

        if (type == "simple_note")
        {
            foreach(Simple_Note SN in SNdict[chunkID].SNbatch)
            {
                if (noteID == SN.editorID)
                {
                    SNdict[chunkID].SNbatch = SNdict[chunkID].SNbatch.Where(Simple_Note => Simple_Note.editorID != noteID).ToArray();
                }
            }
            
        }
        else if (type == "wall_note")
        {
            foreach(Wall_Note WN in WNdict[chunkID].WNbatch)
            {
                if (noteID == WN.editorID)
                {
                    WNdict[chunkID].WNbatch = WNdict[chunkID].WNbatch.Where(Wall_Note => Wall_Note.editorID != noteID).ToArray();
                }
            }
        }
        else if (type == "cracker_note")
        {
            foreach(Cracker_Note CN in CNdict[chunkID].CNbatch)
            {
                if (noteID == CN.editorID)
                {
                    CNdict[chunkID].CNbatch = CNdict[chunkID].CNbatch.Where(Cracker_Note => Cracker_Note.editorID != noteID).ToArray();
                }
            }
        }

        Destroy(Selected_Note);
        PunchableToggle.GetComponent<Toggle>().interactable = true;
    }

    public void ImportFromJson()
    {
        FreeNoteIDs = new int[0];
        foreach (Transform child in Note_Container.transform)       // Все имеющиеся ноты удаляются при импорте
        {
            if (child.tag == "Note")
            {
                Destroy(child.gameObject);
                Occupied_Coords.Remove(child.gameObject.GetComponent<SimpleNoteStats>().occupied_space);
            }
        }

        SNdict = new Dictionary<int, SNchunk>();
        WNdict = new Dictionary<int, WNchunk>();
        CNdict = new Dictionary<int, CNchunk>();
        SceneObjects = new Dictionary<int, SceneObject>();

        string chartPath = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Chart.json");
        string objectsPath = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Objects.json");
        string text;

        if (!File.Exists(chartPath))
        {
            File.Create(chartPath).Dispose();
        }
        if (!File.Exists(objectsPath))
        {
            File.Create(objectsPath).Dispose();
        }

        text = File.ReadAllText(chartPath);

        AllNotes = JsonUtility.FromJson<AllNoteList>(text);
        
        if (AllNotes == null)
        {
            AllNotes = new AllNoteList();
            return;
        }

        if (AllNotes.SimpleNotes.simple_note.Length != 0)
        {
            AllSimpleNotes = AllNotes.SimpleNotes;
        }
        if (AllNotes.WallNotes.wall_note.Length != 0)
        {
            AllWallNotes = AllNotes.WallNotes;
        }
        if (AllNotes.CrackerNotes.cracker_note.Length != 0)
        {
            AllCrackerNotes = AllNotes.CrackerNotes;
        }

        foreach(Simple_Note SN in AllSimpleNotes.simple_note)
        {
            int ChunkNum = (int)Math.Ceiling(SN.delay / 0.7f);
            // if (ChunkNum == 0)
            // {
            //     ChunkNum = 1;
            // }

            SNchunk newSNChunk = new SNchunk();
            SNdict.TryAdd(ChunkNum, newSNChunk);
            Array.Resize(ref SNdict[ChunkNum].SNbatch, SNdict[ChunkNum].SNbatch.Length + 1);
            SNdict[ChunkNum].SNbatch[SNdict[ChunkNum].SNbatch.Length - 1] = SN;

            if (SN.editorID > LastNoteID)
            {
                LastNoteID = SN.editorID;
            }
            Occupied_Coords.Add((float)Math.Round(SN.delay * 400f + SN.rand_lanes[0]));
        }
        foreach(Wall_Note WN in AllWallNotes.wall_note)
        {
            int ChunkNum = (int)Math.Ceiling(WN.delay / 0.7f);
            // if (ChunkNum == 0)
            // {
            //     ChunkNum = 1;
            // }
            WNchunk newWNChunk = new WNchunk();
            WNdict.TryAdd(ChunkNum, newWNChunk);
            Array.Resize(ref WNdict[ChunkNum].WNbatch, WNdict[ChunkNum].WNbatch.Length + 1);
            WNdict[ChunkNum].WNbatch[WNdict[ChunkNum].WNbatch.Length - 1] = WN;

            if (WN.editorID > LastNoteID)
            {
                LastNoteID = WN.editorID;
            }
            Occupied_Coords.Add((float)Math.Round(WN.delay * 400f + WN.rand_lanes[0]));
        }
        foreach(Cracker_Note CN in AllCrackerNotes.cracker_note)
        {
            int ChunkNum = (int)Math.Ceiling(CN.delay / 0.7f);
            // if (ChunkNum == 0)
            // {
            //     ChunkNum = 1;
            // }
            CNchunk newCNChunk = new CNchunk();
            CNdict.TryAdd(ChunkNum, newCNChunk);
            Array.Resize(ref CNdict[ChunkNum].CNbatch, CNdict[ChunkNum].CNbatch.Length + 1);
            CNdict[ChunkNum].CNbatch[CNdict[ChunkNum].CNbatch.Length - 1] = CN;

            if (CN.editorID > LastNoteID)
            {
                LastNoteID = CN.editorID;
            }
            Occupied_Coords.Add((float)Math.Round(CN.delay * 400f + CN.rand_lanes[0]));
        }


        text = File.ReadAllText(objectsPath);

        AllSceneObjects = JsonUtility.FromJson<SceneObjectList>(text);

        if (AllSceneObjects == null)
        {
            AllSceneObjects = new SceneObjectList();
            return;
        }

        foreach (SceneObject SO in AllSceneObjects.scene_object)
        {
            int editorID = SO.editorID;
            SceneObjects.TryAdd(editorID, SO);

            if (SO.editorID > LastObjectID)
            {
                LastObjectID = SO.editorID;
            }
        }

        foreach (float OC in Occupied_Coords)
        {
            float delay = (float)Math.Round((OC - (OC % 20f)) / 400f, 2);
            int ChunkID = (int)Math.Ceiling(delay / 0.7f);
            OCdict.TryAdd(ChunkID, new List<int>());
            OCdict[ChunkID].Add((int)OC);
        }


        // Debug.Log(SNdict[1].SNbatch.Length);
        // StartCoroutine(PlaceImportedNotes());
    }

    // public void CheckSN()
    // {
    //     Debug.Log(SNdict[1].SNbatch.Length);
    // }

    public void DrawChunks(bool ForceDraw)
    {
        if (DrawnChunks[1] == CurrentChunk & ForceDraw == false)
        {
            // Debug.Log("dddd");
            return;
        }

        int[] ChunksToCheck = new int[] {CurrentChunk -1, CurrentChunk, CurrentChunk +1};
        // Debug.Log(ChunksToCheck[0]);

        int[] ChunksToDraw = ChunksToCheck.Except(DrawnChunks).ToArray();
        int[] ChunksToDelete = DrawnChunks.Except(ChunksToCheck).ToArray();

        if (ForceDraw)
        {
            ChunksToDraw = ChunksToCheck;
        }
        
        foreach (int i in ChunksToDelete)
        {
            foreach(Transform child in Note_Container.transform)
            {
                if (child.CompareTag("Note"))
                {
                    if ((int)Math.Ceiling(child.GetComponent<SimpleNoteStats>().delay / 0.7f) == i)
                    {
                        Destroy(child.gameObject);
                    }
                }
                else if (child.CompareTag("Trigger"))
                {
                    if ((int)Math.Ceiling(child.GetComponent<TriggerValues>().delay / 0.7f) == i)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
        
        foreach (int i in ChunksToDraw)
        {
            if (SNdict.ContainsKey(i))
            {
                foreach (Simple_Note SN in SNdict[i].SNbatch)
                {
                    DrawSimpleNote(SN);
                }
            }

            if (WNdict.ContainsKey(i))
            {
                foreach (Wall_Note WN in WNdict[i].WNbatch)
                {
                    DrawWallNote(WN);
                }
            }

            if (CNdict.ContainsKey(i))
            {
                foreach (Cracker_Note CN in CNdict[i].CNbatch)
                {
                    DrawCrackerNote(CN);
                }
            }



            if (ATdict.ContainsKey(i))
            {
                foreach (Alpha_Trigger AT in ATdict[i].ATbatch)
                {
                    DrawAlphaTrigger(AT);
                }
            }

            if (STdict.ContainsKey(i))
            {
                foreach (Sprite_Trigger ST in STdict[i].STbatch)
                {
                    DrawSpriteTrigger(ST);
                }
            }
        }

        
        
        DrawnChunks = ChunksToCheck;
    }


    IEnumerator MusicGuidelineStart()           // Запускает плейбек
    {
        TimeSinceCall = 0f;         // Обнулится, и сразу начнет обновлятся через Update
        TimeOnTrack = 0f;
        float RequiredTime = 0f;
        Debug.Log(Time.timeScale);
        yield return new WaitUntil(() => TimeSinceCall >= 0.05f);
        while (PlaybackActive)
        {
            TimeOnTrack += 0.05f;
            RequiredTime = TimeOnTrack + 0.05f;
            YParallax += 20f;
            // Note_Container.transform.position += new Vector3(0f, -20f, 0f);
            // Note_Container.transform.position = NCInitialPos - new Vector3(0f, 20f * YParallax / 20f);
            Note_Container.transform.position = NCInitialPos - new Vector3(0f, YParallax * 2 * scaleFactor);
            // Debug.Log(YParallax);
            yield return new WaitUntil(() => TimeSinceCall >= RequiredTime); 
        }
    }

    public void ToggleObjectListWindow()
    {
        string buttonText;
        if (!ObjectWindow.activeInHierarchy)
        {
            ObjectWindow.SetActive(true);
            buttonText = "NOTE MENU";
            AddObjectButton.SetActive(true);
            ObjectEditPage.SetActive(false);
            ObjectPageSwitcher.SetActive(true);
        }
        else
        {
            ObjectWindow.SetActive(false);
            AddObjectButton.SetActive(false);
            ObjectEditPage.SetActive(false);
            ObjectPageSwitcher.SetActive(false);
            buttonText = "OBJECT MENU";
        }
        PageToggleButton.GetComponentInChildren<TMP_Text>().text = buttonText;

        DisplayObjects();
    }

    public void ToggleAddObjectWindow()
    {
        ObjectEditPage.SetActive(true);            
        AddObjectButton.SetActive(false);
        ObjectWindow.SetActive(false);
        ObjectPageSwitcher.SetActive(false);
        ObjectEditMode = false;

        ObjectEditPage.transform.Find("TitleText").GetComponent<TMP_Text>().text = "Create Object";

        ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text = "";
        ObjectEditPage.transform.Find("X_Input").GetComponent<TMP_InputField>().SetTextWithoutNotify("0");
        ObjectEditPage.transform.Find("Y_Input").GetComponent<TMP_InputField>().SetTextWithoutNotify("0");
        ObjectEditPage.transform.Find("Z_Input").GetComponent<TMP_InputField>().SetTextWithoutNotify("10");
        ObjectEditPage.transform.Find("AlphaInput").GetComponent<TMP_InputField>().SetTextWithoutNotify("100");
        ObjectEditPage.transform.Find("ScaleInput").GetComponent<TMP_InputField>().SetTextWithoutNotify("1");
        ObjectEditPage.transform.Find("ImageContainer").GetChild(0).GetComponent<RawImage>().texture = MissingTexturePic;
        ObjectEditPage.transform.Find("ImageContainer").GetChild(1).GetComponent<TMP_Text>().text = "";
    }

    public void OpenObjectEdit(GameObject IDtext)
    {
        ObjectEditPage.transform.Find("TitleText").GetComponent<TMP_Text>().text = "Edit Object";
        ObjectEditPage.SetActive(true);
        AddObjectButton.SetActive(false);
        ObjectPageSwitcher.SetActive(false);
        ObjectEditMode = true;

        string objectID = IDtext.GetComponent<TMP_Text>().text;
        objectID = objectID.Replace("ID: ", "").Trim();
        // objectID = objectID.Replace("\u200b", "").Trim();
        // Debug.Log(objectID);
        int ID = int.Parse(objectID);

        SceneObject SO = SceneObjects[ID];

        ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text = string.Join(" ", SO.objectIDs).Trim();

        string imagePath = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Sprites", SO.spriteName);
        if (File.Exists(imagePath))
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            Texture2D imageTexture = new Texture2D(2, 2);
            imageTexture.LoadImage(imageBytes);
            ObjectEditPage.transform.Find("ImageContainer").GetChild(0).GetComponent<RawImage>().texture = imageTexture;
        }
        else
        {
            Texture2D imageTexture = MissingTexturePic;
            ObjectEditPage.transform.Find("ImageContainer").GetChild(0).GetComponent<RawImage>().texture = imageTexture;
        }

        Debug.Log(SO.Xpos);
        // ObjectEditPage.transform.Find("X_Input").GetChild(0).Find("Text").GetComponent<TMP_Text>().text = SO.Xpos.ToString();
        // ObjectEditPage.transform.Find("Y_Input").GetChild(0).Find("Text").GetComponent<TMP_Text>().text = SO.Ypos.ToString();
        ObjectEditPage.transform.Find("ImageContainer").GetChild(1).GetComponent<TMP_Text>().text = SO.spriteName;

        ObjectEditPage.transform.Find("X_Input").GetComponent<TMP_InputField>().SetTextWithoutNotify(SO.Xpos.ToString());
        ObjectEditPage.transform.Find("Y_Input").GetComponent<TMP_InputField>().SetTextWithoutNotify(SO.Ypos.ToString());
        ObjectEditPage.transform.Find("Z_Input").GetComponent<TMP_InputField>().SetTextWithoutNotify(SO.zOrder.ToString());
        ObjectEditPage.transform.Find("AlphaInput").GetComponent<TMP_InputField>().SetTextWithoutNotify(SO.alpha.ToString());
        ObjectEditPage.transform.Find("ScaleInput").GetComponent<TMP_InputField>().SetTextWithoutNotify(SO.scale.ToString());

        ObjectEditPage.transform.Find("IDtext").GetComponent<TMP_Text>().text = ID.ToString();
        
        ObjectWindow.SetActive(false);
    }

    public void GetSpriteWindows()
    {
        SpriteWindows = new GameObject[5];
        SpriteWindows[0] = SpriteWindow.transform.Find("WindowFrame_1").gameObject;
        SpriteWindows[1] = SpriteWindow.transform.Find("WindowFrame_2").gameObject;
        SpriteWindows[2] = SpriteWindow.transform.Find("WindowFrame_3").gameObject;
        SpriteWindows[3] = SpriteWindow.transform.Find("WindowFrame_4").gameObject;
        SpriteWindows[4] = SpriteWindow.transform.Find("WindowFrame_5").gameObject;
    }

    public void GetObjectWindows()
    {
        ObjectWindows = new GameObject[5];
        ObjectWindows[0] = ObjectWindow.transform.Find("ObjectFrame_1").gameObject;
        ObjectWindows[1] = ObjectWindow.transform.Find("ObjectFrame_2").gameObject;
        ObjectWindows[2] = ObjectWindow.transform.Find("ObjectFrame_3").gameObject;
        ObjectWindows[3] = ObjectWindow.transform.Find("ObjectFrame_4").gameObject;
        ObjectWindows[4] = ObjectWindow.transform.Find("ObjectFrame_5").gameObject;
    }

    public void SetOWimage(GameObject Icon)
    {
        if (!ObjectEditPage.activeInHierarchy)
        {
            return;
        }
        Texture IconTexture = Icon.GetComponent<RawImage>().texture;

        ObjectEditPage.transform.Find("ImageContainer").GetChild(0).GetComponent<RawImage>().texture = IconTexture;
        string spriteName = Icon.transform.parent.parent.Find("TextContainer").GetChild(0).GetComponent<TMP_Text>().text;
        ObjectEditPage.transform.Find("ImageContainer").GetChild(1).GetComponent<TMP_Text>().text = spriteName;
    }

    IEnumerator FadeTextFlash()
    {
        TextFlash.SetActive(true);
        float timer = 0f;
        while (timer < 1.5f)
        {
            TextFlash.GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f - timer / 1.5f);
            TextFlash.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(255f, 255f, 255f, 1f - timer / 1.5f);
            yield return null;
            timer += Time.deltaTime;
        }
        TextFlash.SetActive(false);

        TextFadeCoroutine = null;
    }

    public void FlashWarning(string WarningText)
    {
        if (TextFadeCoroutine != null)
        {
            StopCoroutine(TextFadeCoroutine);
        }

        TextFlash.transform.GetChild(0).GetComponent<TMP_Text>().text = WarningText;
        
        TextFadeCoroutine = StartCoroutine(FadeTextFlash());
    }

    public void WriteObject()
    {
        if (ObjectEditPage.transform.Find("ImageContainer").GetChild(1).GetComponent<TMP_Text>().text == "")
        {
            FlashWarning("Click a sprite to set it on the object!");
            return;
        }

        // Debug.Log(ObjectEditPage.transform.Find("ImageContainer").GetChild(1).GetComponent<TMP_Text>().text);
        SceneObject SO = new SceneObject();

        string GroupString = ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text;
        if (GroupString == "")
        {
            SO.objectIDs = new List<int>();
        }
        else
        {
            List<int> Groups = GroupString.Split(' ').Select(int.Parse).ToList();
            SO.objectIDs = Groups;
        }

        // Debug.Log(SO.objectIDs.Count);

        string XposString = ObjectEditPage.transform.Find("X_Input").GetChild(0).Find("Text").GetComponent<TMP_Text>().text;
        string YposString = ObjectEditPage.transform.Find("Y_Input").GetChild(0).Find("Text").GetComponent<TMP_Text>().text;
        XposString = XposString.Replace("\u200b", "").Trim();
        YposString = YposString.Replace("\u200b", "").Trim();
        int rawInput;
        float scaleInput;
        if (!int.TryParse(XposString, out rawInput))
        {
            XposString = "0";
        }
        if (!int.TryParse(YposString, out rawInput))
        {
            YposString = "0";
        }
        // Debug.Log(XposString.Length);
        float Xpos = int.Parse(XposString);
        float Ypos = int.Parse(YposString);
        SO.Xpos = Xpos;
        SO.Ypos = Ypos;

        string AlphaString = ObjectEditPage.transform.Find("AlphaInput").GetChild(0).Find("Text").GetComponent<TMP_Text>().text;
        AlphaString = AlphaString.Replace("\u200b", "").Trim();
        if (!int.TryParse(AlphaString, out rawInput))
        {
            AlphaString = "100";
        }
        float Alpha = int.Parse(AlphaString);
        if (Alpha > 100)
        {
            Alpha = 100;
        }
        SO.alpha = Alpha;

        string ZString = ObjectEditPage.transform.Find("Z_Input").GetChild(0).Find("Text").GetComponent<TMP_Text>().text;
        ZString = ZString.Replace("\u200b", "").Trim();
        if (!int.TryParse(ZString, out rawInput))
        {
            ZString = "10";
        }
        int Z = int.Parse(ZString);
        SO.zOrder = Z;

        string ScaleString = ObjectEditPage.transform.Find("ScaleInput").GetChild(0).Find("Text").GetComponent<TMP_Text>().text;
        ScaleString = ScaleString.Replace("\u200b", "").Trim();
        if (!float.TryParse(ScaleString, out scaleInput))
        {
            ScaleString = "1";
        }
        float Scale = float.Parse(ScaleString);
        Scale = (float)Math.Round(Scale, 2);
        if (Scale == 0)
        {
            Scale = 0.01f;
        }
        else if (Scale > 100)
        {
            Scale = 100f;
        }
        SO.scale = Scale;
        // Debug.Log(Alpha);

        if (ObjectEditMode)
        {
            SO.editorID = int.Parse(ObjectEditPage.transform.Find("IDtext").GetComponent<TMP_Text>().text);
        }
        else
        {
            SO.editorID = LastObjectID + 1;
            LastObjectID += 1;
        }

        SO.spriteName = ObjectEditPage.transform.Find("ImageContainer").GetChild(1).GetComponent<TMP_Text>().text;

        SceneObjects[SO.editorID] = SO;

        // Debug.Log(SO.Xpos);

        ObjectPageCount = (int)Math.Ceiling(SceneObjects.Count / 5f);
        if (ObjectPageCount == 0)
        {
            ObjectPageCount = 1;
        }
        ToggleObjectListWindow();
        
    }

    public void AddObjectGroup(GameObject TextInput)
    {
        string groupValue = TextInput.GetComponent<TMP_Text>().text;
        groupValue = groupValue.Replace("\u200b", "").Trim();

        string groupListString = ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text;
        List<string> groups = groupListString.Split(' ').ToList();

        if (groupListString != "" & !groups.Contains(groupValue))
        {
            ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text += " " + groupValue;
        }
        else if (groupListString == "" & !groups.Contains(groupValue))
        {
            ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text += groupValue;
        }
    }

    public void RemoveObjectGroup(GameObject TextInput)
    {
        string groupValue = TextInput.GetComponent<TMP_Text>().text;
        groupValue = groupValue.Replace("\u200b", "").Trim();

        string groupListString = ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text;
        List<string> groups = groupListString.Split(' ').ToList();

        if (groupListString != "" & groups.Contains(groupValue))
        {
            groups.Remove(groupValue);
            string newGroupList = string.Join(" ", groups).Trim();
            // string newGroupList = ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text.Replace(groupValue, "");
            // newGroupList = newGroupList.Replace("  ", " ");
            // newGroupList = newGroupList.Replace("\u200b", "").Trim();
            ObjectEditPage.transform.Find("GroupListContainer").GetChild(0).GetComponent<TMP_Text>().text = newGroupList;
        }
    }

    public void DisplayObjects()
    {
        Dictionary<int, SceneObject> ObjectBatch = new Dictionary<int, SceneObject>();
        ObjectBatch = SceneObjects.Skip((ObjectPageNumber - 1) * 5).Take(ObjectPageNumber * 5).ToDictionary(x => x.Key, x => x.Value);
        int batchCount = ObjectBatch.Count;

        int count = 0;
        foreach (GameObject objectFrame in ObjectWindows)
        {
            if (count + 1 <= batchCount)
            {
                objectFrame.transform.GetChild(0).gameObject.SetActive(true);

                KeyValuePair<int, SceneObject> SOdictPair = ObjectBatch.ElementAt(count);
                SceneObject SO = SOdictPair.Value;

                // Debug.Log(SO.spriteName);
                string imagePath = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Sprites", SO.spriteName);
                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    Texture2D imageTexture = new Texture2D(2, 2);
                    imageTexture.LoadImage(imageBytes);
                    objectFrame.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = imageTexture;
                }
                else
                {
                    Texture2D imageTexture = MissingTexturePic;
                    objectFrame.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = imageTexture;
                }

                objectFrame.transform.GetChild(0).Find("IDtext").GetComponent<TMP_Text>().text = "ID: " + SO.editorID.ToString();

                objectFrame.transform.GetChild(0).Find("GroupListText").GetComponent<TMP_Text>().text = "Groups: " + string.Join(" ", SO.objectIDs).Trim();
            }
            else
            {
                objectFrame.transform.GetChild(0).gameObject.SetActive(false);
            }
            count += 1;
        }
    }

    public void DeleteSceneObject(GameObject IDtext)
    {
        string IDstring = IDtext.GetComponent<TMP_Text>().text;
        IDstring = IDstring.Replace("ID: ", "").Trim();
        int ID = int.Parse(IDstring);

        SceneObjects.Remove(ID);
        ObjectPageCount = (int)Math.Ceiling(SceneObjects.Count / 5f);
        if (ObjectPageCount == 0)
        {
            ObjectPageCount = 1;
        }

        if (ObjectPageNumber > ObjectPageCount)
        {
            ObjectPageNumber = ObjectPageCount;
        }

        ObjectPageText.GetComponent<TMP_Text>().text = ObjectPageNumber.ToString();
        DisplayObjects();
    }

    public void SwitchObjectPage(int pageDelta)
    {
        if (ObjectPageNumber + pageDelta > ObjectPageCount)
        {
            ObjectPageNumber = ObjectPageCount;
        }
        else if (ObjectPageNumber + pageDelta < 1)
        {
             ObjectPageNumber = 1;
        }
        else
        {
            ObjectPageNumber += pageDelta;
        }

        if (ObjectPageNumber == 0)
        {
            ObjectPageNumber = 1;
        }

        ObjectPageText.GetComponent<TMP_Text>().text = ObjectPageNumber.ToString();
        DisplayObjects();
    }

    public void TogglePauseMenu(bool active)
    {
        if (active == true)
        {
            Paused = true;
        }
        else
        {
            Paused = false;
        }

        PauseMenu.SetActive(active);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("CustomBattleMenu");
    }

    public void SaveQuit()
    {
        ExportChart();
        SceneManager.LoadScene("CustomBattleMenu");
    }

    void Start()
    {
        Time.timeScale = 1f;
        BattleName = BattleManagerScript.battleName;


        SimpleNoteSprites = new Sprite[] { OneWide, TwoWide, ThreeWide, FourWide, FiveWide };
        SimpleShieldNoteSprites = new Sprite[] { OneWideS, TwoWideS, ThreeWideS, FourWideS, FiveWideS };
        WallNoteSprites = new Sprite[] { WallShield, Wall };

        SimpleNoteOutlines = new Sprite[] { OneWideOutline, TwoWideOutline, ThreeWideOutline, FourWideOutline, FiveWideOutline };
        SimpleShieldNoteOutlines = new Sprite[] { OneWideSOutline, TwoWideSOutline, ThreeWideSOutline, FourWideSOutline, FiveWideSOutline };
        WallOutlines = new Sprite[] { ShieldWallOutline, RegularWallOutline };

        WidthButtonsContainer = Parameter_Panel.transform.Find("WidthButtons").gameObject;

        scaleFactor = Screen.height / 1080f;
        NotesPerScreen = Screen.height / (scaleFactor * baseNoteScaling);
        
        // Debug.Log(NotesPerScreen);
        Note_Container.transform.position -= new Vector3(0f, (float)Screen.height * -0.5f, 0f);
        NCInitialPos = Note_Container.transform.position;
        SetSelectedColor(DefaultColorButton);
        SetSelectedSpeedButton(DefaultSpeedButton);

        ImportScreen.SetActive(true);
        TextFlash.SetActive(false);

        StartCoroutine(LoadSong());

        ResizeContainer();

        GetSpriteWindows();

        SpriteWindowPage = 1;
        RefreshSpriteList();
        DisplaySprites();

        SpritePageCount = (int)Math.Ceiling(((float)AllBattleSprites.battle_sprite.Length) / 5f);
        // Debug.Log(SpritePageCount);

        ObjectPageNumber = 1;
        GetObjectWindows();
        
        PauseMenu.SetActive(false);
        ObjectWindow.SetActive(false);
        ObjectPageSwitcher.SetActive(false);
        ObjectEditPage.SetActive(false);
        AddObjectButton.SetActive(false);

        ImportFromJson();
        ObjectPageCount = (int)Math.Ceiling(((float)SceneObjects.Count) / 5f);
        Debug.Log(ObjectPageCount);
        Vector2 NCRectTransform = Note_Container.GetComponent<RectTransform>().anchorMin;
        Lane_X = new List<float>() {NCRectTransform.x - 180f, NCRectTransform.x - 92f, NCRectTransform.x - 4f, NCRectTransform.x + 84f, NCRectTransform.x + 172f, NCRectTransform.x + 260f, NCRectTransform.x + 325f, NCRectTransform.x + 390f, NCRectTransform.x + 455f};

        CurrentChunk = 0;
        DrawChunks(true);

        TimeSinceCall = 0f;

        // CalculateNCPosition();
    }


    public float PlayFromTime()
    {
        float time = 0;
        if (sound != null)
        {
            time = (YParallax) / 400f; //                 /400
        }
        if(YParallax <= 0) { time = 0; }
        return time;
    }

    public void ScrollOnWheel(Vector3 Scroll) // Вынес скролл на колесико в метод
    {
        if (Scroll.y > 0)      // убрал условие !PlaybackActive(мб не надо)       
        {
            for (int i = 0; i < YParallaxMultiplier; i++)
            {
                // Note_Container.transform.position -= new Vector3(0f, 20f, 0f);
                YParallax += 20f;
            }
            // Note_Container.transform.position = Note_Container.transform.InverseTransformPoint(transform.TransformPoint(NCInitialPos - new Vector3(0f, GlobalNCStep * (YParallax + 300f) / 20f))) ;
            // Note_Container.transform.position = NCInitialPos - new Vector3(0f, 20f * (YParallax + 0f) / 20f);
            Note_Container.transform.position = NCInitialPos - new Vector3(0f, YParallax * 2 * scaleFactor);
            // Note_Container.transform.position -= new Vector3(0f, 20 * YParallaxMultiplier, 0f);
            // YParallax += 20f * YParallaxMultiplier;
            sound.time = PlayFromTime();

            // DrawChunks(false);
        }
        else if (Scroll.y < 0 & YParallax > 0f) // убрал условие !PlaybackActive(мб не надо)
        {
           if (YParallax - 20f * YParallaxMultiplier < 0f)
            {
                YParallax = 0f;
                Note_Container.transform.position = new Vector3(Note_Container.transform.position.x, NCInitialPos.y, 1f);

                // DrawChunks(false);
            }
            else
            {
                for (int i = 0; i < YParallaxMultiplier; i++)
                {
                // Note_Container.transform.position += new Vector3(0f, 20f, 0f);
                YParallax -= 20f;
                }
                // Note_Container.transform.position = Note_Container.transform.InverseTransformPoint(transform.TransformPoint(NCInitialPos - new Vector3(0f, GlobalNCStep * (YParallax + 300f) / 20f)));
                // Note_Container.transform.position = NCInitialPos - new Vector3(0f, 20f * (YParallax + 0f) / 20f);
                Note_Container.transform.position = NCInitialPos - new Vector3(0f, YParallax * 2 * scaleFactor);
                // Note_Container.transform.position += new Vector3(0f, 20f * YParallaxMultiplier, 0f);
                // YParallax -= 20f * YParallaxMultiplier;
                sound.time = PlayFromTime();

                // DrawChunks(false);
            }
            // ResizeContainer();
            // Debug.Log("Resized");
        }

        timestamp_value = (YParallax + 0f) / 400f;
        timestamp1.GetComponent<TMP_Text>().text = timestamp_value.ToString();
        timestamp2.GetComponent<TMP_Text>().text = Math.Round(timestamp_value + 0.25f, 2).ToString();
        timestamp3.GetComponent<TMP_Text>().text = Math.Round(timestamp_value - 0.25f, 2).ToString();
        // Debug.Log(YParallax);

        
    }

    public void SetScrollMultiplier(float mult)
    {
        YParallaxMultiplier = mult;
    }

    public void ResizeContainer()       // Подгоняет anchor у контейнера нот под его позицию, чтоб не съезжал при скролле и изменении окна
    {
        // Debug.Log(Chart_Container.transform.position);
        // Debug.Log(Note_Container.transform.position);
        Vector3[] NCcorners = new Vector3[4];
        Note_Container.GetComponent<RectTransform>().GetWorldCorners(NCcorners);
        // Debug.Log(NCcorners[0]);


        RectTransform t = Note_Container.GetComponent<RectTransform>();
        RectTransform pt = Note_Container.GetComponent<RectTransform>().parent as RectTransform;

        if(t == null || pt == null) return;

        Vector2 OriginalSize = t.rect.size;
        Vector3 OriginalPos = t.position;

        Vector2 newAnchorsMin = new Vector2(0.5f,
                                         t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        // Vector2 newAnchorsMax = new Vector2(0.5f,
        //                                  t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        Vector2 newAnchorsMax = new Vector2(0.5f,
                                         t.anchorMax.y + t.offsetMax.y / pt.rect.height);

        t.anchorMin = newAnchorsMin;
        t.anchorMax = newAnchorsMax;
        t.offsetMin = t.offsetMax = new Vector2(0, 0);

        t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, OriginalSize.x);
        // t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, OriginalSize.y);
        t.position = OriginalPos;

        

        // NCInitialPos = OriginalPos;
    }


    public void ExportToJson()
    {
        AllNotes.SimpleNotes = new SimpleNoteList();
        AllNotes.WallNotes = new WallNoteList();
        AllNotes.CrackerNotes = new CrackerNoteList();

        Simple_Note[] SNlist = AllNotes.SimpleNotes.simple_note;
        Wall_Note[] WNlist = AllNotes.WallNotes.wall_note;
        Cracker_Note[] CNlist = AllNotes.CrackerNotes.cracker_note;


        foreach (Transform child in Note_Container.transform)
        {
            if (child.tag == "Note")
            {
                SimpleNoteStats stats = child.GetComponent<SimpleNoteStats>();
                if (stats.type == "simple_note")
                {
                    Simple_Note SN = new Simple_Note();
                    SN.color = stats.color;
                    SN.rand_lanes = stats.rand_lanes;
                    SN.delay = stats.delay;
                    SN.speed = stats.speed;
                    // SN.speed = 20;
                    SN.width = stats.width;
                    SN.editorID = stats.editorID;
                    
                    Array.Resize(ref SNlist, SNlist.Length + 1);
                    SNlist[SNlist.Length - 1] = new Simple_Note();
                    SNlist[SNlist.Length - 1] = SN;
                }
                else if (stats.type == "wall_note")
                {
                    Wall_Note WN = new Wall_Note();
                    WN.color = stats.color;
                    WN.rand_lanes = stats.rand_lanes;
                    WN.delay = stats.delay;
                    WN.speed = stats.speed;
                    WN.editorID = stats.editorID;
                    
                    Array.Resize(ref WNlist, WNlist.Length + 1);
                    WNlist[WNlist.Length - 1] = new Wall_Note();
                    WNlist[WNlist.Length - 1] = WN;
                }
                else if (stats.type == "cracker_note")
                {
                    Cracker_Note CN = new Cracker_Note();
                    CN.color = stats.color;
                    CN.rand_lanes = stats.rand_lanes;
                    CN.delay = stats.delay;
                    CN.speed = stats.speed;
                    CN.editorID = stats.editorID;
                    
                    Array.Resize(ref CNlist, CNlist.Length + 1);
                    CNlist[CNlist.Length - 1] = new Cracker_Note();
                    CNlist[CNlist.Length - 1] = CN;
                }
            }
        }
        // Debug.Log(SNlist.Length);
        AllNotes.SimpleNotes.simple_note = SNlist;
        AllNotes.WallNotes.wall_note = WNlist;
        AllNotes.CrackerNotes.cracker_note = CNlist;




        string ChartString = JsonUtility.ToJson(AllNotes, true);
        string FolderPath = Path.Combine(Application.persistentDataPath, "Battles", BattleName);
        File.WriteAllText(FolderPath + "/Chart.json", ChartString);
    }

    public void ExportChart()           // Новый метод
    {
        AllNotes.SimpleNotes = new SimpleNoteList();
        AllNotes.WallNotes = new WallNoteList();
        AllNotes.CrackerNotes = new CrackerNoteList();

        Simple_Note[] SNlist = AllNotes.SimpleNotes.simple_note;
        Wall_Note[] WNlist = AllNotes.WallNotes.wall_note;
        Cracker_Note[] CNlist = AllNotes.CrackerNotes.cracker_note;


        AllSceneObjects = new SceneObjectList();
        SceneObject[] SOlist = AllSceneObjects.scene_object;

        foreach (KeyValuePair<int, SNchunk> i in SNdict)
        {
            foreach (Simple_Note SN in i.Value.SNbatch)
            {
                if (SN.color[0] != 0)
                {
                    SN.absorbable = true;
                }
                else
                {
                    SN.absorbable = false;
                }

                Array.Resize(ref SNlist, SNlist.Length + 1);
                SNlist[SNlist.Length - 1] = new Simple_Note();
                SNlist[SNlist.Length - 1] = SN;
            }
        }
        foreach (KeyValuePair<int, WNchunk> i in WNdict)
        {
            foreach (Wall_Note WN in i.Value.WNbatch)
            {
                if (WN.color[0] != 0)
                {
                   WN.absorbable = true;
                }
                else
                {
                    WN.absorbable = false;
                }

                Array.Resize(ref WNlist, WNlist.Length + 1);
                WNlist[WNlist.Length - 1] = new Wall_Note();
                WNlist[WNlist.Length - 1] = WN;
            }
        }
        foreach (KeyValuePair<int, CNchunk> i in CNdict)
        {
            foreach (Cracker_Note CN in i.Value.CNbatch)
            {
                if (CN.color[0] != 0)
                {
                    CN.absorbable = true;
                }
                else
                {
                    CN.absorbable = false;
                }

                CN.punchable = true;

                Array.Resize(ref CNlist, CNlist.Length + 1);
                CNlist[CNlist.Length - 1] = new Cracker_Note();
                CNlist[CNlist.Length - 1] = CN;
            }
        }

        AllNotes.SimpleNotes.simple_note = SNlist;
        AllNotes.WallNotes.wall_note = WNlist;
        AllNotes.CrackerNotes.cracker_note = CNlist;


        foreach (KeyValuePair<int, SceneObject> i in SceneObjects)
        {
            SceneObject SO = i.Value;

            Array.Resize(ref SOlist, SOlist.Length + 1);
            SOlist[SOlist.Length - 1] = new SceneObject();
            SOlist[SOlist.Length - 1] = SO;
        }

        AllSceneObjects.scene_object = SOlist;

        string ChartString = JsonUtility.ToJson(AllNotes, true);
        string FolderPath = Path.Combine(Application.persistentDataPath, "Battles", BattleName);
        File.WriteAllText(FolderPath + "/Chart.json", ChartString);

        string ObjectsString = JsonUtility.ToJson(AllSceneObjects, true);
        FolderPath = Path.Combine(Application.persistentDataPath, "Battles", BattleName);
        File.WriteAllText(FolderPath + "/Objects.json", ObjectsString);
    }

    public void SelectAndLoadSprite()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open file", "", "png", false);

        if (paths == null || paths.Length == 0)
        {
            return;
        }
        // Debug.Log(paths[0]);

        string sourceFilePath = paths[0];

        if (!File.Exists(sourceFilePath))
        {
            return;
        }

        string battleSpriteDirectory = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Sprites");

        if (!Directory.Exists(battleSpriteDirectory))
        {
            Directory.CreateDirectory(battleSpriteDirectory);
        }

        string fileName = Path.GetFileName(sourceFilePath);
        string destinationPath = Path.Combine(battleSpriteDirectory, fileName);

        if (File.Exists(destinationPath))
        {
            return;
        }

        File.Copy(sourceFilePath, destinationPath);

        RefreshSpriteList();
        SpritePageCount = (int)Math.Ceiling(((float)AllBattleSprites.battle_sprite.Length) / 5f);
        DisplaySprites();
    }

    public void RefreshSpriteList()
    {
        string battleSpriteDirectory = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Sprites");
        string[] allSprites = Directory.GetFiles(battleSpriteDirectory, "*.png").Select(Path.GetFileName).ToArray();
        
        AllBattleSprites = new BattleSpriteList();
        Array.Resize(ref AllBattleSprites.battle_sprite, allSprites.Length);

        if (allSprites.Length == 0 || allSprites == null)
        {
            return;
        }

        int loopCount = 0;
        foreach (string spriteName in allSprites)
        {
            Battle_Sprite BS = new Battle_Sprite();
            BS.spriteName = allSprites[loopCount];
            AllBattleSprites.battle_sprite[loopCount] = BS;
            loopCount += 1;
        }

        SpritePageCount = (int)Math.Ceiling(((float)AllBattleSprites.battle_sprite.Length) / 5f);
    }

    public void DisplaySprites()
    {
        string battleSpriteDirectory = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Sprites");
        string[] allSprites = Directory.GetFiles(battleSpriteDirectory, "*.png").Select(Path.GetFileName).ToArray();
        string[] spriteBatch = new string[5];

        // Debug.Log(allSprites.Length);

        int count = 0;
        for (int i = 5 * (SpriteWindowPage - 1); i <= 4 + 5 * (SpriteWindowPage - 1); i++)
        {
            // if (allSprites[i] != null)
            if (i < allSprites.Length)
            {
                // Debug.Log(i);
                // Debug.Log(allSprites.Length);
                spriteBatch[count] = allSprites[i];
            }
            count += 1;
        }

        count = 0;
        foreach (GameObject spriteFrame in SpriteWindows)
        {
            if (spriteBatch[count] != null)
            {
                SpriteWindows[count].transform.GetChild(0).gameObject.SetActive(true);

                GameObject spriteImage = SpriteWindows[count].transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

                string imagePath = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Sprites", spriteBatch[count]);
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Texture2D imageTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                imageTexture.filterMode = FilterMode.Point;
                imageTexture.Apply(true);
                imageTexture.LoadImage(imageBytes);

                spriteImage.GetComponent<RawImage>().texture = imageTexture;
                SpriteWindows[count].GetComponent<SpriteWindowValues>().SpriteName = spriteBatch[count];

                GameObject TextObj = spriteFrame.transform.GetChild(0).Find("TextContainer").GetChild(0).gameObject;
                string Stext = spriteBatch[count];
                TMP_Text text = TextObj.GetComponent<TMP_Text>();
                text.text = Stext;
            }
            else
            {
                SpriteWindows[count].transform.GetChild(0).gameObject.SetActive(false);
            }
            count += 1;
        }
    }

    public void DeleteSprite(GameObject Button)
    {
        string spriteToDelete = Button.GetComponentInParent<SpriteWindowValues>().SpriteName;
        Debug.Log(spriteToDelete);
        string filePath = Path.Combine(Application.persistentDataPath, "Battles", BattleName, "Sprites", spriteToDelete);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else
        {
            Debug.Log(filePath);
        }
        RefreshSpriteList();
        
        if (SpriteWindowPage > SpritePageCount)
        {
            SpriteWindowPage = SpritePageCount;
            string pageText = "PAGE " + SpriteWindowPage;
            TMP_Text text = SpritePageText.GetComponent<TMP_Text>();
            text.text = pageText;
        }
        DisplaySprites();
    }

    public void ChangePage(int step)
    {
        if ((SpriteWindowPage != SpritePageCount) || step < 0)
        {
            if (!(SpriteWindowPage == 1 & step < 0))
            {
                SpriteWindowPage += step;
            }
        }

        DisplaySprites();
        Debug.Log(SpriteWindowPage);

        string pageText = "PAGE " + SpriteWindowPage;
        TMP_Text text = SpritePageText.GetComponent<TMP_Text>();
        text.text = pageText;
    }

    private void ChangeDrawMode()
    {
        DrawModeSwitch = !DrawModeSwitch;
        Debug.Log(DrawModeSwitch);
        if (DrawModeSwitch == false)
        {
            NoteDrawInput = Input.GetMouseButton;
            FlashWarning("Draw mode: HOLD");
        }
        else
        {
            NoteDrawInput = Input.GetMouseButtonDown;
            FlashWarning("Draw mode: SINGLE");
        }

    }

    void Update()
    {
        CurrentChunk = (int)Math.Ceiling(Math.Round(YParallax / 400f / 0.7, 2));
        DrawChunks(false);
        // Debug.Log(CurrentChunk);
        if (PlaybackActive)
        {
            TimeSinceCall += Time.deltaTime;
        }

        Vector3 Scroll = Mouse.current.scroll.ReadValue();
        MousePosition = new Vector3(Input.mousePosition.x / MainCanvas.GetComponent<RectTransform>().anchoredPosition.x, Input.mousePosition.y + YParallax, 0f);
        // Debug.Log(YParallax);
        // Debug.Log(transform.TransformPoint(Note_Container.transform.position));

        float YOffset = Note_Container.transform.InverseTransformPoint(0f, Note_Container.transform.position.y - YParallax, 0f).y;
        // Debug.Log(Note_Container.transform.InverseTransformPoint(0f, Note_Container.transform.position.y - YParallax, 0f).y);

        Vector2 NCRectTransform = Note_Container.GetComponent<RectTransform>().anchorMin;
        Lane_X = new List<float>() {NCRectTransform.x - 180f, NCRectTransform.x - 92f, NCRectTransform.x - 4f, NCRectTransform.x + 84f, NCRectTransform.x + 172f, NCRectTransform.x + 260f, NCRectTransform.x + 325f, NCRectTransform.x + 390f, NCRectTransform.x + 455f};
        // Trigger_Lane_X = new List<float>() {NCRectTransform.x + 220f, NCRectTransform.x + 280f, NCRectTransform.x + 340f, NCRectTransform.x + 400f};
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeDrawMode();
        }

        Debug.Log(MousePosition);


        // if (Input.mousePosition.x < 725f & Input.mousePosition.x > 390f)
        if (MousePosition.x > 0.71f & MousePosition.x < 1.48f & Importing == false)
        {
            GetClosestLane();
            Cursor.transform.position = transform.TransformPoint(new Vector3(Lane_X[(int)Closest_Lane] - 8f, Local_Y + YOffset, 1f));    // Ставит курсор
            Cursor.transform.position = new Vector3(Cursor.transform.position.x, Closest_Y, 1f);
            // Debug.Log(Closest_Y);

            if (Paused)
            {
                return;
            }

            // if (Selected_Tool_Button != null)
            // {
            //     AddNote();
            // }

            if (NoteDrawInput(0) & Selected_Tool_Button == SimpleNoteButton)
            {
                AddNote(SimpleNote);
            }
            else if (NoteDrawInput(0) & Selected_Tool_Button == WallNoteButton)
            {
                AddNote(WallNote);
            }
            else if (NoteDrawInput(0) & Selected_Tool_Button == CrackerNoteButton)
            {
                AddNote(CrackerNote);
            }
            else if (NoteDrawInput(0) & Selected_Tool_Button == AlphaTriggerButton)
            {
                AddTrigger(AlphaTrigger);
            }
            else if (NoteDrawInput(0) & Selected_Tool_Button == SpriteTriggerButton)
            {
                AddTrigger(SpriteTrigger);
            }
        }
        else
        {
            Cursor.transform.position = new Vector3(-10f, -10f, 1f);            // скрывает курсор
        }


        if (Input.GetKeyUp(KeyCode.Delete) & Selected_Note != null)
        {
            // Occupied_Coords.Remove(Selected_Note.GetComponent<SimpleNoteStats>().occupied_space);
            // Array.Resize(ref FreeNoteIDs, FreeNoteIDs.Length + 1);
            // FreeNoteIDs[FreeNoteIDs.Length - 1] = Selected_Note.GetComponent<SimpleNoteStats>().editorID;

            // Destroy(Selected_Note);
            DeleteNote();
        }


        if (Selected_Note != null)          // Тогглит кнопки сбоку экрана, чтоб не нажимались, если нечего ими задавать
        {
            if (Selected_Note.GetComponent<SimpleNoteStats>().type == "simple_note")
            {
                
                WidthButtonsContainer.transform.Find("Width-1").GetComponent<Button>().interactable = true;
                WidthButtonsContainer.transform.Find("Width-2").GetComponent<Button>().interactable = true;
                WidthButtonsContainer.transform.Find("Width-3").GetComponent<Button>().interactable = true;
                WidthButtonsContainer.transform.Find("Width-4").GetComponent<Button>().interactable = true;
                WidthButtonsContainer.transform.Find("Width-5").GetComponent<Button>().interactable = true;
            }
            else
            {
                WidthButtonsContainer.transform.Find("Width-1").GetComponent<Button>().interactable = false;
                WidthButtonsContainer.transform.Find("Width-2").GetComponent<Button>().interactable = false;
                WidthButtonsContainer.transform.Find("Width-3").GetComponent<Button>().interactable = false;
                WidthButtonsContainer.transform.Find("Width-4").GetComponent<Button>().interactable = false;
                WidthButtonsContainer.transform.Find("Width-5").GetComponent<Button>().interactable = false;
            }

            SpeedValueObject.GetComponent<TMP_Text>().text = SpeedValueText;
            DelayValueObject.GetComponent<TMP_Text>().text = DelayValueText;
        }
        else
        {
            WidthButtonsContainer.transform.Find("Width-1").GetComponent<Button>().interactable = false;
            WidthButtonsContainer.transform.Find("Width-2").GetComponent<Button>().interactable = false;
            WidthButtonsContainer.transform.Find("Width-3").GetComponent<Button>().interactable = false;
            WidthButtonsContainer.transform.Find("Width-4").GetComponent<Button>().interactable = false;
            WidthButtonsContainer.transform.Find("Width-5").GetComponent<Button>().interactable = false;

            SpeedValueObject.GetComponent<TMP_Text>().text = "";
            DelayValueObject.GetComponent<TMP_Text>().text = "";
        }

        
        ScrollOnWheel(Scroll);

        // ResizeContainer();

        // CalculateNCPosition();

        if (Input.GetKeyUp(KeyCode.Space))          // Запуск плейбека на пробел
            {
                   
                PlaybackActive = !PlaybackActive;
                if (PlaybackActive == true)
                {
                sound.time = PlayFromTime();
                sound.Play();
                PlayBackCoroutine = StartCoroutine(MusicGuidelineStart());
                
                }
                else
                {
                sound.Pause();
                StopCoroutine(PlayBackCoroutine);
                }
            }
        
    }
}