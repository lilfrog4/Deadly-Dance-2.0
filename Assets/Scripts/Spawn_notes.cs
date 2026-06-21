using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System.Linq;


public class Spawn_notes : MonoBehaviour
{
    public bool CampaignBattle;
    private string Folder;
    public GameObject AudioSource;
    public GameObject simpleNote;

    public GameObject wallNote;

    public GameObject crackerNote;

    public TextAsset ChartData;

    private List<float> laneList = new List<float>() {-4f, -2f, 0f, 2f, 4f};

    public float loopDelay;          // Длина всего чарта, по сути

    [SerializeField] private bool Allow_Looping = true;
    private bool loopable = true;           // Для контроля лупов

    public string BattleName;
    public GameObject SceneObjectManager;
    public GameObject SceneObjectPrefab;


    [SerializeField] private Sprite One_Wide_Sprite;
    [SerializeField] private Sprite Two_Wide_Sprite;
    [SerializeField] private Sprite Three_Wide_Sprite;
    [SerializeField] private Sprite Four_Wide_Sprite;
    [SerializeField] private Sprite Five_Wide_Sprite;
    [SerializeField] private Sprite One_Wide_Shield_Sprite;
    [SerializeField] private Sprite Two_Wide_Shield_Sprite;
    [SerializeField] private Sprite Three_Wide_Shield_Sprite;
    [SerializeField] private Sprite Four_Wide_Shield_Sprite;
    [SerializeField] private Sprite Five_Wide_Shield_Sprite;
    [SerializeField] private Sprite Regular_Wall;
    [SerializeField] private Sprite Shield_Wall;

    private Sprite[] SimpleNoteSprites;
    private Sprite[] SimpleShieldNoteSprites;
    private Sprite[] WallSprites;




    [System.Serializable]
    public class SceneObject
    {
        public int editorID;            // Уникальное id
        public List<int> objectIDs;            // Неуникальные id для редактора
        public string spriteName;       // Спрайт, с которым объект начинает
        public float alpha;             // Альфа, с которым начинает
        public float Xpos;              // Стартовая X позиция
        public float Ypos;              // Стартовая Y позиция
        public float scale;             // Стартовый множитель масштаба
        public int zOrder;              // Порядок по Z
    }

    public class SceneObjectList
    {
        public SceneObject[] scene_object = new SceneObject[0];
    }

    public SceneObjectList AllSceneObjects = new SceneObjectList();
    public Dictionary<int, List<GameObject>> SOdict = new Dictionary<int, List<GameObject>>();

    public Dictionary<string, Sprite> SpriteDict = new Dictionary<string, Sprite>();


    [System.Serializable]
                                                // КЛАССЫ НОТ
    public class Simple_note
    {
        public int editorID;
        public List<float> color;
        public List<int> rand_lanes;
        public float delay;
        public int speed;
        public int width;
        public bool absorbable;
        public bool punchable;
    }
                                                // КЛАССЫ СПИСКОВ ДЛЯ НОТ
    [System.Serializable]
    public class SimpleNotes_list
    {
        public Simple_note[] simple_note;
    }

    public SimpleNotes_list simplenote_list = new SimpleNotes_list();


    [System.Serializable]
    public class Wall_note
    {
        public int editorID;
        public List<float> color;
        public List<int> rand_lanes;
        public float delay;
        public int speed;
        public bool absorbable;
        public bool punchable;
    }

    [System.Serializable]
    public class WallNotes_list
    {
        public Wall_note[] wall_note;
    }

    public WallNotes_list wallnote_list = new WallNotes_list();

    [System.Serializable]
    public class Cracker_note
    {
        public int editorID;
        public List<float> color;
        public List<int> rand_lanes;
        public float delay;
        public int speed;
        public bool absorbable;
        public bool punchable;
    }

    [System.Serializable]
    public class CrackerNotes_list
    {
        public Cracker_note[] cracker_note;
    }

    public CrackerNotes_list crackernote_list = new CrackerNotes_list();

    [System.Serializable]
    public class AllNoteList
    {
        public SimpleNotes_list SimpleNotes = new SimpleNotes_list();
        public WallNotes_list WallNotes = new WallNotes_list();
        public CrackerNotes_list CrackerNotes = new CrackerNotes_list();
    }
    public AllNoteList AllNotes = new AllNoteList();


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

