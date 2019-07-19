# Custom data source API specification

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
    "fields": {
        "field": string,
        "type": "string" | "number" | "date",
        "caption": string,
        "folder": string,
        "aggregations": string[]
    }[],
    "sorted": boolean
}
```
| Parameters            | Description                                                                                                                                               |
| --------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `fields`              | `Array`<br> Array of field objects.                                                                                                                       |
| `fields.field`        | `string`<br> Field's unique name that is used in the requests.                                                                                            |
| `fields.type`         | `string`<br> Field's type. Supported values: `"string"`, `"number"`, `"date"`.                                                                            |
| `fields.caption`      | `string` *optional*<br> Field's caption that appears on the UI.                                                                                           |
| `fields.folder`       | `string` *optional*<br> Field's folder that is used to organize groups of fields in the Field List. Supports nesting via `/` (e.g. `"Folder/Subfolder"`). |
| `fields.aggregations` | `string[]` *optional*<br> Array of supported aggregations in case the field is used as measure. *TDB: add supported values*                               |
| `sorted`              | `boolean` *optional*<br> If `true`, the fields will be displayed in the same order in the Field List.                                                     |

## 2.2. Members request

#### Request
```json
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
```json
{
    "members": {
        "caption": string | number | timestamp
    }[],
    "sorted": boolean,
    "page": number,
    "pageTotal": number
}
```
| Parameters        | Description                                                                                                                                                       |
| ----------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `members`         | `Array`<br> Array of members.                                                                                                                                     |
| `members.caption` | `string \| number \| timestamp`<br> Member's caption. In case of `number` field it should be of type `number`. In case of `date` filed it should be Unix timestamp. |
| `sorted`          | `boolean` *optional*<br> If `true`, the fields will be displayed in the same order in the Field List.                                                             |
| `page`            | `number` *optional*<br> Current page number. Starts from `0`.                                                                                                     |
| `pageTotal`       | `number` *optional*<br> Total number of pages. It can be used to load members by parts.                                                                           |

### 2.2.1. Example response for `string` field
```json
TBD
```

### 2.2.2. Example response for `number` field
```json
TBD
```

### 2.2.3. Example response for `date` field
```json
TBD
```

## 2.3. Select request for pivot table

## 2.4. Select request for flat table

## 2.5. Select request for drill-through view