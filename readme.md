# Flexmonster custom data source API server

Flexmonster custom data source API is designed for summarized data retrieval from a server to Flexmonster Pivot.

This API will work for projects with their own data access layer, where filtering and aggregation are delegated to a server and Flexmonster receives ready-to-show data.

- [Documentation](https://github.com/flexmonster/api-data-source/wiki/API-documentation)
- [Quick start guide to custom data source API implementation](https://github.com/flexmonster/api-data-source/wiki/Quick-start-guide)
- [Sample Node.js server](#sample-nodejs-server)
- [Sample .NET Core server](#sample-net-core-server)

## Sample Node.js server

Inside the `/server-nodejs` folder you can find the sample Node.js server that implements Flexmonster's custom data source API. All requests from Flexmonster Pivot Table are handled by `http://localhost:3400/api/cube` endpoints. Raw data is stored in JSON format in the `/server-nodejs/data` folder. The file name matches the `index` property of the `dataSource` configuration object.

To start the server:

```
cd server-nodejs
npm install
npm start
```

On the client-side, the configuration looks as follows (check `/client/index.html`):
```javascript
new Flexmonster({
    container: "#pivot",
    report: {
        "dataSource": {
            "type": "api",
            "url": "http://localhost:3400/api/cube",
            "index": "fm-product-sales"
        }
    }
});
```

## Sample .NET Core server

Inside the `/server-dotnetcore` folder you can find another Flexmonster's custom data source API implementation - sample .NET Core server. All requests from Flexmonster Pivot Table are handled by `http://localhost:3400/api/cube` endpoints. Raw data is stored in the `/data` folder in JSON and CSV formats. If it is a file name matches the `index` property of the `dataSource` configuration object.

To start the server:

```
cd server-dotnetcore
dotnet restore
dotnet run
```

On the client-side, the configuration looks like in [Sample Node.js server](#sample-nodejs-server)

By default, server use data stored in JSON format. This option configured in `appsettings.json`

```
 "DataSource": {
    "DataSourceName": "json"
  }
```

If you want to use CSV you need to change it like this: 

```
 "DataSource": {
    "DataSourceName": "csv"
  }
```

To use database you must add type of database(MySQL, MSSQL and PostgreSQL are supported), add conection string and query to retrieve data. `DatabaseType` property must be one of the following `mysql`, `mssql` or `postgresql`. On client side `index` in configuration must be one from `Indexes` property. It will execute query you specified.

```
"DataSource": {
    "DataSourceName": "database",
    "DatabaseType": "mysql",
    "ConnectionStrings": {
      "DefaultConnection": "CONNECTION_STRING"
    },
    "Indexes": {
      "index": "QUERY_TO_RETRIEVE_DATA"
    }
  }
```

Server can work only with one data source in the same time.
