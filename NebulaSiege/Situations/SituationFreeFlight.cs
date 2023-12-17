using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using System.Collections.Generic;

namespace NebulaSiege.Situations
{
    internal class SituationFreeFlight : SituationBase
    {
        public SituationFreeFlight(EngineCore core)
            : base(core,
                  "Free Flight",
                  "Theres nothing in this quadrant or the next that will threaten us.")
        {
            TotalWaves = 5;
        }

        readonly List<NsEngineCallbackEvent> events = new List<NsEngineCallbackEvent>();

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
        }

        private void FirstShowPlayerCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }
    }
}
