using UnityEngine;
using UnityEditor;

namespace Trionfi
{
    [System.Serializable]
    public class TREnviroment : SerializableDictionary<string, Color> { }

    [System.Serializable]
    public class TRKeyboardEvent : SerializableDictionary<KeyCode, TRKeyboardShortCut> { }

    public class TRStageObject : ScriptableObject
    {
        public static readonly string assetName = "Stage.asset";

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
        TRKeyboardEvent keyEvent = new TRKeyboardEvent()
        { };
    }
}

#if false
    public int pBundleVersionGlobal = 0;
    public int maxSynthCount = 4;
    public int bonusLevelsPerSynth = 5;

    public List<CharacterRank> ranks = new List<CharacterRank>();
    public List<CharacterDefinition> characters = new List<CharacterDefinition>();

    Dictionary<string, CharacterDefinition> _dictionary;

    public int[] townHallLevelLimits;
    public int[] townHallCostLimits;
    public int[] expLevelsN;
    public int[] expLevelsR;
    public int[] expLevelsSR;
    public int[] expLevels; // SSR

    public CharacterDefinition GetCharacter(string idString)
    {
        if (_dictionary == null)
        {
            BuildDict();
        }

        if (!_dictionary.ContainsKey(idString))
        {
            return null;
        }

        var character = _dictionary[idString];
        return character;

    }

    public CharacterRank GetRank(string idString)
    {
        foreach (var rank in ranks)
        {
            if (rank.idString.Equals(idString))
            {
                return rank;
            }
        }
        return null;
    }

    void BuildDict()
    {
        _dictionary = new Dictionary<string, CharacterDefinition>();
        foreach (var character in characters)
        {
            _dictionary[character.idString] = character;
        }
    }

#if false
    public void FromJSON(JSONNode node)
    {
        characters = new List<CharacterDefinition>();
        ranks = new List<CharacterRank>();

        JSONArray characterJsonArray = node["characters"].AsArray;
        JSONArray characterRankJsonArray = node["ranks"].AsArray;

        for (int i = 0; i < characterJsonArray.Count; i++)
        {
            JSONNode characterNode = characterJsonArray[i];
            characters.Add(new CharacterDefinition(characterNode));
        }

        for (int i = 0; i < characterRankJsonArray.Count; i++)
        {
            JSONNode characterRankNode = characterRankJsonArray[i];
            ranks.Add(new CharacterRank(characterRankNode));
        }
    }

    public void SpecialFromJSON(JSONNode node)
    {
        List<CharacterDefinition> tempCharacters = characters;
        List<CharacterRank> tempRanks = ranks;

        characters = new List<CharacterDefinition>();
        ranks = new List<CharacterRank>();

        JSONArray characterJsonArray = node["characters"].AsArray;
        JSONArray characterRankJsonArray = node["ranks"].AsArray;

        for (int i = 0; i < characterJsonArray.Count; i++)
        {
            JSONNode characterNode = characterJsonArray[i];
            characters.Add(new CharacterDefinition(characterNode));

            if (i >= tempCharacters.Count)
                continue;

            characters[i].prefab = tempCharacters[i].prefab;
            characters[i].damageRadius = tempCharacters[i].damageRadius;
            characters[i].basicAttackId = tempCharacters[i].basicAttackId;
            characters[i].skillCount = tempCharacters[i].skillCount;
            characters[i].skillIds = tempCharacters[i].skillIds;
            characters[i].passiveSkillCount = tempCharacters[i].passiveSkillCount;
            characters[i].passiveSkillIds = tempCharacters[i].passiveSkillIds;
        }

        for (int i = 0; i < characterRankJsonArray.Count; i++)
        {
            JSONNode characterRankNode = characterRankJsonArray[i];
            ranks.Add(new CharacterRank(characterRankNode));
            ranks[i].stars = tempRanks[i].stars;
        }
    }

    public JSONNode ToJSON()
    {
        JSONClass jsonClass = new JSONClass();
        JSONArray characterArray = new JSONArray();
        JSONArray characterRankArray = new JSONArray();

        for (int i = 0; i < characters.Count; i++)
            characterArray.Add(characters[i].ToJSON());

        for (int i = 0; i < ranks.Count; i++)
            characterRankArray.Add(ranks[i].ToJSON());

        jsonClass.Add("characters", characterArray);
        jsonClass.Add("ranks", characterRankArray);

        return jsonClass;
    }

