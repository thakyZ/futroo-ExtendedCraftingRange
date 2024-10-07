#nullable enable
using PugMod;
using System.Linq;

namespace Sailie.ExtendedCraftingRange
{
  public class ExtendedCraftingRange : IMod
  {
    private static ExtendedCraftingRange instance = null!;

    public const string VERSION = "1.0.2";
    public const string NAME = "ExtendedCraftingRange";
    public const string AUTHOR = "Sailie";

    private LoadedMod? modInfo;

    private ModConfig? config;

    internal static ModConfig? Config => instance?.config;

    public void EarlyInit()
    {
      instance = this;
      modInfo = API.ModLoader.LoadedMods.FirstOrDefault(obj => obj.Handlers.Contains(this));
      Logger.Init(modInfo?.Metadata.name ?? NAME);
      if (modInfo is null)
      {
        Logger.Error($"Failed to load {NAME}!");
        return;
      }
      Logger.Info($"Version: {VERSION}");
      config = new ModConfig(modInfo.Metadata.name);
      Logger.Info("Mod loaded successfully!");
      API.ModLoader.ApplyHarmonyPatch(modInfo.ModId, typeof(CraftingHandler_Patch));
    }

    public void Init() {}

    public void Shutdown() {}

    public void ModObjectLoaded(UnityEngine.Object obj) {}

    public void Update() {}
  }
}
