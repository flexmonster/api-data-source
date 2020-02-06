using NetCoreServer.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NetCoreServer.Parsers
{
    public class JSONParser : IParser
    {
        private readonly string _fullFilePath;
        private readonly JsonSerializerOptions _serializerOptions;
        public JSONParser(string index, JsonSerializerOptions serializerOptions)
        {
            _fullFilePath = $"./data/{index}.json";
            _serializerOptions = serializerOptions;
        }
        public IEnumerable<Dictionary<string, List<Value>>> Parse()
        {
            using FileStream fileStream = new FileStream(_fullFilePath, FileMode.Open);
            List<Dictionary<string, Value>> dataRows = JsonSerializer.DeserializeAsync<List<Dictionary<string, Value>>>(fileStream, _serializerOptions).Result;
            yield return ToColumnView(dataRows);
        }

        /// <summary>
        /// Change list of rows to list of columns
        /// </summary>
        /// <param name="dataValues">List of rows</param>
        /// <returns></returns>
        private Dictionary<string, List<Value>> ToColumnView(List<Dictionary<string, Value>> dataValues)
        {
            var columnView = new Dictionary<string, List<Value>>();
            if (dataValues != null && dataValues.Count > 0)
            {
                var firstElement = dataValues[0];
                foreach (var field in firstElement)
                {
                    columnView.Add(field.Key, new List<Value>(dataValues.Count));
                }
                int i = 0;
                foreach (var dataElement in dataValues)
                {
                    foreach (var field in dataElement)
                    {
                        columnView.TryGetValue(field.Key, out List<Value> value);
                        if (value != null) value.Add(field.Value);
                    }
                    i++;
                }
            }
            ReplaceNullsWithEmptyValues(ref columnView);
            return columnView;
        }

        private void ReplaceNullsWithEmptyValues(ref Dictionary<string, List<Value>> columnView)
        {
            foreach (var column in columnView)
            {
                int i = 0;
                foreach (var value in column.Value)
                {
                    if (value == null)

                    {
                        columnView[column.Key][i] = new Value("");
                    }
                    i++;
                }
            }
        }
    }
}
