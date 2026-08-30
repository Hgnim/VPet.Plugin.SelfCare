using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.SelfCare
{
    public class SelfCare : MainPlugin {
        public SelfCare(IMainWindow mainwin) : base(mainwin) {
		}
		public override string PluginName => "SelfCare";
	}
}
