# Custom data source API documentation

- [Custom data source API documentation](#custom-data-source-api-documentation)
- [1. Front-end specification](#1-front-end-specification)
  - [1.1. Flexmonster report configuration](#11-flexmonster-report-configuration)
- [2. Back-end specification](#2-back-end-specification)
  - [2.1. Fields request](#21-fields-request)
  - [2.2. Members request](#22-members-request)
    - [2.2.1. Example with a `string` field](#221-example-with-a-string-field)
    - [2.2.2. Example with a `number` field](#222-example-with-a-number-field)
    - [2.2.3. Example with a `date` field](#223-example-with-a-date-field)
  - [2.3. Select request for pivot table](#23-select-request-for-pivot-table)
    - [2.3.1. Example with one value](#231-example-with-one-value)
    - [2.3.2. Example with two values](#232-example-with-two-values)
    - [2.3.3. Example with a field in rows](#233-example-with-a-field-in-rows)
    - [2.3.4. Example with fields in rows and columns](#234-example-with-fields-in-rows-and-columns)
    - [2.3.5. Example with exclude members filter](#235-example-with-exclude-members-filter)
    - [2.3.6. Example with include/exclude members filter on several fields](#236-example-with-includeexclude-members-filter-on-several-fields)
    - [2.3.7. Example with a conditional filter on labels for `string` field](#237-example-with-a-conditional-filter-on-labels-for-string-field)
    - [2.3.8. Example with a conditional filter on labels for `number` field](#238-example-with-a-conditional-filter-on-labels-for-number-field)
    - [2.3.9. Example with a conditional filter on dates for `date` field](#239-example-with-a-conditional-filter-on-dates-for-date-field)
    - [2.3.10. Example with a conditional filter on values](#2310-example-with-a-conditional-filter-on-values)
  - [2.4. Select request for flat table](#24-select-request-for-flat-table)
    - [2.4.1. Example](#241-example)
  - [2.5. Select request for drill-through view](#25-select-request-for-drill-through-view)
    - [2.5.1. Example](#251-example)

# 1. Front-end specification

## 1.1. Flexmonster report configuration

In order to connect to your custom data source API, you need to configre Flexmonster's `report.dataSource` as follows:
```typescript
"report": {
    "dataSource": {
        "dataSourceType": "api",
        "url": "http://localhost:3400",
        "index": "data-set-123"
    }
}
```
| Prameters        | Description                                                            |
| ---------------- | ---------------------------------------------------------------------- |
| `dataSourceType` | `string`<br> Type of the data source. Should be `"api"`.               |
| `url`            | `string`<br> API endpoint.                                             |
| `index`          | `string`<br> Data set identificator.                                   |
| `requestHeaders` | `Object` *optional*<br> Additional custom headers in key-value format. |

# 2. Back-end specification

Flexmonster sends a sequence of POST requests to the API endpoint in JSON format.

All requests have `index` and `type` properties in the request body. There are 3 types of requests that can be distinguished by `type` value: 
- `fields` - request for all fields with their types (i.e., meta object or schema),
- `members` - request for all members of the field,
- `select` - request for data.

## 2.1. Fields request

**Request**
```typescript
{
    "type": "fields"
    "index": string
}
```

**Response**
```typescript
{
    "fields"[]: {
        "field": string,
        "type": "string" | "number" | "date",
        "caption": string,
        "folder": string,
        "aggregations": string[]
    },
    "sorted": boolean
}
```
| Parameters            | Description                                                                                                                                                                                                                                                                                         |
| --------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `fields`              | `Array`<br> Array of field objects.                                                                                                                                                                                                                                                                 |
| `fields.field`        | `string`<br> Field's unique name that is used in the requests.                                                                                                                                                                                                                                      |
| `fields.type`         | `string`<br> Field's type. Supported values: `"string"`, `"number"`, `"date"`.                                                                                                                                                                                                                      |
| `fields.caption`      | `string` *optional*<br> Field's caption that appears on the UI.                                                                                                                                                                                                                                     |
| `fields.folder`       | `string` *optional*<br> Field's folder that is used to organize groups of fields in the Field List. Supports nesting via `/` (e.g. `"Folder/Subfolder"`).                                                                                                                                           |
| `fields.aggregations` | `string[]` *optional*<br> Array of supported aggregation functions for the field. Supported values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`. |
| `sorted`              | `boolean` *optional*<br> If `true`, the fields will be displayed in the same order in the Field List.                                                                                                                                                                                               |

## 2.2. Members request

**Request**
```typescript
{
    "type": "members"
    "index": string,
    "field": string,
    "page": number
}
```
| Parameters | Description                                                                                                                                                                                        |
| ---------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `field`    | `string`<br> Field's unique name.                                                                                                                                                                  |
| `page`     | `number`<br> Page number. It can be used to load members by parts. If response contains `pageTotal` parameter, additional requests will be performed to load the remaining pages. Starts from `0`. |

**Response**
```typescript
{
    "members"[]: {
        "value": string | number,
        "id": string
    },
    "sorted": boolean,
    "page": number,
    "pageTotal": number
}
```
| Parameters      | Description                                                                                                                                          |
| --------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| `members`       | `Array`<br> Array of members.                                                                                                                        |
| `members.value` | `string \| number`<br> Member's value. In case of `number` field it should be of type `number`. In case of `date` field it should be Unix timestamp. |
| `members.id`    | `string` *optional*<br> Member's id. Supported only for `string` fields. If defined, it is used in queries and in responses to identify member.      |
| `sorted`        | `boolean` *optional*<br> If `true`, the members order from the response will be used as AZ order on the UI.                                          |
| `page`          | `number` *optional*<br> Current page number. Starts from `0`.                                                                                        |
| `pageTotal`     | `number` *optional*<br> Total number of pages. It can be used to load members by parts.                                                              |

### 2.2.1. Example with a `string` field
**Request**
```json
{
    "index": "data-set-123",
    "type": "members",
    "field": "city",
    "page": 0
}
```
**Response**
```json
{
    "members": [
        { "value": "Toronto" }, 
        { "value": "Montreal" }, 
        { "value": "New York" }
    ]
}
```

### 2.2.2. Example with a `number` field
**Request**
```json
{
    "index": "data-set-123",
    "type": "members",
    "field": "price",
    "page": 0
}
```
**Response**
```json
{
    "members": [
        { "value": 10 }, 
        { "value": 28 }, 
        { "value": 30 }
    ]
}
```

### 2.2.3. Example with a `date` field
**Request**
```json
{
    "index": "data-set-123",
    "type": "members",
    "field": "order_date",
    "page": 0
}
```
**Response**
```json
{
    "members": [
        { "value": 1562889600000 }, 
        { "value": 1564617600000 }, 
        { "value": 1564963200000 }
    ]
}
```

## 2.3. Select request for pivot table

**Request**
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
        },
        "filter"[]: {
            "field": string,
            "fieldType": "string" | "number" | "date",
            "include": string[],
            "exclude": string[],
            "query": {
                (condition): string | number
            },
            "value": {
                "field": string,
                "func": string
            }
        }
    },
    "page": number
}
```
| Parameters                       | Description                                                                                                                                                                                                                                                                                                                                              |
| -------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `query`                          | `Object`<br> Query object.                                                                                                                                                                                                                                                                                                                               |
| `query.aggs`                     | `Object`<br> Query aggregations.<br> The part of query that specifies what data should be aggregated and how.                                                                                                                                                                                                                                            |
| `query.aggs.values`              | `Array`<br> Values to aggregate.<br> Fields that have `fields.aggregations` defined in schema can be selected for the query as values.                                                                                                                                                                                                                   |
| `query.aggs.values.field`        | `string`<br> Field's unique name.                                                                                                                                                                                                                                                                                                                        |
| `query.aggs.values.func`         | `string`<br> Aggregation function. Supported values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`. For each field the list of supported aggregations is defined in the response to the fields request. |
| `query.aggs.by`                  | `Object`<br> Fields to aggregate by.                                                                                                                                                                                                                                                                                                                     |
| `query.aggs.by.rows`             | `string[]`<br> Field unique names in rows.                                                                                                                                                                                                                                                                                                               |
| `query.aggs.by.cols`             | `string[]`<br> Field unique names in columns.                                                                                                                                                                                                                                                                                                            |
| `query.filter`                   | <a id="query-filter"></a>`Array` *optional*<br> Query filters.<br> The part of query that specifies what filters should be applied.<br> Each field with filter is represented by a separate object.                                                                                                                                                      |
| `query.filter.field`             | `string`<br> Field's unique name to apply filter to.                                                                                                                                                                                                                                                                                                     |
| `query.filter.fieldType`         | `string`<br> Field's type.                                                                                                                                                                                                                                                                                                                               |
| `query.filter.include`           | `string[]`<br> Field members to include.                                                                                                                                                                                                                                                                                                                 |
| `query.filter.exclude`           | `string[]`<br> Field members to exclude.                                                                                                                                                                                                                                                                                                                 |
| `query.filter.query`             | `Object`<br> Conditional filter.                                                                                                                                                                                                                                                                                                                         |
| `query.filter.query.(condition)` | `string \| number`<br> ***(condition)*** - condition to apply (e.g. `greater`, `less`). <br> Value for the condition.                                                                                                                                                                                                                                    |
| `query.filter.value`             | `Object`<br> Value to apply a conditional filter to.                                                                                                                                                                                                                                                                                                     |
| `query.filter.value.field`       | `string`<br> Field's unique name.                                                                                                                                                                                                                                                                                                                        |
| `query.filter.value.func`        | `string`<br> Aggregation function. Supported values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`. For each field the list of supported aggregations is defined in the response to the fields request. |
| `page`                           | <a id="p-page"></a>`number`<br> Page number. It can be used to load data by parts. If response contains `pageTotal` parameter, additional requests will be performed to load the remaining pages. Starts from `0`.                                                                                                                                       |

**Response**
```typescript
{
    "aggs"[]: {
        "values": {
            (field): {
                (func): number
            }
        },
        "keys": {
            (field): string
        }
    },
    "page": number,
    "pageTotal": number
}
```
| Parameters                   | Description                                                                                                                  |
| ---------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| `aggs`                       | `Array`<br> Aggregated data.                                                                                                 |
| `aggs.values`                | `Object`<br> Numeric values that are calculated for specific tuple.                                                          |
| `aggs.values.(field)`        | `Object`<br> ***(field)*** - field's unique name.                                                                            |
| `aggs.values.(field).(func)` | `number`<br> ***(func)*** - aggregation function. <br> Result of the calculation.                                            |
| `aggs.keys`                  | `Object` *optional*<br> Field's keys that describes specific tuple. In case it is not defined, values are treated as totals. |
| `aggs.keys.(field)`          | `string`<br> ***(field)*** - field's unique name. <br> Field's member.                                                       |
| `page`                       | <a id="p-page-res"></a>`number` *optional*<br> Current page number. Starts from `0`.                                         |
| `pageTotal`                  | <a id="p-page-total-res"></a>`number` *optional*<br> Total number of pages. It can be used to load members by parts.         |

### 2.3.1. Example with one value

**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "aggs": {
            "values": [{
                "func": "sum",
                "field": "price"
            }]
        }
    },
    "page": 0
}
```
**Response**
```json
{
    "aggs": [{
        "values": {
            "price": {
                "sum": 123
            }
        }
    }]
}
```

### 2.3.2. Example with two values

**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "aggs": {
            "values": [{
                "func": "sum", 
                "field": "price"
            }, {
                "func": "sum", 
                "field": "quantity"
            }]
        }
    },
    "page": 0
}
```
**Response**
```json
{
    "aggs": [{
        "values": {
            "price": {
                "sum": 123
            },
            "quantity": {
                "sum": 5
            }
        }
    }]
}
```

### 2.3.3. Example with a field in rows
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "aggs": {
            "values": [{
                "func": "sum", 
                "field": "price"
            }],
            "by": {
                "rows": ["city"]
            }
        }
    },
    "page": 0
}
```
**Response**
```json
{
    "aggs": [{
        "values": {
            "price": {
                "sum": 123
            }
        }
    }, {
        "keys": {
            "city": "Toronto"
        },
        "values": {
            "price": {
                "sum": 100
            }
        }
    }, {
        "keys": {
            "city": "New York"
        },
        "values": {
            "price": {
                "sum": 23
            }
        }
    }]
}
```

### 2.3.4. Example with fields in rows and columns
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "aggs": {
            "values": [{
                "func": "sum", 
                "field": "price"
            }],
            "by": {
                "rows": ["city"],
                "cols": ["color"]
            }
        }
    },
    "page": 0
}
```
**Response**
```json
{
    "aggs": [{
        "values": {
            "price": {
                "sum": 48
            }
        }
    }, {
        "keys": {
            "city": "New York"
        },
        "values": {
            "price": {
                "sum": 20
            }
        }
    }, {
        "keys": {
            "city": "Toronto"
        },
        "values": {
            "price": {
                "sum": 28
            }
        }
    }, {
        "keys": {
            "color": "blue"
        },
        "values": {
            "price": {
                "sum": 38
            }
        }
    }, {
        "keys": {
            "color": "red"
        },
        "values": {
            "price": {
                "sum": 10
            }
        }
    }, {
        "keys": {
            "city": "New York",
            "color": "blue"
        },
        "values": {
            "price": {
                "sum": 20
            }
        }
    }, {
        "keys": {
            "city": "Toronto",
            "color": "blue"
        },
        "values": {
            "price": {
                "sum": 18
            }
        }
    }, {
        "keys": {
            "city": "Toronto",
            "color": "red"
        },
        "values": {
            "price": {
                "sum": 10
            }
        }
    }]
}
```

### 2.3.5. Example with exclude members filter
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "filter": [{
            "field": "city",
            "exclude": ["New York", "Montreal"]
        }],
        "aggs": {
            "values": [{
                "func": "sum",
                "field": "price"
            }],
            "by": {
                "rows": ["city"]
            }
        }
    }
}
```
**Response**
```
Format is the same as above
```

### 2.3.6. Example with include/exclude members filter on several fields
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "filter": [{
                "field": "color",
                "include": ["blue"]
            },
            {
                "field": "city",
                "exclude": ["New York", "Montreal"]
            }
        ],
        "aggs": {
            "values": [{
                "func": "sum",
                "field": "price"
            }],
            "by": {
                "rows": ["city"]
            }
        }
    }
}
```
**Response**
```
Format is the same as above
```

### 2.3.7. Example with a conditional filter on labels for `string` field
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "filter": [{
            "field": "city",
            "fieldType": "string",
            "query": {
                "begin": "toro"
            }
        }],
        "aggs": {
            "values": [{
                "func": "sum",
                "field": "price"
            }],
            "by": {
                "rows": ["city"]
            }
        }
    }
}
```
**Response**
```
Format is the same as above
```

### 2.3.8. Example with a conditional filter on labels for `number` field
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "filter": [{
            "field": "quantity",
            "fieldType": "number",
            "query": {
                "greater": 2
            }
        }],
        "aggs": {
            "values": [{
                "func": "sum",
                "field": "price"
            }],
            "by": {
                "rows": ["quantity"]
            }
        }
    }
}
```
**Response**
```
Format is the same as above
```

### 2.3.9. Example with a conditional filter on dates for `date` field
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "filter": [{
            "field": "order_date",
            "fieldType": "date",
            "query": {
                "equal": 1564617600000
            }
        }],
        "aggs": {
            "values": [{
                "func": "sum",
                "field": "price"
            }],
            "by": {
                "rows": ["order_date"]
            }
        }
    }
}
```
**Response**
```
Format is the same as above
```

### 2.3.10. Example with a conditional filter on values
**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "filter": [{
            "field": "city",
            "fieldType": "string",
            "query": {
                "top": 3
            },
            "value": {
                "func": "sum",
                "field": "price"
            }
        }],
        "aggs": {
            "values": [{
                "func": "sum",
                "field": "price"
            }],
            "by": {
                "rows": ["city"]
            }
        }
    }
}
```
**Response**
```
Format is the same as above
```

## 2.4. Select request for flat table

**Request**
```typescript
{
    "type": "select"
    "index": string,
    "query": {
        "fields"[]: {
            "field": string
        },
        "filter"[]: {
            "field": string,
            "fieldType": "string" | "number" | "date",
            "include": string[],
            "exclude": string[],
            "query": {
                (condition): string | number
            },
            "value": {
                "field": string,
                "func": string
            }
        },
        "aggs": {
            "values"[]: {
                "field": string,
                "func": string
            }
        }
    },
    "page": number
}
```
| Parameters                | Description                                                                                                                                                                                                                                                                                                                                              |
| ------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `query`                   | `Object`<br> Query object.                                                                                                                                                                                                                                                                                                                               |
| `query.fields`            | `Array`<br> Array of fields (columns) to include in the response.                                                                                                                                                                                                                                                                                        |
| `query.fields.field`      | `string`<br> Field's unique name.                                                                                                                                                                                                                                                                                                                        |
| `query.filter`            | `Array` *optional*<br> See [`query.filter`](#query-filter).                                                                                                                                                                                                                                                                                              |
| `query.aggs`              | `Array` *optional*<br> Query column totals.                                                                                                                                                                                                                                                                                                              |
| `query.aggs.values`       | `Array`<br> Columns to aggregate totals for.<br> Fields that have `fields.aggregations` defined in schema can be selected for this part of the query.                                                                                                                                                                                                    |
| `query.aggs.values.field` | `string`<br> Field's unique name.                                                                                                                                                                                                                                                                                                                        |
| `query.aggs.values.func`  | `string`<br> Aggregation function. Supported values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`. For each field the list of supported aggregations is defined in the response to the fields request. |
| `page`                    | `number`<br> Page number. See [`page`](#p-page).                                                                                                                                                                                                                                                                                                         |

**Response**
```typescript
{
    "fields"[]: {
        "fields": string
    },
    "hits"[]: [
        (index): string | number
    ],
    "aggs"[]: {
        "values": {
            (field): {
                (func): number
            }
        }
    },
    "page": number,
    "pageTotal": number
}
```
| Parameters                   | Description                                                                           |
| ---------------------------- | ------------------------------------------------------------------------------------- |
| `fields`                     | `Array`<br> Array of fields (columns) included in the response.                       |
| `fields.field`               | `string`<br> Field's unique name.                                                     |
| `hits`                       | `Array`<br> Two-dimensional array that contains data.                                 |
| `hits[]`                     | `[(index): string \| number]`<br> ***(index)*** - field (column) index.<br> Data row. |
| `aggs`                       | `Array` *optional*<br> Column totals.                                                 |
| `aggs.values`                | `Object`<br> Numeric values that are calculated for column totals.                    |
| `aggs.values.(field)`        | `Object`<br> ***(field)*** - field's unique name.                                     |
| `aggs.values.(field).(func)` | `number`<br> ***(func)*** - aggregation function.<br> Result of the calculation.      |
| `page`                       | `number` *optional*<br> Current page number. See [`page`](#p-page-res).               |
| `pageTotal`                  | `number` *optional*<br> Total number of pages. See [`pageTotal`](#p-page-total-res).  |

### 2.4.1. Example

**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "fields": [
            { "field": "country" },
            { "field": "city" },
            { "field": "price" },
            { "field": "quantity" }
        ],
        "aggs": {
            "values": [{
                "func": "sum", 
                "field": "price"
            }, {
                "func": "sum", 
                "field": "quantity"
            }]
        }
    },
    "page": 0
}
```
**Response**
```json
{
    "fields": [
        { "field": "country" },
        { "field": "city" },
        { "field": "price" },
        { "field": "quantity" }
    ],
    "hits": [
        ["Canada", "Toronto", 53, 2],
        ["...", "...", 1, 1]
    ],
    "aggs": [{
        "values": {
            "price": {
                "sum": 123
            },
            "quantity": {
                "sum": 5
            }
        }
    }]
}
```

## 2.5. Select request for drill-through view

**Request**
```typescript
{
    "type": "select"
    "index": string,
    "query": {
        "fields"[]: {
            "field": string
        },
        "filter"[]: {
            "field": string,
            "fieldType": "string" | "number" | "date",
            "include": string[],
            "exclude": string[],
            "query": {
                (condition): string | number
            },
            "value": {
                "field": string,
                "func": string
            }
        },
        "limit": number
    },
    "page": number
}
```
| Parameters           | Description                                                                                                                        |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| `query`              | `Object`<br> Query object.                                                                                                         |
| `query.fields`       | `Array`<br> Array of fields (columns) to include in the response.                                                                  |
| `query.fields.field` | `string`<br> Field's unique name.                                                                                                  |
| `query.filter`       | `Array`<br> See [`query.filter`](#query-filter).                                                                                   |
| `query.limit`        | `number`<br> The maximum number of records that should be included in the response. Сonfigurable on the client. Default is `1000`. |
| `page`               | `number`<br> Page number. See [`page`](#p-page).                                                                                   |

**Response**
```typescript
{
    "fields"[]: {
        "fields": string
    },
    "hits"[]: [
        (index): string | number
    ],
    "page": number,
    "pageTotal": number
}
```
| Parameters     | Description                                                                           |
| -------------- | ------------------------------------------------------------------------------------- |
| `fields`       | `Array`<br> Array of fields (columns) included in the response.                       |
| `fields.field` | `string`<br> Field's unique name.                                                     |
| `hits`         | `Array`<br> Two-dimensional array that contains data.                                 |
| `hits[]`       | `[(index): string \| number]`<br> ***(index)*** - field (column) index.<br> Data row. |
| `page`         | `number` *optional*<br> Current page number. See [`page`](#p-page-res).               |
| `pageTotal`    | `number` *optional*<br> Total number of pages. See [`pageTotal`](#p-page-res).        |

### 2.5.1. Example

**Request**
```json
{
    "index": "data-set-123",
    "type": "select",
    "query": {
        "fields": [
            { "field": "country" },
            { "field": "city" },
            { "field": "price" },
            { "field": "quantity" }
        ],
        "filter": {
            "field": "country", 
            "include": ["Canada"]
        },
        "limit": 1000
    },
    "page": 0
}
```
**Response**
```json
{
    "fields": [
        { "field": "country" },
        { "field": "city" },
        { "field": "price" },
        { "field": "quantity" }
    ],
    "hits": [
        ["Canada", "Toronto", 53, 2],
        ["Canada", "...", 1, 1]
    ]
}
```