[System.Serializable]
public class CharacterRank
{

    public string idString = "";
    public int maxLevel = 40;

    public float experienceMultiplier = 1f;
    public float statusMultiplier = 1f;
    public float levelBreakMultiplier = 0.025f;

    public float reviveTimePerLevel = 0; //Not used anymore
    public float baseReviveTime = 0;
    public float reviveCoeffByRarity = 0;
    public float reviveCoeffPerLevel = 0;

    public int stars = 1;

    public CharacterRank(JSONNode node)
    {
        FromJSON(node);
    }

    public JSONNode ToJSON()
    {
        var jsonClass = new JSONClass();

        jsonClass.Add("idString", new JSONData(idString));
        jsonClass.Add("maxLevel", new JSONData(maxLevel));
        jsonClass.Add("experienceMultiplier", new JSONData(experienceMultiplier));
        jsonClass.Add("statusMultiplier", new JSONData(statusMultiplier));
        jsonClass.Add("levelBreakMultiplier", new JSONData(levelBreakMultiplier));
        jsonClass.Add("reviveTimePerLevel", new JSONData(reviveTimePerLevel));
        jsonClass.Add("baseReviveTime", new JSONData(baseReviveTime));
        jsonClass.Add("reviveCoeffByRarity", new JSONData(reviveCoeffByRarity));
        jsonClass.Add("reviveCoeffPerLevel", new JSONData(reviveCoeffPerLevel));
        return jsonClass;
    }

    public void FromJSON(JSONNode node)
    {
        idString = node["idString"].Value;
        maxLevel = node["maxLevel"].AsInt;
        experienceMultiplier = node["experienceMultiplier"].AsInt;
        statusMultiplier = node["statusMultiplier"].AsFloat;
        levelBreakMultiplier = node["levelBreakMultiplier"].AsFloat;
        reviveTimePerLevel = node["reviveTimePerLevel"].AsFloat;
        baseReviveTime = node["baseReviveTime"].AsFloat;
        reviveCoeffByRarity = node["reviveCoeffByRarity"].AsFloat;
        reviveCoeffPerLevel = node["reviveCoeffPerLevel"].AsFloat;
    }

}

[System.Serializable]
public class CharacterDefinition
{

    public string idString;
    public string name
    {
        get
        {
            return Language.Get(idString, "HeroName");
        }
    }

    public string basicAttackId;

    public int skillCount = 1;
    public string[] skillIds;
    public int passiveSkillCount = 1;
    public string[] passiveSkillIds;

    public string EquipmentId { get { return _equipmentId; } private set { _equipmentId = value; } }
    [SerializeField]
    private string _equipmentId;

    public int index;
    public int cost = 5;

    public float dailyGachaWeight = 1f;
    public float premiumGachaWeight = 1f;

    public string auraEffect = "";

    public CharacterType type = CharacterType.Melee_Kinsetsu;
    public PositionType positionType = PositionType.Ground_Chijou;
    public string rankId = "";

    public float baseHP;
    public float baseAttackPower;

    public float movementSpeed;
    public float range;
    public float damageRadius = 0;

    public int bundleVersion = 0;

    public GameObject prefab;

    public int maxMaxLevel
    {
        get
        {
            return this.rank.maxLevel;// + (4 * CharacterDatabase.instance.bonusLevelsPerSynth);
        }
    }

    public SpecialAttackType specialAttackFlags
    {
        get
        {
            switch (this.type)
            {
                case CharacterType.Melee_Kinsetsu:
                    return SpecialAttackType.Wall;

                case CharacterType.Magic_Mahou:
                    return SpecialAttackType.Ground | SpecialAttackType.Hero;


                case CharacterType.AirMagic_KuchuMahou:
                    return SpecialAttackType.Ground | SpecialAttackType.Hero;

                case CharacterType.Healer_Kaifuku:
                    return 0;

                case CharacterType.Range_Yumi:
                    return SpecialAttackType.Air | SpecialAttackType.Hero;

                case CharacterType.Tank_Tate:
                    return SpecialAttackType.DefenseStructure;

                default:
                    return 0;

            }
        }
    }

