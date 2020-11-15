using System;
using Metanous.Model.Core.Search;

namespace Training.Api.Controllers.Base
{
    public class ServiceContext
    {
        public static readonly string Key = typeof(ServiceContext).Name;

        public Includes Includes { get; set; }

        public string UserName { get; set; }

        public string EntityId { get; set; }

        public string ContentLanguage { get; set; }

        public bool IsApplication { get; set; }

        public string ApplicationId { get; set; }

        public DateTimeOffset? IfModifiedSince { get; set; }
        
        public ServiceContext()
        {
            Includes = new Includes();
        }
    }
}