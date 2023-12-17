using NebulaSiege.Engine;
using NebulaSiege.Situations;
using NebulaSiege.TickControllers.BaseClasses;
using System.Collections.Generic;
using System.Linq;

namespace NebulaSiege.Controller
{
    internal class SituationTickController : UnvectoredTickControllerBase<SituationBase>
    {
        public SituationBase CurrentSituation { get; private set; }
        public List<SituationBase> Situations { get; private set; } = new();

        public SituationTickController(EngineCore core)
            : base(core)
        {
        }

        public override void ExecuteWorldClockTick()
        {
            if (CurrentSituation?.State == HgSituationState.Ended)
            {
                if (AdvanceSituation() == false)
                {
                    Core.Events.QueueTheDoorIsAjar();
                }
            }
        }

        public void ClearScenarios()
        {
            lock (Situations)
            {
                foreach (var obj in Situations)
                {
                    obj.EndSituation();
                }
            }

            CurrentSituation = null;
            Situations.Clear();
        }

        public void Reset()
        {
            lock (Situations)
            {
                ClearScenarios();

                Situations.Add(new SituationDebuggingGalore(Core));
                Situations.Add(new SituationIrlenFormations(Core));
                Situations.Add(new SituationPhoenixAmbush(Core));
                Situations.Add(new SituationScinzadSkirmish(Core));
                Situations.Add(new SituationFreeFlight(Core));
            }
        }

        public bool Select(string situationName)
        {
            lock (Situations)
            {
                if (CurrentSituation != null)
                {
                    Situations.Remove(CurrentSituation);
                }

                var selectedSituation = Situations.Where(o => o.Name == situationName).FirstOrDefault();

                if (selectedSituation != null)
                {
                    CurrentSituation = selectedSituation;
                }
                else
                {
                    CurrentSituation = null;
                    return false;
                }
            }
            return true;
        }

        public bool StartCurrent()
        {
            if (CurrentSituation != null)
            {
                CurrentSituation.BeginSituation();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true of the situation is advanced, returns FALSE if we have have no more situations in the queue.
        /// </summary>
        /// <returns></returns>
        public bool AdvanceSituation()
        {
            lock (Situations)
            {
                if (CurrentSituation != null)
                {
                    Situations.Remove(CurrentSituation);
                }

                if (Situations.Count > 0)
                {
                    Core.Player.Hide();
                    CurrentSituation = Situations[0];
                    CurrentSituation.BeginSituation();
                }
                else
                {
                    CurrentSituation = null;
                    return false;
                }
            }
            return true;
        }
    }
}
