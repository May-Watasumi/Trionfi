using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
//using SimpleJSON;
using System.Linq;

namespace Trionfi
{
    public class TREditorMenu : Editor
    {
        [MenuItem("Trionfi/Initialize")]
        static void Initialize()
        {
            TRStageObject stage = CreateInstance<TRStageObject>();
            TRActorManagerObject actor = CreateInstance<TRActorManagerObject>();

            AssetDatabase.CreateAsset(stage, Trionfi.assetPath + TRStageObject.assetName);
            AssetDatabase.CreateAsset(actor, Trionfi.assetPath + TRActorManagerObject.assetName);
            AssetDatabase.ImportAsset(Trionfi.assetPath + TRStageObject.assetName);
            AssetDatabase.ImportAsset(Trionfi.assetPath + TRActorManagerObject.assetName);
        }
    }
}
#if false
[CustomEditor(typeof(ActorDataBase))]
public class ActorDataEditor : Editor
{
    public ActorDataEditor current { get { return target as ActorDataEditor; } }

#region LABELS

    const string DELETE = "Delete";
    const string CANCEL = "Cancel";

    public const string BASE_HP = "Level 1 HP";
    public const string BASE_ATKPWR = "Level 1 Attack";

    public const string RANK = "Rank";
    public const string CHARACTER = "Character";
    public const string SETTINGS = "Settings";

    public const string ATTACK_POWER = "Attack";
    public const string HP = "HP";

    public const string MAX_ATTACK_POWER = "Max Attack";
    public const string MAX_HP = "Max HP";

    public const string SYNTH_COUNT = "合成回数";
    public const string SYNTH_LEVEL_INCREMENT = "合成最大レベル増加";

    public const string POSITION = "Position";
    public const string CHARACTER_TYPE = "Type";

    public const string MAX_LEVEL = "Max Level";
    public const string RANK_MULTIPLIER = "Rank Multiplier";
    public const string LEVEL_BREAK_MULTIPLIER = "Level Break Multiplier";
    public const string EXP_MULTIPLIER = "EXP Multiplier";
    public const string BASE_REVIVE_TIME = "BaseReviveTime";
    public const string REVIVE_TIME_PER_LEVEL = "ReviveTimePerLevel";

#endregion

    string selectedToolbarTab = "Character";

    SerializedProperty pBundleVersionGlobal;

    SerializedProperty pMaxSynthCount;
    SerializedProperty pBonusLevelsPerSynth;

    SerializedProperty pRanks;
    SerializedProperty pCharacters;

    List<SerializedProperty> filteredUnits = null;

    //    RankSerializedPropertyAdaptor rankListAdaptor;

    int selectedUnitIndex = 0;
    bool confirmDelete = false;

    ReorderableList reorderList;


    SerializedProperty actorList;

    void OnEnable()
    {
        actorList = this.serializedObject.FindProperty("actorList");
        /*
                pBundleVersionGlobal = this.serializedObject.FindProperty("pBundleVersionGlobal");
                pMaxSynthCount = this.serializedObject.FindProperty("maxSynthCount");
                pBonusLevelsPerSynth = this.serializedObject.FindProperty("bonusLevelsPerSynth");

                pRanks = this.serializedObject.FindProperty("ranks");
                pCharacters = this.serializedObject.FindProperty("characters");
        */
        reorderList = new ReorderableList(this.serializedObject, actorList);

        //        rankListAdaptor = new RankSerializedPropertyAdaptor(pRanks);

        UpdateFilter();

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ToolbarGUI();

        switch (selectedToolbarTab)
        {
            case "Actor":
                //                ActorGUI();
                break;
            case "Sort":
                SortGUI();
                break;
            case "Settings":
                //               SettingsGUI();
                break;
        }

        serializedObject.ApplyModifiedProperties();

        //        CharacterImportExportGUI();
    }

