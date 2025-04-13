using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

namespace Trionfi
{
    [Serializable]
    public abstract class TRSerializerWindowBase : SingletonMonoBehaviour<TRSerializerWindowBase>
    {
        public enum Mode { Save, Load }

        protected const string SaveDataNameBase = "SaveData";
        protected const string fileName = "saveinfo.bin";
        //        string fullPath;

        public string subjectText = "SaveData";

        protected GZCrypter gzCrypter = new GZCrypter();
        protected TRCrypterBase crypter = null;

        protected int currentPage = 0;
        protected Mode currentMode = Mode.Load;
        protected bool deletMode = false;

        [SerializeField]
        protected int dataCount = 10;
        [SerializeField]
        protected int pageCount = 1;

        [SerializeField]
        protected Button[] infoButton;
        
        public abstract string pageString { get; set; }
        public abstract string modeString { get; set; }
        protected abstract void SetDateText(int a, string text);
        protected abstract void SetInfoText(int a, string text);

        public void Begin(Mode mode)
        {
            modeString = mode == Mode.Load ? "Load" : "Save";
            currentMode = mode;
            UpdatePage();
        }
        public void End()
        {
            gameObject.SetActive(false);
            Trionfi.Instance.currentMessageWindow.gameObject.SetActive(true);
            Trionfi.Instance.systemMenuWindow.gameObject.SetActive(true);
            Trionfi.Instance.currentMessageWindow.Restart();
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
                    SetDateText(a, info.date);
                    SetInfoText(a, info.subject);
                    infoButton[a].enabled = true;
                }
                else
                {
                    SetInfoText(a, "No Data");
                    SetDateText(a, string.Empty);
                    
                    infoButton[a].enabled = currentMode == Mode.Save ? true : false;
                }
            }

            pageString = "Page " + (currentPage + 1).ToString() + "/" + pageCount.ToString();
        }

        [SerializeField]
        public SerializableDictionary<int, TRSaveDataInfo> dataDict = new SerializableDictionary<int, TRSaveDataInfo>();

        public TRSaveDataInfo GetSaveDataInfo(int num)
        {
           return dataDict.ContainsKey(num) ? dataDict[num] : null;
        }

        public void LoadInfo()
        {
            string path = Application.persistentDataPath + "/" + Trionfi.Instance.titleName + "/" + fileName;

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
            Directory.CreateDirectory(Application.persistentDataPath + "/" + Trionfi.Instance.titleName);

            string path = Application.persistentDataPath + "/" + Trionfi.Instance.titleName + "/" + fileName;

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

                Directory.CreateDirectory(Application.persistentDataPath + "/" + Trionfi.Instance.titleName);

                File.WriteAllBytes(Application.persistentDataPath + "/" + Trionfi.Instance.titleName + "/" + SaveDataNameBase + num.ToString("D3") + ".bin", binData);
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

        protected void DeleteData(int num)
        {
            string path = Application.persistentDataPath + "/" + Trionfi.Instance.titleName + "/" + SaveDataNameBase + num.ToString("D3") + ".bin";

			if (!File.Exists(path))
				return;

			File.Delete(path);

            UpdatePage();
		}

		public void LoadData(int num)
        {
            TRVirtualMachine.Instance.BeginLoad(num);
            gameObject.SetActive(false);
        }

        public TRSerializeData DeserializeFromFile(int num)
        {
            byte[] binData = File.ReadAllBytes(Application.persistentDataPath + "/" + Trionfi.Instance.titleName + "/" + SaveDataNameBase + num.ToString("D3") + ".bin");

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

            if (deletMode)
                DeleteData(num);
            else
            {
                if (currentMode == Mode.Load)
                {
                    LoadData(num);
                }
                else
                {
                    SaveData(num);
                }
            }
        }

        public void OnDeleteButton()
        {
            deletMode = !deletMode;
        
        }

        public void PageDown()
        {
            if (currentPage <= 0)
                return;

            currentPage--;

            UpdatePage();
        }

        public void PageUp()
        {
            if (currentPage + 1 >= pageCount)
                return;

            currentPage++;

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
