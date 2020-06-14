// Copyright (c) 2020 Allan Mobley. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobsites.Cosmos.Identity
{
    /// <summary>
    ///     Unix datetime converter.
    /// </summary>
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///     Writes datetime as Unix timestamp in seconds.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime time)
            {
                long totalSeconds = (long)(time - UnixStartTime).TotalSeconds;
                writer.WriteValue(totalSeconds);
            }
            else
            {
                throw new ArgumentException("Invalid value. Expected datetime.");
            }
        }

        /// <summary>
        ///     Reads Unix timestamp in seconds as datetime.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer)
            {
                throw new Exception("Invalid token. Expected integer.");
            }

            double totalSeconds;
            
            try
            {
                totalSeconds = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new Exception("Invalid double value.");
            }

            return UnixStartTime.AddSeconds(totalSeconds);
        }
    }
}
