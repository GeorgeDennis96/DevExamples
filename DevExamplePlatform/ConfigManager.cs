using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExample.Platform
{
    public static class ConfigManager
    {
        private static List<Tuple<string, string>> DefaultValues = new List<Tuple<string, string>>
        {
            new Tuple<string,string>("mongodb-connection-string","mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&directConnection=true&ssl=false"),

            // Stripe Test Key
            new Tuple<string,string>("stripe-api-key",""),
        };

        public static string GetVariable(string VariableName)
        {
            try
            {
                var EnvVar = Environment.GetEnvironmentVariable(VariableName);
                if (EnvVar == null)
                {
                    return DefaultValues.Single(x => x.Item1 == VariableName).Item2;
                }
                return EnvVar;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Requested Variable: " + VariableName);
                Console.WriteLine(ex.ToString()); ;
            }

            return null;
        }

    }
}
