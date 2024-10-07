#nullable enable
using CoreLib.Data.Configuration;

namespace Sailie.ExtendedCraftingRange
{
  public class ModConfig
  {
    public ConfigEntry<int> NearbyDistance { get; }
    public ModConfig(string modName)
    {
      ConfigFile file = new($"{modName}/config.cfg", true);
      NearbyDistance = file.Bind("General", nameof(NearbyDistance), 7);
    }
  }
}
