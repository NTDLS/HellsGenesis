namespace AI2D.Engine
{
    public class EngineSettings
    {
        #region Debug settings.
        public bool HighlightNatrualBounds { get; set; } = false;
        public bool HighlightAllActors { get; set; } = false;
        #endregion

        public bool AutoZoomWhenMoving { get; set; } = true;
    }
}
