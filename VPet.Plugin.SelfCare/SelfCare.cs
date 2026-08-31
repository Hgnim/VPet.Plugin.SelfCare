using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
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
			//MessageBox.Show("test");
			if (MW.GameSavesData.GameSave.Feeling / MW.GameSavesData.GameSave.FeelingMax > 2 / 3) {
				int sOrP = ran.Next(0, 1 + 1);
				switch (sOrP) {
					case 0: {
							int i;
							do {
								i = ran.Next(0, ws.Count);
								//MessageBox.Show(i.ToString());
							} while ((ws[i].LevelLimit > MW.GameSavesData.GameSave.Level));
							//调用UI线程
							MW.Dispatcher.Invoke(() =>
								MW.Main.StartWork(lla.AdjustBeforeStart((Work)ws[i].Clone()))
							);
							//MessageBox.Show(ws[i].Name);
						}break;
					case 1:
					default:{
							int i;
							do {
								i = ran.Next(0, ss.Count);
								//MessageBox.Show(i.ToString());
							} while (ss[i].LevelLimit > MW.GameSavesData.GameSave.Level);
							MW.Dispatcher.Invoke(() =>
								MW.Main.StartWork(lla.AdjustBeforeStart((Work)ss[i].Clone()))
							);
							//MessageBox.Show(ss[i].Name);
						}
						break;
				}
			}
			else {
				int i ;
				do {
					i = ran.Next(0, ps.Count);
					//MessageBox.Show(i.ToString());
				} while (ps[i].LevelLimit > MW.GameSavesData.GameSave.Level);
				MW.Dispatcher.Invoke(() =>
					MW.Main.StartWork(lla.AdjustBeforeStart((Work)ps[i].Clone()))
				);
				//MessageBox.Show(ps[i].Name);
			}
		}

		

		bool isWork = false;
		void WorkStart(Work work) {
			isWork = true;
		}
		void WorkEnd(FinishWorkInfo finishWorkInfo) {
			isWork = false;
		}

		ushort waitTick = 0;
		ushort waitTick_growth = 0;
		/// <summary>
		/// 游戏每个tick的调用
		/// </summary>
		void TickElapsed(object sender, ElapsedEventArgs e) {
			/*if (MW.Main.NowWork != null) {
			}*/
			if (!isWork 
				&& MW.GameSavesData.GameSave.Mode is IGameSave.ModeType.Nomal or IGameSave.ModeType.Happy or IGameSave.ModeType.PoorCondition
				&& MW.Main.State==Main.WorkingState.Nomal) {
				if (waitTick == 0) {
					waitTick = (ushort)ran.Next(1, 5);
					waitTick_growth = 0;
				}
				else {
					if (waitTick != waitTick_growth)
						waitTick_growth++;
					else {
						//MessageBox.Show("start");
						isWork = true;
						DoWork();
						waitTick = 0;
					}
				}
			}
		}
	}
}
