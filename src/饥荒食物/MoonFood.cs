using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;

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
		Harmony.CreateAndPatchAll(typeof(MoonFood), (string)null);
		((BaseUnityPlugin)this).Logger.LogInfo((object)"Plugin 晓月食物 is loaded!");
	}

	private void Update()
	{
		if (Input.GetKeyUp((KeyCode)292))
		{
			GameManager.GiveCard(UniqueIDScriptable.GetFromID<CardData>("cd2791a90dfe4056b57574b9d961b15d"), false);
			GameManager.GiveCard(UniqueIDScriptable.GetFromID<CardData>("0c9fc45aa9d941afbfd892a4057c937f"), false);
		}
	}

	private static CardData utc(string uniqueID)
	{
		return UniqueIDScriptable.GetFromID<CardData>(uniqueID);
	}

	public static CardData 生成料理(string name, string guid, string cardName, string cardDescription, string cardNeed, string cardStat, float usage, float spoilTime)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		CardData val = utc("5dea9d144f3e41a6850c9fa202279d38");
		CardData val2 = ScriptableObject.CreateInstance<CardData>();
		val2 = Object.Instantiate<CardData>(val);
		((UniqueIDScriptable)val2).UniqueID = guid;
		((Object)val2).name = name;
		((UniqueIDScriptable)val2).Init();
		val2.CardDescription.DefaultText = cardDescription;
		val2.CardDescription.ParentObjectID = guid;
		val2.CardDescription.LocalizationKey = "";
		Texture2D val3 = new Texture2D(200, 300);
		string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName + ".png";
		if (!File.Exists(path))
		{
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
		}
		ImageConversion.LoadImage(val3, File.ReadAllBytes(path));
		Sprite cardImage = Sprite.Create(val3, new Rect(0f, 0f, (float)((Texture)val3).width, (float)((Texture)val3).height), Vector2.zero);
		val2.CardImage = cardImage;
		val2.CardName.DefaultText = cardName;
		val2.CardName.ParentObjectID = guid;
		val2.CardName.LocalizationKey = "";
		string[] array = cardNeed.Split('|');
		Array.Sort(array);
		string key = string.Join("|", array);
		food_dict[key] = val2;
		string[] array2 = cardStat.Split('，');
		int num = 0;
		string[] array3 = array2;
		foreach (string text in array3)
		{
			if (text == "")
			{
				((CardAction)val2.DismantleActions[0]).StatModifications[num].ValueModifier = new Vector2(0f, 0f);
			}
			else
			{
				((CardAction)val2.DismantleActions[0]).StatModifications[num].ValueModifier = new Vector2(float.Parse(text), float.Parse(text));
			}
			num++;
		}
		((OptionalFloatValue)val2.SpoilageTime).FloatValue = spoilTime;
		val2.SpoilageTime.MaxValue = spoilTime;
		((OptionalFloatValue)val2.UsageDurability).FloatValue = usage;
		val2.UsageDurability.MaxValue = usage;
		return val2;
	}

	public static string 标签计算(InGameCardBase card)
	{
		string result = "任意";
		CardData cardModel = card.CardModel;
		if (cardModel != null && cardModel.CardTags.Length != 0)
		{
			CardTag[] cardTags = cardModel.CardTags;
			foreach (CardTag val in cardTags)
			{
				if (((val != null) ? ((Object)val).name : null) == null)
				{
					continue;
				}
				string name = ((Object)val).name;
				string text = name;
				switch (text)
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
				if (text == null || text.IndexOf("Mushroom") <= 0)
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
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		CardData cardModel = card.CardModel;
		string result = LocalizedString.op_Implicit(cardModel.CardName);
		if (cardModel != null && cardModel.CardTags.Length != 0)
		{
			CardTag[] cardTags = cardModel.CardTags;
			foreach (CardTag val in cardTags)
			{
				if (((val != null) ? ((Object)val).name : null) != null)
				{
					switch (((Object)val).name)
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

	public static string 生成空格(int 数量)
	{
		string text = "";
		for (int i = 0; i < 数量; i++)
		{
			text += "\u3000";
		}
		return text;
	}

	private static void 添加种田(string GivenGuid, string ReceiGuid, string ActionName, string ActionDescription, int duration, string ProduceGuid = "")
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		CardData val = utc(GivenGuid);
		CardData val2 = utc(ReceiGuid);
		if (((Object)(object)val == (Object)null) | ((Object)(object)val2 == (Object)null))
		{
			return;
		}
		LocalizedString val3 = default(LocalizedString);
		val3.DefaultText = ActionName;
		val3.ParentObjectID = "";
		val3.LocalizationKey = "Guil-更多水果_Dummy";
		LocalizedString val4 = val3;
		val3 = default(LocalizedString);
		val3.DefaultText = ActionDescription;
		val3.ParentObjectID = "";
		val3.LocalizationKey = "Guil-更多水果_Dummy";
		LocalizedString val5 = val3;
		CardOnCardAction val6 = new CardOnCardAction(val4, val5, duration);
		Array.Resize(ref val6.CompatibleCards.TriggerCards, 1);
		val6.CompatibleCards.TriggerCards[0] = val;
		val6.GivenCardChanges.ModType = (CardModifications)3;
		if (ProduceGuid != "")
		{
			CardData val7 = utc(ProduceGuid);
			if ((Object)(object)val7 != (Object)null)
			{
				CardsDropCollection val8 = new CardsDropCollection();
				val8.CollectionName = "产出";
				val8.CollectionWeight = 1;
				CardDrop val9 = default(CardDrop);
				val9.DroppedCard = val7;
				val9.Quantity = new Vector2Int(1, 1);
				CardDrop[] value = (CardDrop[])(object)new CardDrop[1] { val9 };
				Traverse.Create((object)val8).Field("DroppedCards").SetValue((object)value);
				((CardAction)val6).ProducedCards = (CardsDropCollection[])(object)new CardsDropCollection[1] { val8 };
			}
		}
		if (val2.CardInteractions != null)
		{
			Array.Resize(ref val2.CardInteractions, val2.CardInteractions.Length + 1);
			val2.CardInteractions[val2.CardInteractions.Length - 1] = val6;
		}
	}

	private static void 添加栽培土交互(string GivenGuid, string ReceiGuid, string ActionName, string ActionDescription, int duration, string ProduceGuid = "")
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		CardData val = utc(GivenGuid);
		CardData val2 = utc(ReceiGuid);
		if (((Object)(object)val == (Object)null) | ((Object)(object)val2 == (Object)null))
		{
			return;
		}
		LocalizedString val3 = default(LocalizedString);
		val3.DefaultText = ActionName;
		val3.ParentObjectID = "";
		val3.LocalizationKey = "Guil-更多水果_Dummy";
		LocalizedString val4 = val3;
		val3 = default(LocalizedString);
		val3.DefaultText = ActionDescription;
		val3.ParentObjectID = "";
		val3.LocalizationKey = "Guil-更多水果_Dummy";
		LocalizedString val5 = val3;
		CardOnCardAction val6 = new CardOnCardAction(val4, val5, duration);
		Array.Resize(ref val6.CompatibleCards.TriggerCards, 1);
		val6.CompatibleCards.TriggerCards[0] = val;
		val6.GivenCardChanges.ModType = (CardModifications)3;
		((CardAction)val6).NotBaseAction = true;
		((CardAction)val6).UseMiniTicks = (MiniTicksBehavior)1;
		val6.WorksBothWays = true;
		if (ProduceGuid != "")
		{
			CardData val7 = utc(ProduceGuid);
			if ((Object)(object)val7 != (Object)null)
			{
				((CardAction)val6).ReceivingCardChanges.ModType = (CardModifications)2;
				((CardAction)val6).ReceivingCardChanges.TransformInto = val7;
				((CardAction)val6).ReceivingCardChanges.TransferFuel = true;
			}
		}
		if (val2.CardInteractions != null)
		{
			Array.Resize(ref val2.CardInteractions, val2.CardInteractions.Length + 1);
			val2.CardInteractions[val2.CardInteractions.Length - 1] = val6;
		}
	}

	public static CardData[] 生成草本植物(string name, string[] guid, string[] cardName, string[] cardDescription, string whereToFindGuid, int CollectionWeight, int getNum, int plantNum, bool caneat, string eatEffect, float spoilTime, int proDays)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Expected O, but got Unknown
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		CardData[] array = (CardData[])(object)new CardData[3];
		CardData val = ScriptableObject.CreateInstance<CardData>();
		val = Object.Instantiate<CardData>(utc("860762b307d74caf84d314790448f9f6"));
		((UniqueIDScriptable)val).UniqueID = guid[0];
		((Object)val).name = name;
		((UniqueIDScriptable)val).Init();
		val.CardDescription.DefaultText = cardDescription[0];
		val.CardDescription.ParentObjectID = ((UniqueIDScriptable)val).UniqueID;
		Texture2D val2 = new Texture2D(200, 300);
		string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName[0] + ".png";
		if (!File.Exists(path))
		{
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
		}
		ImageConversion.LoadImage(val2, File.ReadAllBytes(path));
		Sprite cardImage = Sprite.Create(val2, new Rect(0f, 0f, (float)((Texture)val2).width, (float)((Texture)val2).height), Vector2.zero);
		val.CardImage = cardImage;
		val.CardName.DefaultText = cardName[0];
		val.CardName.ParentObjectID = ((UniqueIDScriptable)val).UniqueID;
		if (!caneat)
		{
			val.DismantleActions.Clear();
		}
		else if (!(eatEffect == "蔬菜"))
		{
			if (eatEffect == "水果")
			{
				float[] array2 = new float[6] { 20f, 7.3333335f, 5f, 3f, 43.333332f, -10f };
				for (int i = 0; i < 6; i++)
				{
					((CardAction)val.DismantleActions[0]).StatModifications[i].ValueModifier = new Vector2(array2[i], array2[i]);
				}
				((CardAction)val.DismantleActions[0]).StatModifications[4].Stat = UniqueIDScriptable.GetFromID<GameStat>("ca65984e668e95344a7009df3dcfb052");
			}
			else
			{
				for (int j = 0; j < 6; j++)
				{
					((CardAction)val.DismantleActions[0]).StatModifications[j].ValueModifier = Vector2.zero;
				}
			}
		}
		else
		{
			float[] array3 = new float[6] { 13.125f, 2.625f, 2.5f, -2f, 32.5f, -1.25f };
			for (int k = 0; k < 6; k++)
			{
				((CardAction)val.DismantleActions[0]).StatModifications[k].ValueModifier = new Vector2(array3[k], array3[k]);
			}
		}
		((OptionalFloatValue)val.SpoilageTime).FloatValue = spoilTime;
		val.SpoilageTime.MaxValue = spoilTime;
		array[0] = val;
		CardData val3 = ScriptableObject.CreateInstance<CardData>();
		val3 = Object.Instantiate<CardData>(utc("4b17f870e9ca41eb95bf7d08aa387528"));
		((UniqueIDScriptable)val3).UniqueID = guid[1];
		((Object)val3).name = name + "丛";
		((UniqueIDScriptable)val3).Init();
		val3.CardDescription.DefaultText = cardDescription[1];
		val3.CardDescription.ParentObjectID = ((UniqueIDScriptable)val3).UniqueID;
		Texture2D val4 = new Texture2D(200, 300);
		path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName[1] + ".png";
		if (!File.Exists(path))
		{
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
		}
		ImageConversion.LoadImage(val4, File.ReadAllBytes(path));
		cardImage = Sprite.Create(val4, new Rect(0f, 0f, (float)((Texture)val4).width, (float)((Texture)val4).height), Vector2.zero);
		val3.CardImage = cardImage;
		val3.CardName.DefaultText = cardName[1];
		val3.CardName.ParentObjectID = ((UniqueIDScriptable)val3).UniqueID;
		CardDrop val5 = default(CardDrop);
		val5.DroppedCard = val;
		val5.Quantity = new Vector2Int(getNum, getNum);
		CardDrop[] value = (CardDrop[])(object)new CardDrop[1] { val5 };
		Traverse.Create((object)((CardAction)val3.DismantleActions[0]).ProducedCards[0]).Field("DroppedCards").SetValue((object)value);
		array[1] = val3;
		CardData val6 = ScriptableObject.CreateInstance<CardData>();
		val6 = Object.Instantiate<CardData>(utc("192eb567170e4fa491c18ea3e5f5ec03"));
		((UniqueIDScriptable)val6).UniqueID = guid[2];
		((Object)val6).name = name + "田";
		((UniqueIDScriptable)val6).Init();
		val6.CardName.DefaultText = cardName[2];
		val6.CardName.ParentObjectID = ((UniqueIDScriptable)val6).UniqueID;
		for (int l = 0; l <= 2; l++)
		{
			val5.DroppedCard = val3;
			val5.Quantity = new Vector2Int(plantNum * (l + 1), plantNum * (l + 1));
			CardDrop[] array4 = (CardDrop[])(object)new CardDrop[1] { val5 };
			Traverse.Create((object)val6.Progress.OnFull.ProducedCards[l]).Field("DroppedCards").SetValue((object)value);
		}
		val6.Progress.MaxValue = proDays * 96;
		array[2] = val6;
		CardData val7 = utc(whereToFindGuid);
		if (Object.op_Implicit((Object)(object)val7))
		{
			CardsDropCollection val8 = new CardsDropCollection();
			val8.CollectionName = "植株";
			val8.CollectionWeight = CollectionWeight;
			CardDrop val9 = default(CardDrop);
			val9.DroppedCard = val3;
			val9.Quantity = new Vector2Int(1, 1);
			CardDrop[] value2 = (CardDrop[])(object)new CardDrop[1] { val9 };
			Traverse.Create((object)val8).Field("DroppedCards").SetValue((object)value2);
			Array.Resize(ref ((CardAction)val7.DismantleActions[0]).ProducedCards, ((CardAction)val7.DismantleActions[0]).ProducedCards.Length + 1);
			((CardAction)val7.DismantleActions[0]).ProducedCards[((CardAction)val7.DismantleActions[0]).ProducedCards.Length - 1] = val8;
		}
		添加种田(((UniqueIDScriptable)val).UniqueID, "0308e91cdd2d6aa44a9ed0f8187f88d3", "种植" + LocalizedString.op_Implicit(val.CardName), "把" + LocalizedString.op_Implicit(val.CardName) + "种植在田中", 2, ((UniqueIDScriptable)val6).UniqueID);
		return array;
	}

	public static void 添加Tag(CardData card, string tagname)
	{
		Array.Resize(ref card.CardTags, card.CardTags.Length + 1);
		card.CardTags[card.CardTags.Length - 1] = plantTagDict[tagname];
	}

	public static void 联动精耕细作(CardData 植株, string 联动后植株内部名, string 联动植株Guid, string 水分需求, string 环境需求1, string 环境需求2, string 螨虫, string 真菌, string 寿命, string 产物进度, string 生长度, string 肥力, string 土壤疏松度, string 农药, string 是否小体型)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		CardData val = ScriptableObject.CreateInstance<CardData>();
		val = Object.Instantiate<CardData>(utc("cb27f80532de40c5a164202ed138090c"));
		((UniqueIDScriptable)val).UniqueID = 联动植株Guid;
		((Object)val).name = 联动后植株内部名;
		((UniqueIDScriptable)val).Init();
		val.CardName.DefaultText = LocalizedString.op_Implicit(植株.CardName) + "植株";
		val.CardName.LocalizationKey = "";
		val.CardName.ParentObjectID = ((UniqueIDScriptable)val).UniqueID;
		val.CardDescription.DefaultText = "我应该把它放入泥土中，等待它成熟。\n植株特性：";
		val.CardDescription.LocalizationKey = "";
		val.CardDescription.ParentObjectID = ((UniqueIDScriptable)val).UniqueID;
		添加栽培土交互(((UniqueIDScriptable)植株).UniqueID, "5b01eb40bb8245a091086c584538238d", "制作植株", "将植株放入栽培土中", 0, 联动植株Guid);
		CardDrop val2 = default(CardDrop);
		val2.DroppedCard = 植株;
		val2.Quantity = new Vector2Int(2, 2);
		CardDrop[] value = (CardDrop[])(object)new CardDrop[1] { val2 };
		Traverse.Create((object)val.SpecialDurability1.OnFull.ProducedCards[0]).Field("DroppedCards").SetValue((object)value);
		if (!(水分需求 == "喜水"))
		{
			if (水分需求 == "喜旱")
			{
				添加Tag(val, "tag_Plant_DirtFavor");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 水分需求;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_WaterFavor");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 水分需求;
		}
		if (!(环境需求1 == "喜潮"))
		{
			if (环境需求1 == "喜干")
			{
				添加Tag(val, "tag_Plant_DryFavor");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 环境需求1;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_DampFavor");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 环境需求1;
		}
		if (!(环境需求2 == "喜光"))
		{
			if (环境需求2 == "喜暗")
			{
				添加Tag(val, "tag_Plant_DarkFavor");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 环境需求2;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_LightFavor");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 环境需求2;
		}
		if (!(螨虫 == "耐螨虫"))
		{
			if (螨虫 == "怕螨虫")
			{
				添加Tag(val, "tag_Plant_MiteFear");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 螨虫;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_MiteProof");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 螨虫;
		}
		if (!(真菌 == "耐真菌"))
		{
			if (真菌 == "怕真菌")
			{
				添加Tag(val, "tag_Plant_FungiFear");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 真菌;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_FungiProof");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 真菌;
		}
		if (!(寿命 == "长寿"))
		{
			if (寿命 == "短寿")
			{
				添加Tag(val, "tag_Plant_ShortLive");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 寿命;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_LongLive");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 寿命;
		}
		if (!(产物进度 == "高产"))
		{
			if (产物进度 == "低产")
			{
				添加Tag(val, "tag_Plant_LowProduce");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 产物进度;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_HighProduce");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 产物进度;
		}
		if (!(生长度 == "速生"))
		{
			if (生长度 == "慢生")
			{
				添加Tag(val, "tag_Plant_GrowSlow");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 生长度;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_GrowFast");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 生长度;
		}
		if (!(肥力 == "肥田"))
		{
			if (肥力 == "耗肥")
			{
				添加Tag(val, "tag_Plant_ConsumeFertilize");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 肥力;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_Fertilize");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 肥力;
		}
		if (!(土壤疏松度 == "松土"))
		{
			if (土壤疏松度 == "板结")
			{
				添加Tag(val, "tag_Plant_AirLess");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 土壤疏松度;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_AirMore");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 土壤疏松度;
		}
		if (!(农药 == "杀螨虫"))
		{
			if (农药 == "杀真菌")
			{
				添加Tag(val, "tag_Plant_AntiFungi");
				val.CardDescription.DefaultText = val.CardDescription.DefaultText + 农药;
			}
		}
		else
		{
			添加Tag(val, "tag_Plant_AntiMite");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + 农药;
		}
		if (是否小体型 == "是")
		{
			添加Tag(val, "tag_Plant_Small");
			val.CardDescription.DefaultText = val.CardDescription.DefaultText + "小体型";
		}
		GameLoad.Instance.DataBase.AllData.Add((UniqueIDScriptable)(object)val);
	}

	public static void 生成炼金(string name, string guid, string cardName, string cardDescription, string cardNeed, float 产出数量 = 1f)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)utc(guid) == (Object)null)
		{
			CardData val = ScriptableObject.CreateInstance<CardData>();
			val = Object.Instantiate<CardData>(utc("175ef2773fd84a3c816e93c4307da58f"));
			((UniqueIDScriptable)val).UniqueID = guid;
			((Object)val).name = name;
			((UniqueIDScriptable)val).Init();
			val.CardDescription.DefaultText = cardDescription;
			val.CardDescription.ParentObjectID = guid;
			val.CardDescription.LocalizationKey = "";
			Texture2D val2 = new Texture2D(200, 300);
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName + ".png";
			if (!File.Exists(path))
			{
				path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\无图.png";
			}
			ImageConversion.LoadImage(val2, File.ReadAllBytes(path));
			Sprite cardImage = Sprite.Create(val2, new Rect(0f, 0f, (float)((Texture)val2).width, (float)((Texture)val2).height), Vector2.zero);
			val.CardImage = cardImage;
			val.CardName.DefaultText = cardName;
			val.CardName.ParentObjectID = guid;
			val.CardName.LocalizationKey = "";
			string[] array = cardNeed.Split('|');
			Array.Sort(array);
			string key = string.Join("|", array);
			val.SpecialDurability4.MaxValue = 产出数量;
			lianjin_dict[key] = val;
		}
		else
		{
			string[] array2 = cardNeed.Split('|');
			Array.Sort(array2);
			string key2 = string.Join("|", array2);
			CardData val3 = utc(guid);
			val3.SpecialDurability4.MaxValue = 产出数量;
			lianjin_dict[key2] = val3;
		}
	}

	public static void 验证炼金(CardAction action, InGameCardBase card)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		if (card == null || card.CardsInInventory.Count <= 0)
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
				if ((Object)(object)allCard != (Object)null)
				{
					if ((Object)(object)allCard.ContainedLiquidModel != (Object)null)
					{
						array[num] = LocalizedString.op_Implicit(allCard.ContainedLiquidModel.CardName);
					}
					else
					{
						array[num] = LocalizedString.op_Implicit(allCard.CardModel.CardName);
					}
					num++;
				}
			}
		}
		Array.Sort(array);
		string text = string.Join("|", array);
		Debug.Log((object)("名称判断：" + text));
		lianjin_dict.TryGetValue(text, out var value);
		if (Object.op_Implicit((Object)(object)value))
		{
			List<int> list = new List<int>();
			int num2 = 0;
			foreach (InventorySlot item2 in card.CardsInInventory)
			{
				if (item2.AllCards.Count > 0)
				{
					foreach (InGameCardBase allCard2 in item2.AllCards)
					{
						if ((Object)(object)allCard2 != (Object)null && (Object)(object)allCard2.ContainedLiquidModel != (Object)null)
						{
							InGameCardBase containedLiquid = allCard2.ContainedLiquid;
							containedLiquid.CurrentLiquidQuantity -= 300f;
							list.Add(num2);
						}
					}
				}
				num2++;
			}
			MBSingleton<GameManager>.Instance.ClearCardInventory(card, true, list);
			CardDrop val = default(CardDrop);
			val.DroppedCard = value;
			val.Quantity = new Vector2Int((int)value.SpecialDurability4.MaxValue, (int)value.SpecialDurability4.MaxValue);
			CardDrop[] value2 = (CardDrop[])(object)new CardDrop[1] { val };
			Traverse.Create((object)action.ProducedCards[0]).Field("DroppedCards").SetValue((object)value2);
		}
		else
		{
			CardDrop val2 = default(CardDrop);
			val2.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
			val2.Quantity = new Vector2Int(1, 1);
			CardDrop[] value3 = (CardDrop[])(object)new CardDrop[1] { val2 };
			Traverse.Create((object)action.ProducedCards[0]).Field("DroppedCards").SetValue((object)value3);
		}
	}

	public static void 容器合并(CardAction action, InGameCardBase card)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		if (card == null || card.CardsInInventory.Count <= 0)
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
				if ((Object)(object)allCard != (Object)null)
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
			List<int> list = new List<int>();
			MBSingleton<GameManager>.Instance.ClearCardInventory(card, true, list);
			CardDrop val = default(CardDrop);
			val.DroppedCard = cont_dict[num3];
			val.Quantity = new Vector2Int(1, 1);
			CardDrop[] value = (CardDrop[])(object)new CardDrop[1] { val };
			Traverse.Create((object)action.ProducedCards[0]).Field("DroppedCards").SetValue((object)value);
		}
		else
		{
			CardDrop val2 = default(CardDrop);
			val2.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
			val2.Quantity = new Vector2Int(1, 1);
			CardDrop[] value2 = (CardDrop[])(object)new CardDrop[1] { val2 };
			Traverse.Create((object)action.ProducedCards[0]).Field("DroppedCards").SetValue((object)value2);
		}
	}

	public static CardData 生成容器(string name, string guid, string cardName, string cardDescription, int contain)
	{
		CardData val = utc("247cb2c7dc0f4bc0814e91d794900c40");
		CardData val2 = ScriptableObject.CreateInstance<CardData>();
		val2 = Object.Instantiate<CardData>(val);
		((UniqueIDScriptable)val2).UniqueID = guid;
		((Object)val2).name = name;
		((UniqueIDScriptable)val2).Init();
		val2.CardDescription.DefaultText = cardDescription;
		val2.CardDescription.ParentObjectID = guid;
		val2.CardDescription.LocalizationKey = "";
		val2.CardName.DefaultText = cardName;
		val2.CardName.ParentObjectID = guid;
		val2.CardName.LocalizationKey = "";
		val2.MaxLiquidCapacity = 300 * contain;
		return val2;
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
			Debug.Log((object)(tagname + "不存在"));
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
	public static void SomePatch()
	{
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
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
			Debug.Log((object)"没路径！");
		}
		foreach (string item2 in list)
		{
			string[] array = item2.Split('#');
			if (array.Length >= 8)
			{
				CardData item = 生成料理(array[0], array[1], array[2], array[3], array[4], array[5], float.Parse(array[6]), float.Parse(array[7]));
				GameLoad.Instance.DataBase.AllData.Add((UniqueIDScriptable)(object)item);
			}
		}
		配方补充("20eb4938d55244f1bdec102dd1391038", "肉|肉|任意|任意");
		string text2 = "";
		int num = 0;
		foreach (KeyValuePair<string, CardData> item3 in food_dict)
		{
			string key = item3.Key;
			CardData value = item3.Value;
			string text3 = LocalizedString.op_Implicit(value.CardName) + "=" + key;
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
		GuideEntry[] array2 = Object.FindObjectsOfType<GuideEntry>();
		GuideEntry[] array3 = array2;
		foreach (GuideEntry val in array3)
		{
			if (((Object)val).name.IndexOf("高级炉灶") >= 0)
			{
				val.OverrideDescription.DefaultText = val.OverrideDescription.DefaultText + "\n" + text2;
			}
		}
		CardData val2 = utc("cb27f80532de40c5a164202ed138090c");
		if (Object.op_Implicit((Object)(object)val2))
		{
			CardTag[] array4 = Object.FindObjectsOfType<CardTag>();
			CardTag[] array5 = array4;
			foreach (CardTag val3 in array5)
			{
				if (((Object)val3).name.StartsWith("tag_Plant_"))
				{
					plantTagDict[((Object)val3).name] = val3;
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
						GameLoad.Instance.DataBase.AllData.Add((UniqueIDScriptable)(object)array7[0]);
						GameLoad.Instance.DataBase.AllData.Add((UniqueIDScriptable)(object)array7[1]);
						GameLoad.Instance.DataBase.AllData.Add((UniqueIDScriptable)(object)array7[2]);
						if (array6[17] == "是" && Object.op_Implicit((Object)(object)val2))
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
			Debug.Log((object)"没路径！");
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
			Debug.Log((object)"没路径！");
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
		if (LocalizedString.op_Implicit(_Action.ActionName) == "高级烹饪" && _Action.ActionName.LocalizationKey == "GuilPot")
		{
			if (_ReceivingCard != null && _ReceivingCard.CardsInInventory.Count > 0)
			{
				int x = 0;
				string[] 内容物 = new string[4];
				string[] 内容物标签 = new string[4];
				string[] 内容物混合 = new string[4];
				foreach (InventorySlot isa2 in _ReceivingCard.CardsInInventory)
				{
					if (isa2.AllCards.Count <= 0)
					{
						continue;
					}
					foreach (InGameCardBase card2 in isa2.AllCards)
					{
						if ((Object)(object)card2 != (Object)null)
						{
							if ((Object)(object)card2.ContainedLiquidModel != (Object)null)
							{
								内容物[x] = LocalizedString.op_Implicit(card2.ContainedLiquidModel.CardName);
								内容物标签[x] = 标签计算(card2.ContainedLiquid);
								内容物混合[x] = 混合计算(card2.ContainedLiquid);
							}
							else
							{
								内容物[x] = LocalizedString.op_Implicit(card2.CardModel.CardName);
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
				Debug.Log((object)("名称判断：" + 原料));
				Debug.Log((object)("混合判断：" + 原料2));
				Debug.Log((object)("标签判断：" + 原料3));
				CardData 产物 = null;
				if (!food_dict.TryGetValue(原料, out 产物) && !food_dict.TryGetValue(原料2, out 产物))
				{
					food_dict.TryGetValue(原料3, out 产物);
				}
				if (Object.op_Implicit((Object)(object)产物))
				{
					List<int> exceptList = new List<int>();
					int y = 0;
					foreach (InventorySlot isa in _ReceivingCard.CardsInInventory)
					{
						if (isa.AllCards.Count > 0)
						{
							foreach (InGameCardBase card1 in isa.AllCards)
							{
								if ((Object)(object)card1 != (Object)null && (Object)(object)card1.ContainedLiquidModel != (Object)null)
								{
									InGameCardBase containedLiquid = card1.ContainedLiquid;
									containedLiquid.CurrentLiquidQuantity -= 300f;
									exceptList.Add(y);
								}
							}
						}
						y++;
					}
					MBSingleton<GameManager>.Instance.ClearCardInventory(_ReceivingCard, true, exceptList);
					CardDrop cd2 = default(CardDrop);
					cd2.DroppedCard = 产物;
					cd2.Quantity = new Vector2Int(1, 1);
					CardDrop[] cds2 = (CardDrop[])(object)new CardDrop[1] { cd2 };
					Traverse.Create((object)_Action.ProducedCards[0]).Field("DroppedCards").SetValue((object)cds2);
					_Action.StatModifications[0].ValueModifier = new Vector2(1f, 1f);
				}
				else
				{
					CardDrop cd = default(CardDrop);
					cd.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
					cd.Quantity = new Vector2Int(1, 1);
					CardDrop[] cds = (CardDrop[])(object)new CardDrop[1] { cd };
					Traverse.Create((object)_Action.ProducedCards[0]).Field("DroppedCards").SetValue((object)cds);
					_Action.StatModifications[0].ValueModifier = new Vector2(0f, 0f);
				}
			}
		}
		if (LocalizedString.op_Implicit(_Action.ActionName) == "高级炼金" && _Action.ActionName.LocalizationKey == "Guil-炼金")
		{
			验证炼金(_Action, _ReceivingCard);
		}
		if (LocalizedString.op_Implicit(_Action.ActionName) == "容器合并" && _Action.ActionName.LocalizationKey == "Guil-炼金")
		{
			容器合并(_Action, _ReceivingCard);
		}
		while (results.MoveNext())
		{
			yield return results.Current;
		}
	}
}
