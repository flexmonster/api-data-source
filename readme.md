# Flexmonster custom data source API server
[![Flexmonster Pivot Table & Charts](https://cdn.flexmonster.com/landing.png)](https://www.flexmonster.com?r=github)
Website: [www.flexmonster.com](https://www.flexmonster.com?r=github)

## Flexmonster Pivot Table & Charts

Flexmonster Pivot Table & Charts is a powerful and fully customizable JavaScript component for web reporting. It is packed with all core features for data analysis and can easily become a part of your data visualization project. The tool supports popular frameworks like React, Vue, Angular, Blazor, and [more](https://www.flexmonster.com/doc/available-tutorials-integration?r=github). Also, Flexmonster connects to [any data source](https://www.flexmonster.com/doc/supported-data-sources?r=github), including SQL and NoSQL databases, JSON and CSV files, OLAP cubes, and Elasticsearch.

This repository contains sample servers that demonstrate how to implement the [custom data source API](https://www.flexmonster.com/doc/introduction-to-custom-data-source-api?r=github).

The custom data source API is our communication protocol that helps you build a custom data source based on your server. The server must be responsible for loading, processing, and aggregating data. After implementing the protocol on your server, you can load the data from the server into Flexmonster.

Table of contents:
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Tests](#tests)
  - [Related Flexmonster docs](#related-flexmonster-docs)
  - [Support and feedback](#support-and-feedback)
  - [Flexmonster licensing](#flexmonster-licensing)
  - [Social media](#social-media)

## Prerequisites

For the sample Node.js server:
- [Node.js 10 or later](https://nodejs.org/en/)

For the sample .NET Core server:
- [Microsoft .NET 8.0](https://dotnet.microsoft.com/en-us/download)

## Installation

1. Download a `.zip` archive with the sample project or clone it from GitHub with the following command:

```bash
git clone https://github.com/flexmonster/api-data-source.git && cd api-data-source
```

2. Choose one of the servers to run:
    
    - To start the sample Node.js server:

    ```
    cd server-nodejs && npm install && npm start
    ```

    - To start the sample .NET Core server:

    ```
    cd server-dotnetcore && dotnet restore && dotnet run
    ```

3.  Open the `client/index.html` file in a browser to see Flexmonster with the data from the server.

## Tests

If needed, you can check a custom data source API server with our [test suite](https://github.com/flexmonster/api-data-source/tree/master/tests). Run the tests with the following commands: 
```
cd tests && npm install && npm test
```

Note that these tests will work only if one of the sample servers is running. To learn how your server can be tested, [see our documentation](https://www.flexmonster.com/doc/test-custom-data-source-api-server?r=github).

## Related Flexmonster docs

- [A quick overview of the sample Node.js server](https://www.flexmonster.com/doc/pivot-table-with-node-js-server?r=github) — learn more about a sample Node.js server.
- [A quick overview of the sample .NET Core server](https://www.flexmonster.com/doc/pivot-table-with-dot-net-core-server?r=github) — learn more about a sample .NET Core server.
- [Implementing the custom data source API server](https://www.flexmonster.com/doc/implement-custom-data-source-api?r=github) — see how to implement the custom data source API protocol on your server.
- [Custom data source API documentation](https://www.flexmonster.com/api/all-requests?r=github) — check out details on API requests for fetching aggregated data.

## Support and feedback

In case of any issues, visit our [Troubleshooting](https://www.flexmonster.com/doc/typical-errors?r=github) section. You can also search among the [resolved cases](https://www.flexmonster.com/technical-support?r=github) for a solution to your issue.

To share your feedback or ask questions, contact our Tech team by raising a ticket on our [Help Center](https://www.flexmonster.com/help-center?r=github). You can also find a list of samples, technical specifications, and a user interface guide there.

## Flexmonster licensing

To learn about Flexmonster Pivot licenses, visit the [Flexmonster licensing page](https://www.flexmonster.com/pivot-table-editions-and-pricing?r=github). 
If you want to test our product, we provide a 30-day free trial.

If you need any help with your license, fill out our [Contact form](https://www.flexmonster.com/contact-our-team?r=github), and we will get in touch with you.

## Social media

Follow us on social media and stay updated on our development process!

[![LinkedIn](https://img.shields.io/badge/LinkedIn-blue?style=for-the-badge&logo=linkedin&logoColor=white)](https://linkedin.com/company/flexmonster) [![YouTube](https://img.shields.io/badge/YouTube-red?style=for-the-badge&logo=youtube&logoColor=white)](https://youtube.com/user/FlexMonsterPivot) [![Twitter](https://img.shields.io/badge/Twitter-blue?style=for-the-badge&logo=twitter&logoColor=white)](https://twitter.com/flexmonster)
