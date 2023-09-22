namespace NebulaSiege.Engine.Debug
{
    internal class DebugCommandParameterCriterion
    {
        public bool IsNotCriteria { get; set; }
        public string Value { get; private set; }

        public DebugCommandParameterCriterion(string filterText)
        {
            if (filterText[0] == '!')
            {
                IsNotCriteria = true;
                filterText = filterText.Substring(1);
            }

            Value = filterText;
        }
    }
}