    public string specialAttackDescription
    {
        get
        {
            if (this.specialAttackFlags != 0)
            {
                return this.specialAttackFlags.ToString();//(Language.Get(this.specialAttackType.ToString(), "UIStrings");
            }
            else
            {
                return "None";//Language.Get("None", "UIStrings");
            }
        }
    }

    public float SpecialAttackMultiplierForLevel(int level)
    {
        if (level < 10)
        {
            //1~10
            return 1.05f;
        }
        else if (level < 20)
        {
            //11~20
            return 1.10f;
        }
        else if (level < 30)
        {
            //21~30
            return 1.15f;
        }
        else if (level < 40)
        {
            //31~40
            return 1.20f;
        }
        else if (level < 50)
        {
            //41~50
            return 1.25f;
        }
        else if (level < 60)
        {
            //51~60
            return 1.30f;
        }
        else
        {
            //61~80
            return 1.40f;
        }
    }

    public float GetReviveTime(int level)
    {
        //return this.rank.baseReviveTime + (level - 1) * this.rank.reviveTimePerLevel;
        return this.rank.baseReviveTime * this.rank.reviveCoeffByRarity * (1 + this.rank.reviveCoeffPerLevel * level);
    }

    public int ExpRequiredForLevel(int level)
    {
        int index = level - 1;

        int[] resultArray = new int[1];
        switch (this.rank.idString)
        {
            case "N":
                resultArray = CharacterDatabase.instance.expLevelsN;
                break;
            case "R":
                resultArray = CharacterDatabase.instance.expLevelsR;
                break;
            case "SR":
                resultArray = CharacterDatabase.instance.expLevelsSR;
                break;
            case "SSR":
                resultArray = CharacterDatabase.instance.expLevels; // SSR
                break;
        }

        if (resultArray != null)
        {
            return (int)(resultArray[Mathf.Min(index, resultArray.Length - 1)]);// * this.rank.experienceMultiplier);
        }
        else
        {
            return 100;
        }
    }
    /*
	public void PlayVoice(int variation)
	{
		string rankString = rankId + "_" + index.ToString("000");
		string variationName = rankString + "_" + variation.ToString("000");
		MasterAudio.PlaySoundAndForget(rankString,1f,null,0,variationName);
	}

	public void StopVoice()
	{
		string rankString = rankId + "_" + index.ToString("000");
		MasterAudio.StopAllOfSound(rankString);
	}*/

    public CharacterRank rank
    {
        get
        {
            return CharacterDatabase.instance.GetRank(rankId);
        }
    }

    public float HpForLevel(int level)
    {

        int maxLevel = 40;
        float statusMultiplier = 0f;
        float levelBreakMultiplier = 0.025f;

        var rank = CharacterDatabase.instance.GetRank(this.rankId);
        if (rank != null)
        {
            maxLevel = rank.maxLevel;
            statusMultiplier = rank.statusMultiplier;
            levelBreakMultiplier = rank.levelBreakMultiplier;
        }

        return CharacterDatabase.CalculateStatus(level, maxLevel, baseHP, statusMultiplier, levelBreakMultiplier);

    }

    public float AttackPowerForLevel(int level)
    {

        int maxLevel = 40;
        float statusMultiplier = 0f;
        float levelBreakMultiplier = 0.025f;

        var rank = CharacterDatabase.instance.GetRank(this.rankId);
        if (rank != null)
        {
            maxLevel = rank.maxLevel;
            statusMultiplier = rank.statusMultiplier;
            levelBreakMultiplier = rank.levelBreakMultiplier;
        }

        return CharacterDatabase.CalculateStatus(level, maxLevel, baseAttackPower, statusMultiplier, levelBreakMultiplier);

    }

    public CharacterDefinition()
    {

    }

    public CharacterDefinition(JSONNode node)
    {
        FromJSON(node);
    }

    public void FromJSON(JSONNode node)
    {
        idString = node["idString"].Value;
        index = node["index"].AsInt;
        cost = node["cost"].AsInt;
        premiumGachaWeight = node["premiumGachaWeight"].AsFloat;
        dailyGachaWeight = node["dailyGachaWeight"].AsFloat;
        type = (CharacterType)System.Enum.Parse(typeof(CharacterType), node["characterType"]);
        positionType = (PositionType)System.Enum.Parse(typeof(PositionType), node["positionType"]);
        rankId = node["rankId"].Value;
        baseHP = node["baseHP"].AsFloat;
        baseAttackPower = node["baseAttackPower"].AsFloat;
        movementSpeed = node["movementSpeed"].AsFloat;
        range = node["range"].AsFloat;
        bundleVersion = node["bundleVersion"].AsInt;
        if (node["equipment"] != null)
            _equipmentId = node["equipment"].Value;
    }

