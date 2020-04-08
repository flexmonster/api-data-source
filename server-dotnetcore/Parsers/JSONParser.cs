using NetCoreServer.JsonConverters;
using NetCoreServer.Models.DataModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NetCoreServer.Parsers
{
    public class JSONParser : IParser
    {
        private const int CHUNK_SIZE = int.MaxValue / 4;
        private readonly string _fullFilePath;
        private readonly JsonSerializerOptions _serializerOptions;
        private byte[] _buffer = new byte[CHUNK_SIZE];
        private Dictionary<string, dynamic> _dataBlock = new Dictionary<string, dynamic>();
        private Lazy<Dictionary<string, ColumnType>> _dataTypes;
        private int _offset = 0;
        private int inBuffer = 0;
        public JSONParser(string path, JsonSerializerOptions serializerOptions)
        {
            _fullFilePath = path;
            _serializerOptions = serializerOptions;
            _dataTypes = new Lazy<Dictionary<string, ColumnType>>(() => (_serializerOptions.Converters[0] as DataJsonConverter).GetTypes());
        }
        public Dictionary<string, ColumnType> DataTypes
        {
            get
            {
                return _dataTypes.Value;
            }
        }

        public IEnumerable<Dictionary<string, dynamic>> Parse()
        {
            using (FileStream fs = new FileStream(_fullFilePath, FileMode.Open, FileAccess.Read))
            {
                bool canRead = true;
                bool isFirst = true;
                while (canRead)
                {
                    byte[] chunk = new byte[CHUNK_SIZE];
                    if (isFirst)
                    {
                        _offset = 0;
                    }
                    else
                    {
                        _offset = 1;
                        chunk[0] = 91;
                        while (inBuffer != 0)
                        {
                            inBuffer--;
                            chunk[_offset] = _buffer[inBuffer];
                            _offset++;
                        }
                    }
                    var count = fs.Read(chunk, _offset, CHUNK_SIZE - _offset - 1);
                    isFirst = false;
                    if (count < CHUNK_SIZE - _offset - 1) canRead = false;
                    for (int i = count + _offset - 1; i > 0; i--)
                    {
                        if (chunk[i] != 125)
                        {
                            _buffer[inBuffer] = chunk[i];
                            inBuffer++;
                        }
                        else
                        {
                            chunk[i + 1] = 93;
                            break;
                        }
                    }
                    if (_offset > 0)
                    {
                        for (int i = 1; i < CHUNK_SIZE; i++)
                        {
                            if (chunk[i] != 123)
                            {
                                chunk[i] = 32;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    ReadOnlySequence<byte> bytesSpan = new ReadOnlySequence<byte>(chunk);
                    Utf8JsonReader jsonReader = new Utf8JsonReader(bytesSpan);
                    Dictionary<string, dynamic> dataRows = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(ref jsonReader, _serializerOptions);
                    yield return dataRows;
                }
            }
            yield return _dataBlock;
        }
    }
}