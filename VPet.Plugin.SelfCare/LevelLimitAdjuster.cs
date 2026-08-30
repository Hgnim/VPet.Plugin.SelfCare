/*
********************************************
此文件代码来源：https://github.com/MadeSpark/Vpet.Plugin.VpetAPI/blob/126af9c31580851aaa23bd998912773e0a7c0d66/LevelLimitAdjuster.cs
********************************************
*/
using System;
using VPet_Simulator.Core;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.VpetAPI
{
    /// <summary>
    /// 等级限制调整器 - 自动选择最佳倍率
    /// 
    /// VPet的倍率系统：
    /// - LevelLimit是基础档位（如清屏=10）
    /// - 实际等级上限 = (LevelLimit + 10) * 倍率
    /// - 最大倍率 = Min(4000, 桌宠等级) / (LevelLimit + 10)
    /// 
    /// 示例（清屏，LevelLimit=10）：
    /// - 桌宠260级：最大倍率 = 260/20 = 13，实际上限 = 20*13 = 260
    /// - 桌宠100级：最大倍率 = 100/20 = 5，实际上限 = 20*5 = 100
    /// </summary>
    public sealed class LevelLimitAdjuster
    {
        private readonly IMainWindow mw;

        public LevelLimitAdjuster(IMainWindow mainWindow)
        {
            this.mw = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        /// <summary>
        /// 计算最大可用倍率
        /// 公式：Min(4000, 桌宠等级) / (LevelLimit + 10)
        /// </summary>
        private int CalculateMaxMultiplier(GraphHelper.Work work)
        {
            int currentLevel = mw.Core.Save.Level;
            int baseLevelLimit = work.LevelLimit;
            
            // 如果桌宠等级低于基础等级限制，不能使用倍率
            if (currentLevel < baseLevelLimit)
            {
                return 1;
            }
            
            // 计算最大倍率：Min(4000, 桌宠等级) / (LevelLimit + 10)
            int maxMultiplier = Math.Min(4000, currentLevel) / (baseLevelLimit + 10);
            
            // 倍率至少为1
            return Math.Max(1, maxMultiplier);
        }

        /// <summary>
        /// 在开始工作前自动选择最佳倍率
        /// 使用VPet的Double()方法应用倍率
        /// </summary>
        public GraphHelper.Work? AdjustBeforeStart(GraphHelper.Work? work)
        {
            if (work == null)
                return null;

            int currentLevel = mw.Core.Save.Level;
            int baseLevelLimit = work.LevelLimit;
            
            // 计算最大可用倍率
            int maxMultiplier = CalculateMaxMultiplier(work);
            
            // 如果倍率为1，不需要调整
            if (maxMultiplier <= 1)
            {
                return work;
            }
            
            // 应用倍率（使用VPet的Double扩展方法）
            var adjustedWork = work.Double(maxMultiplier);
            
            // 计算调整后的实际等级上限
            int actualLevelLimit = (baseLevelLimit + 10) * maxMultiplier;
            
            System.Diagnostics.Debug.WriteLine(
                $"[自动倍率] {work.Name}: 档位{baseLevelLimit} x{maxMultiplier} = 上限{actualLevelLimit} (桌宠{currentLevel}级)");
            
            return adjustedWork;
        }
    }
}