    public JSONNode ToJSON()
    {
        var jsonClass = new JSONClass();

        jsonClass.Add("idString", new JSONData(idString));
        jsonClass.Add("index", new JSONData(index));
        jsonClass.Add("cost", new JSONData(cost));
        jsonClass.Add("dailyGachaWeight", new JSONData(dailyGachaWeight));
        jsonClass.Add("premiumGachaWeight", new JSONData(premiumGachaWeight));
        jsonClass.Add("characterType", new JSONData(type.ToString()));
        jsonClass.Add("positionType", new JSONData(positionType.ToString()));
        jsonClass.Add("rankId", new JSONData(rankId));
        jsonClass.Add("baseHP", new JSONData(baseHP));
        jsonClass.Add("baseAttackPower", new JSONData(baseAttackPower));
        jsonClass.Add("movementSpeed", new JSONData(movementSpeed));
        jsonClass.Add("range", new JSONData(range));
        jsonClass.Add("bundleVersion", new JSONData(bundleVersion));
        jsonClass.Add("equipment", new JSONData(_equipmentId));

        return jsonClass;
    }

}

[System.Serializable]
public class HeroInstance
{

    public HeroInstance()
    {

    }

    public HeroInstance(JSONNode node)
    {
        FromJSON(node);
    }

    public CharacterDefinition definition
    {
        get
        {
            var def = CharacterDatabase.instance.GetCharacter(this.characterId);
            if (def == null)
            {
                Debug.LogError("No definition for " + this.characterId);
            }
            return def;
        }
    }

    public float maxHealth
    {
        get
        {
            if (definition == null) // temporally for takenoko hero
            {
                Debug.LogError("Temp code is being used!!!");
                return 600;
            }
            else
            {
                return (int)this.definition.HpForLevel(this.level);
            }
        }
    }

    public int rarity
    {
        get
        {
            int rare = 0;
            switch (this.definition.rankId)
            {
                case "Normal":
                case "N":
                    rare = 1;
                    break;
                case "R":
                    rare = 2;
                    break;
                case "SR":
                    rare = 3;
                    break;
                case "SSR":
                    rare = 4;
                    break;
            }
            return rare;
        }
    }

    public float attackPower
    {
        get
        {
            return this.definition.AttackPowerForLevel(this.level);
        }
    }

    public int maxLevel
    {
        get
        {
            return this.definition.rank.maxLevel;// +(limitBreakLevel * 5);
        }
    }

    public float hpPercent
    {
        get
        {
            return health / maxHealth;
        }
    }

    public string id;
    public string characterId;

    public float health = 1;

    public int level = 1;
    public int experience = 0;
    public int synthCount = 0;
    public int activeSkillIndex = 0;

    public bool locked = false;

    public string lastSentTime;     // date time to string

    public bool isTakenoko = false;

    public int altarId = 0;

    public int limitBreakLevel = 0;

    public double timeTillRevive = 0;

    public bool IsPartyMember
    {
        get
        {
            return this.altarId > 0;
        }
    }

    public float specialMultiplier
    {
        get
        {
            if (level < 10)
            {
                //1~9
                return 1.05f;
            }
            else if (level < 20)
            {
                //10~19
                return 1.10f;
            }
            else if (level < 30)
            {
                //20~29
                return 1.15f;
            }
            else if (level < 40)
            {
                //30~39
                return 1.20f;
            }
            else if (level < 50)
            {
                //40~49
                return 1.25f;
            }
            else if (level < 60)
            {
                //50~59
                return 1.30f;
            }
            else
            {
                //60~80
                return 1.40f;
            }
        }
    }

    public bool[] skillUnlocked = new bool[3];

    public bool IsSkillUnlocked(int index)
    {
        return skillUnlocked[index];
    }

    public System.DateTime TimeOfLastRegen { get; set; }
    public System.DateTime TimeOfDeath { get; set; }

    public int[] skillLevel = { 1, 1, 1 };
    public int[] skillEXP = { 0, 0, 0 };

