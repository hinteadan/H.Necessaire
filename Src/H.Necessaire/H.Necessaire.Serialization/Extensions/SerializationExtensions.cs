using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Serialization
{
    public static class SerializationExtensions
    {
        public static string ToJsonArray<T>(this IEnumerable<T> value, string defaultTo = null, bool isPrettyPrinted = false)
        {
            if (!value?.Any() ?? true)
                return defaultTo;

            return JsonConvert.SerializeObject(value, isPrettyPrinted ? Formatting.Indented : Formatting.None);
        }

        public static string ToJsonObject<T>(this T value, string defaultTo = null, bool isPrettyPrinted = false)
        {
            if (value == null)
                return defaultTo;

            return JsonConvert.SerializeObject(value, isPrettyPrinted ? Formatting.Indented : Formatting.None);
        }

        public static T JsonToObject<T>(this string jsonString, T defaultTo = default)
        {
            return jsonString.ParseJsonToObject(defaultTo).Payload;
        }

        public static OperationResult<T> TryJsonToObject<T>(this string jsonString, T defaultTo = default)
        {
            return jsonString.ParseJsonToObject(defaultTo);
        }

        public static Note[] DeserializeToNotes(this string json, Note[] defaultTo = null)
        {
            if (string.IsNullOrWhiteSpace(json))
                return defaultTo;

            Note[] result = null;

            new Action(() => result = JsonConvert.DeserializeObject<Note[]>(json) ?? new Note[0]).TryOrFailWithGrace(numberOfTimes: 1, onFail: x => result = null);
            if (result == null)
                new Action(() => result = Note.FromDictionary(JsonConvert.DeserializeObject<Dictionary<string, string>>(json))).TryOrFailWithGrace(numberOfTimes: 1, onFail: x => result = null);

            return result ?? defaultTo;
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
