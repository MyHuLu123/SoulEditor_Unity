using TMPro;
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AddChart : MonoBehaviour
{
    public TMP_InputField _folder;
    public TMP_InputField _file;
    public TMP_InputField _bpm;
    public TMP_InputField _name;
    public InputField _composer;
    public TMP_InputField _chart;
    public TMP_InputField _illustrate;
    public TMP_InputField _type;
    public TMP_InputField _level;
    public GetMusicFile _musicfile;
    [Serializable]
    public struct NoteData
    {
        public string type;
        public double beat_start;
        public double beat_end;
        public double time_start;
        public double time_end;
        public int targetbox;
        public Color color;
        public double xoffset;
        public double yoffset;
        public double speedoffset;
        public double angleoffset;


    };
    [Serializable]
    public struct BoxData
    {
        public double x;
        public double y;
        public double speed;
        public Color color;
        public double angle;

    };
    [Serializable]
    public struct EventData
    {
        public string Object;
        public double time;
        public double during;
        public double start;
        public double end;
        public string Tween;
        public string Ease;

    };
    [Serializable]
    public struct ChartData
    {
        public string name;
        public string composer;
        public string chart;
        public string illustrate;
        public string type;
        public string level;
        public float bpm;
        public int boxnum;
        public BoxData[] boxes;
        public int notenum;
        public NoteData[] notes;
        public int eventnum;
        public EventData[] events; 
        

    };
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    private void OnClick()
    {
        ChartData data = new ChartData();
        if (_musicfile._path.Length > 0)
        {
            var path = Application.persistentDataPath + "/LoadLevel.txt";
            if (!float.TryParse(_bpm.text, out data.bpm))
            {
                Debug.LogError("Wrong BPM!");
                return;
            }       
            data.name = _name.text;
            data.composer = _composer.text;
            data.chart = _chart.text;
            data.illustrate = _illustrate.text;
            data.type = _type.text;
            data.level = _level.text;
            data.boxnum = data.notenum = data.eventnum = 0;
            string jsonData = JsonUtility.ToJson(data);
            string FolderName = _folder.text;
            string FileName = _file.text + ".json";
            string FolderPath = Path.Combine(Application.persistentDataPath, FolderName);
            string FilePath = Path.Combine(FolderPath, FileName);
            string pMusicPath = "";
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                pMusicPath = @"file://" + _musicfile._path;
            }
            else if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                pMusicPath = @"file:///" + _musicfile._path;
            }
            string MusicPath = FolderPath+"/"+_name.text+".mp3";
            if (!string.IsNullOrEmpty(FolderPath) && !Directory.Exists(FolderPath))
            {
                string[] contents = { FolderName, FileName, data.name };
                File.WriteAllLines(path, contents);
                
                Directory.CreateDirectory(FolderPath);
                WWW ww = new WWW(pMusicPath);
                while (!ww.isDone) { }
                var buffer = ww.bytes;
                if (File.Exists(MusicPath))
                    File.Delete(MusicPath);
                var ws = File.Create(MusicPath);
                ws.Write(buffer, 0, buffer.Length);
                ws.Close();
                ww.Dispose();
                File.WriteAllText(FilePath, jsonData);
                Debug.Log("Saved!Path:"+FolderPath);

                var HistoryPath = Application.persistentDataPath + "/NowFiles.txt";
                File.AppendAllLines(HistoryPath,contents);

                SceneManager.LoadScene("edit");
            }
            else
            {
                Debug.LogError("Failed!");
                return;
            }
        }
        else
        {
            Debug.LogError("Warning!No Music Selected!");
        }
    }

}
