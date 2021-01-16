using System;

namespace H.Necessaire
{
    public static class AzureExtensions
    {
        static readonly DateTime azureTableStorageMinDateTime = new DateTime(1601, 1, 1);

        public static DateTime ToAzureTableStorageSafeMinDate(this DateTime dateTime)
        {
            if (dateTime < azureTableStorageMinDateTime)
                return azureTableStorageMinDateTime;

            return dateTime;
        }

        public static DateTime ToNetMinDateFromAzureTableStorageMinDate(this DateTime dateTime)
        {
            if (dateTime == azureTableStorageMinDateTime)
                return DateTime.MinValue;

            return dateTime;
        }
    }
}
