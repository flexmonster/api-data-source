using NetCoreServer.JsonConverters;
using NetCoreServer.Models;
using NetCoreServer.Models.Fields;
using NetCoreServer.Models.Handshake;
using NetCoreServer.Models.Members;
using NetCoreServer.Models.Select;
using NetCoreServer.Comparators;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NetCoreServer.Models.DataModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NetCoreServer.DataLoaders;

namespace NetCoreServer.Controllers
{

    [ApiController]
    public class CubeController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        private readonly IConfiguration _configuration;

        const string API_VERSION = "2.8.0";

        const int MEMBERS_PAGE_SIZE = 50000;

        const int SELECT_PAGE_SIZE = 50000;

        public CubeController(IConfiguration configuration, IMemoryCache cache)
        {
            _cache = cache;
            _configuration = configuration;
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
        public IActionResult PostFields([FromBody]FieldsRequest request)
        {
            object response = null;
            if (request.Type == RequestType.Fields)
            {
                try
                {
                    response = GetShema(request.Index);
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
                return Content("Incorrect request for this endpoint.");
            }
            return new JsonResult(response, new JsonSerializerOptions { IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        /// <summary>
        /// Members request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/api/cube/members")]
        [HttpPost]
        public IActionResult PostMembers([FromBody]MembersRequest request)
        {
            MembersResponse response = null;
            JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new MembersResponseJsonConverter() } };
            if (request.Type == RequestType.Members)
            {
                try
                {
                    response = GetMembers(request.Index, request.Page, request.Field);
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
                return Content("Incorrect request for this endpoint.");
            }
            return new JsonResult(response, options);

        }

        /// <summary>
        /// Select request
        /// </summary>
        /// <param name="request">requst</param>
        /// <returns></returns>
        [Route("/api/cube/select")]
        [HttpPost]
        public IActionResult PostSelect([FromBody]SelectRequest request)
        {
            SelectResponse response = null;

            if (request.Type == RequestType.Select)
            {
                try
                {
                    response = SelectData(request.Index, request.Query, request.Page);
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
                return Content("Incorrect request for this endpoint.");
            }
            return new JsonResult(response, new JsonSerializerOptions { IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new SelectResponseJsonConverter() } });
        }
        /// <summary>
        /// Load schema and create object based on it
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        private Schema GetShema(string index)
        {
            return _cache.GetOrCreate(index + "schema",
                (cacheEntry) =>
                {
                    cacheEntry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(240);
                    Schema schema = new Schema();
                    schema.Sorted = false;
                    schema.Aggregations.Any = new List<string> { "count", "distinctcount" };
                    schema.Aggregations.Date = new List<string> { "count", "distinctcount", "min", "max" };
                    schema.Aggregations.Number = new List<string> { "sum", "average", "count", "distinctcount", "min", "max" };
                    schema.Filters.Any.Members = true;
                    schema.Filters.Any.Query = true;
                    schema.Filters.Any.ValueQuery = true;
                    var data = LoadData(index);
                    var firstElement = data.GetRow(0);
                    foreach (var member in firstElement)
                    {
                        FieldModel field = new FieldModel();
                        field.UniqueName = member.Key;
                        if (member.Value.StringValue != null)
                        {
                            field.Type = "string";
                        }
                        else if (member.Value.DateValue != null)
                        {
                            field.Type = "date";
                        }
                        else
                        {
                            field.Type = "number";
                        }
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
        private IDataStructure LoadData(string index)
        {
            return _cache.GetOrCreate(index,
                (cacheEntry) =>
                {
                    cacheEntry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(60);
                    using (ParserFactory parserFactory = new ParserFactory(_configuration, index))
                    {
                        var dataSourceType = _configuration.GetValue<DataSourceType>("DataSource:DataSourceName");
                        var parser = parserFactory.CreateParser(dataSourceType);
                        DataLoader dataLoader = new DataLoader(parser);
                        return dataLoader.Load();
                    }
                });
        }
        /// <summary>
        /// Gets field's members
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="page">page number to load</param>
        /// <param name="field">field's name</param>
        /// <returns></returns>
        private MembersResponse GetMembers(string index, int page, FieldModel field)
        {
            IDataStructure data = LoadData(index);
            MembersResponse response = new MembersResponse();
            var members = data.GetColumn(field.UniqueName).Distinct().ToList();
            if (Enum.TryParse(members.First().StringValue, out Month _) || Enum.TryParse(members.First().StringValue, out ShortMonth _))
            {
                members.Sort(new MonthComparator());
                response.Sorted = true;
            }
            int pageTotal = (int)Math.Ceiling(members.Count / (double)MEMBERS_PAGE_SIZE);
            if (pageTotal > 1)
            {
                response.Page = (page == 0) ? 0 : page;
                response.PageTotal = pageTotal;
            }
            int from = page * MEMBERS_PAGE_SIZE;
            int size = Math.Min(members.Count, from + MEMBERS_PAGE_SIZE);

            for (int i = from; i < size; i++)
            {
                response.Members.Add(members[i]);
            }
            return response;
        }

        /// <summary>
        /// Select data according to query
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="query">query</param>
        /// <param name="page">page number to load</param>
        /// <returns></returns>
        private SelectResponse SelectData(string index, Query query, int page)
        {
            DataSlice data = new DataSlice(LoadData(index));
            SelectResponse response = new SelectResponse();
            if (query.Filter != null)
            {
                data.FilterData(query.Filter);
            }
            if (query.Aggs != null && query.Aggs.Values != null)
            {
                response.Aggs = new List<Aggregation>();
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
                if (pageTotal > 1)
                {
                    response.Page = (page == 0) ? 0 : page;
                    response.PageTotal = pageTotal;
                    int from = page * SELECT_PAGE_SIZE;
                    int size = Math.Min(response.Aggs.Count - from, SELECT_PAGE_SIZE);
                    response.Aggs = response.Aggs.GetRange(from, size);
                }
            }
            if (query.Fields != null)
            {
                for (int i = 0; i < query.Fields.Count; i++)
                {
                    response.Fields.Add(query.Fields[i]);
                }
                response.Hits = new List<List<Value>>();
                var limit = query.Limit == 0 ? data.DataColumnIndexes.Count() : Math.Min(query.Limit, data.DataColumnIndexes.Count());

                for (int i = 0; i < limit; i++)
                {
                    var row = query.Fields.Select(field => { return DataSlice.Data.GetValue(field.UniqueName, data.DataColumnIndexes[i]); }).ToList();
                    response.Hits.Add(row);
                }

                int pageTotal = (int)Math.Ceiling((double)response.Hits.Count / MEMBERS_PAGE_SIZE);
                if (pageTotal > 1)
                {
                    response.Page = (page == 0) ? 0 : page;
                    response.PageTotal = pageTotal;
                    int from = page * MEMBERS_PAGE_SIZE;
                    int size = Math.Min(response.Hits.Count, from + MEMBERS_PAGE_SIZE);
                    response.Hits = response.Hits.GetRange(from, size);
                }
            }
            return response;
        }
    }
}