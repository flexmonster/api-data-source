using NetCoreServer.Models.Fields;
using NetCoreServer.Models.Select;
using NetCoreServer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreServer.Models.DataModels
{
    public class DataSlice
    {
        public int[] DataColumnIndexes { get; set; }

        public static IDataStructure Data { get; set; }
        public DataSlice(IDataStructure data)
        {
            Data = data;
            var dataVauesCount = Data.Count();
            DataColumnIndexes = new int[dataVauesCount];
            for (int i = 0; i < dataVauesCount; i++)
            {
                DataColumnIndexes[i] = i;
            }
        }

        public DataSlice(int[] indexes)
        {
            DataColumnIndexes = indexes;
        }

        /// <summary>
        /// Filters data 
        /// </summary>
        /// <param name="filters">filters to apply</param>
        public void FilterData(List<Filter> filters)
        {
            if (filters.Count == 0)
            {
                return;
            }
            foreach (var filter in filters)
            {
                if (filter.Field.Type == "date")
                {
                    if (filter.Include != null)
                    {
                        foreach (var includeValue in filter.Include)
                        {
                            if (includeValue.NumberValue != null)
                            {
                                includeValue.FromUnixTimestamp();
                            }
                        }
                    }
                    if (filter.Exclude != null)
                    {
                        foreach (var excludeValue in filter.Exclude)
                        {
                            if (excludeValue.NumberValue != null)
                            {
                                excludeValue.FromUnixTimestamp();
                            }
                        }
                    }
                    if (filter.Query != null)
                    {
                        if (filter.Query.Single().Value.ListNumberValue == null)
                        {
                            filter.Query.Single().Value.FromUnixTimestamp();
                        }
                    }
                }
                if (filter.Value == null)
                {

                    var indexes = new List<int>();
                    foreach (var index in DataColumnIndexes)
                    {
                        if (CheckFilter(Data.GetValue(filter.Field.UniqueName, index), filter))
                        {
                            indexes.Add(index);
                        }
                    }
                    DataColumnIndexes = indexes.ToArray();
                }
                if (filter.Value != null)
                {
                    var calculatedTotalsAggregation = new List<Aggregation>();
                    CalcByFields(new List<FieldModel> { filter.Field }, null, new List<FieldFuncValue> { filter.Value }, ref calculatedTotalsAggregation);
                    var calculatedTotals = new Dictionary<Value, double>();
                    foreach (var agg in calculatedTotalsAggregation)
                    {
                        calculatedTotals.Add(agg.Keys[filter.Field.UniqueName], agg.Values[filter.Value.Field.UniqueName][filter.Value.Func]);
                    }
                    CheckValueFilterQuery(ref calculatedTotals, filter);
                    DataColumnIndexes = DataColumnIndexes.Where(elem => calculatedTotals.ContainsKey(Data.GetValue(filter.Field.UniqueName, elem))).ToArray();
                }
            }
        }

        /// <summary>
        ///  Check if calculated data meets filter's requirement
        /// </summary>
        /// <param name="calculatedTotals">Calculated data with given agregation</param>
        /// <param name="filter"></param>
        private void CheckValueFilterQuery(ref Dictionary<Value, double> calculatedTotals, Filter filter)
        {
            if (filter.Query.ContainsKey("top"))
            {
                calculatedTotals = calculatedTotals.OrderByDescending(elem => elem.Value)
                    .Take((int)filter.Query["top"].NumberValue).ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("bottom"))
            {
                calculatedTotals = calculatedTotals.OrderBy(elem => elem.Value)
                    .Take((int)filter.Query["bottom"].NumberValue).ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value == filter.Query["equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("not_equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value != filter.Query["not_equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("greater"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value > filter.Query["greater"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("greater_equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value >= filter.Query["greater_equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("less"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value < filter.Query["less"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("less_equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value <= filter.Query["less_equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("between"))
            {
                var from = filter.Query["between"].ListNumberValue[0];
                var to = filter.Query["between"].ListNumberValue[1];
                calculatedTotals = calculatedTotals.Where(elem => elem.Value >= from && elem.Value <= to)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            if (filter.Query.ContainsKey("not_between"))
            {
                var from = filter.Query["not_between"].ListNumberValue[0];
                var to = filter.Query["not_between"].ListNumberValue[1];
                calculatedTotals = calculatedTotals.Where(elem => elem.Value < from || elem.Value > to)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
        }

        /// <summary>
        /// Checks whether the data item meets the filter query
        /// </summary>
        /// <param name="dataElement">data value</param>
        /// <param name="filter">filter to apply</param>
        private bool CheckFilter(Value dataElement, Filter filter)
        {
            if (dataElement == null)
            {
                return false;
            }
            if (filter.Include != null)
            {
                return filter.Include.Contains(dataElement);
            }
            else if (filter.Exclude != null)
            {
                return !filter.Exclude.Contains(dataElement);
            }
            else if (filter.Query != null)
            {
                if (filter.Field.Type == "date")
                {
                    return CheckDateFilterQuery(dataElement.DateValue, filter.Query);
                }
                else if (filter.Field.Type == "number")
                {
                    return CheckNumberFilterQuery(dataElement.NumberValue, filter.Query);
                }
                else
                {
                    return CheckStringFilterQuery(dataElement.StringValue, filter.Query);
                }
            }
            return true;
        }

        /// <summary>
        /// Checks whether the DateTime meets the query condition
        /// </summary>
        /// <param name="dataElementValue">DateTime data value</param>
        /// <param name="query">Query object</param>
        private bool CheckDateFilterQuery(DateTime? dataElementValue, Dictionary<string, Value> query)
        {
            if (query.ContainsKey("equal"))
            {
                var date = query["equal"].DateValue;
                return date.Equals(dataElementValue);
            }
            if (query.ContainsKey("not_equal"))
            {
                var date = query["not_equal"].DateValue;
                return !date.Equals(dataElementValue);
            }
            if (query.ContainsKey("after"))
            {
                var date = query["after"].DateValue;
                return dataElementValue?.CompareTo(date) > 0;
            }
            if (query.ContainsKey("after_equal"))
            {
                var date = query["after_equal"].DateValue;
                return dataElementValue?.CompareTo(date) >= 0;
            }
            if (query.ContainsKey("before"))
            {
                var date = query["before"].DateValue;
                return dataElementValue?.CompareTo(date) < 0;
            }
            if (query.ContainsKey("before_equal"))
            {
                var date = query["before_equal"].DateValue;
                return dataElementValue?.CompareTo(date) <= 0;
            }
            if (query.ContainsKey("between"))
            {
                var v1 = query["between"].ListNumberValue[0];
                var v2 = query["between"].ListNumberValue[1];
                var dataElementValueInUnix = dataElementValue?.ToUnixTimestamp();
                return dataElementValueInUnix >= v1 && dataElementValueInUnix <= v2;
            }
            if (query.ContainsKey("not_between"))
            {
                var v1 = query["not_between"].ListNumberValue[0];
                var v2 = query["not_between"].ListNumberValue[1];
                var dataElementValueInUnix = dataElementValue?.ToUnixTimestamp();
                return dataElementValueInUnix < v1 || dataElementValueInUnix > v2;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the number value meets the query condition
        /// </summary>
        /// <param name="dataElementValue">Number value(store as double)</param>
        /// <param name="query">Query object</param>
        private bool CheckNumberFilterQuery(double? dataElementValue, Dictionary<string, Value> query)
        {
            if (!dataElementValue.HasValue) return false;
            if (query.ContainsKey("equal"))
            {
                return dataElementValue == query["equal"].NumberValue;
            }
            if (query.ContainsKey("not_equal"))
            {
                return dataElementValue != query["not_equal"].NumberValue;
            }
            if (query.ContainsKey("greater"))
            {
                return dataElementValue > query["greater"].NumberValue;
            }
            if (query.ContainsKey("greater_equal"))
            {
                return dataElementValue >= query["greater_equal"].NumberValue;
            }
            if (query.ContainsKey("less"))
            {
                return dataElementValue < query["less"].NumberValue;
            }
            if (query.ContainsKey("less_equal"))
            {
                return dataElementValue <= query["less_equal"].NumberValue;
            }
            if (query.ContainsKey("between"))
            {
                var v1 = query["between"].ListNumberValue[0];
                var v2 = query["between"].ListNumberValue[1];
                return dataElementValue >= v1 && dataElementValue <= v2;
            }
            if (query.ContainsKey("not_between"))
            {
                var v1 = query["not_between"].ListNumberValue[0];
                var v2 = query["not_between"].ListNumberValue[1];
                return dataElementValue < v1 || dataElementValue > v2;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the number value meets the query condition
        /// </summary>
        /// <param name="dataElementValue">String value</param>
        /// <param name="query">Query object</param>
        private bool CheckStringFilterQuery(string dataElementValue, Dictionary<string, Value> query)
        {
            var dataElementValueToLower = dataElementValue.ToLower();
            if (query.ContainsKey("equal"))
            {
                return dataElementValueToLower == query["equal"].StringValue.ToLower();
            }
            if (query.ContainsKey("not_equal"))
            {
                return dataElementValueToLower != query["not_equal"].StringValue.ToLower();
            }
            if (query.ContainsKey("begin"))
            {
                return dataElementValueToLower.StartsWith(query["begin"].StringValue.ToLower());
            }
            if (query.ContainsKey("not_begin"))
            {
                return !dataElementValueToLower.StartsWith(query["not_begin"].StringValue.ToLower());
            }
            if (query.ContainsKey("end"))
            {
                return dataElementValueToLower.EndsWith(query["end"].StringValue.ToLower());
            }
            if (query.ContainsKey("not_end"))
            {
                return !dataElementValueToLower.EndsWith(query["not_end"].StringValue.ToLower());
            }
            if (query.ContainsKey("contain"))
            {
                return dataElementValueToLower.Contains(query["contain"].StringValue.ToLower());
            }
            if (query.ContainsKey("not_contain"))
            {
                return !dataElementValueToLower.Contains(query["not_contain"].StringValue.ToLower());
            }
            if (query.ContainsKey("greater"))
            {
                return dataElementValueToLower.CompareTo(query["greater"].StringValue.ToLower()) > 0;
            }
            if (query.ContainsKey("greater_equal"))
            {
                return dataElementValueToLower.CompareTo(query["greater_equal"].StringValue.ToLower()) >= 0;
            }
            if (query.ContainsKey("less"))
            {
                return dataElementValueToLower.CompareTo(query["less"].StringValue.ToLower()) < 0;
            }
            if (query.ContainsKey("less_equal"))
            {
                return dataElementValueToLower.CompareTo(query["less_equal"].StringValue.ToLower()) <= 0;
            }
            if (query.ContainsKey("between"))
            {
                var v1 = query["between"].ListStringValue[0].ToLower();
                var v2 = query["between"].ListStringValue[1].ToLower();
                return dataElementValueToLower.CompareTo(v1) >= 0 && dataElementValueToLower.CompareTo(v2) <= 0;
            }
            if (query.ContainsKey("not_between"))
            {
                var v1 = query["not_between"].ListStringValue[0].ToLower();
                var v2 = query["not_between"].ListStringValue[1].ToLower();
                return dataElementValueToLower.CompareTo(v1) < 0 || dataElementValueToLower.CompareTo(v2) > 0;
            }
            return false;
        }

        /// <summary>
        /// Groups data by fields and calculates data values. Works recursively
        /// </summary>
        /// <param name="fields">All fields to group by</param>
        /// <param name="fieldsInColumns">Fields in columns to group by</param>
        /// <param name="values">Values to calcluate</param>
        /// <param name="response">Output response</param>
        /// <param name="keys">Key-value pairs that describes specific tuple</param>
        public void CalcByFields(List<FieldModel> fields, List<FieldModel> fieldsInColumns, List<FieldFuncValue> values,
           ref List<Aggregation> response, Dictionary<string, Value> keys = null)
        {
            if (fields.Count < 1)
            {
                return;
            }
            var field = fields[0];
            var fieldsSkipped = fields.Skip(1).ToList();
            var groupsByField = GroupBy(field.UniqueName);
            foreach (var group in groupsByField)
            {
                var subdata = new DataSlice(group.Value.ToArray());
                var item = subdata.CalcValues(values);
                if (item.Values.Count != 0)
                {
                    item.Keys = keys != null ? new Dictionary<string, Value>(keys) : new Dictionary<string, Value>();
                    item.Keys.Add(field.UniqueName, group.Key);
                    response.Add(item);
                    subdata.CalcByFields(fieldsSkipped, fieldsInColumns, values, ref response, item.Keys);
                }
            }

            if ((fieldsInColumns != null) && fieldsInColumns.Count > 0 && fields.Count > fieldsInColumns.Count)
            {
                var colField = fieldsInColumns[0];
                var colsSkipped = fieldsInColumns.Skip(1).ToList();
                var colGroupsByField = GroupBy(colField.UniqueName);
                foreach (var group in colGroupsByField)
                {
                    var subdata = new DataSlice(group.Value.ToArray());
                    var item = subdata.CalcValues(values);
                    if (item.Values.Count != 0)
                    {
                        item.Keys = keys != null ? new Dictionary<string, Value>(keys) : new Dictionary<string, Value>();
                        item.Keys.Add(colField.UniqueName, group.Key);
                        response.Add(item);
                        subdata.CalcByFields(colsSkipped, null, values, ref response, item.Keys);
                    }
                }
            }
        }

        /// <summary>
        /// Groups data by <paramref name="field"></paramref>
        /// </summary>
        /// <param name="field">Field's name</param>    
        /// <returns>Collection of groups</returns>
        private Dictionary<Value, List<int>> GroupBy(string field)
        {
            Dictionary<Value, List<int>> groups = new Dictionary<Value, List<int>>();
            foreach (var index in DataColumnIndexes)
            {
                if (!groups.ContainsKey(Data.GetValue(field, index)))
                {
                    groups.Add(Data.GetValue(field, index), new List<int>());
                }
                groups[Data.GetValue(field, index)].Add(index);
            }
            return groups;
        }


        /// <summary>
        /// Calculates aggregated values
        /// </summary>
        /// <param name="values">Values and its aggregation functions to calculate</param>
        /// <returns>All calculated aggregations by given field</returns>
        public Aggregation CalcValues(List<FieldFuncValue> values)
        {
            Aggregation response = new Aggregation
            {
                Values = new Dictionary<string, Dictionary<string, double>>()
            };
            values.ForEach(
                valrequest =>
                {
                    var calcValue = CalcValue(valrequest.Field, valrequest.Func);
                    if (!double.IsNaN(calcValue))
                    {
                        if (!response.Values.ContainsKey(valrequest.Field.UniqueName))
                        {
                            response.Values.Add(valrequest.Field.UniqueName, new Dictionary<string, double>());
                        }
                        if (!response.Values[valrequest.Field.UniqueName].ContainsKey(valrequest.Func))
                        {
                            response.Values[valrequest.Field.UniqueName].Add(valrequest.Func, calcValue);
                        }
                    }
                });
            return response;
        }

        /// <summary>
        /// Calculates aggregated value for specific field
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="func">aggregation name</param>
        /// <returns>Calculated aggregation</returns>
        private double CalcValue(FieldModel field, string func)
        {
            var validDataColumnIndexes = DataColumnIndexes.Where(index => !(Data.GetValue(field.UniqueName, index).StringValue == "")
            && (Data.GetValue(field.UniqueName, index) != null)).DefaultIfEmpty(-1).ToArray();

            if (func == "count")
            {
                if (validDataColumnIndexes[0] == -1)
                {
                    return 0;
                }
                return validDataColumnIndexes.Count();
            }
            if (func == "distinctcount")
            {
                if (validDataColumnIndexes[0] == -1)
                {
                    return 0;
                }
                return validDataColumnIndexes.Select(index => Data.GetValue(field.UniqueName, index)).Distinct().ToList().Count;
            }
            if (field.Type == "number")
            {
                if (validDataColumnIndexes[0] == -1)
                {
                    return double.NaN;
                }
                if (func == "sum" || func == "none")
                {
                    return validDataColumnIndexes.Sum(index => Data.GetValue(field.UniqueName, index).NumberValue.Value);
                }
                if (func == "average")
                {
                    return validDataColumnIndexes.Average(index => Data.GetValue(field.UniqueName, index).NumberValue.Value);
                }
                if (func == "min")
                {
                    return validDataColumnIndexes.Min(index => Data.GetValue(field.UniqueName, index).NumberValue.Value);
                }
                if (func == "max")
                {
                    return validDataColumnIndexes.Max(index => Data.GetValue(field.UniqueName, index).NumberValue.Value);
                }
            }
            if (field.Type == "date")
            {
                if (func == "min")
                {
                    return validDataColumnIndexes.Min(index => Data.GetValue(field.UniqueName, index).DateValue.Value.ToUnixTimestamp());
                }
                if (func == "max")
                {
                    return validDataColumnIndexes.Max(index => Data.GetValue(field.UniqueName, index).DateValue.Value.ToUnixTimestamp());
                }
            }
            return 0;
        }

    }
}