using HG.Engine;
using HG.Engine.Types;
using System.Collections.Generic;

namespace HG.Situations
{
    internal class SituationFreeFlight : SituationBase
    {
        public SituationFreeFlight(EngineCore core)
            : base(core, "Free Flight")
        {
            TotalWaves = 5;
        }

        readonly List<HgEngineCallbackEvent> events = new List<HgEngineCallbackEvent>();

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
        }

        private void FirstShowPlayerCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }
    }
}
