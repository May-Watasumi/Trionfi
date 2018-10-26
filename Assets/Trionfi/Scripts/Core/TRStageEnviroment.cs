using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trionfi
{
    public class TRActorInfo
    {
        public int imageID = -1;
        public int emotionID = -1;
        public bool hasVoice = false;
        public string prefix;
    }

    public class TRStageEnviroment : MonoBehaviour
    {
        [System.Serializable]
        public class TREnviroment : SerializableDictionary<string, Color> { }
        [System.Serializable]
        public class TRKeyboardEvent : SerializableDictionary<KeyCode, TRKeyboardShortCut> { }
        [System.Serializable]
        public class TRActPattern : SerializableDictionary<string, string> { }

        [SerializeField]
        TREnviroment bgEnviroment = new TREnviroment()
        {
            { "昼" , new Color(1.0f, 1.0f, 1.0f) },
            { "夕" , new Color(1.0f, 0.375f, 0.0625f, 0.75f) },
            { "夜" , new Color(0.0f, 0.0f, 0.25f, 0.75f) },
        };

        [SerializeField]
        TREnviroment charaEnviroment = new TREnviroment()
        {
            { "昼" , new Color(1.0f, 1.0f, 1.0f) },
            { "夕" , new Color(1.0f, 0.0f, 0.0f, 1.0f) },
            { "夜" , new Color(0.25f, 0.0f, 0.5f, 1.0f) },
        };

        [SerializeField]
        TextAsset CharacterNameListCSV;

        [SerializeField]
        TextAsset CharacterEmotionPatternListCSV;

        public Dictionary<string, TRActorInfo >actorInfo = new Dictionary<string, TRActorInfo>();
        public TRActPattern actSuffix = new TRActPattern();

        public Dictionary<string, int> imageState = new Dictionary<string, int>();

        // Use this for initialization
        void Start()
        {
            if (CharacterNameListCSV != null)
            {
                System.Text.StringBuilder _text = new System.Text.StringBuilder(CharacterNameListCSV.text);
                CsvReadWrite.CsvReader csvReader = new CsvReadWrite.CsvReader(_text);
                List<string> _row = null;
                do
                {
                    _row = csvReader.ReadRow();

                    if (_row != null)
                    {
                        actorInfo[_row[0]] = new TRActorInfo();
                        actorInfo[_row[0]].prefix = _row[1];

                        int result = 0;

                        actorInfo[_row[0]].hasVoice = _row[2].ToLower() == "true" || (int.TryParse(_row[2], out result) && result != 0) ? true : false;
                    }
                    else
                        break;
                }
                while (true);
            }

            if (CharacterEmotionPatternListCSV != null)
            {
                System.Text.StringBuilder _text = new System.Text.StringBuilder(CharacterEmotionPatternListCSV.text);
                CsvReadWrite.CsvReader csvReader = new CsvReadWrite.CsvReader(_text);
                List<string> _row = null;
                do
                {
                    _row = csvReader.ReadRow();

                    if (_row != null)
                        actSuffix[_row[0]] = _row[1];
                    else
                        break;
                }
                while (true);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
