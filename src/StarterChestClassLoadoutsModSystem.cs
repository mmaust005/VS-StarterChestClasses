using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace StarterChestClassLoadouts
{
	public class StarterChestClassLoadoutsModSystem : ModSystem
	{
		const string ClassLoadoutsDirName = "StarterChestClasses";
		const string PackagedClassLoadoutsPath = "config/classes/";

		ICoreServerAPI sapi;
		readonly Dictionary<string, StarterChest.StarterChestLoadout> classLoadouts = new Dictionary<string, StarterChest.StarterChestLoadout>();

		public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;

		public override void StartServerSide(ICoreServerAPI api)
		{
			sapi = api;
			LoadClassLoadouts();

			StarterChest.StarterChestModSystem starterChest = sapi.ModLoader.GetModSystem<StarterChest.StarterChestModSystem>();
			if (starterChest == null)
			{
				// modinfo.json declares a hard dependency on "starterchest", so the mod loader
				// should never actually let this run without it - this is just a defensive check.
				sapi.Logger.Error("[StarterChestClasses] Could not find the Starter Chest mod - this addon has no effect without it.");
				return;
			}

			starterChest.RegisterLoadoutProvider(ProvideLoadout, IsReady);
		}

		// Loadouts live one-per-file under ModConfig/StarterChestClasses/, named "<classcode>.json"
		// (e.g. "hunter.json") - so a mod author or community member adding support for a new class
		// is just one file to drop in, and a server with many class mods installed doesn't end up
		// with one unwieldy config file. The folder is seeded once from the packaged vanilla class
		// defaults and never touched again afterwards, same as the base mod's own config; anything
		// added later (built-in or drag-and-dropped) is picked up on every server start by scanning
		// whatever's actually in the folder.
		void LoadClassLoadouts()
		{
			string classDir = Path.Combine(sapi.GetOrCreateDataPath("ModConfig"), ClassLoadoutsDirName);

			if (!Directory.Exists(classDir))
			{
				Directory.CreateDirectory(classDir);
				SeedPackagedClassLoadouts(classDir);
			}

			classLoadouts.Clear();
			foreach (string filePath in Directory.GetFiles(classDir, "*.json"))
			{
				string classCode = Path.GetFileNameWithoutExtension(filePath).ToLowerInvariant();
				try
				{
					var loadout = JsonConvert.DeserializeObject<StarterChest.StarterChestLoadout>(File.ReadAllText(filePath));
					if (loadout != null) classLoadouts[classCode] = loadout;
				}
				catch (Exception e)
				{
					sapi.Logger.Error("[StarterChestClasses] Failed to parse class loadout '{0}': {1}. Skipping this file.", filePath, e.Message);
				}
			}

			sapi.Logger.Notification("[StarterChestClasses] Loaded {0} class loadout(s) from '{1}'.", classLoadouts.Count, classDir);
		}

		void SeedPackagedClassLoadouts(string classDir)
		{
			List<IAsset> assets = sapi.Assets.GetMany(PackagedClassLoadoutsPath, "starterchestclasses");
			foreach (IAsset asset in assets)
			{
				string destPath = Path.Combine(classDir, Path.GetFileName(asset.Location.Path));
				try
				{
					File.WriteAllBytes(destPath, asset.Data);
				}
				catch (Exception e)
				{
					sapi.Logger.Error("[StarterChestClasses] Failed to seed class loadout '{0}': {1}", destPath, e.Message);
				}
			}
		}

		// The character class the player picked at creation, e.g. "hunter" - null if unset.
		static string GetClassCode(IServerPlayer player) => player.Entity.WatchedAttributes.GetString("characterClass", null);

		// The vanilla CharacterSystem assigns a default class (the first entry, "commoner")
		// immediately on character creation, well before the player's real pick is submitted, so
		// characterClass is never actually empty/null and can't be used as a "not ready yet" signal
		// on its own. "createCharacter" is the player mod-data flag CharacterSystem itself sets to
		// true exactly when the selection dialog is submitted (see CharacterSystem.onCharacterSelection),
		// so that's the precise signal to wait for instead.
		bool IsReady(IServerPlayer player) => player.GetModData("createCharacter", false);

		StarterChest.StarterChestLoadoutResult ProvideLoadout(IServerPlayer player)
		{
			string classCode = GetClassCode(player);
			if (string.IsNullOrEmpty(classCode)) return null;
			if (!classLoadouts.TryGetValue(classCode.ToLowerInvariant(), out StarterChest.StarterChestLoadout loadout)) return null;

			return new StarterChest.StarterChestLoadoutResult
			{
				Loadout = loadout,
				DisplayName = ResolveClassDisplayName(player.LanguageCode, classCode),
			};
		}

		// Vanilla's own "characterclass-<code>" lang key gives a properly localized, capitalized
		// class name (e.g. "Hunter"). Falls back to the raw class code if that key is missing (e.g.
		// a class added by another mod that doesn't ship a translation for it).
		static string ResolveClassDisplayName(string langCode, string classCode)
		{
			string key = "characterclass-" + classCode;
			string resolved = Lang.GetL(langCode, key);
			return resolved == key ? classCode : resolved;
		}
	}
}
