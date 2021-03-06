//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//
#if NET40
using System;
using System.Collections.Generic;
using System.Dynamic;
using ServiceStack.Text.Json;

namespace ServiceStack.Text.Common
{
	internal static class DeserializeDynamic<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		private static readonly ParseStringDelegate CachedParseFn;
		static DeserializeDynamic()
		{
			CachedParseFn = ParseDynamic;
		}

		public static ParseStringDelegate Parse
		{
			get { return CachedParseFn; }
		}

        public static IDynamicMetaObjectProvider ParseDynamic(string value)
        {
            var index = VerifyAndGetStartIndex(value, typeof(ExpandoObject));

            var result = new ExpandoObject();

            if (JsonTypeSerializer.IsEmptyMap(value)) return result;

            var container = (IDictionary<String, Object>) result;

            var tryToParsePrimitiveTypes = JsConfig.TryToParsePrimitiveTypeValues;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);

                var mapKey = Serializer.UnescapeString(keyValue);

                if (JsonUtils.IsJsObject(elementValue)) {
                    container[mapKey] = ParseDynamic(elementValue);
                } else if (JsonUtils.IsJsArray(elementValue)) {
                    container[mapKey] = DeserializeList<List<object>, TSerializer>.Parse(elementValue);
                } else if (tryToParsePrimitiveTypes) {
                    container[mapKey] = DeserializeType<TSerializer>.ParsePrimitive(elementValue);
                } else {
                    container[mapKey] = Serializer.UnescapeString(elementValue);
                }

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

		private static int VerifyAndGetStartIndex(string value, Type createMapType)
		{
			var index = 0;
			if (!Serializer.EatMapStartChar(value, ref index))
			{
				//Don't throw ex because some KeyValueDataContractDeserializer don't have '{}'
				Tracer.Instance.WriteDebug("WARN: Map definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
					JsWriter.MapStartChar, createMapType != null ? createMapType.Name : "Dictionary<,>", value.Substring(0, value.Length < 50 ? value.Length : 50));
			}
			return index;
		}
	}
}
#endif