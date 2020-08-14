# Flexmonster custom data source API server
[![Flexmonster Pivot Table & Charts](https://www.flexmonster.com/fm_uploads/2020/06/GitHub_fm.png)](https://flexmonster.com)
Website: www.flexmonster.com

## Flexmonster Pivot Table & Charts

Flexmonster Pivot is a powerful JavaScript tool for interactive web reporting. It allows you to visualize and analyze data from JSON, CSV, SQL, NoSQL, Elasticsearch, and OLAP data sources fast and conveniently. Flexmonster is designed to integrate seamlessly with any client-side framework and can be easily embedded into your application.

This repository holds the source code for the Flexmonster custom data source API project.

Flexmonster custom data source API is designed for summarized data retrieval from a server to Flexmonster Pivot.
This API will work for projects with their own data access layer, where filtering and aggregation are delegated to a server and Flexmonster receives ready-to-show data.

The table of contents:
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)

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

    - To start the sample .NET Code server:

    ```
    cd server-dotnetcore
    dotnet restore
    dotnet run
    ```

## Usage

For details on usage, refer to our documentation:

- [A quick overview of the sample Node.js server](https://www.flexmonster.com/doc/a-quick-overview-of-the-sample-node-js-server/) 
- [A quick overview of the sample .NET Core server](https://www.flexmonster.com/doc/a-quick-overview-of-the-sample-net-core-server/)
- [Quick start guide to the custom data source API implementation](https://www.flexmonster.com/doc/implementing-the-custom-data-source-api-server/)
- [API documentation](https://www.flexmonster.com/api/all-requests/)
