using System;

namespace Metanous.Model.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class IgnoreChangesForAuditTracingAttribute : Attribute
    {
    }
}