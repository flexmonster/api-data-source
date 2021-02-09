const axios = require('axios');
const expect = require('chai').expect;
var config = require('../config.json');

const url = config.url + "/select";

describe('Select request for drill-through view', function () {

    it('Grand total drill-throught', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "fields": [{ "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }] }, "page": 0 }

        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }]);
            expect(response.data.hits).to.deep.include(["Cars", 1563753600000, 23360]);
            expect(response.data.hits).to.deep.include(["Components", 1591920000000, 717]);
            expect(response.data.hits).to.deep.include(["Cars", 1567555200000, 53599]);
            expect(response.data.hits).to.deep.include(["Components", 1542844800000, 336]);
            expect(response.data.hits).to.deep.include(["Components", 1543017600000, 16]);
            expect(response.data.hits).to.deep.include(["Components", 1569456000000, 331]);
            expect(response.data.hits).to.deep.include(["Accessories", 1565481600000, 242]);
            expect(response.data.hits).to.deep.include(["Cars", 1563753600000, 23360]);
            expect(response.data.hits).to.deep.include(["Accessories", 1599004800000, 1654]);
            expect(response.data.hits).to.deep.include(["Bikes", 1547510400000, 2360]);
            expect(response.data.hits).to.deep.include(["Cars", 1550361600000, 6606]);
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

    it('Grand total drill-throught(nulls)', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Discount" }] }, "page": 0 }

        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Discount" }]);
            expect(response.data.hits).to.satisfy((members) => {
                return members.some(
                    (item) => {
                        return (item[1] === config.emptyValue) ? true : false;
                    });
            });
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

    it('Grand total(with filter)', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "between": [1518991200000, 1535835599999] } }], "fields": [{ "uniqueName": "Order Date" }, { "uniqueName": "Country" }, { "uniqueName": "Discount" }] }, "page": 0 }

        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Order Date" }, { "uniqueName": "Country" }, { "uniqueName": "Discount" }]);
            expect(response.data.hits).to.satisfy((members) => {
                return members.some(
                    (item) => {
                        return (item[2] === config.emptyValue) ? true : false;
                    });
            });
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

    it('Limit : 100', function (done) {

        const requestBody = { "type": "select", "index": "data", "query": { "limit": 100, "fields": [{ "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }] }, "page": 0 }

        axios.post(url, requestBody).then(function (response) {
            expect(response.status).to.equal(200);
            expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }]);
            expect(response.data.hits).to.have.lengthOf(100);
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

    //implementing this filter is required for basic functionality
    describe('Include filter', function () {
        it('Empty response', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "purple" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "Australia" }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(0);
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

        it('Include filter - 1', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "purple" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "Canada" }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(3);
                expect(response.data.hits).to.deep.include(["purple", "Canada", 971]);
                expect(response.data.hits).to.deep.include(["purple", "Canada", 955]);
                expect(response.data.hits).to.deep.include(["purple", "Canada", 708]);
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

        it('Include filter - 2', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "yellow" }] }, { "field": { "uniqueName": "Category" }, "include": [{ "member": "Accessories" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "France" }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(4);
                expect(response.data.hits).to.deep.include(["yellow", "Accessories", "France", 150]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Include filter - 3', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "yellow" }] }, { "field": { "uniqueName": "Category" }, "include": [{ "member": "Accessories" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "France" }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(4);
                expect(response.data.hits).to.deep.include(["yellow", "Accessories", "France", 150]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Include filter - 4', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "green" }] }, { "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "France" }] }, { "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1564531200000 }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }] }, "page": 0 }


            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(1);
                expect(response.data.hits).to.deep.include(["green", "Cars", "France", 1564531200000, 44932]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
    });

    describe('Include + exclude filter', function () {
        it('Include + exclude filter - 1', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }, { "field": { "uniqueName": "Category" }, "exclude": [{ "member": "Components" }] }, { "field": { "uniqueName": "Country" }, "include": [{ "member": "Australia" }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(4);
                expect(response.data.hits).to.deep.include(["blue", "Bikes", "Australia", 6688]);
                expect(response.data.hits).to.deep.include(["blue", "Bikes", "Australia", 7833]);
                expect(response.data.hits).to.deep.include(["blue", "Bikes", "Australia", 8393]);
                expect(response.data.hits).to.deep.include(["blue", "Bikes", "Australia", 8499]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Include + exclude filter - 2', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "Canada" }] }, { "field": { "uniqueName": "Category" }, "exclude": [{ "member": "Bikes" }, { "member": "Components" }] }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "purple" }] }, { "field": { "uniqueName": "Color" }, "exclude": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Category" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Country" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(3);
                expect(response.data.hits).to.deep.include(["Canada", "Clothing", "purple", 971]);
                expect(response.data.hits).to.deep.include(["Canada", "Clothing", "purple", 955]);
                expect(response.data.hits).to.deep.include(["Canada", "Clothing", "purple", 708]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

    });


    describe('String filters', function () {
        it('Equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "equal": "Australia" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "white" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(4);
                expect(response.data.hits).to.deep.include(["Australia", 1596931200000, "white", 6829]);
                expect(response.data.hits).to.deep.include(["Australia", 1521504000000, "white", 7310]);
                expect(response.data.hits).to.deep.include(["Australia", 1520294400000, "white", 2038]);
                expect(response.data.hits).to.deep.include(["Australia", 1555459200000, "white", 6152]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_equal": "Australia" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "white" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(200);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] !== "Australia") ? true : false;
                        });
                });
                expect(response.data.hits).to.deep.include(["United Kingdom", 1552176000000, "white", 12766]);
                expect(response.data.hits).to.deep.include(["United States", 1567987200000, "white", 20699]);
                expect(response.data.hits).to.deep.include(["France", 1592265600000, "white", 7043]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Begin filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "begin": "U" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "yellow" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(8);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] === "United Kingdom") || (item[0] === "United States") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not begin filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_begin": "U" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "yellow" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(32);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] !== "United Kingdom") && (item[0] !== "United States") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('End filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "end": "a" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "purple" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(3);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] === "Canada") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not end filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_end": "a" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "purple" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(7);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] !== "Canada") && (item[0] !== "Australia") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Contain filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "contain": "e" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(265);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] !== "Canada") && (item[0] !== "Australia") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not contain filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_contain": "e" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(66);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] === "Canada") || (item[0] === "Australia") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Greater filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "greater": "France" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(220);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] !== "Canada") && (item[0] !== "Australia" && (item[0] !== "France")) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Greater equal filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "greater_equal": "France" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(265);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] === "Germany") || (item[0] === "United Kingdom") || (item[0] === "United States") || (item[0] === "France") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Less filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "less": "France" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(66);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return ((item[0] === "Canada") || (item[0] === "Australia")) && (item[0] !== "France") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Less equal filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "less_equal": "France" } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(111);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] === "Canada") || (item[0] === "Australia") || (item[0] === "France") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Between filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "between": ["France", "United Kingdom"] } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "green" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(31);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] === "United Kingdom") || (item[0] === "Germany") || (item[0] === "France") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not between filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "not_between": ["France", "United Kingdom"] } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "green" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(128);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] === "Canada") || (item[0] === "Australia") || (item[0] === "United States") ? true : false;
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
        it('Equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "between": [1538265600000, 1538351999999] } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(1);
                expect(response.data.hits).to.deep.include(["United States", 1538265600000, "green", 976]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "not_between": [1538265600000, 1538351999999] } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(998);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] < 1538265600000) || (item[1] > 1538351999999) ? true : false;
                        });
                });
                expect(response.data.hits).to.deep.include(["United Kingdom", 1552176000000, "white", 12766]);
                expect(response.data.hits).to.deep.include(["United States", 1567987200000, "white", 20699]);
                expect(response.data.hits).to.deep.include(["France", 1592265600000, "white", 7043]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('After filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "after": 1546387200000 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "red" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(155);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] > 1546387200000) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('After equal filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "after_equal": 1546387200000 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "red" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(156);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.some(
                        (item) => {
                            return (item[1] >= 1546387200000) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Before filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "before": 1546387200000 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "red" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(99);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] < 1546387200000) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Before equal filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "before_equal": 1546387200000 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "red" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(100);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.some(
                        (item) => {
                            return (item[1] <= 1546387200000) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Between filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "between": [1546387200000, 1546559999999] } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(2);
                expect(response.data.hits).to.deep.include(["Germany", 1546387200000, "red", 683]);
                expect(response.data.hits).to.deep.include(["Canada", 1546473600000, "white", 838]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not between filter', function (done) {
            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Order Date" }, "query": { "not_between": [1546387200000, 1546559999999] } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(997);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] < 1546387200000) || (item[1] > 1546559999999) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

    });

    describe('Number filters', function () {
        it('Equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "equal": 50 } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(7);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] === 50) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Not equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "not_equal": 50 } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(992);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] !== 50) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Greater filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "Australia" }] }, { "field": { "uniqueName": "Discount" }, "query": { "greater": 50 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }
            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(20);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] > 50) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
        it('Greater equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "Canada" }] }, { "field": { "uniqueName": "Discount" }, "query": { "greater_equal": 50 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "white" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(40);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.some(
                        (item) => {
                            return (item[1] >= 50) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
        it('Less filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "France" }] }, { "field": { "uniqueName": "Discount" }, "query": { "less": 50 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(15);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] < 50) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
        it('Less equal filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "France" }] }, { "field": { "uniqueName": "Discount" }, "query": { "less_equal": 50 } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "blue" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(17);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.some(
                        (item) => {
                            return (item[1] <= 50) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
        it('Between filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "between": [20, 60] } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "purple" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(1);
                expect(response.data.hits).to.deep.include(["United States", 60, "purple", 422]);
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
        it('Not between filter', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Discount" }, "query": { "not_between": [20, 60] } }, { "field": { "uniqueName": "Color" }, "include": [{ "member": "purple" }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Color" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(5);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[1] < 20) || (item[1] > 60) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

    });

    describe('Multiple filters', function () {
        it('Multiple filter - 1', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "equal": "Canada" } }, { "field": { "uniqueName": "Business Type" }, "query": { "contain": "r" } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(156);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] == "Canada") && (item[1] !== "Specialty Bike Shop") ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Multiple filter - 2', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "equal": "Canada" } }, { "field": { "uniqueName": "Discount" }, "query": { "greater": 50 } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(87);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return (item[0] == "Canada") && (item[1] > 50) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('Multiple filter - 3', function (done) {

            const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "query": { "begin": "u" } }, { "field": { "uniqueName": "Order Date" }, "query": { "after": 1557446400000 } }, { "field": { "uniqueName": "Discount" }, "query": { "less": 80 } }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }] }, "page": 0 }

            axios.post(url, requestBody).then(function (response) {
                expect(response.status).to.equal(200);
                expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Order Date" }, { "uniqueName": "Discount" }, { "uniqueName": "Price" }]);
                expect(response.data.hits).to.have.lengthOf(102);
                expect(response.data.hits).to.satisfy((members) => {
                    return members.every(
                        (item) => {
                            return ((item[0] === "United Kingdom") || (item[0] === "United States")) && (item[1] > 1557446400000) && (item[2] < 80) ? true : false;
                        });
                });
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });


    });

    if (config.hierarchy) {
        describe('Hierarchy filters', function () {
            it('Hierarchy filter - 1', function (done) {

                const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "Germany", "filter": { "field": { "uniqueName": "Color" }, "include": [{ "member": "red", "filter": { "field": { "uniqueName": "Business Type" }, "include": [{ "member": "Value Added Reseller" }] } }] } }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Color" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Color" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }]);
                    expect(response.data.hits).to.have.lengthOf(53);
                    expect(response.data.hits).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                return (item[0] == "Germany") && (item[1] == "red") && (item[2] == "Value Added Reseller") ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Hierarchy filter - 2', function (done) {

                const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Country" }, "include": [{ "member": "Canada", "filter": { "field": { "uniqueName": "Color" }, "include": [{ "member": "white" }] } }] }], "fields": [{ "uniqueName": "Country" }, { "uniqueName": "Color" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Country" }, { "uniqueName": "Color" }, { "uniqueName": "Business Type" }, { "uniqueName": "Price" }]);
                    expect(response.data.hits).to.have.lengthOf(81);
                    expect(response.data.hits).to.satisfy((members) => {
                        return members.every(
                            (item) => {
                                return (item[0] == "Canada") && (item[1] == "white") ? true : false;
                            });
                    });
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Hierarchy filter with date', function (done) {

                const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "blue", "filter": { "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars", "filter": { "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1523318400000 }] } }] } }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }]);
                    expect(response.data.hits).to.have.lengthOf(1);
                    expect(response.data.hits).to.deep.include(["blue", "Cars", 1523318400000, 30384]);
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Hierarchy filter with number - 1', function (done) {

                const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "green", "filter": { "field": { "uniqueName": "Category" }, "include": [{ "member": "Accessories", "filter": { "field": { "uniqueName": "Quantity" }, "include": [{ "member": 66 }] } }] } }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Quantity" }, { "uniqueName": "Price" }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Quantity" }, { "uniqueName": "Price" }]);
                    expect(response.data.hits).to.have.lengthOf(1);
                    expect(response.data.hits).to.deep.include(["green", "Accessories", 66, 680]);
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

            it('Hierarchy filter with number - 2', function (done) {

                const requestBody = { "type": "select", "index": "data", "query": { "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "blue", "filter": { "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars", "filter": { "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1523318400000 }] } }] } }] }], "fields": [{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }] }, "page": 0 }
                axios.post(url, requestBody).then(function (response) {
                    expect(response.status).to.equal(200);
                    expect(response.data.fields).to.deep.include.members([{ "uniqueName": "Color" }, { "uniqueName": "Category" }, { "uniqueName": "Order Date" }, { "uniqueName": "Price" }]);
                    expect(response.data.hits).to.have.lengthOf(1);
                    expect(response.data.hits).to.deep.include(["blue", "Cars", 1523318400000, 30384]);
                    done();
                })
                    .catch(function (error) {
                        done(error);
                    });
            });

        });
    }

});