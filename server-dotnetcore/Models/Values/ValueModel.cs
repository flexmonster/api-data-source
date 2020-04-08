using System;
using System.Collections.Generic;

namespace NetCoreServer.Models
{
    public class ValueObject
    {
        public ValueObject()
        {
        }

        public ValueObject(List<string> value)
        {
            ListStringValue = value;
        }

        public ValueObject(List<double> value)
        {
            ListNumberValue = value;
        }

        public ValueObject(string value)
        {
            StringValue = value;
        }

        public ValueObject(double value)
        {
            NumberValue = value;
        }

        public List<string> ListStringValue { get; set; }

        public List<double> ListNumberValue { get; set; }

        public string StringValue { get; set; }

        public double? NumberValue { get; set; }
    }
}