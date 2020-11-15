using System;
using Newtonsoft.Json;

namespace Metanous.Model.Core.Model
{
    public abstract class ServiceModelBase: IEquatable<ServiceModelBase>
    {
        protected ServiceModelBase()
        {
            Guid = Guid.NewGuid();
        }

        // https://stackoverflow.com/questions/3330989/order-of-serialized-fields-using-json-net#comment33365412_14035431
        [JsonProperty(Order = -2)]
        public Guid Guid { get; set; }

        public bool IsObsolete { get; set; }

        public DateTimeOffset? ModifyDate { get; set; }

        public bool Equals(ServiceModelBase other)
        {
            return other != null && Guid.Equals(other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ServiceModelBase)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Guid.GetHashCode();
        }
    }
}