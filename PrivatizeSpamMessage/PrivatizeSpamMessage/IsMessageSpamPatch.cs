using Sandbox.Engine.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Torch.Managers.PatchManager;
using Torch.Managers.PatchManager.MSIL;

namespace PrivatizeSpamMessage.PrivatizeSpamMessage
{
    [PatchShim]
    public static class IsMessageSpamPatch
    {
        internal static readonly MethodInfo isMessageSpam = typeof(MyMultiplayerBase).GetMethod("IsMessageSpam", BindingFlags.NonPublic | BindingFlags.Static);
        internal static readonly MethodInfo transpiler = typeof(IsMessageSpamPatch).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
        public static void Patch(PatchContext ctx)
        {
            ctx.GetPattern(isMessageSpam).Transpilers.Add(transpiler);

            Plugin.Log.Info("IsMessageSpam Patched Successfully!");
        }
        private static IEnumerable<MsilInstruction> Transpiler(IEnumerable<MsilInstruction> ins)
        {
            var insList = ins.ToList();
            for (int i = 0; i < insList.Count; i++)
            {
                if (insList[i].OpCode == OpCodes.Ldc_I4_1 && insList[i + 1].OpCode != OpCodes.Ret)
                {
                    insList[i] = insList[i].CopyWith(OpCodes.Ldc_I4_3);
                }
            }
            return insList;
        }
    }
}
