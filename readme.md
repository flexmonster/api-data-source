# Flexmonster custom data source API server
[![Flexmonster Pivot Table & Charts](https://cdn.flexmonster.com/landing.png)](https://flexmonster.com)
Website: www.flexmonster.com

## Flexmonster Pivot Table & Charts

Flexmonster Pivot is a powerful JavaScript tool for interactive web reporting. It allows you to visualize and analyze data from JSON, CSV, SQL, NoSQL, Elasticsearch, and OLAP data sources quickly and conveniently. Flexmonster is designed to integrate seamlessly with any client-side framework and can be easily embedded into your application.

This repository holds the source code for a project that demonstrates how to implement the [custom data source API](https://www.flexmonster.com/doc/introduction-to-custom-data-source-api/).

Flexmonster custom data source API is designed for summarized data retrieval from a server to Flexmonster Pivot.
This API will work for projects with their own data access layer, where filtering and aggregation are delegated to a server and Flexmonster receives ready-to-show data.

The table of contents:
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Tests](#tests)

## Prerequisites

To run the custom data source API project, you will need Node.js and npm. [Get it here](https://docs.npmjs.com/downloading-and-installing-node-js-and-npm) if it's not already installed on your machine.

## Installation

1. Download the `.zip` archive with the sample project or clone it from GitHub with the following command:

```bash
git clone https://github.com/flexmonster/api-data-source.git
```

2. Run the project using one of the following ways:
    
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

## Usage

For details on usage, refer to our documentation:

- [A quick overview of the sample Node.js server](https://www.flexmonster.com/doc/a-quick-overview-of-the-sample-node-js-server/) 
- [A quick overview of the sample .NET Core server](https://www.flexmonster.com/doc/a-quick-overview-of-the-sample-net-core-server/)
- [Implementing the custom data source API server](https://www.flexmonster.com/doc/implementing-the-custom-data-source-api-server/)
- [API documentation](https://www.flexmonster.com/api/all-requests/)

## Tests

If needed, you can check a custom data source API server with our [test suite](https://github.com/flexmonster/api-data-source/tree/master/tests). Run the tests with the following commands: 

   ```
    cd tests
    npm install
    npm test
   ```

Note that these tests will work only if one of the sample servers is running. To learn how your server can be tested, [see our documentation](https://www.flexmonster.com/doc/test-custom-data-source-api-server/).
