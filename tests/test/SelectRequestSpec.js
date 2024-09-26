const axios = require('axios');
const expect = require('chai').expect;
var config = require('../config.json');

const url = config.url + "/select";

describe('Select request', function () {

    it('Select request', function (done) {
        const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] } }, "page": 0 }

        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.aggs).to.have.lengthOf(36);
            expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 4480 } }, "keys": { "Color": "red", "Category": "Clothing" } });
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

    describe('Select request aggregations', function () {
        it('Aggregations - sum', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Discount" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(36);
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "sum": 44350 } } });
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "sum": 9462 } }, "keys": { "Color": "white" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Aggregations - average', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "average", "field": { "uniqueName": "Price" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(36);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "average": 6538.25 } }, "keys": { "Color": "purple", "Category": "Bikes" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "average": 1167 } }, "keys": { "Color": "green", "Category": "Accessories" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Aggregations - average(with nulls)', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "average", "field": { "uniqueName": "Discount" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(36);
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "average": 65.5 } }, "keys": { "Color": "purple", "Category": "Bikes" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "average": 48 } }, "keys": { "Color": "white", "Category": "Accessories" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Aggregations - count', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "count", "field": { "uniqueName": "Discount" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(36);
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "count": 881 } } });
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "count": 2 } }, "keys": { "Color": "yellow", "Category": "Clothing" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Aggregations - distinctcount', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "distinctcount", "field": { "uniqueName": "Discount" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(36);
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "distinctcount": 101 } } });
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "distinctcount": 2 } }, "keys": { "Color": "yellow", "Category": "Clothing" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Aggregations - min', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "min", "field": { "uniqueName": "Discount" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(36);
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "min": 0 } } });
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "min": 60 } }, "keys": { "Color": "purple" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
        it('Aggregations - max', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "max", "field": { "uniqueName": "Discount" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(36);
                expect(response.data.aggs).to.deep.include({ "values": { "Discount": { "max": 100 } } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Aggregations(with dates) - 2', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }] }, "values": [{ "func": "max", "field": { "uniqueName": "Order Date" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(7);
                expect(response.data.aggs).to.deep.include({ "values": { "Order Date": { "max": 1599004800000 } }, "keys": { "Color": "purple" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Aggregations - 3', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Discount" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] } }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(1714);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 6221870 } } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
    });
    describe('Select request expand', function () {
        it('Expand(rows) - 1', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }, { "uniqueName": "Country" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 18757 } }, "keys": { "Color": "blue", "Country": "United States", "Category": "Bikes" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 22089 } }, "keys": { "Color": "blue", "Country": "Australia", "Category": "Components" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 18615 } }, "keys": { "Color": "blue", "Country": "France", "Category": "Components" } });

                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Expand(rows) - 2', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }, { "uniqueName": "Country" }, { "uniqueName": "Business Type" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "Germany" }] }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 1046 } }, "keys": { "Color": "blue", "Country": "Germany", "Business Type": "Specialty Bike Shop", "Category": "Clothing" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 238558 } }, "keys": { "Color": "blue", "Country": "Germany", "Business Type": "Value Added Reseller", "Category": "Cars" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 12026 } }, "keys": { "Color": "blue", "Country": "Germany", "Business Type": "Specialty Bike Shop" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Expand(rows) - 3', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }, { "uniqueName": "Country" }, { "uniqueName": "Business Type" }, { "uniqueName": "Order Date" }, { "uniqueName": "Quantity" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "Germany" }] }, { "field": { "uniqueName": "Business Type" }, "include": [{ "member": "Warehouse" }] }, { "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1515110400000 }] }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 192 } }, "keys": { "Color": "blue", "Country": "Germany", "Business Type": "Warehouse", "Order Date": 1515110400000, "Category": "Components" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Expand(columns)', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Discount" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "United States" }] }, { "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1519516800000 }] }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 15310 } }, "keys": { "Color": "white", "Category": "Cars", "Country": "United States", "Order Date": 1519516800000, "Discount": 49 } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Expand(columns+rows) - 1', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }, { "uniqueName": "Business Type" }, { "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Discount" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }, { "func": "average", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "green" }] }, { "field": { "uniqueName": "Business Type" }, "include": [{ "member": "Warehouse" }] }, { "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "Australia" }] }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(0);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Expand(columns+rows) - 2', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }, { "uniqueName": "Business Type" }], "cols": [{ "uniqueName": "Category" }, { "uniqueName": "Country" }] }, "values": [{ "func": "average", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "green" }] }, { "field": { "uniqueName": "Category" }, "include": [{ "member": "Bikes" }] }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "average": 2499.75 } }, "keys": { "Color": "green", "Business Type": "Specialty Bike Shop", "Category": "Bikes", "Country": "Germany" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "average": 4886 } }, "keys": { "Color": "green", "Business Type": "Specialty Bike Shop", "Category": "Bikes", "Country": "United Kingdom" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "average": 5171.75 } }, "keys": { "Color": "green", "Business Type": "Specialty Bike Shop", "Category": "Bikes", "Country": "United States" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "average": 5187.75 } }, "keys": { "Color": "green", "Business Type": "Value Added Reseller", "Category": "Bikes", "Country": "Australia" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
    });
    describe('Select request filters', function () {

        describe('Number filters', function () {
            it('Filters(number) - equal ', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "equal": 0 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(10);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount === 0) || (item.keys.Discount === undefined)) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(number) - not equal ', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "not_equal": 0 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(485);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount !== 0) || (item.keys.Discount === undefined)) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(number) - greater', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "greater": 90 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(55);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount > 90) || (item.keys.Discount === undefined)) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(number) - greater equal ', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "greater_equal": 90 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(61);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount >= 90) || (item.keys.Discount === undefined)) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(number) - less ', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "less": 10 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(50);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount < 10) || (item.keys.Discount === undefined)) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(number) - less equal ', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "less_equal": 10 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(54);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount <= 10) || (item.keys.Discount === undefined)) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(number) - between ', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "between": [5, 20] } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(81);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount >= 5) && (item.keys.Discount <= 20)) || (item.keys.Discount === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(number) - not between ', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Discount" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "not_between": [5, 20] } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(408);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Discount < 5) || (item.keys.Discount > 20)) || (item.keys.Discount === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });
        });

        describe('Date filters', function () {

            it('Filters(date) - between', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "between": [1535760000000, 1541116799999] } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(107);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys["Order Date"] >= 1535760000000) && (item.keys["Order Date"] <= 1541116799999)) || (item.keys["Order Date"] === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(date) - not between', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "not_between": [1535760000000, 1541116799999] } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(1422);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys["Order Date"] < 1535760000000) || (item.keys["Order Date"] > 1541116799999)) || (item.keys["Order Date"] === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(date) - before', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "before": 1535846400000 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(382);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys["Order Date"] < 1535846400000) || (item.keys["Order Date"] === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(date) - before equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "before_equal": 1535846400000 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(385);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys["Order Date"] <= 1535846400000) || (item.keys["Order Date"] === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(date) - after', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "after": 1535846400000 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(1145);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys["Order Date"] > 1535846400000) || (item.keys["Order Date"] === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(date) - after equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Order Date" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "after_equal": 1535846400000 } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(1148);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys["Order Date"] >= 1535846400000) || (item.keys["Order Date"] === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

        });

        describe('String filters', function () {
            it('Filters(string) - equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "equal": "Canada" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(14);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Country === "Canada") || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - not equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_equal": "Canada" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(38);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Country !== "Canada") || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - begin', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "begin": "U" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(20);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country === "United States") || (item.keys.Country === "United Kingdom")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - not begin', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_begin": "U" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(32);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country !== "United States") && (item.keys.Country !== "United Kingdom")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - end', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "end": "a" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(20);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country === "Australia") || (item.keys.Country === "Canada")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - not end', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_end": "a" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(32);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country !== "Australia") && (item.keys.Country !== "Canada")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - contain', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "contain": "u" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(26);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country === "Australia") || (item.keys.Country === "United Kingdom") || (item.keys.Country === "United States")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - not contain', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_contain": "u" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(26);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country !== "Australia") && (item.keys.Country !== "United Kingdom") && (item.keys.Country !== "United States")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - less', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "less": "Canada" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(12);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Country === "Australia") || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - less equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "less_equal": "Canada" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(20);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country === "Canada") || (item.keys.Country === "Australia") && (item.keys.Country !== "United States")) || (item.keys["Order Date"] === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - greater', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "greater": "Canada" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(32);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country !== "Australia") && (item.keys.Country !== "Canada")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - greater equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "greater_equal": "Canada" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(39);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country !== "Australia")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - between', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "between": ["Canada", "Germany"] } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(26);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country === "Canada") || (item.keys.Country === "France") || (item.keys.Country === "Germany")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(string) - not between', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }], "cols": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_between": ["Canada", "Germany"] } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(26);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return ((item.keys.Country !== "Canada") && (item.keys.Country !== "France") && (item.keys.Country !== "Germany")) || (item.keys.Country === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

        });

        it('Filters(string) - contains + begin', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "contain": "l" } }, { "field": { "uniqueName": "Category" }, "query": { "begin": "C" } }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(12);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 3794 } }, "keys": { "Color": "purple", "Category": "Clothing" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 957494 } } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Filters(string) + expand - 1', function (done) {

            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }, { "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "contain": "l" } }, { "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars", "filter": { "field": { "uniqueName": "Category" }, "query": { "begin": "C" } } }] }, { "field": { "uniqueName": "Country" }, "query": { "not_end": "y" } }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 266541 } }, "keys": { "Color": "blue", "Category": "Cars", "Country": "United Kingdom" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 309813 } }, "keys": { "Color": "blue", "Category": "Cars", "Country": "Canada" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Filters(string) + expand - 2', function (done) {
            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }, { "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "contain": "l" } }, { "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars", "filter": { "field": { "uniqueName": "Category" }, "query": { "begin": "C" } } }] }, { "field": { "uniqueName": "Country" }, "query": { "not_end": "y" } }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 266541 } }, "keys": { "Color": "blue", "Category": "Cars", "Country": "United Kingdom" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 309813 } }, "keys": { "Color": "blue", "Category": "Cars", "Country": "Canada" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Filters + expand', function (done) {
            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Country" }, { "uniqueName": "Category" }], "cols": [{ "uniqueName": "Color" }, { "uniqueName": "Size" }, { "uniqueName": "Business Type" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }, { "func": "average", "field": { "uniqueName": "Quantity" } }] }, "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "Canada", "filter": { "field": { "uniqueName": "Country" }, "query": { "between": ["Canada", "Germany"] } } }] }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "yellow", "filter": { "field": { "uniqueName": "Color" }, "query": { "contain": "e" } } }] }, { "field": { "uniqueName": "Size" }, "include": [{ "member": "97 oz", "filter": { "field": { "uniqueName": "Size" }, "query": { "begin": "9" } } }] }, { "field": { "uniqueName": "Business Type" }, "include": [{ "member": "Value Added Reseller" }] }, { "field": { "uniqueName": "Order Date" }, "query": { "before": 1535846400000 } }, { "field": { "uniqueName": "Discount" }, "query": { "between": [5, 50] } }, { "field": { "uniqueName": "Quantity" }, "query": { "between": [126, 483] } }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 1241 }, "Quantity": { "average": 483 } }, "keys": { "Country": "Canada", "Category": "Accessories", "Color": "yellow", "Size": "97 oz", "Business Type": "Value Added Reseller" } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Filters(number) - two number filters and two string filters ', function (done) {
            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Quantity" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "contain": "l" } }, { "field": { "uniqueName": "Discount" }, "query": { "less": 50 } }, { "field": { "uniqueName": "Quantity" }, "query": { "between": [126, 483] } }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(13);
                expect(response.data.aggs).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            if (item.keys === undefined) {
                                return true;
                            }
                            return (((item.keys.Quantity >= 126) && (item.keys.Quantity <= 483)) || (item.keys.Quantity === undefined)) && ((item.keys.Discount < 50) || (item.keys.Discount === undefined)) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Filters(date + number)', function (done) {
            const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Category" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "contain": "l" } }, { "field": { "uniqueName": "Discount" }, "query": { "less": 50 } }, { "field": { "uniqueName": "Order Date" }, "query": { "between": [1535760000000, 1541116799999] } }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.aggs).to.have.lengthOf(4);
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 3348 } }, "keys": { "Color": "blue" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 3348 } }, "keys": { "Color": "blue", "Category": "Components" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 3348 } }, "keys": { "Category": "Components" } });
                expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 3348 } } });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
    });

    if (config.valueFilters) {
        describe('Value filters', function () {
            it('Filters(value) - top', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "top": 3 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(27);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "red") || (item.keys.Color === "white") || (item.keys.Color === "green") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - bottom', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "bottom": 3 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(25);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "purple") || (item.keys.Color === "yellow") || (item.keys.Color === "blue") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "equal": 2179775 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(14);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "red") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - not equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "not_equal": 2179775 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(38);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color !== "red") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - greater', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "greater": 1046834 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(27);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "red") || (item.keys.Color === "white") || (item.keys.Color === "green") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - greater equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }], "cols": [{ "uniqueName": "Country" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "greater_equal": 1046834 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(34);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.some(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "blue") ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - less', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "less": 1046834 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(3);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "purple") || (item.keys.Color === "yellow") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 85302 } } });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - less equal', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "less_equal": 1046834 }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(4);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.some(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "blue") ? true : false;
                            });
                    });
                    expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 1132136 } } });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });


            it('Filters(value) - between', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "between": [1046834, 1806444] }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(4);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "blue") || (item.keys.Color === "green") || (item.keys.Color === "white") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 3956793 } } });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Filters(value) - not between', function (done) {
                const requestBody = { "type": "select", "querytype": "select", "index": "data", "query": { "aggs": { "by": { "rows": [{ "uniqueName": "Color" }] }, "values": [{ "func": "sum", "field": { "uniqueName": "Price" } }] }, "filter": [{ "field": { "uniqueName": "Color" }, "query": { "not_between": [1046834, 1806444] }, "value": { "field": { "uniqueName": "Price" }, "func": "sum" } }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.aggs).to.have.lengthOf(4);
                    expect(response.data.aggs).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                if (item.keys === undefined) {
                                    return true;
                                }
                                return (item.keys.Color === "purple") || (item.keys.Color === "red") || (item.keys.Color === "yellow") || (item.keys.Color === undefined) ? true : false;
                            });
                    });
                    expect(response.data.aggs).to.deep.include({ "values": { "Price": { "sum": 2265077 } } });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

        });
    }
});
