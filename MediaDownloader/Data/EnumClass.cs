namespace MediaDownloader.Data
{
    public abstract class EnumClass
    {
        protected EnumClass(string displayName) { DisplayName = displayName; }

        public string DisplayName { get; }

        public override string ToString() { return DisplayName; }
    }
}
