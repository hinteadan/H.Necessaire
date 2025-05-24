namespace H.Necessaire.Runtime.MAUI.Core
{
    static class ConsumerInfo
    {
        static readonly Lazy<ConsumerPlatformInfo> consumerPlatformInfo = new Lazy<ConsumerPlatformInfo>(BuildConsumerPlatformInfo);
        public static ConsumerPlatformInfo Platform => consumerPlatformInfo.Value;

        static ConsumerPlatformInfo BuildConsumerPlatformInfo()
        {
            string deviceModel = DeviceInfo.Model.NullIfEmpty();
            string deviceManufacturer = DeviceInfo.Manufacturer.NullIfEmpty();
            string deviceName = DeviceInfo.Name.NullIfEmpty();
            string deviceVersionString = DeviceInfo.VersionString.NullIfEmpty();

            string deviceTag = $"{deviceManufacturer} {deviceName} {deviceModel} {deviceVersionString}".Trim().NullIfEmpty();

            return new ConsumerPlatformInfo
            {
                IsMobile = DeviceInfo.Idiom.In(DeviceIdiom.Phone),
                IsWindows = DeviceInfo.Platform.In(DevicePlatform.WinUI),
                Platform = DeviceInfo.Platform.ToString(),
                PlatformVersion = DeviceInfo.VersionString,
                UserAgent = deviceTag,
                Attributes = [
                    DeviceInfo.Idiom.ToString().NoteAs("Idiom"),
                ],
            }.And(x =>
            {
                x.WindowsVersion = x.IsWindows == true ? x.PlatformVersion : null;
            });
        }
    }
}
