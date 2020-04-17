using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NetCoreServer.Comparators;
using NetCoreServer.DataStorages;
using NetCoreServer.Extensions;
using NetCoreServer.JsonConverters;
using NetCoreServer.Models;
using NetCoreServer.Models.DataModels;
using NetCoreServer.Models.Fields;
using NetCoreServer.Models.Handshake;
using NetCoreServer.Models.Members;
using NetCoreServer.Models.Select;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetCoreServer.Controllers
{
    [ApiController]
    public class CubeController : ControllerBase
    {
        private const string API_VERSION = "2.8.5";
        private const int MEMBERS_PAGE_SIZE = 50000;
        private const int SELECT_PAGE_SIZE = 50000;

        private readonly IMemoryCache _cache;

        private readonly IDataStorage _dataStorage;

        public CubeController(IMemoryCache cache, IDataStorage dataStorage)
        {
            _cache = cache;
            _dataStorage = dataStorage;
        }

        /// <summary>
        /// Handshake requst
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/api/cube/handshake")]
        [HttpPost]
        public IActionResult Handshake([FromBody]HandshakeRequst request)
        {
            object response = null;
            if (request.Type == RequestType.Handshake)
            {
                response = new { version = API_VERSION };
            }
            return new JsonResult(response);
        }

        /// <summary>
        /// Fields requst
        /// </summary>
        /// <param name="request">requst</param>
        /// <returns></returns>
        [Route("/api/cube/fields")]
        [HttpPost]
        public async Task<IActionResult> PostFields([FromBody]FieldsRequest request)
        {
            object response = null;
            if (request.Index == null)
            {
                Response.StatusCode = 400;
                return new JsonResult("Index property is missing.");
            }
            if (request.Type == RequestType.Fields)
            {
                try
                {
                    response = await GetShema(request.Index);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Response.StatusCode = 500;
                    return Content(e.Message);
                }
            }
            if (response == null)
            {
                Response.StatusCode = 400;
                return new JsonResult("Incorrect request for this endpoint.");
            }
            return new JsonResult(response, new JsonSerializerOptions { IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new ColumnTypeJsonConverter() } });
        }

        /// <summary>
        /// Members request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/api/cube/members")]
        [HttpPost]
        public async Task<IActionResult> PostMembers([FromBody]MembersRequest request)
        {
            if (request.Index == null)
            {
                Response.StatusCode = 400;
                return new JsonResult("Index property is missing.");
            }
            if (request.Type == RequestType.Members)
            {
                try
                {
                    var response = await GetMembers(request.Index, request.Page, request.Field);
                    return Content(response, "application/json");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Response.StatusCode = 500;
                    return new JsonResult(e.Message);
                }
            }
            Response.StatusCode = 400;
            return Content("Incorrect request for this endpoint.");
        }

        /// <summary>
        /// Select request
        /// </summary>
        /// <param name="request">requst</param>
        /// <returns></returns>
        [Route("/api/cube/select")]
        [HttpPost]
        public async Task<IActionResult> PostSelect([FromBody]SelectRequest request)
        {
            string response = null;

            if (request.Index == null)
            {
                Response.StatusCode = 400;
                return new JsonResult("Index property is missing");
            }
            if (request.Type == RequestType.Select)
            {
                try
                {
                    response = await SelectData(request.Index, request.Query, request.Page);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Response.StatusCode = 500;
                    return Content("Server error");
                }
            }
            if (response == null)
            {
                Response.StatusCode = 400;
                return new JsonResult("Incorrect request for this endpoint.");
            }
            return Content(response);
        }

        /// <summary>
        /// Load schema and create object based on it
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        private async Task<Schema> GetShema(string index)
        {
            return await _cache.GetOrCreateAsync(index + "schema",
                async (cacheEntry) =>
                {
                    cacheEntry.SetSize(1);
                    cacheEntry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(240);
                    Schema schema = new Schema();
                    schema.Sorted = false;
                    schema.Aggregations.Any = new List<string> { "count", "distinctcount" };
                    schema.Aggregations.Date = new List<string> { "count", "distinctcount", "min", "max" };
                    schema.Aggregations.Number = new List<string> { "sum", "average", "count", "distinctcount", "min", "max" };
                    schema.Filters.Any.Members = true;
                    schema.Filters.Any.Query = true;
                    schema.Filters.Any.ValueQuery = true;
                    var data = await LoadData(index);
                    var firstElement = data.GetNameAndTypes();
                    foreach (var elem in firstElement)
                    {
                        FieldModel field = new FieldModel();
                        field.UniqueName = elem.Key;
                        field.Type = elem.Value;
                        schema.Fields.Add(field);
                    }
                    return schema;
                });
        }

        /// <summary>
        /// Load data from file {index}.json
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        private async Task<IDataStructure> LoadData(string index)
        {
            return await _dataStorage.GetOrAddAsync(index);
        }

        private async Task<string> GetMembers(string index, int page, FieldModel field)
        {
            return (await _cache.GetOrCreateAsync(index + field.UniqueName,
                   async (cacheEntry) =>
                   {
                       cacheEntry.SetSize(1);
                       JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new MembersResponseJsonConverter() } };
                       IDataStructure data = await LoadData(index);
                       var namesAndTypes = data.GetNameAndTypes();
                       DataSlice dataSlice = new DataSlice(data);
                       List<object> members = null;
                       bool sorted = false;
                       if (namesAndTypes[field.UniqueName] == ColumnType.stringType)
                       {
                           var column = DataSlice.Data.GetColumn<string>(field.UniqueName);
                           var stringMembers = dataSlice.DataColumnIndexes.Select(index => column[index]).Distinct().ToList();
                           if (stringMembers.Count != 0)
                           {
                               var first = stringMembers.First();
                               if (Enum.TryParse(first.ToString(), out Month m) || Enum.TryParse(first.ToString(), out ShortMonth m1))
                               {
                                   stringMembers.Sort(new MonthComparator<string>());
                                   sorted = true;
                               }
                           }
                           members = stringMembers.ConvertAll<object>(new Converter<string, object>(str => (object)str));
                       }
                       else
                       {
                           var column = DataSlice.Data.GetColumn<double?>(field.UniqueName);
                           members = dataSlice.DataColumnIndexes.Select(index => column[index] as object).Distinct().ToList();
                       }

                       int pageTotal = (int)Math.Ceiling(members.Count / (double)MEMBERS_PAGE_SIZE);
                       pageTotal = pageTotal == 0 ? 1 : pageTotal;
                       string[] responses = new string[pageTotal];
                       int currentPage = 0;
                       while (currentPage < pageTotal)
                       {
                           MembersResponse response = new MembersResponse();
                           response.Sorted = sorted;
                           response.Page = currentPage;
                           response.PageTotal = pageTotal;
                           int from = currentPage * MEMBERS_PAGE_SIZE;
                           int size = Math.Min(members.Count, from + MEMBERS_PAGE_SIZE);
                           for (int i = from; i < size; i++)
                           {
                               response.Members.Add(members[i]);
                           }
                           responses[currentPage] = JsonSerializer.Serialize(response, options);
                           currentPage++;
                       }
                       return responses;
                   }))[page];
        }

        /// <summary>
        /// Gets field's members
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="page">page number to load</param>
        /// <param name="field">field's name</param>
        /// <returns></returns>
        /// <summary>
        /// Select data according to query
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="query">query</param>
        /// <param name="page">page number to load</param>
        /// <returns></returns>
        private async Task<string> SelectData(string index, Query query, int page)
        {
            var hash = CalculateMD5Hash(index + JsonSerializer.Serialize(query));
            return (await _cache.GetOrCreateAsync(hash,
              async (cacheEntry) =>
              {
                  cacheEntry.SetSize(1);
                  var rawData = await LoadData(index);
                  var nameTypes = rawData.GetNameAndTypes();
                  DataSlice data = new DataSlice(rawData);
                  SelectResponse response = new SelectResponse();
                  string[] responses = null;
                  JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new SelectResponseJsonConverter() } };
                  if (query.Filter != null)
                  {
                      data.FilterData(query.Filter);
                  }
                  if (query.Aggs != null && query.Aggs.Values != null)
                  {
                      response.Aggs = new List<Aggregation>();
                      query.Aggs.Values.ForEach(aggvalue => aggvalue.Field.Type = nameTypes[aggvalue.Field.UniqueName]);
                      if (query.Aggs.By != null)
                      {
                          if (query.Aggs.By.Rows == null)
                          {
                              query.Aggs.By.Rows = new List<FieldModel>();
                          }
                          if (query.Aggs.By.Cols == null)
                          {
                              query.Aggs.By.Cols = new List<FieldModel>();
                          }
                          if (data.DataColumnIndexes.Count() != 0)
                          {
                              var tempAggs = new List<Aggregation>();
                              var concatedFields = query.Aggs.By.Rows.Union(query.Aggs.By.Cols).ToList();
                              concatedFields.ForEach(concatedField => concatedField.Type = nameTypes[concatedField.UniqueName]);
                              data.CalcByFields(concatedFields, query.Aggs.By.Cols, query.Aggs.Values, ref tempAggs);
                              response.Aggs = tempAggs;
                              response.Aggs.Add(data.CalcValues(query.Aggs.Values));
                          }
                          else
                          {
                              response.Aggs = new List<Aggregation>();
                          }
                      }
                      else
                      {
                          response.Aggs.Add(data.CalcValues(query.Aggs.Values));
                      }
                      int pageTotal = (int)Math.Ceiling((double)response.Aggs.Count / SELECT_PAGE_SIZE);
                      pageTotal = pageTotal == 0 ? 1 : pageTotal;
                      responses = new string[pageTotal];
                      int currentPage = 0;
                      while (currentPage < pageTotal)
                      {
                          SelectResponse partitialResponse = new SelectResponse();
                          partitialResponse.Page = currentPage;
                          partitialResponse.PageTotal = pageTotal;
                          int from = currentPage * SELECT_PAGE_SIZE;
                          int size = Math.Min(response.Aggs.Count - from, SELECT_PAGE_SIZE);
                          partitialResponse.Aggs = response.Aggs.GetRange(from, size);
                          responses[currentPage] = JsonSerializer.Serialize(partitialResponse, options);
                          currentPage++;
                      }
                  }
                  if (query.Fields != null)
                  {
                      for (int i = 0; i < query.Fields.Count; i++)
                      {
                          query.Fields[i].Type = nameTypes[query.Fields[i].UniqueName];
                          response.Fields.Add(query.Fields[i]);
                      }
                      response.Hits = new List<List<object>>();
                      var limit = query.Limit == 0 ? data.DataColumnIndexes.Count() : Math.Min(query.Limit, data.DataColumnIndexes.Count());

                      for (int i = 0; i < limit; i++)
                      {
                          List<object> row = new List<object>();
                          query.Fields.ForEach(field =>
                          {
                              if (field.Type == ColumnType.stringType)
                              {
                                  row.Add(DataSlice.Data.GetColumn<string>(field.UniqueName)[data.DataColumnIndexes[i]]);
                              }
                              else
                              {
                                  row.Add(DataSlice.Data.GetColumn<double?>(field.UniqueName)[data.DataColumnIndexes[i]]);
                              }
                          });

                          response.Hits.Add(row);
                      }
                      int pageTotal = (int)Math.Ceiling((double)response.Hits.Count / SELECT_PAGE_SIZE);
                      pageTotal = pageTotal == 0 ? 1 : pageTotal;
                      responses = new string[pageTotal];
                      int currentPage = 0;
                      while (currentPage < pageTotal)
                      {
                          SelectResponse partitialResponse = new SelectResponse();
                          partitialResponse.Page = currentPage;
                          partitialResponse.PageTotal = pageTotal;
                          partitialResponse.Fields = response.Fields;
                          partitialResponse.Aggs = response.Aggs;
                          int from = currentPage * SELECT_PAGE_SIZE;
                          int size = Math.Min(response.Hits.Count - from, SELECT_PAGE_SIZE);
                          partitialResponse.Hits = response.Hits.GetRange(from, size);
                          responses[currentPage] = JsonSerializer.Serialize(partitialResponse, options);
                          currentPage++;
                      }
                  }
                  return responses;
              }))[page];
        }



        private string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}