    [System.Serializable]
    public class AllTriggerList
    {
        public AlphaTriggerList AlphaTriggers = new AlphaTriggerList();
        public SpriteTriggerList SpriteTriggers = new SpriteTriggerList();
    }
    public AllTriggerList AllTriggers = new AllTriggerList();


    private void LoadBattleData()
    {
        if (CampaignBattle == false)
        {
            AudioSource.GetComponent<AudioSource>().clip = BattleManagerScript.battleSong;
            BattleName = BattleManagerScript.battleName;
        }
        

        string chartPath = Path.Combine(Folder, "Battles", BattleName, "Chart.json");
        string objectsPath = Path.Combine(Folder, "Battles", BattleName, "Objects.json");
        string triggersPath = Path.Combine(Folder, "Battles", BattleName, "Triggers.json");

        string chartString = File.ReadAllText(chartPath);
        string objectsString = File.ReadAllText(objectsPath);
        string triggersString = File.ReadAllText(triggersPath);

        if (!(chartString == ""))
        {
            AllNotes = JsonUtility.FromJson<AllNoteList>(chartString);
        }
        if (!(objectsString == ""))
        {
            AllSceneObjects = JsonUtility.FromJson<SceneObjectList>(objectsString);
        }
        if (!(triggersString == ""))
        {
            AllTriggers = JsonUtility.FromJson<AllTriggerList>(triggersString);
        }

        AllAlphaTriggers = AllTriggers.AlphaTriggers;
        AllSpriteTriggers = AllTriggers.SpriteTriggers;


        simplenote_list = AllNotes.SimpleNotes;
        wallnote_list = AllNotes.WallNotes;
        crackernote_list = AllNotes.CrackerNotes;

        string battleSpriteDirectory = Path.Combine(Folder, "Battles", BattleName, "Sprites");
        string[] allSprites = Directory.GetFiles(battleSpriteDirectory, "*.png").Select(Path.GetFileName).ToArray();

        foreach (string spriteName in allSprites)
        {
            string filePath = Path.Combine(battleSpriteDirectory, spriteName);
            SpriteDict[spriteName] = LoadSprite(filePath);
        }
    
        foreach (SceneObject SO in AllSceneObjects.scene_object)
        {
            GameObject ScObj = Instantiate(SceneObjectPrefab, new Vector3(SO.Xpos / 100f, SO.Ypos / 100f, SO.zOrder / -10f), transform.rotation);
            ScObj.transform.SetParent(SceneObjectManager.transform, false);
            
            SpriteRenderer ScObjSR = ScObj.GetComponent<SpriteRenderer>();
            if (SpriteDict.ContainsKey(SO.spriteName))
            {
                ScObjSR.sprite = SpriteDict[SO.spriteName];
            }
            
            ScObjSR.color = new Color(ScObjSR.color.r, ScObjSR.color.g, ScObjSR.color.b, SO.alpha / 100f);
            ScObj.transform.localScale *= SO.scale;

            foreach (int GroupID in SO.objectIDs)
            {
                SOdict.TryAdd(GroupID, new List<GameObject>());
                SOdict[GroupID].Add(ScObj);
            }
            
            // if (SO.objectIDs.Count == 0)
            // {
            //     GameObject ScObj = Instantiate(SceneObjectPrefab, new Vector3(0f, 0f, 0f), transform.rotation);
            //     ScObj.transform.SetParent(SceneObjectManager.transform, false);
            //     ScObj.GetComponent<SpriteRenderer>().sprite = SpriteDict[SO.spriteName];
            // }
        }
    }

    private Sprite LoadSprite(string filePath)
    {
        byte[] textureBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        // texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.Apply(true);
        texture.LoadImage(textureBytes);

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        return Sprite.Create(texture, rect, pivot, 100f, 0, SpriteMeshType.FullRect);
    }

