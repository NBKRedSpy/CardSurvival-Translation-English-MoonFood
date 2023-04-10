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
using BepInEx.Configuration;

namespace 饥荒食物
{
	[BepInPlugin("Plugin.MoonFood", "MoonFood", "1.0.0")]
public class MoonFood : BaseUnityPlugin
{

		ConfigEntry<bool> EnableDebugKeys;

		private void Awake() //起始代码
		{

		 LocalizationStringUtility.Init(
			 Config.Bind<bool>("Debug", "LogCardInfo", false, "If true, will output the localization keys for the cards. 如果为真，将输出卡片的本地化密钥。").Value,
			 Info.Location,
			 Logger
		);


		EnableDebugKeys = Config.Bind<bool>("Debug", "启用的调试键 (EnabledDebugKeys)", false, "如果为真，将启用调试键 (If true, will enable the debug keys)");

		Harmony.CreateAndPatchAll(typeof(MoonFood));
			Logger.LogInfo("Plugin 晓月食物 is loaded!");
	}

		private void Update()
		{
			if (EnableDebugKeys.Value && Input.GetKeyUp(KeyCode.F11)) {
				GameManager.GiveCard(UniqueIDScriptable.GetFromID<CardData>("cd2791a90dfe4056b57574b9d961b15d"), false);
				GameManager.GiveCard(UniqueIDScriptable.GetFromID<CardData>("0c9fc45aa9d941afbfd892a4057c937f"), false);
			}
		}
		//====================================================================================
		private static CardData utc(String uniqueID) //ID读卡
		{
			return UniqueIDScriptable.GetFromID<CardData>(uniqueID);
		}
		//===================================自定义食物=======================================
		public static Dictionary<string, CardData> food_dict = new Dictionary<string, CardData>();
		public static CardData 生成料理(string name, string guid, string cardName, string cardDescription, string cardNeed, string cardStat, float usage, float spoilTime)
		{
			CardData 料理模板 = utc("5dea9d144f3e41a6850c9fa202279d38");
			CardData liaoli = ScriptableObject.CreateInstance<CardData>();
			liaoli = Instantiate(料理模板);
			liaoli.UniqueID = guid;

			liaoli.name = name;
			liaoli.Init();
			//liaoli.CardType = CardTypes.Item;

			//卡牌描述
			liaoli.CardDescription.DefaultText = cardDescription;
			liaoli.CardDescription.ParentObjectID = guid;
			liaoli.CardDescription.LocalizationKey = "";

			//卡牌图片
			Texture2D texture2D = new Texture2D(200, 300);
			string text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName + ".png";
			if (!File.Exists(text4)) {
				text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + "无图" + ".png";
			}

			texture2D.LoadImage(File.ReadAllBytes(text4));
			Sprite sp = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), Vector2.zero);
			liaoli.CardImage = sp;

			//卡牌名称
			liaoli.CardName.DefaultText = cardName;
			liaoli.CardName.ParentObjectID = guid;
			liaoli.CardName.LocalizationKey = "";

			string[] dummy = cardNeed.Split('|');
			Array.Sort(dummy);
			string dummy2 = string.Join("|", dummy);
			//Debug.Log(cardName + "=@@@=" + dummy2);
			food_dict[dummy2] = liaoli;
			//Debug.Log(cardName + "=" + dummy2);

			//修改DismantleAction效果
			string[] words = cardStat.Split('，');
			int x = 0;
			foreach (var word in words) {
				if (word == "") {
					liaoli.DismantleActions[0].StatModifications[x].ValueModifier = new Vector2(0f, 0f);
				}
				else {
					liaoli.DismantleActions[0].StatModifications[x].ValueModifier = new Vector2(float.Parse(word), float.Parse(word));
				}
				x++;
			}

			liaoli.SpoilageTime.FloatValue = spoilTime;
			liaoli.SpoilageTime.MaxValue = spoilTime;
			liaoli.UsageDurability.FloatValue = usage;
			liaoli.UsageDurability.MaxValue = usage;

