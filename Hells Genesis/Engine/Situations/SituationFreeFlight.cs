using System.Collections.Generic;

namespace HG.Engine.Situations
{
    internal class SituationFreeFlight : BaseSituation
    {
        public SituationFreeFlight(Core core)
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

        private void FirstShowPlayerCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }
    }
}
