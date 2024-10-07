#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using HarmonyLib;
using System.Linq;
using Unity.Entities;

// ReSharper disable once CheckNamespace
namespace Sailie.ExtendedCraftingRange
{
  [SuppressMessage("CodeQuality", "IDE0079")]
  [SuppressMessage("CodeQuality", "RCS1102")]
  [SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "HarmonyLib Naming Sceme")]
  public class CraftingHandler_Patch
  {
    [HarmonyPrefix, HarmonyPatch(typeof(CraftingHandler), nameof(CraftingHandler.GetAnyNearbyChests), new Type[] {})]
    public static bool GetAnyNearbyChests_Prefix(CraftingHandler __instance, ref List<Chest> __result, ref EntityMonoBehaviour ___entityMonoBehaviour)
    {
      var distance = ExtendedCraftingRange.Config?.NearbyDistance.Value ?? 7;
      EntityUtility.GetPrefabSizeAndOffset(___entityMonoBehaviour.entity, ___entityMonoBehaviour.objectInfo, out Vector2Int size, out Vector2Int offset);
      size += offset;
      var playerPosition = ___entityMonoBehaviour.WorldPosition.RoundToInt2();
      __result = Chest.chestsAvailableForCraftingMaterials
        .Where(pair => pair.Key.x - playerPosition.x >= -distance + offset.x
                    && pair.Key.x - playerPosition.x < size.x + distance
                    && pair.Key.y - playerPosition.y >= -distance + offset.y
                    && pair.Key.y - playerPosition.y < size.y + distance
                    && !pair.Value.inventoryHandler.IsEmpty())
        .Select(pair => pair.Value).Take(8).ToList();
      return false;
    }
    /*
    private static MethodInfo Vector2Int_get_x = AccessTools.PropertyGetter(typeof(Vector2Int), nameof(Vector2Int.x));
    private static MethodInfo Vector2Int_get_y = AccessTools.PropertyGetter(typeof(Vector2Int), nameof(Vector2Int.y));

    [HarmonyTranspiler, HarmonyPatch(typeof(CraftingHandler), nameof(CraftingHandler.GetAnyNearbyChests), new Type[] {})]
    public static IEnumerable<CodeInstruction> GetAnyNearbyChests_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
      var found1  = false; // found: IL_003E
      var found2  = false; // found: IL_00F2
      var found3  = false; // found: IL_004E
      var found4  = false; // found: IL_00DC
      var found5  = false; // found: IL_005B
      var found6  = false; // found: IL_005D
      var found7  = false; // found: IL_005E
      var found8  = false; // found: IL_0060
      var found9  = false; // found: IL_0069
      var found10 = false; // found: IL_006B
      var found11 = false; // found: IL_006D
      var found12 = false; // found: IL_006E
      var found13 = false; // found: IL_0070
      var found14 = false; // found: IL_0079
      // if (i == -1 || i == size.x || j == -1 || j == size.y)
      // if (i < offset.x || i >= size.x || j < offset.y || j >= size.y) {
      var instructionsCopy = instructions.ToList();
      for (playerPosition i = 0; i < instructionsCopy.Count; i++) {
        var instruction = instructionsCopy[i];
        if (!found1 && instruction.Is(OpCodes.Ldc_I4_M1, null) && instructionsCopy[i + 2].Calls(Vector2Int_get_x)) {
          found1 = true;
          yield return new CodeInstruction(OpCodes.Ldc_I4_S, -1 * 7);
        } else if (!found2 && instruction.Is(OpCodes.Ldc_I4_M1, null) && instructionsCopy[i + 2].Calls(Vector2Int_get_y)) {
          found2 = true;
          yield return new CodeInstruction(OpCodes.Ldc_I4_S, -1 * 7);
        } else if (!found3 && instruction.Is(OpCodes.Ldc_I4_1, null) && instructionsCopy[i - 1].Calls(Vector2Int_get_y)) {
          found3 = true;
          yield return new CodeInstruction(OpCodes.Ldc_I4_S, 7);
        } else if (!found4 && instruction.Is(OpCodes.Ldc_I4_1, null) && instructionsCopy[i - 1].Calls(Vector2Int_get_x)) {
          found4 = true;
          yield return new CodeInstruction(OpCodes.Ldc_I4_S, 7);
        } else if (!found5 && instructionsCopy[i + 1].Is(OpCodes.Ldloc_S, 4)) {
          found5 = true;
        } else if (found5 && !found6 && instruction.Is(OpCodes.Ldc_I4_M1, null)) {
          yield return new CodeInstruction(OpCodes.Ldloc_S, 2);
          yield return new CodeInstruction(OpCodes.Call, Vector2Int_get_x);
          found6 = true;
        } else if (found6 && !found7 && instruction.opcode == OpCodes.Beq_S) {
          yield return new CodeInstruction(OpCodes.Blt_S, instruction.operand);
          found7 = true;
          if (!found8 && instructionsCopy[i + 1].Is(OpCodes.Ldloc_S, 4)) {
            found8 = true;
          }
        } else if (found8 && !found9 && instruction.opcode == OpCodes.Beq_S) {
          yield return new CodeInstruction(OpCodes.Bge_S, instruction.operand);
          found9 = true;
          if (!found10 && instructionsCopy[i + 1].Is(OpCodes.Ldloc_S, 5)) {
            found10 = true;
          }
        } else if (found10 && !found11 && instruction.Is(OpCodes.Ldc_I4_M1, null)) {
          yield return new CodeInstruction(OpCodes.Ldloc_S, 2);
          yield return new CodeInstruction(OpCodes.Call, Vector2Int_get_x);
          found11 = true;
        } else if (found11 && !found12 && instruction.opcode == OpCodes.Beq_S) {
          yield return new CodeInstruction(OpCodes.Blt_S, instruction.operand);
          found12 = true;
          if (!found13 && instructionsCopy[i + 1].Is(OpCodes.Ldloc_S, 5)) {
            found13 = true;
          }
        } else if (found13 && !found14 && instruction.opcode == OpCodes.Bne_Un_S) {
          yield return new CodeInstruction(OpCodes.Bge_Un_S, instruction.operand);
          found14 = true;
        } else {
          yield return instruction;
        }
      }

      if (!found1)
        Logger.Warn("Did not find IL Code IL_003E");
      if (!found2)
        Logger.Warn("Did not find IL Code IL_00F2");
      if (!found3)
        Logger.Warn("Did not find IL Code IL_004E");
      if (!found4)
        Logger.Warn("Did not find IL Code IL_00DC");
      if (!found5)
        Logger.Warn("Did not find IL Code IL_005B");
      if (!found6)
        Logger.Warn("Did not find IL Code IL_005D");
      if (!found7)
        Logger.Warn("Did not find IL Code IL_005E");
      if (!found8)
        Logger.Warn("Did not find IL Code IL_0060");
      if (!found9)
        Logger.Warn("Did not find IL Code IL_0069");
      if (!found10)
        Logger.Warn("Did not find IL Code IL_006B");
      if (!found11)
        Logger.Warn("Did not find IL Code IL_006D");
      if (!found12)
        Logger.Warn("Did not find IL Code IL_006E");
      if (!found13)
        Logger.Warn("Did not find IL Code IL_0070");
      if (!found14)
        Logger.Warn("Did not find IL Code IL_0079");
    }
    */
  }

  //     List<Chest> list = new List<Chest>();
  /* 0x00138B78 73D421000A   */ // IL_0000: newobj    instance void class [netstandard]System.Collections.Generic.List`1<class Chest>::.ctor()
  /* 0x00138B7D 0A           */ // IL_0005: stloc.0
  //     EntityUtility.GetPrefabSizeAndOffset(this.entityMonoBehaviour.entity, this.entityMonoBehaviour.objectInfo, out size, out offset);
  /* 0x00138B7E 02           */ // IL_0006: ldarg.0
  /* 0x00138B7F 7BB9360004   */ // IL_0007: ldfld     class EntityMonoBehaviour CraftingHandler::entityMonoBehaviour
  /* 0x00138B84 6FA64C0006   */ // IL_000C: callvirt  instance valuetype [Unity.Entities]Unity.Entities.Entity EntityMonoBehaviour::get_entity()
  /* 0x00138B89 02           */ // IL_0011: ldarg.0
  /* 0x00138B8A 7BB9360004   */ // IL_0012: ldfld     class EntityMonoBehaviour CraftingHandler::entityMonoBehaviour
  /* 0x00138B8F 6FA84C0006   */ // IL_0017: callvirt  instance class [Pug.Base]ObjectInfo EntityMonoBehaviour::get_objectInfo()
  /* 0x00138B94 1201         */ // IL_001C: ldloca.s  V_1
  /* 0x00138B96 1202         */ // IL_001E: ldloca.s  V_2
  /* 0x00138B98 2881470006   */ // IL_0020: call      void EntityUtility::GetPrefabSizeAndOffset(valuetype [Unity.Entities]Unity.Entities.Entity, class [Pug.Base]ObjectInfo, valuetype [UnityEngine.CoreModule]UnityEngine.Vector2Int&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector2Int&)
  //     size += offset;
  /* 0x00138B9D 07           */ // IL_0025: ldloc.1
  /* 0x00138B9E 08           */ // IL_0026: ldloc.2
  /* 0x00138B9F 28DB1C000A   */ // IL_0027: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector2Int [UnityEngine.CoreModule]UnityEngine.Vector2Int::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector2Int, valuetype [UnityEngine.CoreModule]UnityEngine.Vector2Int)
  /* 0x00138BA4 0B           */ // IL_002C: stloc.1
  //     playerOffset @playerPosition = this.entityMonoBehaviour.WorldPosition.RoundToInt2();
  /* 0x00138BA5 02           */ // IL_002D: ldarg.0
  /* 0x00138BA6 7BB9360004   */ // IL_002E: ldfld     class EntityMonoBehaviour CraftingHandler::entityMonoBehaviour
  /* 0x00138BAB 6FC34C0006   */ // IL_0033: callvirt  instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EntityMonoBehaviour::get_WorldPosition()
  /* 0x00138BB0 282F00000A   */ // IL_0038: call      valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset [Pug.UnityExtensions]ExtensionMethods::RoundToInt2(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
  /* 0x00138BB5 0D           */ // IL_003D: stloc.3
  //     for (playerPosition i = -1 + offset.x; i < size.x + 1; i++)
  //          ^^^^^^^^^^^^^^^^^^^^^^^^^^
  /* 0x00138BB6 15           */ // IL_003E: ldc.i4.m1
  /* 0x00138BB7 1202         */ // IL_003F: ldloca.s  V_2
  /* 0x00138BB9 28F90F000A   */ // IL_0041: call      instance int32 [UnityEngine.CoreModule]UnityEngine.Vector2Int::get_x()
  /* 0x00138BBE 58           */ // IL_0046: add
  /* 0x00138BBF 1304         */ // IL_0047: stloc.s   V_4
  /* 0x00138BC1 389B000000   */ // IL_0049: br        il_00E9
  // loop start (head: il_00E9)
  //         for (playerPosition j = -1 + offset.y; j < size.y + 1; j++)
  //              ^^^^^^^^^^^^^^^^^^^^^^^^^^
  /* 0x00138BC6 15           */ // IL_004E: ldc.i4.m1
  /* 0x00138BC7 1202         */ // IL_004F: ldloca.s  V_2
  /* 0x00138BC9 28F80F000A   */ // IL_0051: call      instance int32 [UnityEngine.CoreModule]UnityEngine.Vector2Int::get_y()
  /* 0x00138BCE 58           */ // IL_0056: add
  /* 0x00138BCF 1305         */ // IL_0057: stloc.s   V_5
  /* 0x00138BD1 2B78         */ // IL_0059: br.s      il_00D3
  // loop start (head: il_00D3)
  //             if (i == -1 || i == size.x || j == -1 || j == size.y)
  /* 0x00138BD3 1104         */ // IL_005B: ldloc.s   V_4
  /* 0x00138BD5 15           */ // IL_005D: ldc.i4.m1
  /* 0x00138BD6 2E1B         */ // IL_005E: beq.s     il_007B
  /* 0x00138BD8 1104         */ // IL_0060: ldloc.s   V_4
  /* 0x00138BDA 1201         */ // IL_0062: ldloca.s  V_1
  /* 0x00138BDC 28F90F000A   */ // IL_0064: call      instance int32 [UnityEngine.CoreModule]UnityEngine.Vector2Int::get_x()
  /* 0x00138BE1 2E10         */ // IL_0069: beq.s     il_007B
  /* 0x00138BE3 1105         */ // IL_006B: ldloc.s   V_5
  /* 0x00138BE5 15           */ // IL_006D: ldc.i4.m1
  /* 0x00138BE6 2E0B         */ // IL_006E: beq.s     il_007B
  /* 0x00138BE8 1105         */ // IL_0070: ldloc.s   V_5
  /* 0x00138BEA 1201         */ // IL_0072: ldloca.s  V_1
  /* 0x00138BEC 28F80F000A   */ // IL_0074: call      instance int32 [UnityEngine.CoreModule]UnityEngine.Vector2Int::get_y()
  /* 0x00138BF1 3352         */ // IL_0079: bne.un.s  il_00CD
  //                 playerOffset playerOffset = @playerPosition + new playerOffset(i, j);
  /* 0x00138BF3 09           */ // IL_007B: ldloc.3
  /* 0x00138BF4 1104         */ // IL_007C: ldloc.s   V_4
  /* 0x00138BF6 1105         */ // IL_007E: ldloc.s   V_5
  /* 0x00138BF8 735900000A   */ // IL_0080: newobj    instance void [Unity.Mathematics]Unity.Mathematics.playerOffset::.ctor(int32, int32)
  /* 0x00138BFD 285D00000A   */ // IL_0085: call      valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset [Unity.Mathematics]Unity.Mathematics.playerOffset::op_Addition(valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset, valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset)
  /* 0x00138C02 1306         */ // IL_008A: stloc.s   V_6
  //                 if (Chest.chestsAvailableForCraftingMaterials.ContainsKey(playerOffset))
  /* 0x00138C04 7ED5380004   */ // IL_008C: ldsfld    class [netstandard]System.Collections.Generic.Dictionary`2<valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset, class Chest> Chest::chestsAvailableForCraftingMaterials
  /* 0x00138C09 1106         */ // IL_0091: ldloc.s   V_6
  /* 0x00138C0B 6FD521000A   */ // IL_0093: callvirt  instance bool class [netstandard]System.Collections.Generic.Dictionary`2<valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset, class Chest>::ContainsKey(!0)
  /* 0x00138C10 2C33         */ // IL_0098: brfalse.s il_00CD
  //                     Chest chest = Chest.chestsAvailableForCraftingMaterials[playerOffset];
  /* 0x00138C12 7ED5380004   */ // IL_009A: ldsfld    class [netstandard]System.Collections.Generic.Dictionary`2<valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset, class Chest> Chest::chestsAvailableForCraftingMaterials
  /* 0x00138C17 1106         */ // IL_009F: ldloc.s   V_6
  /* 0x00138C19 6FD621000A   */ // IL_00A1: callvirt  instance !1 class [netstandard]System.Collections.Generic.Dictionary`2<valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset, class Chest>::get_Item(!0)
  /* 0x00138C1E 1307         */ // IL_00A6: stloc.s   V_7
  //                     if (!list.Contains(chest) && chest.entityExist)
  /* 0x00138C20 06           */ // IL_00A8: ldloc.0
  /* 0x00138C21 1107         */ // IL_00A9: ldloc.s   V_7
  /* 0x00138C23 6FD721000A   */ // IL_00AB: callvirt  instance bool class [netstandard]System.Collections.Generic.List`1<class Chest>::Contains(!0)
  /* 0x00138C28 2D1B         */ // IL_00B0: brtrue.s  il_00CD
  /* 0x00138C2A 1107         */ // IL_00B2: ldloc.s   V_7
  /* 0x00138C2C 6FC14C0006   */ // IL_00B4: callvirt  instance bool EntityMonoBehaviour::get_entityExist()
  /* 0x00138C31 2C12         */ // IL_00B9: brfalse.s il_00CD
  //                         list.Add(Chest.chestsAvailableForCraftingMaterials[playerOffset]);
  /* 0x00138C33 06           */ // IL_00BB: ldloc.0
  /* 0x00138C34 7ED5380004   */ // IL_00BC: ldsfld    class [netstandard]System.Collections.Generic.Dictionary`2<valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset, class Chest> Chest::chestsAvailableForCraftingMaterials
  /* 0x00138C39 1106         */ // IL_00C1: ldloc.s   V_6
  /* 0x00138C3B 6FD621000A   */ // IL_00C3: callvirt  instance !1 class [netstandard]System.Collections.Generic.Dictionary`2<valuetype [Unity.Mathematics]Unity.Mathematics.playerOffset, class Chest>::get_Item(!0)
  /* 0x00138C40 6FD821000A   */ // IL_00C8: callvirt  instance void class [netstandard]System.Collections.Generic.List`1<class Chest>::Add(!0)
  //         for (playerPosition j = -1 + offset.y; j < size.y + 1; j++)
  //                                                                ^^^
  /* 0x00138C45 1105         */ // IL_00CD: ldloc.s   V_5
  /* 0x00138C47 17           */ // IL_00CF: ldc.i4.1
  /* 0x00138C48 58           */ // IL_00D0: add
  /* 0x00138C49 1305         */ // IL_00D1: stloc.s   V_5
  //         for (playerPosition j = -1 + offset.y; j < size.y + 1; j++)
  //                                          ^^^^^^^^^^^^^^^^^^^^
  /* 0x00138C4B 1105         */ // IL_00D3: ldloc.s   V_5
  /* 0x00138C4D 1201         */ // IL_00D5: ldloca.s  V_1
  /* 0x00138C4F 28F80F000A   */ // IL_00D7: call      instance int32 [UnityEngine.CoreModule]UnityEngine.Vector2Int::get_y()
  /* 0x00138C54 17           */ // IL_00DC: ldc.i4.1
  /* 0x00138C55 58           */ // IL_00DD: add
  /* 0x00138C56 3F78FFFFFF   */ // IL_00DE: blt       il_005B
  // end loop
  //     for (playerPosition i = -1 + offset.x; i < size.x + 1; i++)
  //                                                            ^^^
  /* 0x00138C5B 1104         */ // IL_00E3: ldloc.s   V_4
  /* 0x00138C5D 17           */ // IL_00E5: ldc.i4.1
  /* 0x00138C5E 58           */ // IL_00E6: add
  /* 0x00138C5F 1304         */ // IL_00E7: stloc.s   V_4
  //     for (playerPosition i = -1 + offset.x; i < size.x + 1; i++)
  //                                      ^^^^^^^^^^^^^^^^^^^^
  /* 0x00138C61 1104         */ // IL_00E9: ldloc.s   V_4
  /* 0x00138C63 1201         */ // IL_00EB: ldloca.s  V_1
  /* 0x00138C65 28F90F000A   */ // IL_00ED: call      instance int32 [UnityEngine.CoreModule]UnityEngine.Vector2Int::get_x()
  /* 0x00138C6A 17           */ // IL_00F2: ldc.i4.1
  /* 0x00138C6B 58           */ // IL_00F3: add
  /* 0x00138C6C 3F55FFFFFF   */ // IL_00F4: blt       il_004E
  // end loop
  //     return list;
  /* 0x00138C71 06           */ // IL_00F9: ldloc.0
  /* 0x00138C72 2A           */ // IL_00FA: ret
}
