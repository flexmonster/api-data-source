# Flexmonster custom data source API server
[![Flexmonster Pivot Table & Charts](https://cdn.flexmonster.com/landing.png)](https://www.flexmonster.com?r=github)
Website: [www.flexmonster.com](https://www.flexmonster.com?r=github)

## Flexmonster Pivot Table & Charts

Flexmonster Pivot is a powerful JavaScript tool for interactive web reporting. It allows you to visualize and analyze data from JSON, CSV, SQL, NoSQL, Elasticsearch, and OLAP data sources quickly and conveniently. Flexmonster is designed to integrate seamlessly with any client-side framework and can be easily embedded into your application.

This repository holds the source code for sample servers that demonstrate how to implement the [custom data source API](https://www.flexmonster.com/doc/introduction-to-custom-data-source-api?r=github).

The custom data source API is a communication protocol designed to retrieve aggregated data from a server to Flexmonster Pivot.
The server is responsible for fetching, processing, and aggregating data, which is then passed to Flexmonster in a ready-to-show format.

Table of contents:
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Tests](#tests)
  - [Related Flexmonster docs](#related-flexmonster-docs)

## Prerequisites

For the sample Node.js server:
- [Node.js 10 or later](https://nodejs.org/en/)

For the sample .NET Core server:
- [.NET Core 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Installation

1. Download a `.zip` archive with the sample project or clone it from GitHub with the following command:

```bash
git clone https://github.com/flexmonster/api-data-source.git && cd api-data-source
```

2. Run the project in one of the following ways:
    
    - To start the sample Node.js server:

    ```
    cd server-nodejs
    npm install
    npm start
    ```

    - To start the sample .NET Core server:

    ```
    cd server-dotnetcore
    dotnet restore
    dotnet run
    ```

## Tests

If needed, you can check a custom data source API server with our [test suite](https://github.com/flexmonster/api-data-source/tree/master/tests). Run the tests with the following commands: 

   ```
    cd tests
    npm install
    npm test
   ```

Note that these tests will work only if one of the sample servers is running. To learn how your server can be tested, [see our documentation](https://www.flexmonster.com/doc/test-custom-data-source-api-server?r=github).

## Related Flexmonster docs

For details on usage, refer to our documentation:

- [A quick overview of the sample Node.js server](https://www.flexmonster.com/doc/pivot-table-with-node-js-server?r=github) 
- [A quick overview of the sample .NET Core server](https://www.flexmonster.com/doc/pivot-table-with-dot-net-core-server?r=github)
- [Implementing the custom data source API server](https://www.flexmonster.com/doc/implement-custom-data-source-api?r=github)
- [Custom data source API documentation](https://www.flexmonster.com/api/all-requests?r=github)
