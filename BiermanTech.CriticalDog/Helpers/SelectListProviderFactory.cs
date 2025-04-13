namespace BiermanTech.CriticalDog.Helpers
{
    public static class SelectListProviderFactory
    {
        public static ISelectListProvider GetProvider(int observationTypeId)
        {
            switch (observationTypeId)
            {
                case 5:
                    return new LifeStageFactorsSelectListProvider();
                // Add more cases for other ObservationTypeIds
                default:
                    throw new NotSupportedException($"No provider found for ObservationTypeId: {observationTypeId}");
            }
        }
    }
}