    public void ToolbarGUI()
    {
        //EditorTools.Toolbar("Character Database");
        /*EditorGUILayout.BeginHorizontal();
		if (NGUIEditorTools.DrawPrefixButton("Atlas")) {
			ComponentSelector.Show<UIAtlas>(SelectAtlas);
		}
		pAtlas = NGUIEditorTools.DrawProperty("", serializedObject, "atlas", GUILayout.MinWidth(20f));
		EditorGUILayout.EndHorizontal();*/

        EditorHelper.BeginToolbar();

        if (EditorHelper.ToolbarButton("Actor", selectedToolbarTab))
        {
            selectedToolbarTab = "Actor";
        }

        if (EditorHelper.ToolbarButton("Sort", selectedToolbarTab))
        {
            selectedToolbarTab = "Sort";
        }

        if (EditorHelper.ToolbarButton("Settings", selectedToolbarTab))
        {
            selectedToolbarTab = "Settings";
        }

        GUI.backgroundColor = Color.white;
        EditorHelper.EndToolbar();
    }


#if false
    void ExportCharacterDatabase()
    {
        string path = EditorUtility.SaveFilePanel("Export Character Database", ".", "CharacterDatabase", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string jsonString = ActorDataEditor.instance.ToJSON().ToString();
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            System.IO.File.WriteAllText(path, jsonString);
        }
    }

    void ImportCharacterDatabase()
    {
        string path = EditorUtility.OpenFilePanel("Import Character Database", ".", "json");
        if (!string.IsNullOrEmpty(path))
        {
            JSONNode jsonNode = JSON.Parse(System.IO.File.ReadAllText(path));
            ActorDataEditor.instance.FromJSON(jsonNode);
            // serializedObject.Update(); TODO: How to mark DB as dirty?
        }
    }

    void UnitConfirmDeleteGUI()
    {
        var pUnit = filteredUnits[selectedUnitIndex];
        string unitName = pUnit.FindPropertyRelative("idString").stringValue;

        if (NGUIEditorTools.DrawHeader("Character Delete"))
        {
            NGUIEditorTools.BeginContents();
            EditorGUILayout.LabelField("Are you sure you want to delete " + unitName + "?");
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(CANCEL))
            {
                confirmDelete = false;
            }

            GUI.backgroundColor = Color.red;

            if (GUILayout.Button(DELETE))
            {
                confirmDelete = false;
                DeleteCharacter();
                UpdateFilter();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            NGUIEditorTools.EndContents();
        }
    }
#endif
    void UpdateFilter()
    {
        filteredUnits = new List<SerializedProperty>();

        for (int i = 0; i < actorList.arraySize; i++)
        {
            var pActor = actorList.GetArrayElementAtIndex(i);

            filteredUnits.Add(pActor);
        }

    }

    void UnitSelectorGUI()
    {
#if false
        if (!NGUIEditorTools.DrawHeader("キャラクター リスト"))
        {
            return;
        }

        NGUIEditorTools.BeginContents();
#endif
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("キャラクター追加"))
        {
            NewActor();
            return;
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();

        if (filteredUnits.Count > 0)
        {

            string[] characterList = new string[filteredUnits.Count];
            for (int i = 0; i < filteredUnits.Count; i++)
            {
                SerializedProperty pActor = filteredUnits[i];
                characterList[i] = i.ToString() + " - " + pActor.FindPropertyRelative("actorName").stringValue;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("キャラクター", GUILayout.Width(80f));
            selectedUnitIndex = EditorGUILayout.Popup(selectedUnitIndex, characterList);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("Character list is empty", MessageType.Info);
        }

        selectedUnitIndex = EditorHelper.DrawIndexSelector(selectedUnitIndex, filteredUnits.Count);

        //        NGUIEditorTools.EndContents();
    }

    void NewActor()
    {
        actorList.arraySize += 1;
        UpdateFilter();
    }

    void DeleteActor()
    {
        var pSelectedUnit = filteredUnits[selectedUnitIndex];

        int index = -1;
        for (int i = 0; i < actorList.arraySize; i++)
        {
            var pActor = actorList.GetArrayElementAtIndex(i);

            if (SerializedProperty.EqualContents(pActor, pSelectedUnit))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Debug.Log("Failed to find unit");
        }
        else
        {
            actorList.DeleteArrayElementAtIndex(index);
            filteredUnits = null;
        }
    }
#if false
    void CharacterImportExportGUI()
    {
        if (GUILayout.Button("Import Character Database"))
        {
            ImportCharacterDatabase();
        }

        if (GUILayout.Button("Export Character Database"))
        {
            ExportCharacterDatabase();
        }
    }
#endif

    void SortGUI()
    {
        reorderList.DoLayoutList();
    }
}

#if false
    void SettingsGUI()
    {
        UpdateBundleVersionGUI();

        //RemovePrefabsGUI();

        if (!NGUIEditorTools.DrawHeader("設定"))
        {
            return;
        }
        NGUIEditorTools.BeginContents();

        pMaxSynthCount.intValue = EditorGUILayout.IntField(SYNTH_COUNT, pMaxSynthCount.intValue);
        pBonusLevelsPerSynth.intValue = EditorGUILayout.IntField(SYNTH_LEVEL_INCREMENT, pBonusLevelsPerSynth.intValue);

        if (GUILayout.Button("Update Filter"))
        {
            UpdateFilter();
        }

        NGUIEditorTools.EndContents();

        TownHallLevels();
        TownHallCost();
        ExpGUI();
    }

