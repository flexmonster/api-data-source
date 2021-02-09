using NetCoreServer.Models.DataModels;
using NetCoreServer.Models.Fields;
using NetCoreServer.Models.Select;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreServer.Models
{
    public class DataSlice
    {
        private int _indexesCount;
        public int[] DataColumnIndexes { get; set; }

        public static IDataStructure Data { get; set; }
        private static Dictionary<string, ColumnType> _columnNameTypes;

        public DataSlice(IDataStructure data)
        {
            Data = data;
            _columnNameTypes = Data.GetNameAndTypes();
            var firstNameAndType = _columnNameTypes.First();
            int dataVauesCount = 0;
            if (firstNameAndType.Value == ColumnType.stringType)
            {
                dataVauesCount = Data.GetColumn<string>(firstNameAndType.Key).Count();
            }
            else
            {
                dataVauesCount = Data.GetColumn<double?>(firstNameAndType.Key).Count();
            }
            DataColumnIndexes = new int[dataVauesCount];
            _indexesCount = 0;
            for (int i = 0; i < dataVauesCount; i++)
            {
                DataColumnIndexes[i] = i;
                _indexesCount++;
            }
        }

        public DataSlice(int[] indexes, int count)
        {
            DataColumnIndexes = indexes;
            _indexesCount = count;
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
                filter.Field.Type = _columnNameTypes[filter.Field.UniqueName];
                if (filter.Value == null)
                {
                    ParallelQuery<int> indexes = DataColumnIndexes.AsParallel();
                    if (filter.Include != null)
                    {
                        if (filter.Field.Type == ColumnType.stringType)
                        {
                            CheckIncludeFilter(Data.GetColumn<string>(filter.Field.UniqueName), filter.Include, ref indexes);
                        }
                        else
                        {
                            CheckIncludeFilter(Data.GetColumn<double?>(filter.Field.UniqueName), filter.Include, ref indexes);
                        }
                    }
                    else if (filter.Exclude != null)
                    {
                        if (filter.Field.Type == ColumnType.stringType)
                        {
                            CheckExcludeFilter(Data.GetColumn<string>(filter.Field.UniqueName), filter.Exclude, ref indexes);
                        }
                        else
                        {
                            CheckExcludeFilter(Data.GetColumn<double?>(filter.Field.UniqueName), filter.Exclude, ref indexes);
                        }
                    }
                    else if (filter.Query != null)
                    {
                        if (filter.Field.Type == ColumnType.doubleType)
                        {
                            var column = Data.GetColumn<double?>(filter.Field.UniqueName);
                            indexes = indexes.Where(index => CheckNumberFilterQuery(column[index], filter.Query));
                        }
                        else if (filter.Field.Type == ColumnType.stringType)
                        {
                            var column = Data.GetColumn<string>(filter.Field.UniqueName);
                            indexes = indexes.Where(index => CheckStringFilterQuery(column[index], filter.Query));
                        }
                        else if (filter.Field.Type == ColumnType.dateType)
                        {
                            var column = Data.GetColumn<double?>(filter.Field.UniqueName);
                            indexes = indexes.Where(index => CheckDateFilterQuery(column[index], filter.Query));
                        }
                    }
                    DataColumnIndexes = indexes.ToArray();
                }

                if (filter.Value != null)
                {
                    filter.Value.Field.Type = _columnNameTypes[filter.Value.Field.UniqueName];
                    var calculatedTotalsAggregation = new List<Aggregation>();
                    CalcByFields(new List<FieldModel> { filter.Field }, null, new List<FieldFuncValue> { filter.Value }, ref calculatedTotalsAggregation);
                    var calculatedTotals = new Dictionary<object, double>();
                    foreach (var agg in calculatedTotalsAggregation)
                    {
                        calculatedTotals.Add(agg.Keys[filter.Field.UniqueName], agg.Values[filter.Value.Field.UniqueName][filter.Value.Func]);
                    }
                    CheckValueFilterQuery(ref calculatedTotals, filter);
                    if (filter.Field.Type == ColumnType.stringType)
                    {
                        var column = Data.GetColumn<string>(filter.Field.UniqueName);
                        DataColumnIndexes = DataColumnIndexes.Where(index => calculatedTotals.ContainsKey(column[index])).ToArray();
                    }
                    else
                    {
                        var column = Data.GetColumn<double?>(filter.Field.UniqueName);
                        DataColumnIndexes = DataColumnIndexes.Where(index => calculatedTotals.ContainsKey(column[index])).ToArray();
                    }
                }
            }
        }

        /// <summary>
        ///  Check if calculated data meets filter's requirement
        /// </summary>
        /// <param name="calculatedTotals">Calculated data with given agregation</param>
        /// <param name="filter"></param>
        private void CheckValueFilterQuery(ref Dictionary<object, double> calculatedTotals, Filter filter)
        {
            if (filter.Query.ContainsKey("top"))
            {
                calculatedTotals = calculatedTotals.OrderByDescending(elem => elem.Value)
                    .Take((int)filter.Query["top"].NumberValue).ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("bottom"))
            {
                calculatedTotals = calculatedTotals.OrderBy(elem => elem.Value)
                    .Take((int)filter.Query["bottom"].NumberValue).ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value == filter.Query["equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("not_equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value != filter.Query["not_equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("greater"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value > filter.Query["greater"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("greater_equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value >= filter.Query["greater_equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("less"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value < filter.Query["less"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("less_equal"))
            {
                calculatedTotals = calculatedTotals.Where(elem => elem.Value <= filter.Query["less_equal"].NumberValue)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("between"))
            {
                var from = filter.Query["between"].ListNumberValue[0];
                var to = filter.Query["between"].ListNumberValue[1];
                calculatedTotals = calculatedTotals.Where(elem => elem.Value >= from && elem.Value <= to)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
            else if (filter.Query.ContainsKey("not_between"))
            {
                var from = filter.Query["not_between"].ListNumberValue[0];
                var to = filter.Query["not_between"].ListNumberValue[1];
                calculatedTotals = calculatedTotals.Where(elem => elem.Value < from || elem.Value > to)
                    .ToDictionary(elem => elem.Key, elem => elem.Value);
            }
        }

        /// <summary>
        /// Checks whether the data item present in include filter
        /// </summary>
        /// <param name="dataColumn">data column</param>
        /// <param name="filter">filter to apply</param>
        /// <param name="indexes">index of data items present in slice</param>
        private void CheckIncludeFilter<T>(DataColumn<T> dataColumn, List<HierarchyObject> filter, ref ParallelQuery<int> indexes)
        {
            if (dataColumn.ColumnType == ColumnType.doubleType || dataColumn.ColumnType == ColumnType.dateType)
            {
                var numberFilter = filter.Select(value => value?.Member?.NumberValue);
                indexes = indexes.Where(index => numberFilter.Contains(dataColumn[index] as double?));
            }
            else if (dataColumn.ColumnType == ColumnType.stringType)
            {
                var stringFilter = filter.Select(value => value?.Member?.StringValue);
                indexes = indexes.Where(index => stringFilter.Contains(dataColumn[index] as string));
            }
        }
        /// <summary>
        /// Checks whether the data item present in exclude filter
        /// </summary>
        /// <param name="dataColumn">data column</param>
        /// <param name="filter">filter to apply</param>
        /// <param name="indexes">index of data items present in slice</param>
        private void CheckExcludeFilter<T>(DataColumn<T> dataColumn, List<HierarchyObject> filter, ref ParallelQuery<int> indexes)
        {
            if (dataColumn.ColumnType == ColumnType.doubleType || dataColumn.ColumnType == ColumnType.dateType)
            {
                var numberFilter = filter.Select(value => value?.Member?.NumberValue);
                indexes = indexes.Where(index => !numberFilter.Contains(dataColumn[index] as double?));
            }
            else if (dataColumn.ColumnType == ColumnType.stringType)
            {
                var stringFilter = filter.Select(value => value?.Member?.StringValue == "" ? null : value?.Member?.StringValue);
                indexes = indexes.Where(index => !stringFilter.Contains(dataColumn[index] as string));
            }
        }

        /// <summary>
        /// Checks whether the DateTime meets the query condition
        /// </summary>
        /// <param name="dateElementValue">DateTime data value</param>
        /// <param name="query">Query object</param>
        private bool CheckDateFilterQuery(double? dateElementValue, Dictionary<string, ValueObject> query)
        {
            if (query.ContainsKey("equal"))
            {
                var date = query["equal"].NumberValue;
                return date.Equals(dateElementValue);
            }
            if (query.ContainsKey("not_equal"))
            {
                var date = query["not_equal"].NumberValue;
                return !date.Equals(dateElementValue);
            }
            if (query.ContainsKey("after"))
            {
                var date = query["after"].NumberValue;
                return dateElementValue?.CompareTo(date) > 0;
            }
            if (query.ContainsKey("after_equal"))
            {
                var date = query["after_equal"].NumberValue;
                return dateElementValue?.CompareTo(date) >= 0;
            }
            if (query.ContainsKey("before"))
            {
                var date = query["before"].NumberValue;
                return dateElementValue?.CompareTo(date) < 0;
            }
            if (query.ContainsKey("before_equal"))
            {
                var date = query["before_equal"].NumberValue;
                return dateElementValue?.CompareTo(date) <= 0;
            }
            if (query.ContainsKey("between"))
            {
                var v1 = query["between"].ListNumberValue[0];
                var v2 = query["between"].ListNumberValue[1];
                return dateElementValue >= v1 && dateElementValue <= v2;
            }
            if (query.ContainsKey("not_between"))
            {
                var v1 = query["not_between"].ListNumberValue[0];
                var v2 = query["not_between"].ListNumberValue[1];
                return dateElementValue < v1 || dateElementValue > v2;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the number value meets the query condition
        /// </summary>
        /// <param name="numberElementValue">Number value(store as double)</param>
        /// <param name="query">Query object</param>
        private bool CheckNumberFilterQuery(double? numberElementValue, Dictionary<string, ValueObject> query)
        {
            if (query.ContainsKey("equal"))
            {
                return numberElementValue == query["equal"].NumberValue;
            }
            if (query.ContainsKey("not_equal"))
            {
                return numberElementValue != query["not_equal"].NumberValue;
            }
            if (query.ContainsKey("greater"))
            {
                return numberElementValue > query["greater"].NumberValue;
            }
            if (query.ContainsKey("greater_equal"))
            {
                return numberElementValue >= query["greater_equal"].NumberValue;
            }
            if (query.ContainsKey("less"))
            {
                return numberElementValue < query["less"].NumberValue;
            }
            if (query.ContainsKey("less_equal"))
            {
                return numberElementValue <= query["less_equal"].NumberValue;
            }
            if (query.ContainsKey("between"))
            {
                var v1 = query["between"].ListNumberValue[0];
                var v2 = query["between"].ListNumberValue[1];
                return numberElementValue >= v1 && numberElementValue <= v2;
            }
            if (query.ContainsKey("not_between"))
            {
                var v1 = query["not_between"].ListNumberValue[0];
                var v2 = query["not_between"].ListNumberValue[1];
                return numberElementValue < v1 || numberElementValue > v2;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the number value meets the query condition
        /// </summary>
        /// <param name="stringElementValue">String value</param>
        /// <param name="query">Query object</param>
        private bool CheckStringFilterQuery(string stringElementValue, Dictionary<string, ValueObject> query)
        {
            var dataElementValueToLower = stringElementValue.ToLower();
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
           ref List<Aggregation> response, Dictionary<string, object> keys = null)
        {
            if (fields.Count < 1)
            {
                return;
            }
            var field = fields[0];
            var fieldsSkipped = fields.Skip(1).ToList();
            var groupsByField = GroupBy(field.UniqueName, field.Type);
            foreach (var group in groupsByField)
            {
                var subdata = new DataSlice(group.Value.ToArray(), _indexesCount);
                var item = subdata.CalcValues(values);
                if (item.Values.Count != 0)
                {
                    item.Keys = keys != null ? new Dictionary<string, object>(keys) : new Dictionary<string, object>();
                    item.Keys.Add(field.UniqueName, group.Key);
                    response.Add(item);
                    subdata.CalcByFields(fieldsSkipped, fieldsInColumns, values, ref response, item.Keys);
                }
            }

            if ((fieldsInColumns != null) && fieldsInColumns.Count > 0 && fields.Count > fieldsInColumns.Count)
            {
                var colField = fieldsInColumns[0];
                var colsSkipped = fieldsInColumns.Skip(1).ToList();
                var colGroupsByField = GroupBy(colField.UniqueName, colField.Type);
                foreach (var group in colGroupsByField)
                {
                    var subdata = new DataSlice(group.Value.ToArray(), _indexesCount);
                    var item = subdata.CalcValues(values);
                    if (item.Values.Count != 0)
                    {
                        item.Keys = keys != null ? new Dictionary<string, object>(keys) : new Dictionary<string, object>();
                        item.Keys.Add(colField.UniqueName, group.Key);
                        response.Add(item);
                        subdata.CalcByFields(colsSkipped, null, values, ref response, item.Keys);
                    }
                }
            }
        }

        /// <summary>
        /// Groups data by <paramref name="fieldName"></paramref>
        /// </summary>
        /// <param name="fieldName">Field's name</param>
        /// <param name="type">Field's type</param>
        /// <returns>Collection of groups</returns>
        private Dictionary<object, List<int>> GroupBy(string fieldName, ColumnType type)
        {
            Dictionary<object, List<int>> groups = new Dictionary<object, List<int>>();
            if (type == ColumnType.stringType)
            {
                var column = Data.GetColumn<string>(fieldName);
                foreach (var index in DataColumnIndexes)
                {
                    var value = column[index] == null ? "" : column[index];
                    if (!groups.ContainsKey(value))
                    {
                        groups.Add(value, new List<int>());
                    }
                    groups[value].Add(index);
                }
            }
            else if (type == ColumnType.doubleType || type == ColumnType.dateType)
            {
                var column = Data.GetColumn<double?>(fieldName);
                foreach (var index in DataColumnIndexes)
                {
                    object value = null;
                    if (column[index] != null)
                    {
                        value = column[index];
                    }
                    else
                    {
                        value = "";
                    }
                    if (!groups.ContainsKey(value))
                    {
                        groups.Add(value, new List<int>());
                    }
                    groups[value].Add(index);
                }
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
            if (field.Type == ColumnType.stringType)
            {
                var validDataColumnIndexes = DataColumnIndexes.Where(index => Data.GetColumn<string>(field.UniqueName)[index] != null).DefaultIfEmpty(-1).ToArray();
                if (validDataColumnIndexes[0] == -1)
                {
                    return 0;
                }
                if (func == "count")
                {
                    return validDataColumnIndexes.Count();
                }
                if (func == "distinctcount")
                {
                    return validDataColumnIndexes.Select(index => Data.GetColumn<string>(field.UniqueName)[index]).Distinct().ToList().Count;
                }
            }
            if (field.Type == ColumnType.doubleType || field.Type == ColumnType.dateType)
            {
                var validDataColumnIndexes = DataColumnIndexes.Where(index => Data.GetColumn<double?>(field.UniqueName)[index].HasValue).DefaultIfEmpty(-1).ToArray();
                var column = Data.GetColumn<double?>(field.UniqueName);
                if (validDataColumnIndexes[0] == -1)
                {
                    return double.NaN;
                }
                if (func == "count")
                {
                    return validDataColumnIndexes.Count();
                }
                if (func == "distinctcount")
                {
                    return validDataColumnIndexes.Select(index => column[index]).Distinct().ToList().Count;
                }
                if (func == "sum" || func == "none")
                {
                    return validDataColumnIndexes.Sum(index => column[index].Value);
                }
                if (func == "average")
                {
                    return validDataColumnIndexes.Average(index => column[index].Value);
                }
                if (func == "min")
                {
                    return validDataColumnIndexes.Min(index => column[index].Value);
                }
                if (func == "max")
                {
                    return validDataColumnIndexes.Max(index => column[index].Value);
                }
            }
            return 0;
        }
    }
}