    public int GetSkillLevel(int skillIndex)
    {
        return skillLevel[skillIndex];
    }

    public int GetSkillEXP(int skillIndex)
    {
        return skillEXP[skillIndex];
    }

    public void FullHeal()
    {
        this.health = this.maxHealth;
        this.timeTillRevive = 0;
    }

    public void Heal(float hp)
    {
        this.health += hp;
        if (this.health > this.maxHealth)
        {
            this.health = this.maxHealth;
        }
    }

    public int expForLevelUp
    {
        get
        {
            return this.definition.ExpRequiredForLevel(this.level + 1);
        }
    }

    public float expPercent
    {
        get
        {
            return Mathf.Clamp01((float)experience / (float)expForLevelUp);
        }
    }

    public bool CanLevelUp
    {
        get
        {

            if (level < this.maxLevel)
            {
                if (experience >= this.expForLevelUp)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public void LevelUp(bool keepExperience = false)
    {

        if (!keepExperience)
            experience = 0;
        else
        {
            experience -= expForLevelUp;
        }
        level += 1;
    }

    public void FromJSON(JSONNode node)
    {
        id = node["_id"]["$oid"].Value;
        characterId = node["characterId"].Value;
        health = node["health"].AsFloat;
        level = node["level"].AsInt;
        experience = node["experience"].AsInt;
        synthCount = node["synthCount"].AsInt;
        lastSentTime = node["lastSentTime"].Value;
        altarId = node["altarId"].AsInt;

        if (node["activeSkillIndex"] != null)
        {
            activeSkillIndex = node["activeSkillIndex"].AsInt;
        }
        if (node["limitBreakLevel"] != null)
        {
            limitBreakLevel = node["limitBreakLevel"].AsInt;
        }
        if (node["timeTillRevive"] != null)
        {
            timeTillRevive = node["timeTillRevive"].AsDouble;
        }

        if (node["locked"] != null)
        {
            locked = node["locked"].AsBool;
        }

        JSONArray skillUnlockedJsonArray = node["skillUnlocked"].AsArray;
        skillUnlocked = new bool[3];
        for (int i = 0; i < skillUnlockedJsonArray.Count; i++)
        {
            skillUnlocked[i] = skillUnlockedJsonArray[i].AsBool;
        }

        JSONArray skillLevelArray = node["skillLevel"].AsArray;
        skillLevel = new int[3];
        skillEXP = new int[3];
        for (int i = 0; i < skillLevelArray.Count; i++)
        {
            skillLevel[i] = skillLevelArray[i]["level"].AsInt;
            skillEXP[i] = skillLevelArray[i]["exp"].AsInt;
        }

    }

    public JSONNode ToJSON()
    {
        var jsonClass = new JSONClass();

        jsonClass.Add("id", new JSONData(id));
        jsonClass.Add("characterId", new JSONData(characterId != null ? characterId : ""));
        jsonClass.Add("health", new JSONData(health));
        jsonClass.Add("level", new JSONData(level));
        jsonClass.Add("experience", new JSONData(experience));
        jsonClass.Add("synthCount", new JSONData(synthCount));
        jsonClass.Add("lastSentTime", new JSONData(lastSentTime != null ? lastSentTime : ""));
        jsonClass.Add("altarId", new JSONData(altarId));
        jsonClass.Add("timeTillRevive", new JSONData(timeTillRevive));
        jsonClass.Add("activeSkillIndex", new JSONData(activeSkillIndex));
        jsonClass.Add("limitBreakLevel", new JSONData(limitBreakLevel));

        JSONArray skillUnlockedJsonArray = new JSONArray();
        for (int i = 0; i < skillUnlocked.Length; i++)
        {
            skillUnlockedJsonArray.Add(new JSONData(skillUnlocked[i]));
        }
        jsonClass.Add("skillUnlocked", skillUnlockedJsonArray);

        JSONArray skillLevelArray = new JSONArray();
        for (int i = 0; i < skillLevel.Length; i++)
        {
            JSONClass node = new JSONClass();
            node.Add("level", new JSONData(skillLevel[i]));
            node.Add("exp", new JSONData(skillEXP[i]));
            skillLevelArray.Add(node);
        }
        jsonClass.Add("skillLevel", skillLevelArray);

        return jsonClass;
    }

}
#endif
#endif