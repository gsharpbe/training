using System;
using Training.Dal;

namespace Training.Migrations.Production
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Migrating database...");
            DatabaseUtils.Migrate();

            Console.WriteLine("Done");
        }
    }
}