    public void PlayChart()
    {
        if (simplenote_list.simple_note != null)
        {
            foreach (Simple_note i in simplenote_list.simple_note)              // ПЕРВЫЙ ЦИКЛ ЧАРТА
            {
                StartCoroutine(Spawn_simple_note(i.width, i.speed, i.color, i.rand_lanes, i.delay, i.absorbable, i.punchable));

                if (i.delay > loopDelay)            // Вычисление длины чарта
                {
                    loopDelay = i.delay;
                }
            }
        }
        
        // Debug.Log(loopDelay);

        if (wallnote_list.wall_note != null)
        {
            foreach (Wall_note i in wallnote_list.wall_note)
            {
                StartCoroutine(Spawn_wall_note(i.speed, i.color, i.rand_lanes, i.delay, i.absorbable, i.punchable));

                if (i.delay > loopDelay)            // Вычисление длины чарта
                {
                    loopDelay = i.delay;
                }
            }
        }
        
        if (crackernote_list.cracker_note != null)
        {
            foreach (Cracker_note i in crackernote_list.cracker_note)
            {
                StartCoroutine(Spawn_cracker_note(i.speed, i.color, i.rand_lanes, i.delay, i.absorbable, i.punchable));

                if (i.delay > loopDelay)            // Вычисление длины чарта
                {
                    loopDelay = i.delay;
                }
            }
        }

        if (AllAlphaTriggers.alpha_trigger != null)
        {
            foreach (Alpha_Trigger AT in AllAlphaTriggers.alpha_trigger)
            {
                if (SOdict.ContainsKey(AT.usedGroup))
                {
                    StartCoroutine(ActivateAlphaTrigger(AT.usedGroup, AT.delay, AT.opacity, AT.duration));
                }

                if (AT.delay > loopDelay)
                {
                    loopDelay = AT.delay;
                }
            }
        }

        if (AllSpriteTriggers.sprite_trigger != null)
        {
            foreach (Sprite_Trigger ST in AllSpriteTriggers.sprite_trigger)
            {
                if (SOdict.ContainsKey(ST.usedGroup))
                {
                    StartCoroutine(ActivateSpriteTrigger(ST.usedGroup, ST.delay, ST.spritename));
                }
            }
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (CampaignBattle)
        {
            Folder = Application.streamingAssetsPath;
        }
        else
        {
            Folder = Application.persistentDataPath;
        }

        SimpleNoteSprites = new Sprite[] { One_Wide_Sprite, Two_Wide_Sprite, Three_Wide_Sprite, Four_Wide_Sprite, Five_Wide_Sprite};
        SimpleShieldNoteSprites = new Sprite[] { One_Wide_Shield_Sprite, Two_Wide_Shield_Sprite, Three_Wide_Shield_Sprite, Four_Wide_Shield_Sprite, Five_Wide_Shield_Sprite};
        WallSprites = new Sprite[] { Shield_Wall, Regular_Wall };

        LoadBattleData();
        // Debug.Log(SpriteDict.Count);
        PlayChart();
    }

    void Update()
    {
        if (Allow_Looping)
        {
            if (loopable)
            {
                StartCoroutine(Loop_Chart());
            }
        }
    }

    IEnumerator ActivateSpriteTrigger(int group, float delay, string spritename)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject SO in SOdict[group])
        {
            if (SpriteDict.ContainsKey(spritename))
            {
                SO.GetComponent<SpriteRenderer>().sprite = SpriteDict[spritename];
            }
        }
    }

    IEnumerator ActivateAlphaTrigger(int group, float delay, float opacity, float duration)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject SO in SOdict[group])
        {
            Coroutine AlphaCoroutine = SO.GetComponent<SceneObjectValues>().AlphaCoroutine;

            if (AlphaCoroutine != null)
            {
                Debug.Log("stopped");
                StopCoroutine(AlphaCoroutine);
            }
            SO.GetComponent<SceneObjectValues>().AlphaCoroutine = StartCoroutine(ProcessAlphaTrigger(SO, group, opacity, duration));
        }
    }

    IEnumerator ProcessAlphaTrigger(GameObject SO, int group, float opacity, float duration)
    {
        Debug.Log("alpha");
        SpriteRenderer ObjectSR = SO.GetComponent<SpriteRenderer>();
        float StartAlpha = ObjectSR.color.a;
        float deltaAlpha = opacity - StartAlpha;
        // Debug.Log(StartAlpha);
        // SO.GetComponent<SceneObjectValues>().AlphaCoroutine = StartCoroutine(ActivateAlphaTrigger());
        float timer = 0f;

        while (timer < duration)
        {
            Debug.Log(SO.GetComponent<SceneObjectValues>().AlphaCoroutine);
            float currentAlpha = StartAlpha + deltaAlpha * (timer / duration);
            // Debug.Log(currentAlpha);
            ObjectSR.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
            timer += Time.deltaTime;
        }
        ObjectSR.color = new Color(1f, 1f, 1f, opacity);
    }

    IEnumerator Spawn_simple_note(int width, int speed, List<float> color, List<int> rand_lanes, float delay, bool Absorbable, bool Punchable)
    {
        yield return new WaitForSeconds(delay);         // ВРЕМЯ ПОЯВЛЕНИЯ НОТЫ

        int random_lane_number = Random.Range(0, rand_lanes.Count);         // РАНДОМНЫЕ ЛИНИИ НОТЫ
        int random_file_lane = rand_lanes[random_lane_number];
        float random_lane = laneList[random_file_lane];
        Vector3 spawnpos = new Vector3(random_lane, -4f, transform.position.z);

        float spawn_X_offset = (1 - width) * ((width + 1) % 2);            // оффсет для центра спавна у четных нот, чтоб ровно по линиям лежали


        GameObject InstNote = Instantiate(simpleNote, spawnpos - new Vector3(spawn_X_offset, 0f, 0f), transform.rotation);

        InstNote.GetComponent<NoteMovement_basic>().notespeed = speed;            // СКОРОСТЬ НОТЫ

        ParticleSystem particle_system = InstNote.GetComponentInChildren<ParticleSystem>();           // Цвет частиц

        // particle_system.gameObject.SetActive(false);                                                  // Здесь отключение
        
        ParticleSystem.MainModule particle_system_main = particle_system.main;          
        particle_system_main.startColor = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.9f);
        ParticleSystem.ShapeModule particle_system_shape = particle_system.shape;

        GameObject Glow = InstNote.transform.Find("Glow").gameObject;
        // Glow.GetComponent<SpriteRenderer>().color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.05f);             // Цвет свечения
        if (color[0] == 0)
        {
            Glow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.07f);
        }
        else
        {
            Glow.GetComponent<SpriteRenderer>().color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.05f);
        }

        // MeshFilter Mesh_Filter = InstNote.transform.Find("Model").GetComponent<MeshFilter>();
        SpriteRenderer noteSprite = InstNote.transform.Find("Sprite").GetComponent<SpriteRenderer>();

        

        BoxCollider Note_collider = InstNote.GetComponentInChildren<BoxCollider>();           // ШИРИНА НОТЫ
        if (width == 1)
        {
            // Mesh_Filter.mesh = One_Wide_Mesh;
            // noteSprite.sprite = One_Wide_Sprite;
            Note_collider.size = new Vector3(0.8f, Note_collider.size.y, Note_collider.size.z);
            particle_system_shape.radius = 0.6f;
            Glow.transform.localScale = new Vector3(0.6f, 0.36f, 0.6f);
        }
        else if (width == 2)
        {
            // Mesh_Filter.mesh = Two_Wide_Mesh;
            // noteSprite.sprite = Two_Wide_Sprite;
            Note_collider.size = new Vector3(2.6f, Note_collider.size.y, Note_collider.size.z);
            particle_system_shape.radius = 1.4f;
            Glow.transform.localScale = new Vector3(1.1f, 0.36f, 0.6f);
        }
        else if (width == 3)
        {
            // Mesh_Filter.mesh = Three_Wide_Mesh;
            // noteSprite.sprite = Three_Wide_Sprite;
            Note_collider.size = new Vector3(4.6f, Note_collider.size.y, Note_collider.size.z);
            particle_system_shape.radius = 2.2f;
            Glow.transform.localScale = new Vector3(1.5f, 0.36f, 0.6f);
        }
        else if (width == 4)
        {
            // Mesh_Filter.mesh = Four_Wide_Mesh;
            // noteSprite.sprite = Four_Wide_Sprite;
            Note_collider.size = new Vector3(6.6f, Note_collider.size.y, Note_collider.size.z);
            particle_system_shape.radius = 3f;
            Glow.transform.localScale = new Vector3(1.9f, 0.36f, 0.6f);
        }
        else if (width == 5)
        {
            // Mesh_Filter.mesh = Five_Wide_Mesh;
            // noteSprite.sprite = Five_Wide_Sprite;
            Note_collider.size = new Vector3(8.6f, Note_collider.size.y, Note_collider.size.z);
            particle_system_shape.radius = 4.4f;
            Glow.transform.localScale = new Vector3(2.5f, 0.36f, 0.6f);
        }

        if (Punchable)
        {
            noteSprite.sprite = SimpleNoteSprites[width - 1];
        }
        else
        {
            noteSprite.sprite = SimpleShieldNoteSprites[width - 1];
        }


        noteSprite.color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f);

        InstNote.GetComponent<NoteValues>().Absorbable = Absorbable;
        InstNote.GetComponent<NoteValues>().Punchable = Punchable;
        InstNote.GetComponent<NoteValues>().NoteColor = (int)(color[0] * 1000000 + color[1] * 1000 + color[2]);
        // Renderer InstNoteRenderer = InstNote.transform.Find("Model").GetComponent<Renderer>();                                                                  // ЦВЕТ 3D МОДЕЛИ
        // InstNoteRenderer.material.color = new Color32(System.Convert.ToByte(color[0]), System.Convert.ToByte(color[1]), System.Convert.ToByte(color[2]), 255);  // (т.к. меняем материал, работаем уже с заспавненным объектом, а не префабом)
    }

    IEnumerator Spawn_wall_note(int speed, List<float> color, List<int> rand_lanes, float delay, bool Absorbable, bool Punchable)
    {
        yield return new WaitForSeconds(delay);         // ВРЕМЯ ПОЯВЛЕНИЯ НОТЫ

        int random_lane_number = Random.Range(0, rand_lanes.Count);         // РАНДОМНЫЕ ЛИНИИ НОТЫ
        int random_file_lane = rand_lanes[random_lane_number];
        float random_lane = laneList[random_file_lane];
        Vector3 spawnpos = new Vector3(random_lane, -4f, transform.position.z);

        GameObject InstWallNote = Instantiate(wallNote, spawnpos, transform.rotation);

        SpriteRenderer InstGroundNoteRenderer = InstWallNote.transform.Find("GroundNote").GetComponent<SpriteRenderer>();
        SpriteRenderer InstWallNoteRenderer = InstWallNote.transform.Find("Wall").GetComponent<SpriteRenderer>();

        InstWallNoteRenderer.color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f);
        InstGroundNoteRenderer.color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f);

        GameObject Glow = InstWallNote.transform.Find("Glow").gameObject;
        // Glow.GetComponent<SpriteRenderer>().color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.05f);             // Цвет свечения
        if (color[0] == 0)
        {
            Glow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.07f);
        }
        else
        {
            Glow.GetComponent<SpriteRenderer>().color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.05f);
        }

        ParticleSystem particle_system = InstWallNote.GetComponentInChildren<ParticleSystem>();           // Цвет частиц
        ParticleSystem.MainModule particle_system_main = particle_system.main;          
        particle_system_main.startColor = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.9f);
        ParticleSystem.ShapeModule particle_system_shape = particle_system.shape;
        particle_system_shape.radius = 0.6f;

        InstWallNote.GetComponent<NoteMovement_basic>().notespeed = speed;            // СКОРОСТЬ НОТЫ

        InstWallNote.GetComponent<NoteValues>().Absorbable = Absorbable;
        InstWallNote.GetComponent<NoteValues>().Punchable = Punchable;
        InstWallNote.GetComponent<NoteValues>().NoteColor = (int)(color[0] * 1000000 + color[1] * 1000 + color[2]);

        InstWallNote.transform.Find("Wall").GetComponent<SpriteRenderer>().sprite = WallSprites[Punchable ? 1 : 0];
    }

    IEnumerator Spawn_cracker_note(int speed, List<float> color, List<int> rand_lanes, float delay, bool Absorbable, bool Punchable)
    {
        yield return new WaitForSeconds(delay);         // ВРЕМЯ ПОЯВЛЕНИЯ НОТЫ

        int random_lane_number = Random.Range(0, rand_lanes.Count);         // РАНДОМНЫЕ ЛИНИИ НОТЫ
        int random_file_lane = rand_lanes[random_lane_number];
        float random_lane = laneList[random_file_lane];
        Vector3 spawnpos = new Vector3(random_lane, -4f, transform.position.z);

        GameObject InstCrackerNote = Instantiate(crackerNote, spawnpos, transform.rotation);

        // Renderer InstGroundNoteRenderer = InstCrackerNote.transform.Find("Model").GetComponent<Renderer>();
        SpriteRenderer InstGroundNoteRenderer = InstCrackerNote.transform.Find("GroundNote").GetComponent<SpriteRenderer>();
        SpriteRenderer InstFuseRenderer = InstCrackerNote.transform.Find("Fuse").GetComponent<SpriteRenderer>();
        SpriteRenderer InstFireRenderer = InstCrackerNote.transform.Find("Fire").GetComponent<SpriteRenderer>();

        InstGroundNoteRenderer.color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f);    // Цвет наземной ноты
        InstFuseRenderer.color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f);                                                                          // Цвет фитиля
        InstFireRenderer.color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f);                                                                          // Цвет запала

        GameObject Glow = InstCrackerNote.transform.Find("Glow").gameObject;
        // Glow.GetComponent<SpriteRenderer>().color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.05f);             // Цвет свечения
        if (color[0] == 0)
        {
            Glow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.07f);
        }
        else
        {
            Glow.GetComponent<SpriteRenderer>().color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.05f);
        }

        ParticleSystem particle_system = InstCrackerNote.GetComponentInChildren<ParticleSystem>();           // Цвет частиц
        ParticleSystem.MainModule particle_system_main = particle_system.main;          
        particle_system_main.startColor = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f, 0.9f);
        ParticleSystem.ShapeModule particle_system_shape = particle_system.shape;
        particle_system_shape.radius = 0.6f;

        InstCrackerNote.GetComponent<NoteMovement_basic>().notespeed = speed;            // СКОРОСТЬ НОТЫ

        InstCrackerNote.GetComponent<NoteValues>().Absorbable = Absorbable;
        InstCrackerNote.GetComponent<NoteValues>().Punchable = Punchable;
        InstCrackerNote.GetComponent<NoteValues>().NoteColor = (int)(color[0] * 1000000 + color[1] * 1000 + color[2]);
    }
    
    IEnumerator Loop_Chart()
    {
        loopable = false;
        yield return new WaitForSeconds(loopDelay);
        foreach (Simple_note i in simplenote_list.simple_note)              // ПОВТОРНЫЕ ЦИКЛЫ ЧАРТА (прибавляется loopDelay)
        {
            StartCoroutine(Spawn_simple_note(i.width, i.speed, i.color, i.rand_lanes, i.delay, i.absorbable, i.punchable));
            if (i.delay > loopDelay)
            {
                loopDelay = i.delay;
            }
        }

        foreach (Wall_note i in wallnote_list.wall_note)
        {
            StartCoroutine(Spawn_wall_note(i.speed, i.color, i.rand_lanes, i.delay, i.absorbable, i.punchable));
            if (i.delay > loopDelay)
            {
                loopDelay = i.delay;
            }
        }

        foreach (Cracker_note i in crackernote_list.cracker_note)
        {
            StartCoroutine(Spawn_cracker_note(i.speed, i.color, i.rand_lanes, i.delay, i.absorbable, i.punchable));
            if (i.delay > loopDelay)
            {
                loopDelay = i.delay;
            }
        }

        foreach (Alpha_Trigger AT in AllAlphaTriggers.alpha_trigger)
        {
            if (SOdict.ContainsKey(AT.usedGroup))
            {
                StartCoroutine(ActivateAlphaTrigger(AT.usedGroup, AT.delay, AT.opacity, AT.duration));
            }

            if (AT.delay > loopDelay)
            {
                loopDelay = AT.delay;
            }
            
        }

        loopable = true;
    }
}