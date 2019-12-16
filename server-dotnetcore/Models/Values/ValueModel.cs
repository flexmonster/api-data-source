using System;
using System.Collections.Generic;

namespace NetCoreServer.Models
{
    public class Value
    {
        public Value()
        {

        }

        public Value(List<string> value)
        {
            ListStringValue = value;
        }
        public Value(List<double> value)
        {
            ListNumberValue = value;
        }
        public Value(string value)
        {
            StringValue = value;
        }
        public Value(double value)
        {
            NumberValue = value;
        }

        public Value(DateTime value)
        {
            DateValue = value;
        }


        public List<string> ListStringValue { get; set; }

        public List<double> ListNumberValue { get; set; }

        public string StringValue { get; set; }

        public double? NumberValue { get; set; }

        public DateTime? DateValue { get; set; }

        public override bool Equals(object obj)
        {
            var castedObject = (Value)obj;
            if (StringValue != null)
            {
                return StringValue == castedObject.StringValue;
            }
            else if (DateValue != null)
            {
                return DateValue.Equals(castedObject.DateValue);
            }
            else if (ListStringValue != null)
            {
                return ListStringValue == castedObject.ListStringValue;
            }
            else if (ListNumberValue != null)
            {
                return ListNumberValue == castedObject.ListNumberValue;
            }
            return NumberValue == castedObject.NumberValue;
        }

        public override int GetHashCode()
        {
            if (StringValue != null)
            {
                return StringValue.GetHashCode();
            }
            else if (NumberValue != null)
            {

                return NumberValue.GetHashCode();
            }
            else if (DateValue != null)
            {
                return DateValue.GetHashCode();
            }
            else if (ListStringValue != null)
            {
                return ListStringValue.GetHashCode();
            }
            return ListNumberValue.GetHashCode();
        }

        public override string ToString()
        {
            if (StringValue != null)
            {
                return StringValue.ToString();
            }
            else if (ListStringValue != null)
            {
                return ListStringValue.ToString();
            }
            else if (ListNumberValue != null)
            {
                return ListNumberValue.ToString();
            }
            return NumberValue.ToString();
        }
        public void FromUnixTimestamp()
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.DateValue = dtDateTime.AddMilliseconds(this.NumberValue.Value).ToLocalTime();
            this.NumberValue = 0;
        }


    }
}