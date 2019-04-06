namespace ComplexStorage
{
    public class DefaultSettings : ICFSettings
    {
        private DefaultSettings() { }
        private static DefaultSettings instance = new DefaultSettings();
        public static DefaultSettings Instance => instance;

        public int BlockSize => 512;
        public int HeadersNumber => 4;

        public int IdSize => 18;
        public int NameSize => 100;
        public int HeaderSize => (BlockSize - 8) / HeadersNumber;

        public int DataSize => BlockSize - 8;
        public string RootName => "root";
    }
}