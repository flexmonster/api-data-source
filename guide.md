# Quick guide to custom data source API implementation

This is a step by step guide to implement your own custom data source API server.

- [Quick guide to custom data source API implementation](#quick-guide-to-custom-data-source-api-implementation)
  - [Step 1. Create an endpoint to handle POST requests](#step-1-create-an-endpoint-to-handle-post-requests)
  - [Step 2. Configure Flexmonster report](#step-2-configure-flexmonster-report)
  - [Step 3. Handle the first request for the data structure](#step-3-handle-the-first-request-for-the-data-structure)
  - [Step 4. Handle requests for members](#step-4-handle-requests-for-members)
  - [Step 5. Handle requests for aggregated data](#step-5-handle-requests-for-aggregated-data)
  - [Step 6. (more advanced) Implement filters](#step-6-more-advanced-implement-filters)
  - [Step 7. (more advanced) Return data for the drill-through view](#step-7-more-advanced-return-data-for-the-drill-through-view)
  - [Step 8. (more advanced) Support more aggregation functions](#step-8-more-advanced-support-more-aggregation-functions)

## Step 1. Create an endpoint to handle POST requests

Flexmonster sends a sequence of POST requests to the API endpoint in JSON format. The first step in API implementation is to create an endpoint on your server that will be ready to handle these POST requests.

Note: If Flexmonster Pivot Table is running on a different server, enable CORS.

It is a good idea to check our [sample Node.js server](readme.md#sample-nodejs-server) that implements Flexmonster's custom data source API for a reference. 

## Step 2. Configure Flexmonster report

When the endpoint is ready to receive POST requests from Flexmonster, configure Flexmonster's `report.dataSource` to connect to your custom data source API as follows::
```typescript
"report": {
    "dataSource": {
        "dataSourceType": "api",
        "url": "http://localhost:3400",
        "index": "data-set-123"
    }
}
```
where `url` is your API endpoint and `index` is the identifier of your data set. `index` will be sent with each request.

## Step 3. Handle the first request for the data structure

All requests have `index` and `type` properties in the request body. There are 3 types of requests that can be distinguished by `type` value: 
- `fields` - return all fields with their types (i.e., meta object or schema)
- `members` - return all members of the field
- `select` - return data

The first request that is sent to the endpoint by Flexmonster after configuring the connection is the fields request. Read more details about [fields request](spec.md#21-fields-request) in the documentation and implement response to it for your data set.

Custom data source API supports 3 field types: `"string"`, `"number"`, `"date"`. 
Note, that at least one field in the response should have `aggregations` defined. For example, `"sum"`:
```json
    {
        "field": "Quantity",
        "type": "number",
        "aggregations": ["sum"]
    }
```
This means that the backend will provide aggregated data for this field and it can be selected as a measure in Flexmonster Pivot Table on the client-side.

When the response to the fields request is successfully received by Flexmonster Pivot Table, the Field List with all available fields is shown.

## Step 4. Handle requests for members

The next request to handle is the request for members for the field.

Read more details about [members request](spec.md#22-members-request) in the documentation and implement response for your data set.

Now in the Field List, you will be able to select a string field for rows or for columns and retrieve their members.

## Step 5. Handle requests for aggregated data

When in the Field List a field is selected for rows and/or columns and a numeric field is selected for measures, the [select request](spec.md#23-select-request-for-pivot-table) is sent to the endpoint.

In this step `query.aggs` part of the request should be implemented (select request also has `query.filter` and `query.fields`, but they can be skipped for now):
```typescript
{
    "type": "select"
    "index": string,
    "query": {
        "aggs": {
            "values"[]: {
                "field": string,
                "func": string
            },
            "by": {
                "rows": string[],
                "cols": string[]
            }
        }
    }
}
```

When the responce to this kind of select request is successfully received by Flexmonster, the pivot table is shown.

## Step 6. (more advanced) Implement filters

Filtering is a part of [select requests](spec.md#23-select-request-for-pivot-table).

In this step `query.filter` part of the request should be implemented.

## Step 7. (more advanced) Return data for the drill-through view

[Select request for the drill-through view](spec.md#25-select-request-for-drill-through-view) is also a kind of select requests.

In this step `query.fields` part of the request should be implemented.

## Step 8. (more advanced) Support more aggregation functions

Array of supported aggregations for each field in schema `fields.aggregations` can have the following values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`.

The backend can implement only a subset of them or all of them. You can start with one, for example, `"sum"`, and extend the list of supported aggregations later. Also, you can define different available aggregation for different fields.