    [MenuItem("Trionfi/Database/Character")]
    public static void CreateNewActorDatabase()
    {
        //Get Path of current selection
        Object obj = Selection.activeObject;

        string assetPath = "Assets";

        if (obj != null)
        {
            assetPath = AssetDatabase.GetAssetPath(obj);
        }

        //If we haven't selected a directory then get the directory of currently selected object
        if (!System.IO.Directory.Exists(assetPath))
        {
            assetPath = System.IO.Path.GetDirectoryName(assetPath);
        }

        string name = "ActorDataBase";
        int nameIdx = 0;

        string filepath = assetPath + "/" + name + ".asset";

        //Find a unique name
        if (System.IO.File.Exists(filepath))
        {

            while (System.IO.File.Exists(assetPath + "/" + name + nameIdx + ".asset"))
            {
                nameIdx++;
            }
            //Append the filename
            filepath = assetPath + "/" + name + nameIdx + ".asset";
        }

        ActorDataBase asset = ScriptableObject.CreateInstance<ActorDataBase>();
        AssetDatabase.CreateAsset(asset, filepath);
    }
#if false
    void UpdateBundleVersionGUI()
    {
        if (!NGUIEditorTools.DrawHeader("Update bundle version"))
        {
            return;
        }
        NGUIEditorTools.BeginContents();

        pBundleVersionGlobal.intValue = EditorGUILayout.IntField("Bundle Version Global", pBundleVersionGlobal.intValue);

        if (GUILayout.Button("Update"))
        {
            for (int i = 0; i < pCharacters.arraySize; i++)
            {
                var pCharacter = pCharacters.GetArrayElementAtIndex(i);

                if (pCharacter.FindPropertyRelative("bundleVersion").intValue < pBundleVersionGlobal.intValue)
                {
                    pCharacter.FindPropertyRelative("bundleVersion").intValue = pBundleVersionGlobal.intValue;
                }
            }
        }

        NGUIEditorTools.EndContents();
    }
    void RemovePrefabsGUI()
    {
        if (!NGUIEditorTools.DrawHeader("Clear Prefabs"))
        {
            return;
        }
        NGUIEditorTools.BeginContents();

        //pBundleVersionGlobal.intValue = EditorGUILayout.IntField("Bundle Version Global", pBundleVersionGlobal.intValue);

        if (GUILayout.Button("Clear"))
        {
            for (int i = 0; i < pCharacters.arraySize; i++)
            {
                var pCharacter = pCharacters.GetArrayElementAtIndex(i);
                pCharacter.FindPropertyRelative("prefab").objectReferenceValue = null;
            }
        }

        NGUIEditorTools.EndContents();
    }
#endif

