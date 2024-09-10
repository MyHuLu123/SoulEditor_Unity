using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SFB;
public class GetMusicFile : MonoBehaviour
{
    [HideInInspector]
    public string _path = "";

    // Start is called before the first frame update
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OpenFile);
    }

    // Update is called once per frame
    private void OpenFile()
    {
        var title = "Music Select";
        var extensions = new[] {
            new ExtensionFilter("Music File","mp3","wav","ogg")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel(title,"",extensions, false);
        if (paths.Length > 0)
        {
            _path = paths[0];
            Debug.Log(_path);
            
        }
    }
}
