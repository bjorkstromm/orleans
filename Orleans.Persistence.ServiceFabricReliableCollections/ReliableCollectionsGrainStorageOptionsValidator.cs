namespace Orleans.Hosting
{
    internal sealed class ReliableCollectionsGrainStorageOptionsValidator : IConfigurationValidator
    {
        private readonly ReliableCollectionsStorageOptions _reliableCollectionsStorageOptions;
        private readonly string name;

        public ReliableCollectionsGrainStorageOptionsValidator(ReliableCollectionsStorageOptions reliableCollectionsStorageOptions, string name)
        {
            this._reliableCollectionsStorageOptions = reliableCollectionsStorageOptions;
            this.name = name;
        }

        public void ValidateConfiguration()
        {
            throw new System.NotImplementedException();
        }
    }
}