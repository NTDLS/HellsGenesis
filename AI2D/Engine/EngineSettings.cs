namespace AI2D.Engine
{
    internal class EngineSettings
    {
        #region Debug settings.
        public bool HighlightNatrualBounds { get; set; } = false;
        public bool HighlightAllActors { get; set; } = false;
        #endregion

        public bool AutoZoomWhenMoving { get; set; } = true;
    }
}
