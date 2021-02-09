const axios = require('axios');
const expect = require('chai').expect;
var config = require('../config.json');

const url = config.url + "/select";

describe('Select request for flat table', function () {

    it('Flat request', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "aggs": { "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
            expect(response.data.hits).to.have.lengthOf(999);

            if (response.data.page !== undefined) {
                expect(response.data.page).to.be.a("number");
            }
            if (response.data.pageTotal !== undefined) {
                expect(response.data.pageTotal).to.be.a("number");
            }
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });

    it('Flat request with filters - 1', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "aggs": { "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "Australia" }, { "member": "Canada" }] }, { "field": { "uniqueName": "Order Date" }, "query": { "between": [1530403200000, 1562025599999] } }, { "field": { "uniqueName": "Color" }, "query": { "contain": "l" } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }
        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
            expect(response.data.hits).to.have.lengthOf(33);
            expect(response.data.hits).to.satisfy((members) => {
                return members.every(
                    (item) => {
                        return ((item[0] === "Canada") || (item[0] === "Australia")) && (item[1] >= 1530403200000) && (item[1] <= 1562025599999) && ((item[2] === "blue") || (item[2] === "yellow") || (item[2] === "purple")) ? true : false;
                    });
            });
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });

    it('Flat request with filters - 2', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "aggs": { "values": [{ "func": "sum", "field": { "uniqueName": "Discount" } }, { "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Size" }, "query": { "begin": "1" } }, { "field": { "uniqueName": "Discount" }, "include": [{ "member": config.emptyValue }] }, { "field": { "uniqueName": "Price" }, "include": [{ "member": 1241 }] }], "fields": [{ "uniqueName": "Order Date" }, { "uniqueName": "Size" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }] }, "page": 0 }
        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Order Date" }, { "uniqueName": "Size" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }]);
            expect(response.data.hits).to.have.lengthOf(2);
            expect(response.data.aggs[0].values).to.have.deep.property("Price", { "sum": 2482 });
            expect(response.data.hits).to.deep.include([1562371200000, "152 oz", config.emptyValue, 1241]);
            expect(response.data.hits).to.deep.include([1516838400000, "107 oz", config.emptyValue, 1241]);
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });

    it('Flat request with filters - 3', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "aggs": { "values": [{ "func": "sum", "field": { "uniqueName": "Discount" } }, { "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1514851200000 }, { "member": 1514937600000 }, { "member": 1515024000000 }, { "member": 1515110400000 }] }, { "field": { "uniqueName": "Size" }, "query": { "between": ["1", "2"] } }], "fields": [{ "uniqueName": "Order Date" }, { "uniqueName": "Size" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }] }, "page": 0 }
        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Order Date" }, { "uniqueName": "Size" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }]);
            expect(response.data.hits).to.have.lengthOf(2);
            expect(response.data.aggs[0].values).to.have.deep.property("Price", { "sum": 59705 });
            expect(response.data.aggs[0].values).to.have.deep.property("Discount", { "sum": 12 });
            expect(response.data.hits).to.deep.include([1514851200000, "168 oz", config.emptyValue, 250]);
            expect(response.data.hits).to.deep.include([1514937600000, "172 oz", 12, 59455]);
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });
    
    describe('Select request for flat table with hierarchy', function () {

        it('Flat request with hierarchy - 1', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "aggs": { "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Color" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Color" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(999);
                expect(response.data.aggs[0].values).to.have.deep.property("Price", { "sum": 6221870 });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

    });
});
