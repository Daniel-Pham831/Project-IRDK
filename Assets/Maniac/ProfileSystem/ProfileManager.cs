using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
#endif

namespace Maniac.ProfileSystem
{
    public class ProfileManager
    {
        const string PROFILE_DATA_FOLDER_NAME = "Profiles";
        const string PROFILE_DATA_FILE_NAME_SUFFIX = "json";

        public Dictionary<string, ProfileRecord> recordsCache = new Dictionary<string, ProfileRecord>();

        public void Init()
        {
            PreLoadAllProfileRecordsIntoCache();
        }

        public T Get<T>() where T : ProfileRecord
        {
            if (recordsCache.TryGetValue(typeof(T).Name, out ProfileRecord value))
                return value as T;

            return Load<T>();
        }

        public bool Save(ProfileRecord record,string json = "")
        {
            bool result = false;
            try
            {
                string typeName = record.GetType().Name;
                string savePath = $"{GetProfileFolderPath()}/{typeName}.{PROFILE_DATA_FILE_NAME_SUFFIX}";

                if (json == string.Empty)
                    json = record.ToJson();
                
                File.WriteAllText(savePath, json);
                SaveCache(record);
                result = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return result;
        }
        
        

        public T Load<T>() where T : ProfileRecord
        {
            ProfileRecord record = null;
            string savePath = $"{GetProfileFolderPath()}/{typeof(T).Name}.{PROFILE_DATA_FILE_NAME_SUFFIX}";
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                record = JsonUtility.FromJson<T>(json);
            }
            else
            {
                record = Activator.CreateInstance<T>();
            }

            SaveCache(record);

            return record as T;
        }

        private void SaveCache(ProfileRecord record)
        {
            string typeName = record.GetType().Name;
            if (!recordsCache.ContainsKey(typeName))
            {
                recordsCache.Add(typeName, record);
            }

            recordsCache[typeName] = record;
        }

        private void PreLoadAllProfileRecordsIntoCache()
        {
            Dictionary<string, Type> typeList = GetListTypeBaseOnProfile();

            string savePath = $"{GetProfileFolderPath()}";
            string[] filePaths = Directory.GetFiles(savePath, $"*.{PROFILE_DATA_FILE_NAME_SUFFIX}");
            foreach (string file in filePaths)
            {
                string json = File.ReadAllText(file);
                string fileName = Path.GetFileName(file);
                string fileType = fileName.Replace($".{PROFILE_DATA_FILE_NAME_SUFFIX}", "");
                try
                {
                    if (typeList[fileType] == null) continue;

                    ProfileRecord record =  JsonUtility.FromJson(json,typeList[fileType]) as ProfileRecord;
                    recordsCache.Add(record.GetType().Name, record);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning(e);
                    File.Delete(file);
                }
            }
        }

        private Dictionary<string, Type> GetListTypeBaseOnProfile()
        {
            Dictionary<string, Type> objects = new Dictionary<string, Type>();
            IEnumerable<Type> types = typeof(ProfileRecord).GetAllSubclasses();
            foreach (Type type in types)
            {
                objects.Add(type.Name, type);
            }

            return objects;
        }

        private string GetProfileFolderPath()
        {
            string folderPath = $"{Application.persistentDataPath}/{PROFILE_DATA_FOLDER_NAME}";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

#if UNITY_EDITOR
        [MenuItem("Maniac/Profile/Clear All Game Data")]
        public static void ClearAllRecords()
        {
            bool isClear = EditorUtility.DisplayDialog("Clear All Data?",
                "All Personal data will be clear. Do you want it?", "Yes", "No");
            if (isClear)
            {
                ClearGameData();
                UnityEngine.Debug.Log("Done");
            }
        }

        [MenuItem("Maniac/Profile/Open Persistant folder")]
        public static void OpenPersistantFolder()
        {
            ProcessStartInfo StartInformation = new ProcessStartInfo();
            StartInformation.FileName = Application.persistentDataPath;

            Process.Start(StartInformation);
        }
#endif

        private static void ClearPersistantData()
        {
            //persitent folder
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public static void ClearGameData()
        {
            ClearPersistantData();
            PlayerPrefs.DeleteAll();
        }
    }
}