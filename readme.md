# Flexmonster custom data source API server (beta version)

Flexmonster custom data source API is designed for summarized data retrieval from a server to Flexmonster Pivot.

This API will work for projects with their own data access layer, where filtering and aggregation are delegated to a server and Flexmonster receives ready-to-show data.

- [Documentation](API-documentation)
- [Quick start guide to custom data source API implementation](Quick-start-guide)
- [Sample Node.js server](#sample-nodejs-server)

## Sample Node.js server

Inside the `/server` folder you can find the sample Node.js server that implements Flexmonster's custom data source API. All requests from Flexmonster Pivot Table are handled by `http://localhost:3400/api/cube` endpoint. Raw data is stored in JSON format in the `/server/data` folder as well as it's schema. File name matches the `index` property of the `dataSource` configuration object.

To start the server:

```
cd server
npm install
npm start
```

Then, navigate to `http://localhost:3400` in the browser.

On the client-side, the configuration looks as follows:
```javascript
new Flexmonster({
    container: "#pivot",
    report: {
        "dataSource": {
            "dataSourceType": "api",
            "url": "http://localhost:3400/api/cube",
            "index": "fm-product-sales"
        }
    }
});
```