    void ExpGUI()
    {
        if (NGUIEditorTools.DrawHeader("Experience Levels Rank N"))
        {
            NGUIEditorTools.BeginContents();
            var pExpLevelsN = this.serializedObject.FindProperty("expLevelsN");
            ExpGUIByRank(pExpLevelsN, 40);
            NGUIEditorTools.EndContents();
        }

        if (NGUIEditorTools.DrawHeader("Experience Levels Rank R"))
        {
            NGUIEditorTools.BeginContents();
            var pExpLevelsR = this.serializedObject.FindProperty("expLevelsR");
            ExpGUIByRank(pExpLevelsR, 50);
            NGUIEditorTools.EndContents();
        }

        if (NGUIEditorTools.DrawHeader("Experience Levels Rank SR"))
        {
            NGUIEditorTools.BeginContents();
            var pExpLevelsSR = this.serializedObject.FindProperty("expLevelsSR");
            ExpGUIByRank(pExpLevelsSR, 60);
            NGUIEditorTools.EndContents();
        }

        if (NGUIEditorTools.DrawHeader("Experience Levels Rank SSR"))
        {
            NGUIEditorTools.BeginContents();
            var pExpLevels = this.serializedObject.FindProperty("expLevels");
            ExpGUIByRank(pExpLevels, 70);
            NGUIEditorTools.EndContents();
        }
    }

    void ExpGUIByRank(SerializedProperty array, int size)
    {
        if (array.arraySize != size)
        {
            array.arraySize = size;
        }

        for (int i = 0; i < array.arraySize; i++)
        {

            var pExpLevel = array.GetArrayElementAtIndex(i);

            if ((i + 1) % 5 == 0)
            {
                GUI.backgroundColor = Color.yellow;
            }

            pExpLevel.intValue = EditorGUILayout.IntField((i + 1).ToString(), pExpLevel.intValue);

            GUI.backgroundColor = Color.white;
        }
    }

    void TownHallLevels()
    {
        if (!NGUIEditorTools.DrawHeader("Town Hall Level Limits"))
        {
            return;
        }
        NGUIEditorTools.BeginContents();

        var pTownHallLevelLimits = this.serializedObject.FindProperty("townHallLevelLimits");

        if (pTownHallLevelLimits.arraySize != 10)
        {
            pTownHallLevelLimits.arraySize = 10;
        }

        for (int i = 0; i < pTownHallLevelLimits.arraySize; i++)
        {

            var pTownHallLevelLimit = pTownHallLevelLimits.GetArrayElementAtIndex(i);

            pTownHallLevelLimit.intValue = EditorGUILayout.IntField((i + 1).ToString(), pTownHallLevelLimit.intValue);

        }

        NGUIEditorTools.EndContents();
    }

    void TownHallCost()
    {
        if (!NGUIEditorTools.DrawHeader("Town Hall Cost Limits"))
        {
            return;
        }
        NGUIEditorTools.BeginContents();

        var pTownHallCostLimits = this.serializedObject.FindProperty("townHallCostLimits");

        if (pTownHallCostLimits.arraySize != 10)
        {
            pTownHallCostLimits.arraySize = 10;
        }

        for (int i = 0; i < pTownHallCostLimits.arraySize; i++)
        {

            var pTownHallCostLimit = pTownHallCostLimits.GetArrayElementAtIndex(i);

            pTownHallCostLimit.intValue = EditorGUILayout.IntField((i + 1).ToString(), pTownHallCostLimit.intValue);

        }

        NGUIEditorTools.EndContents();
    }

    void RanksGUI()
    {
        ReorderableListGUI.Title("ランク");
        ReorderableListGUI.ListField(rankListAdaptor, ReorderableListFlags.ShowIndices);
    }

