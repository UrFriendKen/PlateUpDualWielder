using HarmonyLib;
using Kitchen;
using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Entities;
using UnityEngine;

namespace KitchenDualWielder.Patches
{
    [HarmonyPatch]
    static class UpdateTakesDuration_Patch
    {
        static readonly Type TARGET_TYPE = typeof(UpdateTakesDuration);
        const bool IS_ORIGINAL_LAMBDA_BODY = true;
        const int LAMBDA_BODY_INDEX = 1;
        const string TARGET_METHOD_NAME = "";
        const string DESCRIPTION = "Modify duration tool factor calculation"; // Logging purpose of patch

        const int EXPECTED_MATCH_COUNT = 1;

        static readonly List<OpCode> OPCODES_TO_MATCH = new List<OpCode>()
        {
            OpCodes.Ldarg_0,
            OpCodes.Ldfld,
            OpCodes.Ldloc_S,
            OpCodes.Ldfld,
            OpCodes.Ldloca_S,
            OpCodes.Call,
            OpCodes.Brfalse,
            OpCodes.Ldarg_0,
            OpCodes.Ldfld,
            OpCodes.Ldloc_S,
            OpCodes.Ldfld,
            OpCodes.Ldloca_S,
            OpCodes.Call,
            OpCodes.Brfalse,
            OpCodes.Ldloc_S,
            OpCodes.Ldfld,
            OpCodes.Ldarg_2,
            OpCodes.Ldfld,
            OpCodes.Bne_Un,
            OpCodes.Ldloc_1,
            OpCodes.Ldloc_S,
            OpCodes.Ldfld,
            OpCodes.Ldc_R4,
            OpCodes.Sub,
            OpCodes.Add,
            OpCodes.Stloc_1
        };

        // null is ignore
        static readonly List<object> OPERANDS_TO_MATCH = new List<object>()
        {
        };

        static readonly List<OpCode> MODIFIED_OPCODES = new List<OpCode>()
        {
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Ldloc_S,
            OpCodes.Ldarg_2,
            OpCodes.Ldfld,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Ldloca_S,
            OpCodes.Call,
            OpCodes.Brfalse,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Nop,
            OpCodes.Ldloc_1,
            OpCodes.Ldloc_S,
            OpCodes.Ldfld,
            OpCodes.Ldc_R4,
            OpCodes.Sub,
            OpCodes.Add,
            OpCodes.Stloc_1
        };

        // null is ignore
        static readonly List<object> MODIFIED_OPERANDS = new List<object>()
        {
            null,
            null,
            null,
            null,
            typeof(CTakesDuration).GetField("RelevantTool", BindingFlags.Public | BindingFlags.Instance),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            typeof(UpdateTakesDuration_Patch).GetMethod("TryGetFactor", BindingFlags.NonPublic | BindingFlags.Static)
        };

        static bool TryGetFactor(CBeingActedOnBy beingActedOnBy, DurationToolType relevantToolType, out CDurationTool tempDurationTool)
        {
            bool hasRelevantTool = false;
            tempDurationTool = default;
            float factor = 1f;
            Entity interactor = beingActedOnBy.Interactor;
            Main.LogInfo($"interactor.Index = {interactor.Index}");
            if (PatchController.StaticRequire(interactor, out CToolUser toolUser) &&
                PatchController.StaticRequire(toolUser.CurrentTool, out CDurationTool durationTool1) &&
                durationTool1.Type == relevantToolType)
            {
                Main.LogInfo("Tool1");
                hasRelevantTool = true; 
                Main.LogWarning(durationTool1.Factor);
                factor *= durationTool1.Factor;
            }
            if (PatchController.StaticRequire(interactor, out CToolUserSecondHand toolUserSecondHand) &&
                PatchController.StaticRequire(toolUserSecondHand.CurrentTool, out CDurationTool durationTool2) &&
                durationTool2.Type == relevantToolType)
            {
                Main.LogInfo("Tool2");
                hasRelevantTool = true;
                Main.LogWarning(durationTool2.Factor);
                factor *= durationTool2.Factor;
            }
            if (hasRelevantTool)
            {
                tempDurationTool = new CDurationTool()
                {
                    Factor = factor
                };
                Main.LogInfo($"Resulting Factor: {factor}");
            }
            return hasRelevantTool;
        }

