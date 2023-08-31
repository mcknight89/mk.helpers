using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers
{
    public class DataValidatorResultDto{
        public enum DataType{
            Mobile,
            Landline,
            Email,
        }

        public DataType Type {get;set;}

    }

    public static class DataValidator
    {

        public static void Validate(string data)
        {

        }

    }
}