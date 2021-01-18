namespace Training.Model
{
    public static class Routes
    {
        private const string Prefix = "api/v1";

        public const string DomainData = Prefix + "/domaindata";
        public const string Countries = DomainData + "/countries";

        public const string Customers = Prefix + "/customers";
        public const string Projects = Prefix + "/projects";
    }
}
