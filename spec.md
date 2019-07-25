# Custom data source API specification

- [2.2. Members request](#22-members-request)

# 1. Front-end spec

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
| Prameters        | Description                                                           |
| ---------------- | --------------------------------------------------------------------- |
| `dataSourceType` | `string`<br> Type of the data source. Should be `"api"`.              |
| `url`            | `string`<br> API endpoint                                             |
| `index`          | `string`<br> Data set identificator                                   |
| `requestHeaders` | `Object` *optional*<br> Additional custom headers in key-value format |

# 2. Back-end spec

Flexmonster sends a sequence of POST requests to the API endpoint in JSON format.

All requests have `index` and `type` properties in the request body. There are 3 types of requests that can be distinguished by `type` value: 
- `fields` - return all fields with their types - meta object
- `members` - return all members of the field
- `select` - return data

## 2.1. Fields request

#### Request
```typescript
{
    "type": "fields"
    "index": string
}
```

#### Response
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
| Parameters            | Description                                                                                                                                                                                                                                                                                                       |
| --------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `fields`              | `Array`<br> Array of field objects.                                                                                                                                                                                                                                                                               |
| `fields.field`        | `string`<br> Field's unique name that is used in the requests.                                                                                                                                                                                                                                                    |
| `fields.type`         | `string`<br> Field's type. Supported values: `"string"`, `"number"`, `"date"`.                                                                                                                                                                                                                                    |
| `fields.caption`      | `string` *optional*<br> Field's caption that appears on the UI.                                                                                                                                                                                                                                                   |
| `fields.folder`       | `string` *optional*<br> Field's folder that is used to organize groups of fields in the Field List. Supports nesting via `/` (e.g. `"Folder/Subfolder"`).                                                                                                                                                         |
| `fields.aggregations` | `string[]` *optional*<br> Array of supported aggregations in case the field is used as measure. Supported values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`. |
| `sorted`              | `boolean` *optional*<br> If `true`, the fields will be displayed in the same order in the Field List.                                                                                                                                                                                                             |

## 2.2. Members request

#### Request
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

#### Response
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
| Parameters      | Description                                                                                                                                                       |
| --------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `members`       | `Array`<br> Array of members.                                                                                                                                     |
| `members.value` | `string \| number`<br> Member's value. In case of `number` field it should be of type `number`. In case of `date` filed it should be Unix timestamp. |
| `members.id`    | `string` *optional*<br> Member's id. Supported only for `string` fields. If defined, it is used in queries and in responses to identify member.                   |
| `sorted`        | `boolean` *optional*<br> If `true`, the fields will be displayed in the same order in the Field List.                                                             |
| `page`          | `number` *optional*<br> Current page number. Starts from `0`.                                                                                                     |
| `pageTotal`     | `number` *optional*<br> Total number of pages. It can be used to load members by parts.                                                                           |

### 2.2.1. Example for `string` field
#### Request
```json
{
    "index": "data-set-123",
    "type": "members",
    "field": "city",
    "page": 0
}
```
#### Response
```json
{
    "members": [
        { "value": "Toronto" }, 
        { "value": "Montreal" }, 
        { "value": "New York" }
    ]
}
```

### 2.2.2. Example for `number` field
#### Request
```json
{
    "index": "data-set-123",
    "type": "members",
    "field": "price",
    "page": 0
}
```
#### Response
```json
{
    "members": [
        { "value": 10 }, 
        { "value": 28 }, 
        { "value": 30 }
    ]
}
```

### 2.2.3. Example for `date` field
#### Request
```json
{
    "index": "data-set-123",
    "type": "members",
    "field": "order_date",
    "page": 0
}
```
#### Response
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

#### Request
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
| Parameters                       | Description                                                                                                                                                                                                                                                                                                                                                    |
| -------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `query`                          | `Object`<br> *TBD*                                                                                                                                                                                                                                                                                                                                             |
| `query.aggs`                     | `Object`<br> *TBD*                                                                                                                                                                                                                                                                                                                                             |
| `query.aggs.values`              | `Array`<br> *TBD*                                                                                                                                                                                                                                                                                                                                              |
| `query.aggs.values.field`        | `string`<br> Field's name.                                                                                                                                                                                                                                                                                                                                     |
| `query.aggs.values.func`         | `string`<br> Value aggregation function. Supported values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`. For each field the list of supported aggregations is defined in the response to the fields request. |
| `query.aggs.by`                  | `Object`<br> *TBD*                                                                                                                                                                                                                                                                                                                                             |
| `query.aggs.by.rows`             | `string[]`<br> Field names in rows.                                                                                                                                                                                                                                                                                                                            |
| `query.aggs.by.cols`             | `string[]`<br> Field names in columns.                                                                                                                                                                                                                                                                                                                         |
| `query.filter`                   | `Array`<br> *TBD*                                                                                                                                                                                                                                                                                                                                              |
| `query.filter.field`             | `string`<br> Field's name.                                                                                                                                                                                                                                                                                                                                     |
| `query.filter.fieldType`         | `string`<br> Field's type.                                                                                                                                                                                                                                                                                                              |
| `query.filter.include`           | `string[]`<br> *TBD*                                                                                                                                                                                                                                                                                                                                           |
| `query.filter.exclude`           | `string[]`<br> *TBD*                                                                                                                                                                                                                                                                                                                                           |
| `query.filter.query`             | `Object`<br> *TBD*                                                                                                                                                                                                                                                                                                                                             |
| `query.filter.query.(condition)` | `string | number`<br> ***(condition)*** - condition to apply (e.g. `greater`, `less`). <br> Value for the condition.                                                                                                                                                                                                                                           |
| `query.filter.value`             | `Object`<br> *TBD*                                                                                                                                                                                                                                                                                                                                             |
| `query.filter.value.field`       | `string`<br> *TBD*                                                                                                                                                                                                                                                                                                                                             |
| `query.filter.value.func`        | `string`<br> *TBD*                                                                                                                                                                                                                                                                                                                                             |
| `page`                           | `number`<br> Page number. It can be used to load data by parts. If response contains `pageTotal` parameter, additional requests will be performed to load the remaining pages. Starts from `0`.                                                                                                                                                                |

#### Response
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
| `aggs`                       | `Array`<br> *TBD*                                                                                                            |
| `aggs.values`                | `Object`<br> Numeric values that are calculated for specific tuple.                                                          |
| `aggs.values.(field)`        | `Object`<br> ***(field)*** - field's name.                                                                                   |
| `aggs.values.(field).(func)` | `number`<br> ***(func)*** - aggregation function. <br> Result of the calculation.                                            |
| `aggs.keys`                  | `Object` *optional*<br> Field's keys that describes specific tuple. In case it is not defined, values are treated as totals. |
| `aggs.keys.(field)`          | `string`<br> ***(field)*** - field's name. <br> Field's member name.                                                         |
| `page`                       | `number` *optional*<br> Current page number. Starts from `0`.                                                                |
| `pageTotal`                  | `number` *optional*<br> Total number of pages. It can be used to load members by parts.                                      |

### 2.3.1. Example for one value

#### Request
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
#### Response
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

### 2.3.2. Example for two values

#### Request
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
#### Response
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

### 2.3.3. Example with field in rows
#### Request
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
#### Response
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
#### Request
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
#### Response
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

### 2.3.5. Example with exclude fiter by members
#### Request
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
#### Response
```
Format is the same as above
```

### 2.3.6. Example with inlcude/exclude fiter by members by several fields
#### Request
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
#### Response
```
Format is the same as above
```

### 2.3.7. Example with query fiter by labels
#### Request
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
#### Response
```
Format is the same as above
```

### 2.3.8. Example with query fiter by values
#### Request
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
#### Response
```
Format is the same as above
```

### 2.3.9. Example with query fiter by labels for numeric field
#### Request
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
#### Response
```
Format is the same as above
```

### 2.3.10. Example with query fiter by dates for date field
#### Request
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
#### Response
```
Format is the same as above
```

## 2.4. Select request for flat table

#### Request
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
| Parameters                | Description                                                                                                                                                                                                                                                                                                                                                    |
| ------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `query`                   | `Object`<br>                                                                                                                                                                                                                                                                                                                                                   |
| `query.fields`            | `Array`<br> Array of fields (columns) to include in the response.                                                                                                                                                                                                                                                                                              |
| `query.fields.field`      | `string`<br> Field's name.                                                                                                                                                                                                                                                                                                                                     |
| `query.filter`            | `Array` *optional*<br> See **Section 2.3** `query.filter`.                                                                                                                                                                                                                                                                                                     |
| `query.aggs`              | `Array` *optional*<br> Column totals.                                                                                                                                                                                                                                                                                                                          |
| `query.aggs.values`       | `Array`<br>                                                                                                                                                                                                                                                                                                                                                    |
| `query.aggs.values.field` | `string`<br> Field's name.                                                                                                                                                                                                                                                                                                                                     |
| `query.aggs.values.func`  | `string`<br> Value aggregation function. Supported values: `"sum"`, `"count"`, `"distinctcount"`, `"average"`, `"median"`, `"product"`, `"min"`, `"max"`, `"percent"`, `"percentofcolumn"`, `"percentofrow"`, `"index"`, `"stdevp"`, `"stdevs"`, `"none"`. For each field the list of supported aggregations is defined in the response to the fields request. |
| `page`                    | `number`<br> Page number. See **Section 2.3**.                                                                                                                                                                                                                                                                                                                 |

#### Response
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
| `fields.field`               | `string`<br> Field's name.                                                            |
| `hits`                       | `Array`<br> Two-dimensional array that contains data.                                 |
| `hits[]`                     | `[(index): string \| number]`<br> ***(index)*** - field (column) index <br> Data row. |
| `aggs`                       | `Array` *optional*<br> Column totals.                                                 |
| `aggs.values`                | `Object`<br>                                                                          |
| `aggs.values.(field)`        | `Object`<br> ***(field)*** - field's name                                             |
| `aggs.values.(field).(func)` | `number`<br> ***(func)*** - aggregation function <br> Result of the calculation.      |
| `page`                       | `number` *optional*<br> Current page number. See **Section 2.3**.                     |
| `pageTotal`                  | `number` *optional*<br> Total number of pages. See **Section 2.3**.                   |

### 2.4.1. Example

#### Request
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
#### Response
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

#### Request
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
| `query`              | `Object`<br>                                                                                                                       |
| `query.fields`       | `Array`<br> Array of fields (columns) to include in the response.                                                                  |
| `query.fields.field` | `string`<br> Field's name.                                                                                                         |
| `query.filter`       | `Array`<br> See **Section 2.3** `query.filter`.                                                                                    |
| `query.limit`        | `number`<br> The maximum number of records that should be included in the response. Сonfigurable on the client. Default is `1000`. |
| `page`               | `number`<br> Page number. See **Section 2.3**.                                                                                     |

#### Response
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
| `fields.field` | `string`<br> Field's name.                                                            |
| `hits`         | `Array`<br> Two-dimensional array that contains data.                                 |
| `hits[]`       | `[(index): string \| number]`<br> ***(index)*** - field (column) index <br> Data row. |
| `page`         | `number` *optional*<br> Current page number. See **Section 2.3**.                     |
| `pageTotal`    | `number` *optional*<br> Total number of pages. See **Section 2.3**.                   |

### 2.5.1. Example

#### Request
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
#### Response
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
