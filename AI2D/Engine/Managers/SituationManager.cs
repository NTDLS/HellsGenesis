﻿using AI2D.Engine.Situations;
using System.Collections.Generic;

namespace AI2D.Engine.Managers
{
    public class SituationManager
    {
        public Core _core { get; private set; }
        public BaseSituation CurrentSituation { get; private set; }

        public List<BaseSituation> Situations = new();

        public SituationManager(Core core)
        {
            _core = core;
        }

        public void ClearScenarios()
        {
            lock (Situations)
            {
                foreach (var obj in Situations)
                {
                    obj.Cleanup();
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

                Situations.Add(new SituationDebuggingGalore(_core));
                Situations.Add(new SituationScinzadSkirmish(_core));
                Situations.Add(new SituationIrlenFormations(_core));
                Situations.Add(new SituationAvvolAmbush(_core));
            }
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
                    CurrentSituation = Situations[0];
                    CurrentSituation.Execute();
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
