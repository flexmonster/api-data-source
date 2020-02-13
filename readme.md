# Flexmonster custom data source API server
[![Flexmonster Pivot Table & Charts](https://s3.amazonaws.com/flexmonster/github/fm-github-cover.png)](https://flexmonster.com)
Website: www.flexmonster.com

This repository holds the source code for a Flexmonster custom data source API project.

Flexmonster custom data source API is designed for summarized data retrieval from a server to Flexmonster Pivot.
This API will work for projects with their own data access layer, where filtering and aggregation are delegated to a server and Flexmonster receives ready-to-show data.

For more information, refer to our documentation:

- [API documentation](https://www.flexmonster.com/api/all-requests/)
- [Quick start guide to the custom data source API implementation](https://www.flexmonster.com/doc/implementing-the-custom-data-source-api-server/)
- [A quick overview of the sample Node.js server](https://www.flexmonster.com/doc/a-quick-overview-of-the-sample-node-js-server/)
- [A quick overview of the sample .NET Core server](https://www.flexmonster.com/doc/a-quick-overview-of-the-sample-net-core-server/)

## Installation & Usage
### Sample Node.js server

To start the sample Node.js server:

```
cd server-nodejs
npm install
npm start
```

### Sample .NET Core server

To start the sample .NET Code server:

```
cd server-dotnetcore
dotnet restore
dotnet run
```
