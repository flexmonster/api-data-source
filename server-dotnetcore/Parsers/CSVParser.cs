using NetCoreServer.Extensions;
using NetCoreServer.Models;
using NetCoreServer.Models.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetCoreServer.Parsers
{
    public class CSVParser : IParser
    {
        private readonly CSVSerializerOptions _serializerOptions;
        private readonly string _fullFilePath;
        private const ushort CHUNK_SIZE = ushort.MaxValue;

        private Dictionary<string, dynamic> dataBlock;
        private int index;
        private Dictionary<int, string> _columnsNames;
        private Dictionary<string, ColumnType> _dataTypes;

        public Dictionary<string, ColumnType> DataTypes
        {
            get
            {
                return _dataTypes;
            }
        }

        public CSVParser(string path, CSVSerializerOptions serializerOptions)
        {
            _fullFilePath = path;
            _columnsNames = new Dictionary<int, string>();
            _dataTypes = new Dictionary<string, ColumnType>();
            _serializerOptions = serializerOptions;
        }

        public IEnumerable<Dictionary<string, dynamic>> Parse()
        {
            using (StreamReader reader = File.OpenText(_fullFilePath))
            {
                string line = "";
                index = 0;
                string headerLine = reader.ReadLine();
                List<string> readingChunk = new List<string>(CHUNK_SIZE);

                // first lines are required to detect data types
                for (int i = 0; i < 1; i++)
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
                            yield return dataBlock;

                            var dataBlockColumns = dataBlock.Keys.ToList();
                            int i = 0;
                            foreach (var columnName in dataBlockColumns)
                            {
                                if (_dataTypes[columnName] == ColumnType.stringType)
                                {
                                    dataBlock[columnName] = new List<string>();
                                }
                                else
                                {
                                    dataBlock[columnName] = new List<double?>();
                                }
                                i++;
                            }
                        }
                    }
                }

                ParseBlock(readingChunk);
                readingChunk = new List<string>();
                yield return dataBlock;
            }
        }

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
            dataBlock = new Dictionary<string, dynamic>();
            i = 0;
            foreach (var columnName in columnNames)
            {
                List<string> column = new List<string>(firstLines.Count);
                for (int j = 0; j < firstLines.Count; j++)
                {
                    column.Add(lines[j][i]);
                }
                _dataTypes.Add(columnName, DetectType(column));
                _columnsNames.Add(i, columnName);
                if (_dataTypes[columnName] == ColumnType.stringType)
                {
                    dataBlock.Add(columnName, new List<string>());
                }
                else
                {
                    dataBlock.Add(columnName, new List<double?>());
                }
                i++;
            }
        }

        private ColumnType DetectType(List<string> column)
        {
            int dateCount = 0;
            int numberCount = 0;
            int stringCount = 0;
            int emptyCount = 0;
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
                else if (value != "")
                {
                    stringCount++;
                }
                else
                {
                    emptyCount++;
                }
            }
            if (dateCount >= stringCount)
            {
                if (dateCount >= numberCount)
                {
                    return ColumnType.dateType;
                }
            }
            if (stringCount > 0)
            {
                return ColumnType.stringType;
            }
            if (numberCount > 0)
            {
                return ColumnType.doubleType;
            }
            return ColumnType.stringType;
        }

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
                    if (_dataTypes[_columnsNames[currentWord]] == ColumnType.doubleType)
                    {
                        if (double.TryParse(value, out double convertedValue))
                            dataBlock[_columnsNames[currentWord]].Add(convertedValue);
                        else
                            dataBlock[_columnsNames[currentWord]].Add((double?)null);
                    }
                    else if (_dataTypes[_columnsNames[currentWord]] == ColumnType.dateType)
                    {
                        if (DateTime.TryParse(value, out DateTime convertedValue))
                            dataBlock[_columnsNames[currentWord]].Add(convertedValue.ToUnixTimestamp());
                        else
                            dataBlock[_columnsNames[currentWord]].Add((double?)null);
                    }
                    else
                    {
                        if (value != "")
                            dataBlock[_columnsNames[currentWord]].Add(value);
                        else
                            dataBlock[_columnsNames[currentWord]].Add((string)null);
                    }
                    currentWord++;
                    value = "";
                }
                else
                {
                    value += char_;
                }
            }
            if (_dataTypes[_columnsNames[currentWord]] == ColumnType.doubleType)
            {
                if (double.TryParse(value, out double convertedValue))
                    dataBlock[_columnsNames[currentWord]].Add(convertedValue);
                else
                    dataBlock[_columnsNames[currentWord]].Add((double?)null);
            }
            else if (_dataTypes[_columnsNames[currentWord]] == ColumnType.dateType)
            {
                if (DateTime.TryParse(value, out DateTime convertedValue))
                    dataBlock[_columnsNames[currentWord]].Add(convertedValue.ToUnixTimestamp());
                else
                    dataBlock[_columnsNames[currentWord]].Add((double?)null);
            }
            else
            {
                if (value != "")
                    dataBlock[_columnsNames[currentWord]].Add(value);
                else
                    dataBlock[_columnsNames[currentWord]].Add((string)null);
            }
            if (currentWord + 1 < dataBlock.Count)
            {
                for (int i = currentWord + 1; i < dataBlock.Count; i++)
                {
                    if (_dataTypes[_columnsNames[i]] == ColumnType.doubleType)
                    {
                        dataBlock[_columnsNames[i]].Add((double?)null);
                    }
                    else
                    {
                        dataBlock[_columnsNames[i]].Add((string)null);
                    }
                }
            }
        }
    }
}