			//GameLoad.Instance.DataBase.AllData.Add(liaoli);
			return liaoli;
		}
		public static string 标签计算(InGameCardBase card)
		{
			string result = "任意";
			CardData card2 = card.CardModel;
			if (card2?.CardTags.Length > 0) {
				foreach (CardTag tag in card2.CardTags) {
					if (tag?.name != null) {
						switch (tag.name) {
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
							case string x when x.IndexOf("Mushroom") > 0:
								return "蘑菇";
							default:
								break;
						}
					}
				}
			}
			return result;
		}
		public static string 混合计算(InGameCardBase card)
		{
			CardData card2 = card.CardModel;
			string result = card2.CardName;
			if (card2?.CardTags.Length > 0) {
				foreach (CardTag tag in card2.CardTags) {
					if (tag?.name != null) {
						switch (tag.name) {
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
							default:
								break;
					}
				}
				}
			}
			return result;
		}
		public static void 配方补充(string guid, string 配方)
		{
			string[] dummy = 配方.Split('|');
			Array.Sort(dummy);
			string dummy2 = string.Join("|", dummy);
			food_dict[dummy2] = utc(guid);
		}
		public static string 生成空格(int 数量)
		{
			string 空格 = "";
			for (int i = 0; i < 数量; i++) {
				空格 = 空格 + "　";
			}
			return 空格;
		}
		//===================================自定义植物========================================
    /// </summary>
		private static void 添加种田(string GivenGuid, string ReceiGuid, string ActionName, string ActionDescription, int duration, string ProduceGuid = "")
		{
			CardData GivenCard = utc(GivenGuid);//植株
			CardData ReceiCard = utc(ReceiGuid);//田
			if (GivenCard == null | ReceiCard == null) { return; }
			LocalizedString name1 = new LocalizedString
			{
				DefaultText = ActionName,
				ParentObjectID = "",
			};
			name1.SetLocalizationInfo();
			
			LocalizedString name2 = new LocalizedString
			{
				DefaultText = ActionDescription,
				ParentObjectID = "",
			};
			name2.SetLocalizationInfo();
			
			CardOnCardAction action = new CardOnCardAction(name1, name2, duration);

			Array.Resize(ref action.CompatibleCards.TriggerCards, 1);
			action.CompatibleCards.TriggerCards[0] = GivenCard;
			action.GivenCardChanges.ModType = CardModifications.Destroy;

			if (ProduceGuid != "") {
				CardData ProduceCard = utc(ProduceGuid);
				if (ProduceCard != null) {
					CardsDropCollection cdc = new CardsDropCollection();
					cdc.CollectionName = "产出";
					cdc.CollectionWeight = 1;

					CardDrop cd = new CardDrop();
					cd.DroppedCard = ProduceCard;
					cd.Quantity = new Vector2Int(1, 1);
					CardDrop[] cds = new CardDrop[] { cd };
					Traverse.Create(cdc).Field("DroppedCards").SetValue(cds);

					action.ProducedCards = new CardsDropCollection[] { cdc };
				}
			}
			if (ReceiCard.CardInteractions != null) {
				Array.Resize(ref ReceiCard.CardInteractions, ReceiCard.CardInteractions.Length + 1);
				ReceiCard.CardInteractions[ReceiCard.CardInteractions.Length - 1] = action;
			}
		}

		private static void 添加栽培土交互(string GivenGuid, string ReceiGuid, string ActionName, string ActionDescription, int duration, string ProduceGuid = "")
		{
			CardData GivenCard = utc(GivenGuid);//植株
			CardData ReceiCard = utc(ReceiGuid);//田
			if (GivenCard == null | ReceiCard == null) { return; }
			LocalizedString name1 = new LocalizedString
			{
				DefaultText = ActionName,
				ParentObjectID = "",
			};
			name1.SetLocalizationInfo();			
			
			LocalizedString name2 = new LocalizedString
			{
				DefaultText = ActionDescription,
				ParentObjectID = "",
				LocalizationKey = "Guil-更多水果_Dummy"
			};
			name2.SetLocalizationInfo();
			
			
			
			CardOnCardAction action = new CardOnCardAction(name1, name2, duration);

			Array.Resize(ref action.CompatibleCards.TriggerCards, 1);
			action.CompatibleCards.TriggerCards[0] = GivenCard;
			action.GivenCardChanges.ModType = CardModifications.Destroy;
			action.NotBaseAction = true;
			action.UseMiniTicks = MiniTicksBehavior.CostsAMiniTick;
			action.WorksBothWays = true;

			if (ProduceGuid != "") {
				CardData ProduceCard = utc(ProduceGuid);
				if (ProduceCard != null) {
					action.ReceivingCardChanges.ModType = CardModifications.Transform;
					action.ReceivingCardChanges.TransformInto = ProduceCard;
					action.ReceivingCardChanges.TransferFuel = true;
				}
			}
			if (ReceiCard.CardInteractions != null) {
				Array.Resize(ref ReceiCard.CardInteractions, ReceiCard.CardInteractions.Length + 1);
				ReceiCard.CardInteractions[ReceiCard.CardInteractions.Length - 1] = action;
			}
		}

		public static CardData[] 生成草本植物(string name, string[] guid, string[] cardName, string[] cardDescription, string whereToFindGuid, int CollectionWeight, int getNum, int plantNum, bool caneat, string eatEffect, float spoilTime, int proDays)
		{
			CardData[] result = new CardData[3];
			//植物模板
			CardData plant = ScriptableObject.CreateInstance<CardData>();
			plant = Instantiate(utc("860762b307d74caf84d314790448f9f6"));
			plant.UniqueID = guid[0];
			plant.name = name;
			plant.Init();
			//CardDescription
			plant.CardDescription.DefaultText = cardDescription[0];
			plant.CardDescription.ParentObjectID = plant.UniqueID;
			//CardImage
			Texture2D texture2D = new Texture2D(200, 300);
			string text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName[0] + ".png";
			if (!File.Exists(text4)) { text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + "无图" + ".png"; }
			texture2D.LoadImage(File.ReadAllBytes(text4));
			Sprite sp = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), Vector2.zero);
			plant.CardImage = sp;
			//CardName
			plant.CardName.DefaultText = cardName[0];
			plant.CardName.SetLocalizationInfo();

			plant.CardName.ParentObjectID = plant.UniqueID;
			//DismantleActions
			if (caneat == false) {
				plant.DismantleActions.Clear();
			}
			else {
				switch (eatEffect) {//	饱食0	胃1	水2	情绪3	厌倦度4	压力5
					case "蔬菜":
						float[] a1 = new float[] { 13.125f, 2.625f, 2.5f, -2f, 32.5f, -1.25f };
						for (int i = 0; i < 6; i++) {
							plant.DismantleActions[0].StatModifications[i].ValueModifier = new Vector2(a1[i], a1[i]);
						}
						break;
					case "水果":
						float[] a2 = new float[] { 20f, 7.33333333333333f, 5f, 3f, 43.3333333333333f, -10f };
						for (int i = 0; i < 6; i++) {
							plant.DismantleActions[0].StatModifications[i].ValueModifier = new Vector2(a2[i], a2[i]);
						}
						plant.DismantleActions[0].StatModifications[4].Stat = UniqueIDScriptable.GetFromID<GameStat>("ca65984e668e95344a7009df3dcfb052");
						break;
					default:
						for (int i = 0; i < 6; i++) {
							plant.DismantleActions[0].StatModifications[i].ValueModifier = Vector2.zero;
						}
						break;
				}
			}
			plant.SpoilageTime.FloatValue = spoilTime;
			plant.SpoilageTime.MaxValue = spoilTime;
			result[0] = plant;

			//植物丛模板
			CardData plantCong = ScriptableObject.CreateInstance<CardData>();
			plantCong = Instantiate(utc("4b17f870e9ca41eb95bf7d08aa387528"));
			plantCong.UniqueID = guid[1];
			plantCong.name = name + "丛";
			plantCong.Init();
			//CardDescription
			plantCong.CardDescription.DefaultText = cardDescription[1];
  			plantCong.CardDescription.SetLocalizationInfo();

			plantCong.CardDescription.ParentObjectID = plantCong.UniqueID;
			//CardImage
			Texture2D texture2D2 = new Texture2D(200, 300);
			text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName[1] + ".png";
			if (!File.Exists(text4)) { text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + "无图" + ".png"; }
			texture2D2.LoadImage(File.ReadAllBytes(text4));
			sp = Sprite.Create(texture2D2, new Rect(0f, 0f, (float)texture2D2.width, (float)texture2D2.height), Vector2.zero);
			plantCong.CardImage = sp;
			//CardName
			plantCong.CardName.DefaultText = cardName[1];
			plantCong.CardName.ParentObjectID = plantCong.UniqueID;
			//DismantleActions
			CardDrop cd = new CardDrop();
			cd.DroppedCard = plant;
			cd.Quantity = new Vector2Int(getNum, getNum);
			CardDrop[] cds = new CardDrop[] { cd };
			Traverse.Create(plantCong.DismantleActions[0].ProducedCards[0]).Field("DroppedCards").SetValue(cds);
			result[1] = plantCong;

			//植物田模板
			CardData plantTian = ScriptableObject.CreateInstance<CardData>();
			plantTian = Instantiate(utc("192eb567170e4fa491c18ea3e5f5ec03"));
			plantTian.UniqueID = guid[2];
			plantTian.name = name + "田";
			plantTian.Init();
			//CardName
			plantTian.CardName.DefaultText = cardName[2];
			plantTian.CardName.SetLocalizationInfo();	
			plantTian.CardName.ParentObjectID = plantTian.UniqueID;
			//产出
			for (int i1 = 0; i1 <= 2; i1++) {
				cd.DroppedCard = plantCong;
				cd.Quantity = new Vector2Int(plantNum * (i1 + 1), plantNum * (i1 + 1));
				CardDrop[] cds2 = new CardDrop[] { cd };
				Traverse.Create(plantTian.Progress.OnFull.ProducedCards[i1]).Field("DroppedCards").SetValue(cds);
			}
			//成熟天数
			plantTian.Progress.MaxValue = proDays * 96;
			result[2] = plantTian;

			//添加到发现地点
			CardData FindPlace = utc(whereToFindGuid);
			if (FindPlace) {
				CardsDropCollection cdc = new CardsDropCollection();
				cdc.CollectionName = "植株";
				cdc.CollectionWeight = CollectionWeight;

				CardDrop cd3 = new CardDrop();
				cd3.DroppedCard = plantCong;
				cd3.Quantity = new Vector2Int(1, 1);
				CardDrop[] cds3 = new CardDrop[] { cd3 };
				Traverse.Create(cdc).Field("DroppedCards").SetValue(cds3);
				Array.Resize(ref FindPlace.DismantleActions[0].ProducedCards, FindPlace.DismantleActions[0].ProducedCards.Length + 1);
				FindPlace.DismantleActions[0].ProducedCards[FindPlace.DismantleActions[0].ProducedCards.Length - 1] = cdc;
			}

			//添加种田
			添加种田(plant.UniqueID, "0308e91cdd2d6aa44a9ed0f8187f88d3", "种植" + plant.CardName, "把" + plant.CardName + "种植在田中", 2, plantTian.UniqueID);

			return result;
		}

		public static Dictionary<string, CardTag> plantTagDict = new Dictionary<string, CardTag>();

		public static void 添加Tag(CardData card, string tagname)
	{
		Array.Resize(ref card.CardTags, card.CardTags.Length + 1);
		card.CardTags[card.CardTags.Length - 1] = plantTagDict[tagname];
	}

		public static void 联动精耕细作(CardData 植株, string 联动后植株内部名, string 联动植株Guid, string 水分需求, string 环境需求1, string 环境需求2, string 螨虫, string 真菌, string 寿命, string 产物进度, string 生长度, string 肥力, string 土壤疏松度, string 农药, string 是否小体型)
		{
			CardData plant = ScriptableObject.CreateInstance<CardData>();
			plant = Instantiate(utc("cb27f80532de40c5a164202ed138090c"));
			plant.UniqueID = 联动植株Guid;
			plant.name = 联动后植株内部名;
			plant.Init();
			plant.CardName.DefaultText = 植株.CardName + "植株";
  			plant.CardName.SetLocalizationInfo();
			plant.CardName.ParentObjectID = plant.UniqueID;
			plant.CardDescription.DefaultText = "我应该把它放入泥土中，等待它成熟。\n植株特性：";
            plant.CardDescription.SetLocalizationInfo();
			plant.CardDescription.ParentObjectID = plant.UniqueID;
			//添加交互
			添加栽培土交互(植株.UniqueID, "5b01eb40bb8245a091086c584538238d", "制作植株", "将植株放入栽培土中", 0, 联动植株Guid);
			//修改产出
			CardDrop cd = new CardDrop();
			cd.DroppedCard = 植株;
			cd.Quantity = new Vector2Int(2, 2);
			CardDrop[] cds = new CardDrop[] { cd };
			Traverse.Create(plant.SpecialDurability1.OnFull.ProducedCards[0]).Field("DroppedCards").SetValue(cds);
			//应用标签
			//水分需求	环境需求	螨虫	真菌	寿命	产物进度	生长度	肥力	土壤疏松度	农药	是否小体型
			switch (水分需求) {
				case "喜水":
					添加Tag(plant, "tag_Plant_WaterFavor");

					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 水分需求;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "喜旱":
					添加Tag(plant, "tag_Plant_DirtFavor");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 水分需求;
					break;
				default:
					break;
			}
			//喜潮 tag_Plant_DampFavor 喜干 tag_Plant_DryFavor
			switch (环境需求1) {
				case "喜潮":
					添加Tag(plant, "tag_Plant_DampFavor");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 环境需求1;
					plant.CardDescription.SetLocalizationInfo();	
					break;
				case "喜干":
					添加Tag(plant, "tag_Plant_DryFavor");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 环境需求1;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			//喜光 tag_Plant_LightFavor 喜暗 tag_Plant_DarkFavor
			switch (环境需求2) {
				case "喜光":
					添加Tag(plant, "tag_Plant_LightFavor");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 环境需求2;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "喜暗":
					添加Tag(plant, "tag_Plant_DarkFavor");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 环境需求2;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
		}
			//螨虫
			switch (螨虫) {
				case "耐螨虫":
					添加Tag(plant, "tag_Plant_MiteProof");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 螨虫;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "怕螨虫":
					添加Tag(plant, "tag_Plant_MiteFear");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 螨虫;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
        }
			//真菌
			switch (真菌) {
				case "耐真菌":
					添加Tag(plant, "tag_Plant_FungiProof");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 真菌;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "怕真菌":
					添加Tag(plant, "tag_Plant_FungiFear");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 真菌;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			//Spoilage 寿命	
			//寿命SpoilageRate - 寿命SpoilageRate +
			//长寿 tag_Plant_LongLive 短寿 tag_Plant_ShortLive
			switch (寿命) {
				case "长寿":
					添加Tag(plant, "tag_Plant_LongLive");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 寿命;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "短寿":
					添加Tag(plant, "tag_Plant_ShortLive");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 寿命;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			//Sp1 产物进度
			//产物进度Sp1Rate + 产物进度Sp1Rate -
			//高产 tag_Plant_HighProduce	低产 tag_Plant_LowProduce
			switch (产物进度) {
				case "高产":
					添加Tag(plant, "tag_Plant_HighProduce");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 产物进度;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "低产":
					添加Tag(plant, "tag_Plant_LowProduce");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 产物进度;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			/*
			Progress 生长度
			生长度ProgressRate + 生长度ProgressRate -
			速生 tag_Plant_GrowFast 慢生 tag_Plant_GrowSlow
			*/
			switch (生长度) {
				case "速生":
					添加Tag(plant, "tag_Plant_GrowFast");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 生长度;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "慢生":
					添加Tag(plant, "tag_Plant_GrowSlow");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 生长度;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			//肥力 土壤疏松度       农药
			//肥田 tag_Plant_Fertilize 耗肥 tag_Plant_ConsumeFertilize 松土 tag_Plant_AirMore 板结 tag_Plant_AirLess 杀螨虫 tag_Plant_AntiMite 杀真菌 tag_Plant_AntiFungi
			switch (肥力) {
				case "肥田":
					添加Tag(plant, "tag_Plant_Fertilize");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 肥力;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "耗肥":
					添加Tag(plant, "tag_Plant_ConsumeFertilize");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 肥力;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			switch (土壤疏松度) {
				case "松土":
					添加Tag(plant, "tag_Plant_AirMore");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 土壤疏松度;
					plant.CardDescription.SetLocalizationInfo();
					break;
				case "板结":
					添加Tag(plant, "tag_Plant_AirLess");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 土壤疏松度;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			switch (农药) {
				case "杀螨虫":
					添加Tag(plant, "tag_Plant_AntiMite");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 农药;
					plant.CardDescription.SetLocalizationInfo();

					break;
				case "杀真菌":
					添加Tag(plant, "tag_Plant_AntiFungi");
					plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + 农药;
					plant.CardDescription.SetLocalizationInfo();
					break;
				default:
					break;
			}
			if (是否小体型 == "是") {
				添加Tag(plant, "tag_Plant_Small");
				plant.CardDescription.DefaultText = plant.CardDescription.DefaultText + "小体型";
				plant.CardDescription.SetLocalizationInfo();
			}
			GameLoad.Instance.DataBase.AllData.Add(plant);
	}
		//===================================炼金术============================================
		public static Dictionary<string, CardData> lianjin_dict = new Dictionary<string, CardData>();
		public static void 生成炼金(string name, string guid, string cardName, string cardDescription, string cardNeed, float 产出数量 = 1f)
		{
			if (utc(guid) == null) {    //判断是否ME加过卡
				//Debug.Log("没这卡");

				CardData liaoli = ScriptableObject.CreateInstance<CardData>();
				liaoli = Instantiate(utc("175ef2773fd84a3c816e93c4307da58f"));
				liaoli.UniqueID = guid;
				liaoli.name = name;
				liaoli.Init();
				//卡牌描述
				liaoli.CardDescription.DefaultText = cardDescription;
				liaoli.CardDescription.ParentObjectID = guid;
				liaoli.CardDescription.SetLocalizationInfo();

				//卡牌图片
				Texture2D texture2D = new Texture2D(200, 300);
				string text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + cardName + ".png";
				if (!File.Exists(text4)) { text4 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resource\\Picture\\" + "无图" + ".png"; }
				texture2D.LoadImage(File.ReadAllBytes(text4));
				Sprite sp = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), Vector2.zero);
				liaoli.CardImage = sp;

				//卡牌名称
				liaoli.CardName.DefaultText = cardName;
	            liaoli.CardName.SetLocalizationInfo();
			
				liaoli.CardName.ParentObjectID = guid;
				liaoli.CardName.SetLocalizationInfo();
				
				string[] dummy = cardNeed.Split('|');
				Array.Sort(dummy);
				string dummy2 = string.Join("|", dummy);
				liaoli.SpecialDurability4.MaxValue = 产出数量;
				lianjin_dict[dummy2] = liaoli;
			}
			else {
				string[] dummy = cardNeed.Split('|');
				Array.Sort(dummy);
				string dummy2 = string.Join("|", dummy);
				CardData liaoli = utc(guid);
				liaoli.SpecialDurability4.MaxValue = 产出数量;
				lianjin_dict[dummy2] = liaoli;
			}
		}

		public static void 验证炼金(CardAction action, InGameCardBase card)
		{
			//读取容器内容物
			if (card?.CardsInInventory.Count > 0) {
				//每有1个内容物就有1个InventorySlot，然后每个InventorySlot里有1个AllCards，仅使用名称匹配
				int x = 0; string[] 内容物 = new string[4];
				foreach (InventorySlot isa in card.CardsInInventory) {
					if (isa.AllCards.Count > 0) {
						foreach (InGameCardBase card1 in isa.AllCards) {
							if (card1 != null) {
								if (card1.ContainedLiquidModel != null) { 内容物[x] = card1.ContainedLiquidModel.CardName; }//液体处理
								else { 内容物[x] = card1.CardModel.CardName; }
								x++;
							}
						}
					}
				}
				//比较产物并产出物品
				Array.Sort(内容物);
				string 原料 = string.Join("|", 内容物);
				Debug.Log("名称判断：" + 原料);
				lianjin_dict.TryGetValue(原料, out CardData 产物);
				if (产物) {
					List<int> exceptList = new List<int>();
					//销毁原材料
					int y = 0;
					foreach (InventorySlot isa in card.CardsInInventory) {
						if (isa.AllCards.Count > 0) {
							foreach (InGameCardBase card1 in isa.AllCards) {
								if (card1 != null) {
									if (card1.ContainedLiquidModel != null) {  //对于液体的处理
										card1.ContainedLiquid.CurrentLiquidQuantity -= 300;
										exceptList.Add(y);
									}
								}
							}
						}
						y++;
					}
					GameManager.Instance.ClearCardInventory(card, true, exceptList);
					//产出对应道具
					CardDrop cd = new CardDrop();
					cd.DroppedCard = 产物;
					cd.Quantity = new Vector2Int((int)产物.SpecialDurability4.MaxValue, (int)产物.SpecialDurability4.MaxValue);
					CardDrop[] cds = new CardDrop[] { cd };
					Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(cds);
				}
				else { //烹饪失败
					CardDrop cd = new CardDrop();
					cd.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
					cd.Quantity = new Vector2Int(1, 1);
					CardDrop[] cds = new CardDrop[] { cd };
					Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(cds);
				}
			}
		}

		public static void 容器合并(CardAction action, InGameCardBase card)
		{
			//读取容器内容物
			if (card?.CardsInInventory.Count > 0) {
				//每有1个内容物就有1个InventorySlot，然后每个InventorySlot里有1个AllCards，仅使用名称匹配
				int x = 0; string[] 内容物 = new string[4];
				float totalCon = 0;
				foreach (InventorySlot isa in card.CardsInInventory) {
					if (isa.AllCards.Count > 0) {
						foreach (InGameCardBase card1 in isa.AllCards) {
							if (card1 != null) {
								totalCon += card1.CardModel.MaxLiquidCapacity;
								x++;
							}
						}
					}
				}
				int level = (int)Math.Floor(totalCon / 300);
				if (level > 0) {
					if (level > 20 ) { level = 20; }
					List<int> exceptList = new List<int>();
					//销毁原材料
					GameManager.Instance.ClearCardInventory(card, true, exceptList);
					//产出对应道具
					CardDrop cd = new CardDrop();
					cd.DroppedCard = cont_dict[level];
					cd.Quantity = new Vector2Int(1,1);
					CardDrop[] cds = new CardDrop[] { cd };
					Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(cds);
				}
				else { //烹饪失败
					CardDrop cd = new CardDrop();
					cd.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
					cd.Quantity = new Vector2Int(1, 1);
					CardDrop[] cds = new CardDrop[] { cd };
					Traverse.Create(action.ProducedCards[0]).Field("DroppedCards").SetValue(cds);
				}
			}
		}

		//==================================容器整合===========================================
		public static CardData 生成容器(string name, string guid, string cardName, string cardDescription, int contain){
			CardData 料理模板 = utc("247cb2c7dc0f4bc0814e91d794900c40");
			CardData liaoli = ScriptableObject.CreateInstance<CardData>();
			liaoli = Instantiate(料理模板);
			liaoli.UniqueID = guid;
			liaoli.name = name;
			liaoli.Init();

			//卡牌描述
			liaoli.CardDescription.DefaultText = cardDescription;
			liaoli.CardDescription.ParentObjectID = guid;
			liaoli.CardDescription.SetLocalizationInfo();

			//卡牌名称
			liaoli.CardName.DefaultText = cardName;
			liaoli.CardName.ParentObjectID = guid;
			liaoli.CardName.SetLocalizationInfo();

			//修改容量
			liaoli.MaxLiquidCapacity = 300 * contain;
			return liaoli;
		}

		public static Dictionary<int, CardData> cont_dict = new Dictionary<int, CardData>();
		//==================================食物拓展兼容=======================================
		public static Dictionary<string,CardTag> foodTagDict = new Dictionary<string,CardTag>();

		public static void 添加Tag2(CardData card, string tagname)
		{
			Array.Resize(ref card.CardTags, card.CardTags.Length + 1);
			if (foodTagDict.ContainsKey(tagname)) {
				card.CardTags[card.CardTags.Length - 1] = foodTagDict[tagname];
			}
			else {
				Debug.Log(tagname + "不存在");
			}

		}
		//=====================================================================================
		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
		public static void SomePatch(){
			//======================添加食谱========================
			List<string> 食谱List = new List<string> { };
			//从txt文件里读取食谱信息加入食谱List
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MoonFood-EX.txt";
			string content = "";
			if (File.Exists(path)) {
				StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
				content = streamReader.ReadLine();
				do {
					if (content.IndexOf("#") > 0) {
						食谱List.Add(content);
					}
					content = streamReader.ReadLine();
				} while (!streamReader.EndOfStream);
				streamReader.Close();
			}
			else {
				Debug.Log("没路径！");
			}
			//对食谱List逐条添加料理
			foreach (string foodinfo2 in 食谱List) {
				string[] foodinfo = foodinfo2.Split('#');
				if (foodinfo.Length >= 8) {
					CardData acard = 生成料理(foodinfo[0], foodinfo[1], foodinfo[2], foodinfo[3], foodinfo[4], foodinfo[5], float.Parse(foodinfo[6]), float.Parse(foodinfo[7]));
					GameLoad.Instance.DataBase.AllData.Add(acard);
				}
			}

			//兼容只吃料理
			GameStat stat = UniqueIDScriptable.GetFromID<GameStat>("f5dfff8bae87494582da87c0394962bb");
			if (stat) {
				//读取所有的cardTag
				CardTag 料理tag = new CardTag();
				CardTag[] CardTags = FindObjectsOfType<CardTag>();
				foreach (CardTag tag in CardTags) {
					if (tag?.name == "tag_YZ_Cuisine") {
						料理tag = tag;
						break;
					}
				}
				foreach (KeyValuePair<string, CardData> kvp in food_dict) {
					CardData card = kvp.Value;
					Array.Resize(ref card.CardTags, card.CardTags.Length + 1);
					card.CardTags[card.CardTags.Length - 1] = 料理tag;
				}
			}


			//配方补充
			配方补充("20eb4938d55244f1bdec102dd1391038", "肉|肉|任意|任意"); //肉丸

			//写入GuideEntry
			string 菜谱记录 = ""; int 换行计数 = 0;
			foreach (KeyValuePair<string, CardData> kvp in food_dict) {
				string key1 = kvp.Key;
				CardData card = kvp.Value;
				string 待加入文本 = card.CardName + "=" + key1;
				if (待加入文本.Length < 18) {
					待加入文本 += 生成空格((18 - 待加入文本.Length));
				}
				菜谱记录 += 待加入文本;
				换行计数++;
				if (换行计数 == 2) {
					菜谱记录 = $"{菜谱记录}\n";
					换行计数 = 0;
				}
			}
			//读取所有GuideEntry
			GuideEntry[] GuideEntrys = FindObjectsOfType<GuideEntry>();
			foreach (GuideEntry ge in GuideEntrys) {
				if (ge.name.IndexOf("高级炉灶") >= 0) {
					ge.OverrideDescription.DefaultText = ge.OverrideDescription.DefaultText + "\n" + 菜谱记录;
					ge.OverrideDescription.SetLocalizationInfo();
				}
			}

			//======================添加植株========================
			//检查精耕细作是否存在，并读取CardTag
			CardData 精耕细作_植株模板 = utc("cb27f80532de40c5a164202ed138090c");
			if (精耕细作_植株模板) {
				//读取所有的cardTag
				CardTag[] CardTags = FindObjectsOfType<CardTag>();
				foreach (CardTag tag in CardTags) {
					if (tag.name.StartsWith("tag_Plant_")) {
						plantTagDict[tag.name] = tag;
					}
				}
		}
			//从txt里读取自定义植株丛并添加
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MoonPlant-EX.txt";
			content = "";
			if (File.Exists(path)) {
				StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
				content = streamReader.ReadLine();
				do {
					if (content.IndexOf("#") > 0) {
						string[] plantinfo = content.Split('#');
						if (plantinfo.Length >= 17) {
							string[] guids = new string[] { plantinfo[1], plantinfo[2], plantinfo[3], };
							string[] cardNames = new string[] { plantinfo[4], plantinfo[5], plantinfo[6], };
							string[] cardDescriptions = new string[] { plantinfo[7], plantinfo[8] };
							CardData[] result = 生成草本植物(plantinfo[0], guids, cardNames, cardDescriptions, plantinfo[9], int.Parse(plantinfo[10]), int.Parse(plantinfo[11]), int.Parse(plantinfo[12]), plantinfo[13] == "是" ? true : false, plantinfo[14], float.Parse(plantinfo[15]), int.Parse(plantinfo[16]));
							GameLoad.Instance.DataBase.AllData.Add(result[0]);
							GameLoad.Instance.DataBase.AllData.Add(result[1]);
							GameLoad.Instance.DataBase.AllData.Add(result[2]);
							//Debug.Log(plantinfo[17]);
							if (plantinfo[17] == "是" && 精耕细作_植株模板) {
								联动精耕细作(result[0], plantinfo[18], plantinfo[19], plantinfo[20], plantinfo[21], plantinfo[22], plantinfo[23],
									plantinfo[24], plantinfo[25], plantinfo[26], plantinfo[27], plantinfo[28], plantinfo[29], plantinfo[30], plantinfo[31]);
						}
					}
					}
					content = streamReader.ReadLine();
				} while (!streamReader.EndOfStream);
				streamReader.Close();
			}
			else {
				Debug.Log("没路径！");
			}

			//======================添加炼金========================
			path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MoonLian-EX.txt";
			content = "";
			if (File.Exists(path)) {
				StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
				content = streamReader.ReadLine();
				do {
					if (content.IndexOf("#") > 0) {
						string[] plantinfo = content.Split('#');
						if (plantinfo.Length >= 6) {
							生成炼金(plantinfo[0], plantinfo[1], plantinfo[2], plantinfo[3], plantinfo[4], float.Parse(plantinfo[5]));
						}
					}
					content = streamReader.ReadLine();
				} while (!streamReader.EndOfStream);
				streamReader.Close();
			}
			else {
				Debug.Log("没路径！");
			}

			//======================添加容器========================
			List<string> list = new List<string>
			{
				"f3dabe549a41cb30557412c0aa6b7fb5",
				"896ba9c09600d1b0352dcd4bbd6a7f26",
				"afceee4790a649c2aaede8f73fde17fe",
				"b5d8a8c2680bc4c09dbee69a91cce3b4",
				"dde9e2cd3a46b9c1c75401bcc51b4a86",
				"e2fe80f32551fba59f5e2ae0f23e1570",
				"d668b88ec81edbceca20061e32522eeb",
				"ea0859bf64aaa2acbfa976c7dbb6fdee",
				"debaacec3fcabd1a836aba45d4dbbb5e",
				"ed3de93ae318cebf2fdfd135dd5fbd7b",
				"ed82d3d1fe41da65ad501482ba8fbc76",
				"bd3e71f2567ace4deb522b1adcb1f03b",
				"37bfd4dd184b7d150caca71bebddaa59",
				"ca4f61bcdaacb95b7864e2af06ad4e29",
				"a020dece6fe58ea8ec2293ab66444a2c",
				"f2061c91df85985fe2b6fdb467e483d7",
				"a234ea39bece159a4a446c5dc8cbd9ce",
				"8d913e0bb51afafb9a89256fe1ff6a6a",
				"1e288952e349c2cd2fb3c29fcefb8d3c",
				"016517aac68ac69860d6040ba50398ba",
			};		
			for (int i = 1; i <= 20;i++) {
				cont_dict[i] =  生成容器("Guil_晓月食物-" + i.ToString() + "级容器", list[i-1], i.ToString() + "级容器","一个特质的容器，目前等级为" + i.ToString(),i);
			}

			/*======================兼容食物拓展=======================
			//判断soup和食物拓展是否存在
			CardData 葱 = utc("53c04be9d8ea407bb9405c8d3ecd41fa");
			CardData 长柄杓 = utc("51797556981e11edbdf850e085c43d2a");
			if (葱 && 长柄杓) {
				//添加所有的CardTag
				CardTag[] CardTags = FindObjectsOfType<CardTag>();
				foreach (CardTag tag in CardTags) {
					if (tag.name != null) {
						if (tag.name != "") {
							foodTagDict[tag.name] = tag;
						}
					}
				}
				//遍历所有GpTag
				CardTabGroup[] CardTabGroups = FindObjectsOfType<CardTabGroup>();
				foreach (CardTabGroup tag in CardTabGroups) {
					if (tag.TabName != null) {
						switch (tag.TabName.DefaultText) {
						
							TODO:  If this area is uncommented, it must be translated.
							case "调料品":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Condiment");
									}
								}
								break;
							case "鸟类尸体":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_DeadBird");
									}
								}
								break;
							case "鱼类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Fish");
									}
								}
								break;
							case "粉类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Flour");
									}
								}
								break;
							case "花类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Flower");
									}
								}
								break;
							case "水果类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Fruit");
									}
								}
								break;
							case "肉类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Meats");
									}
								}
								break;
							case "小动物肉":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Meats");
									}
								}
								break;
							case "奶类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Milk");
									}
								}
								break;
							case "怪物肉类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Meats");
									}
								}
								break;
							case "蘑菇类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_MushroomM");
									}
								}
								break;
							case "海鲜类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Seafood");
									}
								}
								break;
							case "蔬菜类":
								foreach (CardData card in tag.IncludedCards) {
									if (card.name.StartsWith("FoodExpansion")) {
										添加Tag2(card, "tag_Vegetable");
									}
								}
								break;
							default: {
									break;
								}
						}
					}
				}
			}
			*/
		}


		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameManager), "ActionRoutine")]
		public static IEnumerator ARPatch(IEnumerator results, CardAction _Action, InGameCardBase _ReceivingCard, bool _FastMode, bool _ModifiersAlreadyCollected = false)
		{
			if (_Action.ActionName == "高级烹饪" && _Action.ActionName.LocalizationKey == "GuilPot") {
				//Debug.Log("开始判断");
				//每有1个内容物就有1个InventorySlot，然后每个InventorySlot里有1个AllCards，目前实现名称比对，然后需要实现tag比对
				if (_ReceivingCard?.CardsInInventory.Count > 0) {
					int x = 0;
					string[] 内容物 = new string[4];
					string[] 内容物标签 = new string[4];
					string[] 内容物混合 = new string[4];
					foreach (InventorySlot isa in _ReceivingCard.CardsInInventory) {
						if (isa.AllCards.Count > 0) {
							foreach (InGameCardBase card1 in isa.AllCards) {
								if (card1 != null) {
									if (card1.ContainedLiquidModel != null) {//对于液体的处理
																			 //UnityEngine.Debug.Log(card1.ContainedLiquidModel.CardName);
										内容物[x] = card1.ContainedLiquidModel.CardName;
										内容物标签[x] = 标签计算(card1.ContainedLiquid);
										内容物混合[x] = 混合计算(card1.ContainedLiquid);
									}
									else {
										//UnityEngine.Debug.Log(card1.CardModel.CardName);
										内容物[x] = card1.CardModel.CardName;
										内容物标签[x] = 标签计算(card1);
										内容物混合[x] = 混合计算(card1);
									}
									x++;
								}
							}
						}
					}

					//比较产物并产出物品
					Array.Sort(内容物);
					Array.Sort(内容物标签);
					Array.Sort(内容物混合);
					string 原料 = string.Join("|", 内容物);
					string 原料2 = string.Join("|", 内容物混合);
					string 原料3 = string.Join("|", 内容物标签);
					Debug.Log("名称判断：" + 原料);
					Debug.Log("混合判断：" + 原料2);
					Debug.Log("标签判断：" + 原料3);
					//Debug.Log(原料);
					CardData 产物 = null;
					if (!food_dict.TryGetValue(原料, out 产物)) {
						if (!food_dict.TryGetValue(原料2, out 产物)) {
							food_dict.TryGetValue(原料3, out 产物);
						}
					}
					if (产物) {  //烹饪成功							

						List<int> exceptList = new List<int>();
						//销毁原材料
						int y = 0;
						foreach (InventorySlot isa in _ReceivingCard.CardsInInventory) {
							if (isa.AllCards.Count > 0) {
								foreach (InGameCardBase card1 in isa.AllCards) {
									if (card1 != null) {
										if (card1.ContainedLiquidModel != null) {  //对于液体的处理
											card1.ContainedLiquid.CurrentLiquidQuantity -= 300;
											exceptList.Add(y);
										}
									}
						}
							}
							y++;
						}
						GameManager.Instance.ClearCardInventory(_ReceivingCard, true, exceptList);
						//产出对应道具
						CardDrop cd = new CardDrop();
						cd.DroppedCard = 产物;
						cd.Quantity = new Vector2Int(1, 1);
						CardDrop[] cds = new CardDrop[] { cd };
						Traverse.Create(_Action.ProducedCards[0]).Field("DroppedCards").SetValue(cds);

						_Action.StatModifications[0].ValueModifier = new Vector2(1f, 1f);
					}
					else { //烹饪失败
						CardDrop cd = new CardDrop();
						cd.DroppedCard = UniqueIDScriptable.GetFromID<CardData>("63c8683151734206bc22ebb75994dc20");
						cd.Quantity = new Vector2Int(1, 1);
						CardDrop[] cds = new CardDrop[] { cd };
						Traverse.Create(_Action.ProducedCards[0]).Field("DroppedCards").SetValue(cds);
						_Action.StatModifications[0].ValueModifier = new Vector2(0f, 0f);
					}
			}
		}
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
			while (results.MoveNext()) {
				yield return results.Current;
			}
		}
	}
}