    void ActorGUI()
    {
        UnitSelectorGUI();

        if (filteredUnits == null || filteredUnits.Count == 0)
        {
            return;
        }

        var pCharacter = filteredUnits[selectedUnitIndex];

        var pIdString = pCharacter.FindPropertyRelative("idString");

        var pIndex = pCharacter.FindPropertyRelative("index");
        var pCost = pCharacter.FindPropertyRelative("cost");
        var pPremiumGachaWeight = pCharacter.FindPropertyRelative("premiumGachaWeight");
        var pDailyGachaWeight = pCharacter.FindPropertyRelative("dailyGachaWeight");

        var pRankId = pCharacter.FindPropertyRelative("rankId");

        var pBaseHP = pCharacter.FindPropertyRelative("baseHP");
        var pBaseAttackPower = pCharacter.FindPropertyRelative("baseAttackPower");

        var pCharacterType = pCharacter.FindPropertyRelative("type");
        var pPositionType = pCharacter.FindPropertyRelative("positionType");

        var pBundleVersion = pCharacter.FindPropertyRelative("bundleVersion");
        var pRange = pCharacter.FindPropertyRelative("range");
        var pRadius = pCharacter.FindPropertyRelative("damageRadius");
        var pMovementSpeed = pCharacter.FindPropertyRelative("movementSpeed");

        var rank = current.GetRank(pRankId.stringValue);

        var pBasicAttackId = pCharacter.FindPropertyRelative("basicAttackId");

        var pSkillCount = pCharacter.FindPropertyRelative("skillCount");
        var pSkillIds = pCharacter.FindPropertyRelative("skillIds");
        var pPassiveSkillCount = pCharacter.FindPropertyRelative("passiveSkillCount");
        var pPassiveSkillIds = pCharacter.FindPropertyRelative("passiveSkillIds");

        var pWeapon = pCharacter.FindPropertyRelative("_equipmentId");

        NGUIEditorTools.SetLabelWidth(100);

        if (NGUIEditorTools.DrawHeader("キャラクター"))
        {

            NGUIEditorTools.BeginContents();

            EditorGUILayout.PropertyField(pIdString);

            EditorGUILayout.PropertyField(pIndex);

            pCharacterType.enumValueIndex = (int)(CharacterType)EditorGUILayout.EnumPopup(CHARACTER_TYPE, (CharacterType)pCharacterType.enumValueIndex);

            SelectRankId(pRankId);

            pBaseAttackPower.floatValue = EditorGUILayout.FloatField(BASE_ATKPWR, pBaseAttackPower.floatValue);
            pBaseHP.floatValue = EditorGUILayout.FloatField(BASE_HP, pBaseHP.floatValue);

            pPositionType.enumValueIndex = (int)(PositionType)EditorGUILayout.EnumPopup(POSITION, (PositionType)pPositionType.enumValueIndex);

            pMovementSpeed.floatValue = EditorGUILayout.FloatField("Speed", pMovementSpeed.floatValue);
            pRange.floatValue = EditorGUILayout.FloatField("Range", pRange.floatValue);
            EditorGUILayout.PropertyField(pCost);
            EditorGUILayout.PropertyField(pDailyGachaWeight);
            EditorGUILayout.PropertyField(pPremiumGachaWeight);

            if (pRange.floatValue < 0.5f)
            {
                pRange.floatValue = 0.5f;
            }

            pRadius.floatValue = EditorGUILayout.FloatField("攻撃範囲", pRange.floatValue);

            if (pMovementSpeed.floatValue < 1f)
            {
                pMovementSpeed.floatValue = 1f;
            }

            EditorGUILayout.PropertyField(pBundleVersion);

            LJK.SkillDatabaseEditor.SkillSelector("Basic Attack", SkillCategory.BasicAttack, pBasicAttackId);

            EditorGUILayout.PropertyField(pSkillCount);
            pSkillCount.intValue = Mathf.Clamp(pSkillCount.intValue, 1, 3);

            if (pSkillIds.arraySize != pSkillCount.intValue)
            {
                pSkillIds.arraySize = pSkillCount.intValue;
            }
            for (int i = 0; i < pSkillIds.arraySize; i++)
            {
                var pSkillIdEntry = pSkillIds.GetArrayElementAtIndex(i);
                LJK.SkillDatabaseEditor.SkillSelector("Skill " + (i + 1), SkillCategory.Active, pSkillIdEntry, true);
            }

            EditorGUILayout.PropertyField(pPassiveSkillCount);
            pPassiveSkillCount.intValue = Mathf.Clamp(pPassiveSkillCount.intValue, 1, 3);

            if (pPassiveSkillIds.arraySize != pPassiveSkillCount.intValue)
            {
                pPassiveSkillIds.arraySize = pPassiveSkillCount.intValue;
            }
            for (int i = 0; i < pPassiveSkillIds.arraySize; i++)
            {
                var pPassiveSkillIdEntry = pPassiveSkillIds.GetArrayElementAtIndex(i);
                LJK.SkillDatabaseEditor.SkillSelector("Passive Skill " + (i + 1), SkillCategory.Passive, pPassiveSkillIdEntry, true);
            }

            EquipmentDefinition def = (EquipmentDefinition)EquipmentDatabase.Instance.GetEntryById(pWeapon.stringValue);
            List<string> ids = EquipmentDatabase.Instance.Select(d => d.Id, OrderDirection.Ascending).ToList();
            ids.Insert(0, "None");

            int index = -1;
            if (def != null)
            {
                index = ids.FindIndex(id => id == def.Id);// EquipmentDatabase.Instance.GetEntryIndex( def );
            }

            index = EditorGUILayout.Popup("Weapon", index, ids.ToArray());
            if (index > 0)
                pWeapon.stringValue = ids[index];
            else
                pWeapon.stringValue = string.Empty;

            NGUIEditorTools.EndContents();

        }

        if (NGUIEditorTools.DrawHeader("ステータス計算"))
        {
            NGUIEditorTools.BeginContents();

            if (rank == null)
            {

                EditorGUILayout.HelpBox("Select a rank for more details", MessageType.Info);

            }
            else
            {


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(75));
                EditorGUILayout.LabelField(HP, GUILayout.Width(75));
                EditorGUILayout.LabelField(ATTACK_POWER, GUILayout.Width(75));
                EditorGUILayout.EndHorizontal();

                for (int i = 1; i <= (rank.maxLevel + 20); i++)
                {

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("レベル " + i.ToString(), GUILayout.Width(75));

                    float maxLevelHP = CharacterDatabase.CalculateStatus(i, rank.maxLevel, pBaseHP.floatValue, rank.statusMultiplier, rank.levelBreakMultiplier);
                    float maxLevelAtk = CharacterDatabase.CalculateStatus(i, rank.maxLevel, pBaseAttackPower.floatValue, rank.statusMultiplier, rank.levelBreakMultiplier);

                    if (i > rank.maxLevel)
                    {
                        GUI.backgroundColor = Color.yellow;
                    }

                    EditorGUILayout.FloatField(Mathf.Ceil(maxLevelHP), GUILayout.Width(75));
                    EditorGUILayout.FloatField(Mathf.Ceil(maxLevelAtk), GUILayout.Width(75));

                    GUI.backgroundColor = Color.white;

                    EditorGUILayout.EndHorizontal();

                }

            }

            NGUIEditorTools.EndContents();
        }

