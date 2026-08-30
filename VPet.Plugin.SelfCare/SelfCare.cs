using System;
using System.Collections.Generic;
using System.Timers;
using VPet.Plugin.VpetAPI;
using VPet_Simulator.Core;
using VPet_Simulator.Windows.Interface;
using static VPet_Simulator.Core.GraphHelper;
using static VPet_Simulator.Core.WorkTimer;

namespace VPet.Plugin.SelfCare
{
	public class SelfCare : MainPlugin {
		public SelfCare(IMainWindow mainwin) : base(mainwin) {
		}
		public override string PluginName => "SelfCare";

		LevelLimitAdjuster lla;

		List<Work> ws;
		List<Work> ss;
		List<Work> ps;

		Random ran=new();

		public override void LoadPlugin() {
			//MW.Set.AutoBuy = true;
			//MW.Set.AutoGift = true;

			MW.Main.WorkList(out ws, out ss, out ps);

			lla = new(MW);

			MW.Main.EventTimer.Elapsed += TickElapsed;
			MW.Main.Event_WorkStart += WorkStart;
			MW.Main.Event_WorkEnd += WorkEnd;
		}

		void DoWork() {
			if (MW.GameSavesData.GameSave.Feeling / MW.GameSavesData.GameSave.FeelingMax > 2 / 3) {
				int sOrP = ran.Next(0, 1 + 1);
				switch (sOrP) {
					case 0: {
							int i = ran.Next(0, ws.Count);
							MW.Main.StartWork(lla.AdjustBeforeStart((Work)ws[i].Clone()));
						}break;
					case 1:
					default:{
							int i = ran.Next(0, ss.Count);
							MW.Main.StartWork(lla.AdjustBeforeStart((Work)ss[i].Clone()));
						}break;
				}
			}
			else {
				int i = ran.Next(0, ps.Count);
				MW.Main.StartWork(lla.AdjustBeforeStart((Work)ps[i].Clone()));
			}
		}

		

		Work nowWork = null;
		void WorkStart(Work work) {
			nowWork = work;
		}
		void WorkEnd(FinishWorkInfo finishWorkInfo) {
			nowWork = null;
		}

		ushort waitTick = 0;
		ushort waitTick_growth = 0;
		/// <summary>
		/// 游戏每个tick的调用
		/// </summary>
		void TickElapsed(object sender, ElapsedEventArgs e) {
			/*if (MW.Main.NowWork != null) {
			}*/
			if (nowWork == null) {
				if (waitTick == 0) {
					waitTick = (ushort)ran.Next(1, 5);
					waitTick_growth = 0;
				}
				else {
					if (waitTick != waitTick_growth)
						waitTick_growth++;
					else {
						DoWork();
						waitTick = 0;
					}
				}
			}
		}
	}
}
