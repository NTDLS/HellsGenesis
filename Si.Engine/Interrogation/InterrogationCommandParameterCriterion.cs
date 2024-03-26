namespace Si.Engine.Interrogation
{
    internal class InterrogationCommandParameterCriterion
    {
        public bool IsNotCriteria { get; set; }
        public string Value { get; private set; }

        public InterrogationCommandParameterCriterion(string filterText)
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
