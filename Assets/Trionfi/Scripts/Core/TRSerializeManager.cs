using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{

    [Serializable]
    public class TRSerializeData
    {
        [SerializeField]
        SerializableDictionary<int, TRVariableDictionary> layerParam = new SerializableDictionary<int, TRVariableDictionary>();
        [SerializeField]
        SerializableDictionary<int, TRVariableDictionary> audioParam = new SerializableDictionary<int, TRVariableDictionary>();
        [SerializeField]
        SerializableDictionary<string, TRVariableDictionary> scriptParam = new SerializableDictionary<string, TRVariableDictionary>();

        [SerializeField]
        public string layerJson;
        [SerializeField]
        public string audioJson;
        [SerializeField]
        public string scriptJson;

        [SerializeField]
        public FunctionalObjectInstance[] callStack;
        [SerializeField]
        public TRVariableDictionary variable;

        public string Serialize()
        {
            foreach (KeyValuePair<TRLayerID, TRLayer> instance in Trionfi.Instance.layerInstance)
            {
                layerParam[instance.Key] = instance.Value.tagParam;
            }

            foreach (KeyValuePair<TRAudioID, TRAudio> instance in Trionfi.Instance.audioInstance)
            {
                audioParam[instance.Key] = instance.Value.tagParam;
            }

            foreach (KeyValuePair<string, TRScript> instance in Trionfi.Instance.scriptInstance)
            {
                scriptParam[instance.Key] = instance.Value.tagParam;
            }

            callStack = TRVirtualMachine.Instance.callStack.ToArray();
            variable = TRVirtualMachine.Instance.globalVariableInstance;

            layerJson = JsonConvert.SerializeObject(layerParam);
            audioJson = JsonConvert.SerializeObject(audioParam);
            scriptJson = JsonConvert.SerializeObject(scriptParam);

            //            return JsonUtility.ToJson(this);
            return JsonConvert.SerializeObject(this);
        }

        public IEnumerator Deserialize()
        {
            layerParam = JsonConvert.DeserializeObject<SerializableDictionary<int, TRVariableDictionary>>(layerJson);
            audioParam = JsonConvert.DeserializeObject<SerializableDictionary<int, TRVariableDictionary>>(audioJson);
            scriptParam = JsonConvert.DeserializeObject<SerializableDictionary<string, TRVariableDictionary>>(scriptJson);

            TRVirtualMachine.Instance.callStack.Clear();

            //トップスタックは本体側で入る
            for (int count = 0; count < callStack.Length - 1; count++)
                TRVirtualMachine.Instance.callStack.Push(callStack[count]);

            foreach (KeyValuePair<int, TRVariableDictionary> instance in layerParam)
            {
                if (instance.Value.Count != 0)
                {
                    ImageComponent executer = new ImageComponent();
                    executer.tagParam = instance.Value;
                    yield return executer.TagSyncFunction();
                }

            }

            foreach (KeyValuePair<int, TRVariableDictionary> instance in audioParam)
            {
                if (instance.Value.Count != 0)
                {
                    AudioComponent executer = new AudioComponent();
                    executer.tagParam = instance.Value;
                    yield return executer.TagSyncFunction();
                }
            }

            foreach (KeyValuePair<string, TRVariableDictionary> instance in scriptParam)
            {
                if (instance.Value.Count != 0)
                    yield return Trionfi.Instance.LoadScript(instance.Key);
            }

            if (variable != null)
                TRVirtualMachine.Instance.globalVariableInstance = variable;
        }
    }

    [Serializable]
    public class TRSaveDataInfo
    {
        public string date;
        public string subject;
    }

    [Serializable]
    public class TRSerializeManager : SingletonMonoBehaviour<TRSerializeManager>
    {
        public enum Mode { Save, Load }

		const string SaveDataNameBase = "SaveData";
        const string fileName = "saveinfo.bin";
        //        string fullPath;

        public string subjectText = "SaveData";

        GZCrypter gzCrypter = new GZCrypter();
        TRCrypterBase crypter = null;

        int currentPage = 0;
        Mode currentMode = Mode.Load;

        [SerializeField]
        int dataCount = 10;
        [SerializeField]
        int pageCount = 1;
        [SerializeField]
        Text pageText;// = new Text[dataCount];
        [SerializeField]
        Text modeText;// = new Text[dataCount];
        [SerializeField]
        Text[] dateText;// = new Text[dataCount];
        [SerializeField]
        Text[] infoText;// = new Text[dataCount];
        [SerializeField]
        Button[] infoButton;// = new Text[dataCount];

        public void Begin(Mode mode)
        {
            modeText.text = mode == Mode.Load ? "Load" : "Save";
            currentMode = mode;
            UpdatePage();
        }
        public void End()
        {
            gameObject.SetActive(false);
            Trionfi.Instance.messageWindow.gameObject.SetActive(true);
            Trionfi.Instance.systemMenuWindow.gameObject.SetActive(true);
            Trionfi.Instance.messageWindow.Restart();
        }

        public void UpdatePage()
        {
            for (int a = 0; a < dataCount; a++)
            {
                int num = currentPage  * dataCount + a;

                TRSaveDataInfo info = GetSaveDataInfo(num);
                //                DateTime date = DateTime.TryParse(info.date);
                if (info != null)
                {
                    dateText[a].text = info.date;
                    infoText[a].text = info.subject;
                    infoButton[a].enabled = true;
                }
                else
                {
//                    dateText[a].text = info.date;
                    infoText[a].text = "No Data";

                    if(currentMode == Mode.Load)
                        infoButton[a].enabled = false;
                }
            }

            pageText.text = "Page " + (currentPage + 1).ToString() + "/" + pageCount.ToString();
        }

        [SerializeField]
        public SerializableDictionary<int, TRSaveDataInfo> dataDict = new SerializableDictionary<int, TRSaveDataInfo>();

        public TRSaveDataInfo GetSaveDataInfo(int num)
        {
           return dataDict.ContainsKey(num) ? dataDict[num] : null;
        }

        public void LoadInfo()
        {
            string path = Application.persistentDataPath + "/" + Trionfi.instance.titleName + "/" + fileName;

            if (!File.Exists(path))
                return;

            byte[] binData = File.ReadAllBytes(path);

            if (binData != null)
            {
                string jsonData = gzCrypter.Decrypt(binData);

                dataDict = new SerializableDictionary<int, TRSaveDataInfo>();
                dataDict = JsonConvert.DeserializeObject<SerializableDictionary<int, TRSaveDataInfo>>(jsonData);
            }
        }

        public void SaveInfo()
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + Trionfi.instance.titleName);

            string path = Application.persistentDataPath + "/" + Trionfi.instance.titleName + "/" + fileName;

            string jsonData = JsonConvert.SerializeObject(dataDict);

            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] binData = gzCrypter.Encrypt(jsonData);
                FileStream fs =  File.Create(path);
                fs.Write(binData,0,binData.Length);
                fs.Close();
            }
        }

        public void SaveData(int num)
        {
            if (crypter == null)
                crypter = new GZCrypter();

            TRSerializeData info = new TRSerializeData();
            string jsonData = info.Serialize();

            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] binData = crypter.Encrypt(jsonData);

                Directory.CreateDirectory(Application.persistentDataPath + "/" + Trionfi.instance.titleName);

                File.WriteAllBytes(Application.persistentDataPath + "/" + Trionfi.instance.titleName + "/" + SaveDataNameBase + num.ToString("D3") + ".bin", binData);
                //            PlayerPrefs.SetString(name, data);

                //Info
                DateTime date = DateTime.Now;
                TRSaveDataInfo header = new TRSaveDataInfo();
                header.date = date.ToString();
                header.subject = subjectText;
                dataDict[num] = header;

                UpdatePage();
            }
        }

        public void LoadData(int num)
        {
            TRVirtualMachine.instance.BeginLoad(num);
            gameObject.SetActive(false);
        }

        public TRSerializeData DeserializeFromFile(int num)
        {
            byte[] binData = File.ReadAllBytes(Application.persistentDataPath + "/" + Trionfi.instance.titleName + "/" + SaveDataNameBase + num.ToString("D3") + ".bin");

            if (crypter == null)
                crypter = new GZCrypter();

            string jsonData = crypter.Decrypt(binData);


            if (!string.IsNullOrEmpty(jsonData))
            {

                TRSerializeData info = new TRSerializeData();
                //string data =  PlayerPrefs.GetString(name);

                info = JsonConvert.DeserializeObject<TRSerializeData>(jsonData);
                //info = JsonUtility.FromJson<TRSerializeData>(jsonData);
                return info;
            }
            return null;
        }


        public void OnButton(int id)
        {
            int num = currentPage * dataCount + id;

            if (currentMode == Mode.Load)
            {
                LoadData(num);  
            }
            else
			{
                SaveData(num);
			}
        }

        public void PageDown()
        {
            if (currentPage <= 0)
                return;

            currentPage++;

            UpdatePage();
        }

        public void PageUp()
        {
            if (currentPage + 1 >= pageCount)
                return;

            currentPage--;

            UpdatePage();      
        }

        public void Start()
        {
            LoadInfo();
        }

        public void OnDestroy()
        {
            SaveInfo();
        }
    }
}
