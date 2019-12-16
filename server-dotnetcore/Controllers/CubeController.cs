using NetCoreServer.JsonConverters;
using NetCoreServer.Models;
using NetCoreServer.Models.Fields;
using NetCoreServer.Models.Handshake;
using NetCoreServer.Models.Members;
using NetCoreServer.Models.Select;
using Microsoft.AspNetCore.Mvc;
using NetCoreServer.Comparators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetCoreServer.Controllers
{

    [ApiController]
    public class CubeController : ControllerBase
    {
        private static ConcurrentDictionary<string, Schema> schemaCache = new ConcurrentDictionary<string, Schema>();

        private static ConcurrentDictionary<string, Data> dataCache = new ConcurrentDictionary<string, Data>();

        const int MEMBERS_PAGE_SIZE = 20000;

        const int SELECT_PAGE_SIZE = 100000;

        public CubeController()
        {
        }

        /// <summary>
        /// Handshake requst
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        [Route("/api/cube/handshake")]
        [HttpPost]
        public IActionResult Handshake([FromBody]HandshakeRequst request)
        {
            object response = null;
            if (request.Type == RequestType.Handshake)
            {
                response = new { isSingleEndpointApi = false };
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
        public async Task<IActionResult> PostMembers([FromBody]MembersRequest request)
        {
            MembersResponse response = null;
            JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new MembersResponseJsonConverter() } };
            if (request.Type == RequestType.Members)
            {
                try
                {
                    response = await GetMembers(request.Index, request.Page, request.Field);
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
        public async Task<IActionResult> PostSelect([FromBody]SelectRequest request)
        {
            SelectResponse response = null;

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
                return Content("Incorrect request for this endpoint.");
            }
            return new JsonResult(response, new JsonSerializerOptions { IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new SelectResponseJsonConverter() } });
        }
        /// <summary>
        /// Load schema and create object based on it
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        private async Task<Schema> GetShema(string index)
        {
            if (!schemaCache.ContainsKey(index))
            {

                Schema schema = new Schema();
                schema.Sorted = false;
                schema.Aggregations.Any = new List<string> { "count", "distinctcount" };
                schema.Aggregations.Date = new List<string> { "count", "distinctcount", "min", "max" };
                schema.Aggregations.Number = new List<string> { "sum", "average", "count", "distinctcount", "min", "max" };
                schema.Filters.Any.Members = true;
                schema.Filters.Any.Query = true;
                schema.Filters.Any.ValueQuery = true;
                var data = await LoadData(index, null);
                var firstElement = new Dictionary<string, Value>();
                foreach (var column in data.DataValuesByColumn)
                {
                    firstElement.Add(column.Key, column.Value[0]);
                }
                foreach (var member in firstElement)
                {
                    FieldModel field = new FieldModel();
                    field.Field = member.Key;
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
                schemaCache.TryAdd(index, schema);
            }
            return schemaCache[index];
        }  

        /// <summary>
        /// Load data from file {index}.json
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        private async Task<Data> LoadData(string index, Schema schema)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new ValuesJsonConverter() }
            };
            if (!dataCache.ContainsKey(index))
            {
                string fullFilePath = $"./data/{index}.json";
                using FileStream fileStream = new FileStream(fullFilePath, FileMode.Open);
                List<Dictionary<string, Value>> dataRows = await JsonSerializer.DeserializeAsync<List<Dictionary<string, Value>>>(fileStream, serializerOptions);
                Data data = new Data();
                data.ToColumnView(dataRows, schema);
                dataCache.TryAdd(index, data);
            }
            return dataCache[index];
        }
        private async Task<FieldModel> GetField(string field, string index)
        {
            if (!schemaCache.ContainsKey(index))
            {
                await GetShema(index);
            }
            return schemaCache[index].Fields.Single(elem => elem.Field == field);
        }

        /// <summary>
        /// Gets field's members
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="page">page number to load</param>
        /// <param name="field">field's name</param>
        /// <returns></returns>
        private async Task<MembersResponse> GetMembers(string index, int page, FieldModel field)
        {
            Data data = await LoadData(index, await GetShema(index));
            MembersResponse response = new MembersResponse();
            var members = data.DataValuesByColumn[field.Field].Distinct().ToList();
            if (field.Field == "date_month")
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
        private async Task<SelectResponse> SelectData(string index, Query query, int page)
        {
            DataSlice data = new DataSlice(await LoadData(index, await GetShema(index)));
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
                    response.Fields.Add(GetField(query.Fields[i].Field, index).Result);
                }
                response.Hits = new List<List<Value>>();
                var limit = query.Limit == 0 ? data.DataColumnIndexes.Count() : Math.Min(query.Limit, data.DataColumnIndexes.Count());

                for (int i = 0; i < limit; i++)
                {
                    var row = query.Fields.Select(field => { return DataSlice.Data.DataValuesByColumn[field.Field][data.DataColumnIndexes[i]]; }).ToList();
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