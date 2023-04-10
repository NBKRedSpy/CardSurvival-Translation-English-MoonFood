using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using LocalizationUtilities;

namespace 饥荒食物;

[BepInPlugin("Plugin.MoonFood", "MoonFood", "1.0.0")]
public class MoonFood : BaseUnityPlugin
{
	public static Dictionary<string, CardData> food_dict = new Dictionary<string, CardData>();

	public static Dictionary<string, CardTag> plantTagDict = new Dictionary<string, CardTag>();

	public static Dictionary<string, CardData> lianjin_dict = new Dictionary<string, CardData>();

	public static Dictionary<int, CardData> cont_dict = new Dictionary<int, CardData>();

	public static Dictionary<string, CardTag> foodTagDict = new Dictionary<string, CardTag>();

	private void Awake()
	{

		 LocalizationStringUtility.Init(
			 Config.Bind<bool>("Debug", "LogCardInfo", false, "If true, will output the localization keys for the cards. 如果为真，将输出卡片的本地化密钥。").Value,
			 Info.Location,
			 Logger
		);

		Harmony.CreateAndPatchAll(typeof(MoonFood));
		base.Logger.LogInfo("Plugin 晓月食物 is loaded!");
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F11))
		{
			GameManager.GiveCard(UniqueIDScriptable.GetFromID<CardData>("cd2791a90dfe4056b57574b9d961b15d"), _Complete: false);
			GameManager.GiveCard(UniqueIDScriptable.GetFromID<CardData>("0c9fc45aa9d941afbfd892a4057c937f"), _Complete: false);
		}
	}

	private static CardData utc(string uniqueID)
	{
		return UniqueIDScriptable.GetFromID<CardData>(uniqueID);
	}


    /// <summary>
    /// Dishes
    /// </summary>
    public static CardData 生成料理(string name, string guid, string cardName, string cardDescription, string cardNeed, string cardStat, float usage, float spoilTime)
	{
		CardData original = utc("5dea9d144f3e41a6850c9fa202279d38");
		CardData cardData = ScriptableObject.CreateInstance<CardData>();
		cardData = UnityEngine.Object.Instantiate(original);
		cardData.UniqueID = guid;
		cardData.name = name;
		cardData.Init();
		cardData.CardDescription.DefaultText = cardDescription;
		cardData.CardDescription.ParentObjectID = guid;
		cardData.CardDescription.SetLocalizationInfo();
		Texture2D texture2D = new Texture2D(200, 300);
		string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName + ".png";
		if (!File.Exists(path))
		{
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
		}
		texture2D.LoadImage(File.ReadAllBytes(path));
		Sprite cardImage = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
		cardData.CardImage = cardImage;
		cardData.CardName.DefaultText = cardName;
		cardData.CardName.ParentObjectID = guid;
		cardData.CardName.SetLocalizationInfo();
		string[] array = cardNeed.Split('|');
		Array.Sort(array);
		string key = string.Join("|", array);
		food_dict[key] = cardData;
		string[] array2 = cardStat.Split('，');
		int num = 0;
		string[] array3 = array2;
		foreach (string text in array3)
		{
			if (text == "")
			{
				cardData.DismantleActions[0].StatModifications[num].ValueModifier = new Vector2(0f, 0f);
			}
			else
			{
				cardData.DismantleActions[0].StatModifications[num].ValueModifier = new Vector2(float.Parse(text), float.Parse(text));
			}
			num++;
		}
		cardData.SpoilageTime.FloatValue = spoilTime;
		cardData.SpoilageTime.MaxValue = spoilTime;
		cardData.UsageDurability.FloatValue = usage;
		cardData.UsageDurability.MaxValue = usage;
		return cardData;
	}

	public static string 标签计算(InGameCardBase card)
	{
		string result = "任意";
		CardData cardModel = card.CardModel;
		if ((object)cardModel != null && cardModel.CardTags.Length != 0)
		{
			CardTag[] cardTags = cardModel.CardTags;
			foreach (CardTag cardTag in cardTags)
			{
				if (cardTag?.name == null)
				{
					continue;
				}
				string text = cardTag.name;
				string text2 = text;
				switch (text2)
				{
				case "tag_Fish":
					return "鱼";
				case "tag_Meats":
					return "肉";
				case "tag_BirdMeat":
					return "鸡";
				case "tag_Suger":
					return "糖";
				case "tag_Flour":
					return "面粉";
				case "tag_Fruit":
					return "水果";
				case "tag_Potato":
					return "土豆";
				case "tag_WaterAny":
					return "水";
				case "tag_Water":
					return "水";
				case "tag_Flower":
					return "花";
				case "tag_Milk":
					return "奶";
				case "tag_Egg":
					return "蛋";
				case "tag_Wine":
					return "酒";
				case "tag_Oil":
					return "油";
				case "tag_Tea":
					return "茶";
				case "tag_Seafood":
					return "海鲜";
				case "tag_Vegetable":
					return "蔬菜";
				case "tag_Noodles":
					return "面条";
				case "tag_Nut":
					return "坚果";
				case "tag_Coconut":
					return "椰子";
				}
				if (text2 == null || text2.IndexOf("Mushroom") <= 0)
				{
					continue;
				}
				return "蘑菇";
			}
		}
		return result;
	}

	public static string 混合计算(InGameCardBase card)
	{
		CardData cardModel = card.CardModel;
		string result = cardModel.CardName;
		if ((object)cardModel != null && cardModel.CardTags.Length != 0)
		{
			CardTag[] cardTags = cardModel.CardTags;
			foreach (CardTag cardTag in cardTags)
			{
				if (cardTag?.name != null)
				{
					switch (cardTag.name)
					{
					case "tag_Fish":
						return "鱼";
					case "tag_Meats":
						return "肉";
					case "tag_WaterAny":
						return "水";
					case "tag_Egg":
						return "蛋";
					case "tag_Oil":
						return "油";
					case "tag_Milk":
						return "奶";
					case "tag_Tea":
						return "茶";
					}
				}
			}
		}
		return result;
	}

	public static void 配方补充(string guid, string 配方)
	{
		string[] array = 配方.Split('|');
		Array.Sort(array);
		string key = string.Join("|", array);
		food_dict[key] = utc(guid);
	}

    /// <summary>
    /// Generate space
    /// </summary>
    /// <param name="数量">quantity</param>
    /// <returns></returns>
    public static string 生成空格(int 数量)
	{
		string text = "";
		for (int i = 0; i < 数量; i++)
		{
			text += "\u3000";
		}
		return text;
	}

    /// <summary>
    /// Add farm
    /// </summary>
    private static void 添加种田(string GivenGuid, string ReceiGuid, string ActionName, string ActionDescription, int duration, string ProduceGuid = "")
	{
		CardData cardData = utc(GivenGuid);
		CardData cardData2 = utc(ReceiGuid);
		if ((cardData == null) | (cardData2 == null))
		{
			return;
		}
		LocalizedString localizedString = default(LocalizedString);
		localizedString.DefaultText = ActionName;
		localizedString.ParentObjectID = "";
		localizedString.SetLocalizationInfo();
		LocalizedString localizedString2 = localizedString;

		localizedString = default(LocalizedString);
		localizedString.DefaultText = ActionDescription;
		localizedString.ParentObjectID = "";
		localizedString.SetLocalizationInfo();
		LocalizedString desc = localizedString;

		CardOnCardAction cardOnCardAction = new CardOnCardAction(localizedString2, desc, duration);
		Array.Resize(ref cardOnCardAction.CompatibleCards.TriggerCards, 1);
		cardOnCardAction.CompatibleCards.TriggerCards[0] = cardData;
		cardOnCardAction.GivenCardChanges.ModType = CardModifications.Destroy;
		if (ProduceGuid != "")
		{
			CardData cardData3 = utc(ProduceGuid);
			if (cardData3 != null)
			{
				CardsDropCollection cardsDropCollection = new CardsDropCollection();
				cardsDropCollection.CollectionName = "产出";
				cardsDropCollection.CollectionWeight = 1;
				CardDrop cardDrop = default(CardDrop);
				cardDrop.DroppedCard = cardData3;
				cardDrop.Quantity = new Vector2Int(1, 1);
				CardDrop[] value = new CardDrop[1] { cardDrop };
				Traverse.Create(cardsDropCollection).Field("DroppedCards").SetValue(value);
				cardOnCardAction.ProducedCards = new CardsDropCollection[1] { cardsDropCollection };
			}
		}
		if (cardData2.CardInteractions != null)
		{
			Array.Resize(ref cardData2.CardInteractions, cardData2.CardInteractions.Length + 1);
			cardData2.CardInteractions[cardData2.CardInteractions.Length - 1] = cardOnCardAction;
		}
	}

    /// <summary>
    /// Add cultivation soil interaction
    /// </summary>
    private static void 添加栽培土交互(string GivenGuid, string ReceiGuid, string ActionName, string ActionDescription, int duration, string ProduceGuid = "")
	{
		CardData cardData = utc(GivenGuid);
		CardData cardData2 = utc(ReceiGuid);
		if ((cardData == null) | (cardData2 == null))
		{
			return;
		}
		LocalizedString localizedString = default(LocalizedString);
		localizedString.DefaultText = ActionName;
		localizedString.ParentObjectID = "";
		localizedString.SetLocalizationInfo();
		LocalizedString localizedString2 = localizedString;

		localizedString = default(LocalizedString);
		localizedString.DefaultText = ActionDescription;
		localizedString.ParentObjectID = "";
		localizedString.SetLocalizationInfo();

		LocalizedString desc = localizedString;
		CardOnCardAction cardOnCardAction = new CardOnCardAction(localizedString2, desc, duration);
		Array.Resize(ref cardOnCardAction.CompatibleCards.TriggerCards, 1);
		cardOnCardAction.CompatibleCards.TriggerCards[0] = cardData;
		cardOnCardAction.GivenCardChanges.ModType = CardModifications.Destroy;
		cardOnCardAction.NotBaseAction = true;
		cardOnCardAction.UseMiniTicks = MiniTicksBehavior.CostsAMiniTick;
		cardOnCardAction.WorksBothWays = true;
		if (ProduceGuid != "")
		{
			CardData cardData3 = utc(ProduceGuid);
			if (cardData3 != null)
			{
				cardOnCardAction.ReceivingCardChanges.ModType = CardModifications.Transform;
				cardOnCardAction.ReceivingCardChanges.TransformInto = cardData3;
				cardOnCardAction.ReceivingCardChanges.TransferFuel = true;
			}
		}
		if (cardData2.CardInteractions != null)
		{
			Array.Resize(ref cardData2.CardInteractions, cardData2.CardInteractions.Length + 1);
			cardData2.CardInteractions[cardData2.CardInteractions.Length - 1] = cardOnCardAction;
		}
	}

    /// <summary>
    /// Generate herbal
    /// </summary>
    /// <returns></returns>
    public static CardData[] 生成草本植物(string name, string[] guid, string[] cardName, string[] cardDescription, string whereToFindGuid, int CollectionWeight, int getNum, int plantNum, bool caneat, string eatEffect, float spoilTime, int proDays)
	{
		CardData[] array = new CardData[3];
		CardData cardData = ScriptableObject.CreateInstance<CardData>();
		cardData = UnityEngine.Object.Instantiate(utc("860762b307d74caf84d314790448f9f6"));
		cardData.UniqueID = guid[0];
		cardData.name = name;
		cardData.Init();
		cardData.CardDescription.DefaultText = cardDescription[0];
		cardData.CardDescription.SetLocalizationInfo();

        cardData.CardDescription.ParentObjectID = cardData.UniqueID;
		Texture2D texture2D = new Texture2D(200, 300);
		string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName[0] + ".png";
		if (!File.Exists(path))
		{
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
		}
		texture2D.LoadImage(File.ReadAllBytes(path));
		Sprite cardImage = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
		cardData.CardImage = cardImage;
		cardData.CardName.DefaultText = cardName[0];
        cardData.CardName.SetLocalizationInfo();

        cardData.CardName.ParentObjectID = cardData.UniqueID;
		if (!caneat)
		{
			cardData.DismantleActions.Clear();
		}
		else if (!(eatEffect == "蔬菜")) //vegetable
        {
			if (eatEffect == "水果") //fruit
            {
				float[] array2 = new float[6] { 20f, 7.3333335f, 5f, 3f, 43.333332f, -10f };
				for (int i = 0; i < 6; i++)
				{
					cardData.DismantleActions[0].StatModifications[i].ValueModifier = new Vector2(array2[i], array2[i]);
				}
				cardData.DismantleActions[0].StatModifications[4].Stat = UniqueIDScriptable.GetFromID<GameStat>("ca65984e668e95344a7009df3dcfb052");
			}
			else
			{
				for (int j = 0; j < 6; j++)
				{
					cardData.DismantleActions[0].StatModifications[j].ValueModifier = Vector2.zero;
				}
			}
		}
		else
		{
			float[] array3 = new float[6] { 13.125f, 2.625f, 2.5f, -2f, 32.5f, -1.25f };
			for (int k = 0; k < 6; k++)
			{
				cardData.DismantleActions[0].StatModifications[k].ValueModifier = new Vector2(array3[k], array3[k]);
			}
		}
		cardData.SpoilageTime.FloatValue = spoilTime;
		cardData.SpoilageTime.MaxValue = spoilTime;
		array[0] = cardData;
		CardData cardData2 = ScriptableObject.CreateInstance<CardData>();
		cardData2 = UnityEngine.Object.Instantiate(utc("4b17f870e9ca41eb95bf7d08aa387528"));
		cardData2.UniqueID = guid[1];
		cardData2.name = name + "丛";
		cardData2.Init();
		cardData2.CardDescription.DefaultText = cardDescription[1];
		cardData2.CardDescription.SetLocalizationInfo();

        cardData2.CardDescription.ParentObjectID = cardData2.UniqueID;
		Texture2D texture2D2 = new Texture2D(200, 300);
		path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName[1] + ".png";
		if (!File.Exists(path))
		{
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
		}
		texture2D2.LoadImage(File.ReadAllBytes(path));
		cardImage = Sprite.Create(texture2D2, new Rect(0f, 0f, texture2D2.width, texture2D2.height), Vector2.zero);
		cardData2.CardImage = cardImage;
		cardData2.CardName.DefaultText = cardName[1];
		cardData2.CardName.SetLocalizationInfo();	
		cardData2.CardName.ParentObjectID = cardData2.UniqueID;
		CardDrop cardDrop = default(CardDrop);
		cardDrop.DroppedCard = cardData;
		cardDrop.Quantity = new Vector2Int(getNum, getNum);
		CardDrop[] value = new CardDrop[1] { cardDrop };
		Traverse.Create(cardData2.DismantleActions[0].ProducedCards[0]).Field("DroppedCards").SetValue(value);
		array[1] = cardData2;
		CardData cardData3 = ScriptableObject.CreateInstance<CardData>();
		cardData3 = UnityEngine.Object.Instantiate(utc("192eb567170e4fa491c18ea3e5f5ec03"));
		cardData3.UniqueID = guid[2];
		cardData3.name = name + "田";
		cardData3.Init();
		cardData3.CardName.DefaultText = cardName[2];
		cardData3.CardName.SetLocalizationInfo();	
		cardData3.CardName.ParentObjectID = cardData3.UniqueID;
		for (int l = 0; l <= 2; l++)
		{
			cardDrop.DroppedCard = cardData2;
			cardDrop.Quantity = new Vector2Int(plantNum * (l + 1), plantNum * (l + 1));
			CardDrop[] array4 = new CardDrop[1] { cardDrop };
			Traverse.Create(cardData3.Progress.OnFull.ProducedCards[l]).Field("DroppedCards").SetValue(value);
		}
		cardData3.Progress.MaxValue = proDays * 96;
		array[2] = cardData3;
		CardData cardData4 = utc(whereToFindGuid);
		if ((bool)cardData4)
		{
			CardsDropCollection cardsDropCollection = new CardsDropCollection();
			cardsDropCollection.CollectionName = "植株";
			cardsDropCollection.CollectionWeight = CollectionWeight;
			CardDrop cardDrop2 = default(CardDrop);
			cardDrop2.DroppedCard = cardData2;
			cardDrop2.Quantity = new Vector2Int(1, 1);
			CardDrop[] value2 = new CardDrop[1] { cardDrop2 };
			Traverse.Create(cardsDropCollection).Field("DroppedCards").SetValue(value2);
			Array.Resize(ref cardData4.DismantleActions[0].ProducedCards, cardData4.DismantleActions[0].ProducedCards.Length + 1);
			cardData4.DismantleActions[0].ProducedCards[cardData4.DismantleActions[0].ProducedCards.Length - 1] = cardsDropCollection;
		}
		添加种田(cardData.UniqueID, "0308e91cdd2d6aa44a9ed0f8187f88d3", "种植" + cardData.CardName, string.Concat("把", cardData.CardName, "种植在田中"), 2, cardData3.UniqueID);
		return array;
	}

    /// <summary>
    /// Add to tag
    /// </summary>
    /// <param name="card"></param>
    /// <param name="tagname"></param>
    public static void 添加Tag(CardData card, string tagname)
	{
		Array.Resize(ref card.CardTags, card.CardTags.Length + 1);
		card.CardTags[card.CardTags.Length - 1] = plantTagDict[tagname];
	}

    /// <summary>
    /// Link and intensive cultivation
    /// </summary>
	/// Card Data plant, string internal name of plant after linkage, string linkage plant Guid, string water requirement, 
	/// string environmental requirement 1, string environmental requirement 2, string mites, string fungus, string 
	/// lifespan, string product progress, string growth length, string fertility, string soil porosity, string pesticide, 
	/// string whether it is small
    public static void 联动精耕细作(CardData 植株, string 联动后植株内部名, string 联动植株Guid, string 水分需求, string 环境需求1, string 环境需求2, string 螨虫, string 真菌, string 寿命, string 产物进度, string 生长度, string 肥力, string 土壤疏松度, string 农药, string 是否小体型)
	{
		CardData cardData = ScriptableObject.CreateInstance<CardData>();
		cardData = UnityEngine.Object.Instantiate(utc("cb27f80532de40c5a164202ed138090c"));
		cardData.UniqueID = 联动植株Guid;
		cardData.name = 联动后植株内部名;
		cardData.Init();
		cardData.CardName.DefaultText = string.Concat(植株.CardName, "植株");
		cardData.CardName.SetLocalizationInfo();
		cardData.CardName.ParentObjectID = cardData.UniqueID;
		cardData.CardDescription.DefaultText = "我应该把它放入泥土中，等待它成熟。\n植株特性：";
		cardData.CardDescription.SetLocalizationInfo();
		cardData.CardDescription.ParentObjectID = cardData.UniqueID;
		添加栽培土交互(植株.UniqueID, "5b01eb40bb8245a091086c584538238d", "制作植株", "将植株放入栽培土中", 0, 联动植株Guid);
		CardDrop cardDrop = default(CardDrop);
		cardDrop.DroppedCard = 植株;
		cardDrop.Quantity = new Vector2Int(2, 2);
		CardDrop[] value = new CardDrop[1] { cardDrop };
		Traverse.Create(cardData.SpecialDurability1.OnFull.ProducedCards[0]).Field("DroppedCards").SetValue(value);
		if (!(水分需求 == "喜水"))
		{
			if (水分需求 == "喜旱")
			{
				添加Tag(cardData, "tag_Plant_DirtFavor");

				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 水分需求;
				cardData.CardDescription.SetLocalizationInfo();

            }
		}
		else
		{
			添加Tag(cardData, "tag_Plant_WaterFavor");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 水分需求;
			cardData.CardDescription.SetLocalizationInfo();	
		}
		if (!(环境需求1 == "喜潮"))
		{
			if (环境需求1 == "喜干")
			{
				添加Tag(cardData, "tag_Plant_DryFavor");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 环境需求1;
				cardData.CardDescription.SetLocalizationInfo();	
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_DampFavor");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 环境需求1;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (!(环境需求2 == "喜光"))
		{
			if (环境需求2 == "喜暗")
			{
				添加Tag(cardData, "tag_Plant_DarkFavor");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 环境需求2;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_LightFavor");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 环境需求2;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (!(螨虫 == "耐螨虫"))
		{
			if (螨虫 == "怕螨虫")
			{
				添加Tag(cardData, "tag_Plant_MiteFear");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 螨虫;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_MiteProof");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 螨虫;
            cardData.CardDescription.SetLocalizationInfo();

        }
		if (!(真菌 == "耐真菌"))
		{
			if (真菌 == "怕真菌")
			{
				添加Tag(cardData, "tag_Plant_FungiFear");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 真菌;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_FungiProof");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 真菌;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (!(寿命 == "长寿"))
		{
			if (寿命 == "短寿")
			{
				添加Tag(cardData, "tag_Plant_ShortLive");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 寿命;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_LongLive");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 寿命;
			cardData.CardDescription.SetLocalizationInfo();
			
		}
		if (!(产物进度 == "高产"))
		{
			if (产物进度 == "低产")
			{
				添加Tag(cardData, "tag_Plant_LowProduce");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 产物进度;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_HighProduce");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 产物进度;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (!(生长度 == "速生"))
		{
			if (生长度 == "慢生")
			{
				添加Tag(cardData, "tag_Plant_GrowSlow");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 生长度;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_GrowFast");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 生长度;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (!(肥力 == "肥田"))
		{
			if (肥力 == "耗肥")
			{
				添加Tag(cardData, "tag_Plant_ConsumeFertilize");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 肥力;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_Fertilize");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 肥力;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (!(土壤疏松度 == "松土"))
		{
			if (土壤疏松度 == "板结")
			{
				添加Tag(cardData, "tag_Plant_AirLess");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 土壤疏松度;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_AirMore");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 土壤疏松度;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (!(农药 == "杀螨虫"))
		{
			if (农药 == "杀真菌")
			{
				添加Tag(cardData, "tag_Plant_AntiFungi");
				cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 农药;
				cardData.CardDescription.SetLocalizationInfo();
			}
		}
		else
		{
			添加Tag(cardData, "tag_Plant_AntiMite");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + 农药;
			cardData.CardDescription.SetLocalizationInfo();
		}
		if (是否小体型 == "是")
		{
			添加Tag(cardData, "tag_Plant_Small");
			cardData.CardDescription.DefaultText = cardData.CardDescription.DefaultText + "小体型";
			cardData.CardDescription.SetLocalizationInfo();
		}

		GameLoad.Instance.DataBase.AllData.Add(cardData);
	}

	public static void 生成炼金(string name, string guid, string cardName, string cardDescription, string cardNeed, float 产出数量 = 1f)
	{
		if (utc(guid) == null)
		{
			CardData cardData = ScriptableObject.CreateInstance<CardData>();
			cardData = UnityEngine.Object.Instantiate(utc("175ef2773fd84a3c816e93c4307da58f"));
			cardData.UniqueID = guid;
			cardData.name = name;
			cardData.Init();
			cardData.CardDescription.DefaultText = cardDescription;
			cardData.CardDescription.ParentObjectID = guid;
			cardData.CardDescription.SetLocalizationInfo();

			Texture2D texture2D = new Texture2D(200, 300);
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName + ".png";
			if (!File.Exists(path))
			{
				path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
			}
			texture2D.LoadImage(File.ReadAllBytes(path));
			Sprite cardImage = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
			cardData.CardImage = cardImage;
			cardData.CardName.DefaultText = cardName;
            cardData.CardName.SetLocalizationInfo();
            cardData.CardName.ParentObjectID = guid;
			
			string[] array = cardNeed.Split('|');
			Array.Sort(array);
			string key = string.Join("|", array);
			cardData.SpecialDurability4.MaxValue = 产出数量;
			lianjin_dict[key] = cardData;
		}
		else
		{
			string[] array2 = cardNeed.Split('|');
			Array.Sort(array2);
			string key2 = string.Join("|", array2);
			CardData cardData2 = utc(guid);
			cardData2.SpecialDurability4.MaxValue = 产出数量;
			lianjin_dict[key2] = cardData2;
		}
	}

    /// <summary>
    /// Verification alchemy
    /// </summary>
    public static void 验证炼金(CardAction action, InGameCardBase card)
	{
		if ((object)card == null || card.CardsInInventory.Count <= 0)
		{
			return;
		}
		int num = 0;
		string[] array = new string[4];
		foreach (InventorySlot item in card.CardsInInventory)
		{
			if (item.AllCards.Count <= 0)
			{
				continue;
			}
			foreach (InGameCardBase allCard in item.AllCards)
			{
				if (allCard != null)
				{
					if (allCard.ContainedLiquidModel != null)
					{
						array[num] = allCard.ContainedLiquidModel.CardName;
					}
					else
					{
						array[num] = allCard.CardModel.CardName;
					}
					num++;
				}
			}
		}
		Array.Sort(array);
		string text = string.Join("|", array);
		Debug.Log("名称判断：" + text);
		lianjin_dict.TryGetValue(text, out var value);
		if ((bool)value)
		{
			List<int> list = new List<int>();
			int num2 = 0;
			foreach (InventorySlot item2 in card.CardsInInventory)
			{
				if (item2.AllCards.Count > 0)
				{
					foreach (InGameCardBase allCard2 in item2.AllCards)
					{
						if (allCard2 != null && allCard2.ContainedLiquidModel != null)
						{
							allCard2.ContainedLiquid.CurrentLiquidQuantity -= 300f;
							list.Add(num2);
						}
					}
				}
				num2++;
			}
			MBSingleton<GameManager>.Instance.ClearCardInventory(card, _RemoveAll: true, list);
			CardDrop cardDrop = default(CardDrop);
			cardDrop.DroppedCard = value;
			cardDrop.Quantity = new Vector2Int((int)value.SpecialDurability4.MaxValue, (int)value.SpecialDurability4.MaxValue);
			CardDrop[] value2 = new CardDrop[1] { cardDrop };
			Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(value2);
		}
		else
		{
			CardDrop cardDrop2 = default(CardDrop);
			cardDrop2.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
			cardDrop2.Quantity = new Vector2Int(1, 1);
			CardDrop[] value3 = new CardDrop[1] { cardDrop2 };
			Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(value3);
		}
	}

    /// <summary>
    /// Container merger
    /// </summary>
    /// <param name="action"></param>
    /// <param name="card"></param>
    public static void 容器合并(CardAction action, InGameCardBase card)
	{
		if ((object)card == null || card.CardsInInventory.Count <= 0)
		{
			return;
		}
		int num = 0;
		string[] array = new string[4];
		float num2 = 0f;
		foreach (InventorySlot item in card.CardsInInventory)
		{
			if (item.AllCards.Count <= 0)
			{
				continue;
			}
			foreach (InGameCardBase allCard in item.AllCards)
			{
				if (allCard != null)
				{
					num2 += allCard.CardModel.MaxLiquidCapacity;
					num++;
				}
			}
		}
		int num3 = (int)Math.Floor(num2 / 300f);
		if (num3 > 0)
		{
			if (num3 > 20)
			{
				num3 = 20;
			}
			List<int> exceptionSlots = new List<int>();
			MBSingleton<GameManager>.Instance.ClearCardInventory(card, _RemoveAll: true, exceptionSlots);
			CardDrop cardDrop = default(CardDrop);
			cardDrop.DroppedCard = cont_dict[num3];
			cardDrop.Quantity = new Vector2Int(1, 1);
			CardDrop[] value = new CardDrop[1] { cardDrop };
			Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(value);
		}
		else
		{
			CardDrop cardDrop2 = default(CardDrop);
			cardDrop2.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
			cardDrop2.Quantity = new Vector2Int(1, 1);
			CardDrop[] value2 = new CardDrop[1] { cardDrop2 };
			Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(value2);
		}
	}

	public static CardData 生成容器(string name, string guid, string cardName, string cardDescription, int contain)
	{
		CardData original = utc("247cb2c7dc0f4bc0814e91d794900c40");
		CardData cardData = ScriptableObject.CreateInstance<CardData>();
		cardData = UnityEngine.Object.Instantiate(original);
		cardData.UniqueID = guid;
		cardData.name = name;
		cardData.Init();
		cardData.CardDescription.DefaultText = cardDescription;
		cardData.CardDescription.ParentObjectID = guid;
		cardData.CardDescription.SetLocalizationInfo();	
		cardData.CardName.DefaultText = cardName;
		cardData.CardName.ParentObjectID = guid;
		cardData.CardName.SetLocalizationInfo();
		cardData.MaxLiquidCapacity = 300 * contain;
		return cardData;
	}

	public static void 添加Tag2(CardData card, string tagname)
	{
		Array.Resize(ref card.CardTags, card.CardTags.Length + 1);
		if (foodTagDict.ContainsKey(tagname))
		{
			card.CardTags[card.CardTags.Length - 1] = foodTagDict[tagname];
		}
		else
		{
			Debug.Log(tagname + "不存在");
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
	public static void SomePatch()
	{
		List<string> list = new List<string>();
		string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MoonFood-EX.txt";
		string text = "";
		if (File.Exists(path))
		{
			StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
			text = streamReader.ReadLine();
			do
			{
				if (text.IndexOf("#") > 0)
				{
					list.Add(text);
				}
				text = streamReader.ReadLine();
			}
			while (!streamReader.EndOfStream);
			streamReader.Close();
		}
		else
		{
			Debug.Log("没路径！");
		}
		foreach (string item2 in list)
		{
			string[] array = item2.Split('#');
			if (array.Length >= 8)
			{
				CardData item = 生成料理(array[0], array[1], array[2], array[3], array[4], array[5], float.Parse(array[6]), float.Parse(array[7]));
				GameLoad.Instance.DataBase.AllData.Add(item);
			}
		}
		配方补充("20eb4938d55244f1bdec102dd1391038", "肉|肉|任意|任意");
		string text2 = "";
		int num = 0;
		foreach (KeyValuePair<string, CardData> item3 in food_dict)
		{
			string key = item3.Key;
			CardData value = item3.Value;
			string text3 = string.Concat(value.CardName, "=", key);
			if (text3.Length < 18)
			{
				text3 += 生成空格(18 - text3.Length);
			}
			text2 += text3;
			num++;
			if (num == 2)
			{
				text2 += "\n";
				num = 0;
			}
		}
		GuideEntry[] array2 = UnityEngine.Object.FindObjectsOfType<GuideEntry>();
		GuideEntry[] array3 = array2;
		foreach (GuideEntry guideEntry in array3)
		{
			if (guideEntry.name.IndexOf("高级炉灶") >= 0)
			{
				guideEntry.OverrideDescription.DefaultText = guideEntry.OverrideDescription.DefaultText + "\n" + text2;
			}
		}
		CardData cardData = utc("cb27f80532de40c5a164202ed138090c");
		if ((bool)cardData)
		{
			CardTag[] array4 = UnityEngine.Object.FindObjectsOfType<CardTag>();
			CardTag[] array5 = array4;
			foreach (CardTag cardTag in array5)
			{
				if (cardTag.name.StartsWith("tag_Plant_"))
				{
					plantTagDict[cardTag.name] = cardTag;
				}
			}
		}
		path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MoonPlant-EX.txt";
		text = "";
		if (File.Exists(path))
		{
			StreamReader streamReader2 = new StreamReader(path, Encoding.UTF8);
			text = streamReader2.ReadLine();
			do
			{
				if (text.IndexOf("#") > 0)
				{
					string[] array6 = text.Split('#');
					if (array6.Length >= 17)
					{
						string[] guid = new string[3]
						{
							array6[1],
							array6[2],
							array6[3]
						};
						string[] cardName = new string[3]
						{
							array6[4],
							array6[5],
							array6[6]
						};
						string[] cardDescription = new string[2]
						{
							array6[7],
							array6[8]
						};
						CardData[] array7 = 生成草本植物(array6[0], guid, cardName, cardDescription, array6[9], int.Parse(array6[10]), int.Parse(array6[11]), int.Parse(array6[12]), (array6[13] == "是") ? true : false, array6[14], float.Parse(array6[15]), int.Parse(array6[16]));
						GameLoad.Instance.DataBase.AllData.Add(array7[0]);
						GameLoad.Instance.DataBase.AllData.Add(array7[1]);
						GameLoad.Instance.DataBase.AllData.Add(array7[2]);
						if (array6[17] == "是" && (bool)cardData)
						{
							联动精耕细作(array7[0], array6[18], array6[19], array6[20], array6[21], array6[22], array6[23], array6[24], array6[25], array6[26], array6[27], array6[28], array6[29], array6[30], array6[31]);
						}
					}
				}
				text = streamReader2.ReadLine();
			}
			while (!streamReader2.EndOfStream);
			streamReader2.Close();
		}
		else
		{
			Debug.Log("没路径！");
		}
		path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MoonLian-EX.txt";
		text = "";
		if (File.Exists(path))
		{
			StreamReader streamReader3 = new StreamReader(path, Encoding.UTF8);
			text = streamReader3.ReadLine();
			do
			{
				if (text.IndexOf("#") > 0)
				{
					string[] array8 = text.Split('#');
					if (array8.Length >= 6)
					{
						生成炼金(array8[0], array8[1], array8[2], array8[3], array8[4], float.Parse(array8[5]));
					}
				}
				text = streamReader3.ReadLine();
			}
			while (!streamReader3.EndOfStream);
			streamReader3.Close();
		}
		else
		{
			Debug.Log("没路径！");
		}
		List<string> list2 = new List<string>
		{
			"f3dabe549a41cb30557412c0aa6b7fb5", "896ba9c09600d1b0352dcd4bbd6a7f26", "afceee4790a649c2aaede8f73fde17fe", "b5d8a8c2680bc4c09dbee69a91cce3b4", "dde9e2cd3a46b9c1c75401bcc51b4a86", "e2fe80f32551fba59f5e2ae0f23e1570", "d668b88ec81edbceca20061e32522eeb", "ea0859bf64aaa2acbfa976c7dbb6fdee", "debaacec3fcabd1a836aba45d4dbbb5e", "ed3de93ae318cebf2fdfd135dd5fbd7b",
			"ed82d3d1fe41da65ad501482ba8fbc76", "bd3e71f2567ace4deb522b1adcb1f03b", "37bfd4dd184b7d150caca71bebddaa59", "ca4f61bcdaacb95b7864e2af06ad4e29", "a020dece6fe58ea8ec2293ab66444a2c", "f2061c91df85985fe2b6fdb467e483d7", "a234ea39bece159a4a446c5dc8cbd9ce", "8d913e0bb51afafb9a89256fe1ff6a6a", "1e288952e349c2cd2fb3c29fcefb8d3c", "016517aac68ac69860d6040ba50398ba"
		};
		for (int k = 1; k <= 20; k++)
		{
			cont_dict[k] = 生成容器("Guil_晓月食物-" + k + "级容器", list2[k - 1], k + "级容器", "一个特质的容器，目前等级为" + k, k);
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameManager), "ActionRoutine")]
	public static IEnumerator ARPatch(IEnumerator results, CardAction _Action, InGameCardBase _ReceivingCard, bool _FastMode, bool _ModifiersAlreadyCollected = false)
	{
		if (_Action.ActionName.LocalizationKey == "GuilPot")
		{
			if ((object)_ReceivingCard != null && _ReceivingCard.CardsInInventory.Count > 0)
			{
				int x = 0;
				string[] 内容物 = new string[4];
				string[] 内容物标签 = new string[4];
				string[] 内容物混合 = new string[4];
				foreach (InventorySlot isa in _ReceivingCard.CardsInInventory)
				{
					if (isa.AllCards.Count <= 0)
					{
						continue;
					}
					foreach (InGameCardBase card2 in isa.AllCards)
					{
						if (card2 != null)
						{
							if (card2.ContainedLiquidModel != null)
							{
								内容物[x] = card2.ContainedLiquidModel.CardName;
								内容物标签[x] = 标签计算(card2.ContainedLiquid);
								内容物混合[x] = 混合计算(card2.ContainedLiquid);
							}
							else
							{
								内容物[x] = card2.CardModel.CardName;
								内容物标签[x] = 标签计算(card2);
								内容物混合[x] = 混合计算(card2);
							}
							x++;
						}
					}
				}
				Array.Sort(内容物);
				Array.Sort(内容物标签);
				Array.Sort(内容物混合);
				string 原料 = string.Join("|", 内容物);
				string 原料2 = string.Join("|", 内容物混合);
				string 原料3 = string.Join("|", 内容物标签);
				Debug.Log("名称判断：" + 原料);
				Debug.Log("混合判断：" + 原料2);
				Debug.Log("标签判断：" + 原料3);
				CardData 产物 = null;
				if (!food_dict.TryGetValue(原料, out 产物) && !food_dict.TryGetValue(原料2, out 产物))
				{
					food_dict.TryGetValue(原料3, out 产物);
				}
				if ((bool)产物)
				{
					List<int> exceptList = new List<int>();
					int y = 0;
					foreach (InventorySlot isa2 in _ReceivingCard.CardsInInventory)
					{
						if (isa2.AllCards.Count > 0)
						{
							foreach (InGameCardBase card1 in isa2.AllCards)
							{
								if (card1 != null && card1.ContainedLiquidModel != null)
								{
									card1.ContainedLiquid.CurrentLiquidQuantity -= 300f;
									exceptList.Add(y);
								}
							}
						}
						y++;
					}
					MBSingleton<GameManager>.Instance.ClearCardInventory(_ReceivingCard, _RemoveAll: true, exceptList);
					CardDrop cd2 = default(CardDrop);
					cd2.DroppedCard = 产物;
					cd2.Quantity = new Vector2Int(1, 1);
					CardDrop[] cds2 = new CardDrop[1] { cd2 };
					Traverse.Create(_Action.ProducedCards[0]).Field("DroppedCards").SetValue(cds2);
					_Action.StatModifications[0].ValueModifier = new Vector2(1f, 1f);
				}
				else
				{
					CardDrop cd = default(CardDrop);
					cd.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
					cd.Quantity = new Vector2Int(1, 1);
					CardDrop[] cds = new CardDrop[1] { cd };
					Traverse.Create(_Action.ProducedCards[0]).Field("DroppedCards").SetValue(cds);
					_Action.StatModifications[0].ValueModifier = new Vector2(0f, 0f);
				}
			}
		}
        //todo: Make sure to add these keys into the translation.
        //Alchemy - Refinery
        //Translated from:_Action.ActionName == "高级炼金" _Action.ActionName.LocalizationKey == "Guil-炼金"
        if (_Action.ActionName.LocalizationKey == "Guil-Alchemy_Refinery")
        {
			验证炼金(_Action, _ReceivingCard);
		}

        //Container merger - Refinery (Google Translate)
        //Translated from _Action.ActionName == "容器合并" _Action.ActionName.LocalizationKey == "Guil-炼金"
        if (_Action.ActionName.LocalizationKey == "Guil-ContainerMerger_Refinery")
		{
			容器合并(_Action, _ReceivingCard);
		}
		while (results.MoveNext())
		{
			yield return results.Current;
		}
	}
}
