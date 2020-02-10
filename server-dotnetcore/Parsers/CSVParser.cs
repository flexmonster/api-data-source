using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetCoreServer.Models;


namespace NetCoreServer.Parsers
{
    public class CSVParser : IParser
    {
        private readonly CSVSerializerOptions _serializerOptions;
        private readonly string _fullFilePath;
        private const ushort CHUNK_SIZE = ushort.MaxValue;


        private Dictionary<string, List<Value>> _dataBlock;
        private int index;

        private Dictionary<int, string> _columnNames;
        private Dictionary<int, Func<string, Value>> _columnTypesConvertion;
        public CSVParser(string index, CSVSerializerOptions serializerOptions)
        {
            _fullFilePath = $"./data/{index}.csv";
            _serializerOptions = serializerOptions;
            _columnNames = new Dictionary<int, string>();
            _columnTypesConvertion = new Dictionary<int, Func<string, Value>>();
        }
        public IEnumerable<Dictionary<string, List<Value>>> Parse()
        {

            using (StreamReader reader = File.OpenText(_fullFilePath))
            {
                string line = "";
                index = 0;
                string headerLine = reader.ReadLine();
                List<string> readingChunk = new List<string>(CHUNK_SIZE);

                // first lines are required to detect data types
                for (int i = 0; i < 10; i++)
                {
                    line = reader.ReadLine();
                    if (line != null) readingChunk.Add(line);
                    index++;
                }
                // parse headers
                ParseHeader(headerLine, readingChunk);

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length > 0)
                    {
                        readingChunk.Add(line);
                        index++;
                        if (index % CHUNK_SIZE == 0)
                        {
                            ParseBlock(readingChunk);
                            readingChunk = new List<string>();
                            yield return _dataBlock;

                            var dataBlockColumns = _dataBlock.Keys.ToList();
                            foreach (var column in dataBlockColumns)
                            {
                                _dataBlock[column] = new List<Value>();
                            }
                        }
                    }
                }

                ParseBlock(readingChunk);
                readingChunk = new List<string>();
                yield return _dataBlock;
            }
        }
        /// <summary>
        /// Parse header to get names
        /// </summary>
        /// <param name="header">Header</param>
        /// <param name="firstLines">First lines to detect type</param>
        private void ParseHeader(string header, List<string> firstLines)
        {
            var columnNames = header.Split(_serializerOptions.FieldSeparator);
            string[][] lines = new string[firstLines.Count][];
            int i = 0;
            firstLines.ForEach(line =>
            {
                lines[i] = line.Split(_serializerOptions.FieldSeparator);
                i++;
            });
            _dataBlock = new Dictionary<string, List<Value>>();
            i = 0;
            foreach (var columnName in columnNames)
            {
                List<string> column = new List<string>(firstLines.Count);
                for (int j = 0; j < firstLines.Count; j++)
                {
                    column.Add(lines[j][i]);
                }
                _dataBlock.Add(columnName, new List<Value>());
                _columnNames.Add(i, columnName);
                _columnTypesConvertion.Add(i, DetectType(column));
                i++;
            }

        }

        /// <summary>
        /// Detect Type
        /// </summary>
        /// <param name="column">Values from one column to detect type</param>
        /// <returns></returns>
        private Func<string, Value> DetectType(List<string> column)
        {
            int dateCount = 0;
            int numberCount = 0;
            int stringCount = 0;
            foreach (var value in column)
            {
                if (DateTime.TryParse(value, out _))
                {
                    dateCount++;
                }
                else if (Double.TryParse(value, out _))
                {
                    numberCount++;
                }
                else
                {
                    stringCount++;
                }
            }
            if (dateCount >= numberCount)
            {
                if (dateCount >= stringCount)
                    return (value) =>
                    {
                        if (!string.IsNullOrEmpty(value))
                            return new Value(Convert.ToDateTime(value));
                        return new Value("");
                    };
            }
            else if (numberCount >= stringCount)
            {
                return (value) =>
                {
                    if (!string.IsNullOrEmpty(value))
                        return new Value(Convert.ToDouble(value));
                    return new Value("");
                };
            }
            return (value) => { return new Value(Convert.ToString(value)); };
        }

        /// <summary>
        /// Parse block of data
        /// </summary>
        /// <param name="lines">Rows to be parsed</param>
        private void ParseBlock(List<string> lines)
        {
            lines.ForEach(line =>
            {
                ParseLine(line);
            });
        }
        private void ParseLine(string line)
        {
            bool isQuote = false;
            char char_;
            string value = "";
            int length = line.Length;
            int currentWord = 0;
            for (int i = 0; i < length; i++)
            {
                char_ = line[i];
                if (char_ == _serializerOptions.FieldEnclosureToken)
                {
                    isQuote = !isQuote;
                }
                else
                if (char_ == _serializerOptions.FieldSeparator && !isQuote)
                {
                    _dataBlock[_columnNames[currentWord]].Add(_columnTypesConvertion[currentWord].Invoke(value));
                    currentWord++;
                    value = "";
                }
                else
                {
                    value += char_;
                }
            }
            _dataBlock[_columnNames[currentWord]].Add(_columnTypesConvertion[currentWord].Invoke(value));
            if (currentWord + 1 < _dataBlock.Count)
            {
                for (int i = currentWord + 1; i < _dataBlock.Count; i++)
                {
                    _dataBlock[_columnNames[i]].Add(new Value(""));
                }
            }
        }

    }
}
