using System;
using Metanous.Model.Core.Attributes;

namespace Metanous.Model.Core.Model
{
    public abstract class ModelBase: IEquatable<ModelBase>
    {
        protected ModelBase()
        {
            Guid = Guid.NewGuid();
        }

        public int Id { get; set; }
        public Guid Guid { get; set; }
        public bool IsObsolete { get; set; }

        [IgnoreChangesForAuditTracing]
        public DateTimeOffset? CreateDate { get; set; }
        public string Creator { get; set; }

        [IgnoreChangesForAuditTracing]
        public DateTimeOffset? ModifyDate { get; set; }

        [IgnoreChangesForAuditTracing]
        public string Modifier { get; set; }

        public static bool operator ==(ModelBase left, ModelBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelBase left, ModelBase right)
        {
            return !Equals(left, right);
        }

        public bool Equals(ModelBase other)
        {
            return other != null && Guid.Equals(other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ModelBase)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Guid.GetHashCode();
        }

        public void SetUpdateFields(string modifier = null)
        {
            var now = DateTime.UtcNow;
            if (!CreateDate.HasValue)
            {
                CreateDate = now;
                Creator = modifier;
            }
            ModifyDate = now;
            Modifier = modifier;
        }
    }
}