        if (confirmDelete)
        {
            UnitConfirmDeleteGUI();
        }
        else
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(DELETE))
            {
                confirmDelete = true;
            }
            GUI.backgroundColor = Color.white;
        }
    }

    void SelectRankId(SerializedProperty pRankId)
    {

        string[] rankIdList = new string[pRanks.arraySize];
        int index = 0;

        for (int i = 0; i < pRanks.arraySize; i++)
        {

            rankIdList[i] = pRanks.GetArrayElementAtIndex(i).FindPropertyRelative("idString").stringValue;

            if (pRankId.stringValue.Equals(rankIdList[i]))
            {
                index = i;
            }

        }

        index = EditorGUILayout.Popup(RANK, index, rankIdList);

        pRankId.stringValue = rankIdList[index];

    }

public class RankSerializedPropertyAdaptor : Rotorz.ReorderableList.SerializedPropertyAdaptor
{

    public RankSerializedPropertyAdaptor(SerializedProperty arrayProperty) : base(arrayProperty, 0f)
    {
    }

    public override void DrawItem(Rect position, int index)
    {

        NGUIEditorTools.SetLabelWidth(100f);

        var pRank = this[index];

        var itemHieght = EditorGUI.GetPropertyHeight(this[index], GUIContent.none, false);

        position.height = itemHieght;

        //Id
        var pIdString = pRank.FindPropertyRelative("idString");
        EditorGUI.PropertyField(position, pIdString);

        //Name
        //position.y += itemHieght + 3;
        //var pRankName = pRank.FindPropertyRelative("name");
        //EditorGUI.PropertyField(position,pRankName);

        //Max Level
        position.y += itemHieght + 4;
        var pMaxLevel = pRank.FindPropertyRelative("maxLevel");
        pMaxLevel.intValue = EditorGUI.IntField(position, CharacterDatabaseEditor.MAX_LEVEL, pMaxLevel.intValue);
        //EditorGUI.PropertyField(position,pMaxLevel);

        //Exp Multiplier
        position.y += itemHieght + 4;
        var pExpMultiplier = pRank.FindPropertyRelative("experienceMultiplier");
        pExpMultiplier.floatValue = EditorGUI.FloatField(position, CharacterDatabaseEditor.EXP_MULTIPLIER, pExpMultiplier.floatValue);

        //Status Multiplier
        position.y += itemHieght + 4;
        var pStatusMultiplier = pRank.FindPropertyRelative("statusMultiplier");
        pStatusMultiplier.floatValue = EditorGUI.FloatField(position, CharacterDatabaseEditor.RANK_MULTIPLIER, pStatusMultiplier.floatValue);
        //EditorGUI.PropertyField(position,pStatusMultiplier);

        position.y += itemHieght + 4;
        var pLevelBreakMultiplier = pRank.FindPropertyRelative("levelBreakMultiplier");
        pLevelBreakMultiplier.floatValue = EditorGUI.FloatField(position, CharacterDatabaseEditor.LEVEL_BREAK_MULTIPLIER, pLevelBreakMultiplier.floatValue);

        position.y += itemHieght + 4;
        var pReviveTimePerLevel = pRank.FindPropertyRelative("reviveTimePerLevel");
        pReviveTimePerLevel.floatValue = EditorGUI.FloatField(position, CharacterDatabaseEditor.REVIVE_TIME_PER_LEVEL, pReviveTimePerLevel.floatValue);

        position.y += itemHieght + 4;
        var pBaseReviveTime = pRank.FindPropertyRelative("baseReviveTime");
        pBaseReviveTime.floatValue = EditorGUI.FloatField(position, CharacterDatabaseEditor.BASE_REVIVE_TIME, pBaseReviveTime.floatValue);

        position.y += itemHieght + 4;
        var pReviveCoeffByRarity = pRank.FindPropertyRelative("reviveCoeffByRarity");
        pReviveCoeffByRarity.floatValue = EditorGUI.FloatField(position, "reviveCoeffByRarity", pReviveCoeffByRarity.floatValue);

        position.y += itemHieght + 4;
        var pReviveCoeffPerLevel = pRank.FindPropertyRelative("reviveCoeffPerLevel");
        pReviveCoeffPerLevel.floatValue = EditorGUI.FloatField(position, "reviveCoeffPerLevel", pReviveCoeffPerLevel.floatValue);

        position.y += itemHieght + 5;
        var pStars = pRank.FindPropertyRelative("stars");
        pStars.intValue = EditorGUI.IntField(position, "Stars", pStars.intValue);

    }

    public override float GetItemHeight(int index)
    {
        int spacing = 4;
        return (EditorGUI.GetPropertyHeight(this[index], GUIContent.none, false) + spacing) * 13;
    }

}

#endif

#endif