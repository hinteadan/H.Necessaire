using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Serialization
{
    public static class SerializationExtensions
    {
        public static string ToJsonArray<T>(this IEnumerable<T> value, string defaultTo = null)
        {
            if (!value?.Any() ?? true)
                return defaultTo;

            return JsonConvert.SerializeObject(value);
        }

        public static string ToJsonObject<T>(this T value, string defaultTo = null)
        {
            if (value == null)
                return defaultTo;

            return JsonConvert.SerializeObject(value);
        }

        public static T JsonToObject<T>(this string jsonString, T defaultTo = default)
        {
            return jsonString.ParseJsonToObject(defaultTo).Payload;
        }

        public static OperationResult<T> TryJsonToObject<T>(this string jsonString, T defaultTo = default)
        {
            return jsonString.ParseJsonToObject(defaultTo);
        }

        public static Note[] DeserializeToNotes(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new Note[0];

            Note[] result = null;

            new Action(() => result = JsonConvert.DeserializeObject<Note[]>(json) ?? new Note[0]).TryOrFailWithGrace(numberOfTimes: 1, onFail: x => result = null);
            if (result == null)
                new Action(() => result = Note.FromDictionary(JsonConvert.DeserializeObject<Dictionary<string, string>>(json))).TryOrFailWithGrace(numberOfTimes: 1, onFail: x => result = null);

            return result ?? new Note[0];
        }

        private static OperationResult<T> ParseJsonToObject<T>(this string jsonString, T defaultTo = default)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return OperationResult.Fail("The JSON string is empty").WithPayload(defaultTo);
            }

            OperationResult<T> result = OperationResult.Win().WithPayload(defaultTo);

            new Action(
                () =>
                    result = OperationResult.Win().WithPayload(
                        JsonConvert.DeserializeObject<T>(jsonString)
                    )
                )
                .TryOrFailWithGrace(
                    numberOfTimes: 1,
                    onFail: x => result = OperationResult.Fail(x).WithPayload(defaultTo)
                );

            if (result == null)
                result = OperationResult.Win().WithPayload(defaultTo);

            return result;
        }
    }
}
