using IntellectorLogic.Pieces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Shared.Models;

namespace IntellectorLogic.Serialization
{
    public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Piece).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }

    public class IPieceConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Piece);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject jo = JObject.Load(reader);
            return (PieceType)jo["Type"].Value<int>() switch
            {
                PieceType.Progressor => JsonConvert.DeserializeObject<Progressor>(jo.ToString(), SpecifiedSubclassConversion),
                PieceType.Liberator => JsonConvert.DeserializeObject<Liberator>(jo.ToString(), SpecifiedSubclassConversion),
                PieceType.Dominator => JsonConvert.DeserializeObject<Dominator>(jo.ToString(), SpecifiedSubclassConversion),
                PieceType.Agressor => JsonConvert.DeserializeObject<Agressor>(jo.ToString(), SpecifiedSubclassConversion),
                PieceType.Intellector => JsonConvert.DeserializeObject<Intellector>(jo.ToString(), SpecifiedSubclassConversion),
                PieceType.Defensor => JsonConvert.DeserializeObject<Defensor>(jo.ToString(), SpecifiedSubclassConversion),
                _ => throw new Exception(),
            };
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