        public static MethodBase TargetMethod()
        {
            Type type = IS_ORIGINAL_LAMBDA_BODY ? AccessTools.FirstInner(TARGET_TYPE, t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob{LAMBDA_BODY_INDEX}")) : TARGET_TYPE;
            return AccessTools.FirstMethod(type, method => method.Name.Contains(IS_ORIGINAL_LAMBDA_BODY ? "OriginalLambdaBody" : TARGET_METHOD_NAME));
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> OriginalLambdaBody_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Main.LogInfo($"{TARGET_TYPE.Name} Transpiler");
            if (!(DESCRIPTION == null || DESCRIPTION == string.Empty))
                Main.LogInfo(DESCRIPTION);
            List<CodeInstruction> list = instructions.ToList();

            int matches = 0;
            int windowSize = OPCODES_TO_MATCH.Count;
            for (int i = 0; i < list.Count - windowSize; i++)
            {
                for (int j = 0; j < windowSize; j++)
                {
                    if (OPCODES_TO_MATCH[j] == null)
                    {
                        Main.LogError("OPCODES_TO_MATCH cannot contain null!");
                        return instructions;
                    }

                    string logLine = $"{j}:\t{OPCODES_TO_MATCH[j]}";

                    int index = i + j;
                    OpCode opCode = list[index].opcode;
                    if (j < OPCODES_TO_MATCH.Count && opCode != OPCODES_TO_MATCH[j])
                    {
                        if (j > 0)
                        {
                            logLine += $" != {opCode}";
                            Main.LogInfo($"{logLine}\tFAIL");
                        }
                        break;
                    }
                    logLine += $" == {opCode}";

                    if (j == 0)
                        Debug.Log("-------------------------");

                    if (j < OPERANDS_TO_MATCH.Count && OPERANDS_TO_MATCH[j] != null)
                    {
                        logLine += $"\t{OPERANDS_TO_MATCH[j]}";
                        object operand = list[index].operand;
                        if (OPERANDS_TO_MATCH[j] != operand)
                        {
                            logLine += $" != {operand}";
                            Main.LogInfo($"{logLine}\tFAIL");
                            break;
                        }
                        logLine += $" == {operand}";
                    }
                    Main.LogInfo($"{logLine}\tPASS");

                    if (j == OPCODES_TO_MATCH.Count - 1)
                    {
                        Main.LogInfo($"Found match {++matches}");
                        if (matches > EXPECTED_MATCH_COUNT)
                        {
                            Main.LogError("Number of matches found exceeded EXPECTED_MATCH_COUNT! Returning original IL.");
                            return instructions;
                        }

                        // Perform replacements
                        for (int k = 0; k < MODIFIED_OPCODES.Count; k++)
                        {
                            int replacementIndex = i + k;
                            if (MODIFIED_OPCODES[k] == null || list[replacementIndex].opcode == MODIFIED_OPCODES[k])
                            {
                                continue;
                            }
                            OpCode beforeChange = list[replacementIndex].opcode;
                            list[replacementIndex].opcode = MODIFIED_OPCODES[k];
                            Main.LogInfo($"Line {replacementIndex}: Replaced Opcode ({beforeChange} ==> {MODIFIED_OPCODES[k]})");
                        }

                        for (int k = 0; k < MODIFIED_OPERANDS.Count; k++)
                        {
                            if (MODIFIED_OPERANDS[k] != null)
                            {
                                int replacementIndex = i + k;
                                object beforeChange = list[replacementIndex].operand;
                                list[replacementIndex].operand = MODIFIED_OPERANDS[k];
                                Main.LogInfo($"Line {replacementIndex}: Replaced operand ({beforeChange ?? "null"} ==> {MODIFIED_OPERANDS[k] ?? "null"})");
                            }
                        }
                    }
                }
            }

            Main.LogWarning($"{(matches > 0 ? (matches == EXPECTED_MATCH_COUNT ? "Transpiler Patch succeeded with no errors" : $"Completed with {matches}/{EXPECTED_MATCH_COUNT} found.") : "Failed to find match")}");
            return list.AsEnumerable();
        }
    }
}
