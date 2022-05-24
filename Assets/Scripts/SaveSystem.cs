using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class SaveSystem {
    private static readonly string ROOT_PATH = Application.persistentDataPath;
    public static bool SaveJson<T>(T value, string path) {
        try {
            string output = JsonConvert.SerializeObject(value);

            string savePath = Path.Combine(ROOT_PATH, path);
            string directoryPath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(savePath, output);
            return true;
        }
        catch (Exception e) {
            UnityEngine.Debug.LogException(e);
            return false;
        }
    }
    public static bool LoadJson<T>(out T value, string path) {
        value = default;
        try {
            string savePath = Path.Combine(ROOT_PATH, path);
            if (File.Exists(savePath)) {
                string json = File.ReadAllText(savePath);
                T output = JsonConvert.DeserializeObject<T>(json);
                value = output;
                return true;
            }
            return false;
        }
        catch (Exception e) {
            UnityEngine.Debug.LogException(e);
            return false;
        }
    }

    [MenuItem("Save File/Open Save File")]
    private static void OpenSaveFiles() {
        string folderPath = ROOT_PATH;
        if (Directory.Exists(folderPath)) {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                UseShellExecute = true,
                FileName = folderPath,
                Verb = "open"
            };

            Process.Start(startInfo);
        }
    }

    [MenuItem("Save File/Clear Save Files")]
    private static void ClearSaveFiles() {
        if (EditorUtility.DisplayDialog("Warning!", "Do you want to clear save file?", "Go on!!", "Ok, i'am joke :)")) {
            string folderPath = ROOT_PATH;
            if (Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
        }
    }
}

public class SaveSystemExtension {
    public class Vector2Int {
        public int x, y;
        public Vector2Int(UnityEngine.Vector2Int unityVector2Int) {
            this.x = unityVector2Int.x;
            this.y = unityVector2Int.y;
        }
        public UnityEngine.Vector2Int ToUnityVector2Int() {
            return new UnityEngine.Vector2Int(this.x, this.y);
        }
    }
    public class Vector2 {
        public float x, y;
        public Vector2(UnityEngine.Vector2 unityVector2) {
            this.x = unityVector2.x;
            this.y = unityVector2.y;
        }
        public UnityEngine.Vector2 ToUnityVector2() {
            return new UnityEngine.Vector2(this.x, this.y);
        }
    }
}