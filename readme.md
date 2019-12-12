# Flexmonster custom data source API server (beta version)

Flexmonster custom data source API is designed for summarized data retrieval from a server to Flexmonster Pivot.

This API will work for projects with their own data access layer, where filtering and aggregation are delegated to a server and Flexmonster receives ready-to-show data.

- [Documentation](https://github.com/flexmonster/api-data-source/wiki/API-documentation)
- [Quick start guide to custom data source API implementation](https://github.com/flexmonster/api-data-source/wiki/Quick-start-guide)
- [Sample Node.js server](#sample-nodejs-server)

## Sample Node.js server

Inside the `/server-nodejs` folder you can find the sample Node.js server that implements Flexmonster's custom data source API. All requests from Flexmonster Pivot Table are handled by `http://localhost:3400/api/cube` endpoints. Raw data is stored in JSON format in the `/server-nodejs/data` folder. File name matches the `index` property of the `dataSource` configuration object.

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
