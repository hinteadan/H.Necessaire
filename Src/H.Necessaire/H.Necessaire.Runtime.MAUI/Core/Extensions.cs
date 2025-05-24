using System.Text;

namespace H.Necessaire.Runtime.MAUI.Core
{
    public static class Extensions
    {
        const string displayNamePartsSeparator = "~";

        public static Note[] AppendDeviceInfo(this Note[] notes)
        {
            string deviceID = null;
#if ANDROID

            deviceID = Android.Provider.Settings.Secure.GetString(Platform.CurrentActivity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

#elif IOS
            deviceID = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
#endif
            return
                notes
                .Push(
                    new Note[] {

                        deviceID.NullIfEmpty().NoteAs("Device-NativeID"),

                        DeviceInfo.Model.NullIfEmpty().NoteAs("DeviceInfo-Model"),
                        DeviceInfo.Manufacturer.NullIfEmpty().NoteAs("DeviceInfo-Manufacturer"),
                        DeviceInfo.Name.NullIfEmpty().NoteAs("DeviceInfo-Name"),

                        DeviceInfo.VersionString.NullIfEmpty().NoteAs("DeviceInfo-VersionString"),

                        (DeviceInfo.Version?.Major.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Version-Major"),
                        (DeviceInfo.Version?.Minor.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Version-Minor"),
                        (DeviceInfo.Version?.Build.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Version-Build"),
                        (DeviceInfo.Version?.Revision.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Version-Revision"),
                        (DeviceInfo.Version?.MajorRevision.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Version-MajorRevision"),
                        (DeviceInfo.Version?.MinorRevision.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Version-MinorRevision"),

                        (DeviceInfo.Platform.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Platform"),
                        (DeviceInfo.Idiom.ToString().NullIfEmpty()).NoteAs("DeviceInfo-Idiom"),

                        (DeviceInfo.DeviceType.ToString().NullIfEmpty()).NoteAs("DeviceInfo-DeviceType"),
                        (((int)DeviceInfo.DeviceType).ToString().NullIfEmpty()).NoteAs("DeviceInfo-DeviceTypeID"),
                    }
                    .Where(n => !n.Value.IsEmpty()).ToArrayNullIfEmpty()
                )
                ;
        }

        public static ConsumerIdentity DecorateWithDeviceRuntimeInfo(this ConsumerIdentity consumerIdentity)
        {
            if (consumerIdentity is null)
                return consumerIdentity;

            consumerIdentity.Notes = new Note[0].Push(Note.GetEnvironmentInfo().AppendDeviceInfo());

            StringBuilder displayNameBuilder = new StringBuilder();

            string userName = Environment.UserName.NullIfEmpty();
            string userDomainName = Environment.UserDomainName.NullIfEmpty();
            string machineName = Environment.MachineName.NullIfEmpty();
            string deviceID = consumerIdentity.Notes.Get("Device-NativeID").NullIfEmpty();
            string deviceModel = DeviceInfo.Model.NullIfEmpty();
            string deviceManufacturer = DeviceInfo.Manufacturer.NullIfEmpty();
            string deviceName = DeviceInfo.Name.NullIfEmpty();
            string deviceVersionString = DeviceInfo.VersionString.NullIfEmpty();

            string deviceTag = $"{deviceManufacturer} {deviceName} {deviceModel} {deviceVersionString}".Trim().NullIfEmpty();

            displayNameBuilder

                .AndIf(deviceID != null, x => x.Append(deviceID).Append(displayNamePartsSeparator))

                .AndIf((userName ?? userDomainName) != null, x => x.Append(userName ?? userDomainName).Append(displayNamePartsSeparator))
                .AndIf(machineName != null, x => x.Append(machineName).Append(displayNamePartsSeparator))

                .AndIf(deviceTag != null, x => x.Append(deviceTag).Append(displayNamePartsSeparator))

                .AndIf(x => x.Length > 0, x => x.Remove(x.Length - 1, 1))

                ;

            consumerIdentity.DisplayName = displayNameBuilder.ToString();
            consumerIdentity.IDTag
                = deviceID
                ?? $"{(userName ?? userDomainName)} {machineName}".Trim().NullIfEmpty()
                ?? deviceTag
                ?? consumerIdentity.ID.ToString()
                ;



            consumerIdentity.AsOf = DateTime.UtcNow;

            return consumerIdentity;
        }
    }
}
