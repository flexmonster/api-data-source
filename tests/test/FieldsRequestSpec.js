const axios = require('axios');
const expect = require('chai').expect;
var config = require('../config.json');

const url = config.url + "/fields";

const requestBody = { "type": "fields", "index": "data" }

describe('Fields request', function () {
    let promise;

    before(function () {
        promise = axios.post(url, requestBody);
    });

    it('should return 200 status code', function (done) {
        promise.then(function (response) {
            expect(response.status).to.equal(200);
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });

    it('response type check', function (done) {
        promise.then(function (response) {
            response.data.fields.forEach((item) => {
                expect(item).to.have.property("uniqueName").that.is.a("string");
                expect(item).to.have.property("type").that.is.oneOf(["string", "number", "date"]);
                if (item.caption) {
                    expect(item).to.have.property("caption").that.is.a("string");
                }
                if (item.folder) {
                    expect(item).to.have.property("folder").that.is.a("string");
                }
                if (item.interval) {
                    expect(item.type).to.be("date");
                    expect(item).to.have.property("interval").that.is.a("string");
                }
                if (item.aggregations) {
                    expect(item).to.have.property("aggregations").that.is.a("array");
                    expect(["sum", "count", "distinctcount", "average", "median", "product",
                        "min", "max", "stdevp", "stdevs", "none"]).to.include.members(item.aggregations);
                }
                if (item.filters !== undefined) {
                    expect(typeof item.filters).to.be.oneOf(["boolean", "object"]);
                    expect(item.filters).not.to.be.a("array");
                    if (typeof item.filters === "object") {
                        if (item.filters.members !== undefined) {
                            expect(item.filters.members).to.be.a("boolean");
                        }
                        if (item.filters.query !== undefined) {
                            expect(item.filters.query).to.satisfy(isBooleanOrArray);
                            if (Array.isArray(item.filters.query)) {
                                if (item.type === "string") {
                                    expect(["equal", "not_equal", "begin", "not_begin", "end", "not_end", "contain", "not_contain",
                                        "greater", "greater_equal", "less", "less_equal", "between", "not_between"]).to.include.members(item.filters.query);
                                } else if (item.type === "number") {
                                    expect(["equal", "not_equal", "greater", "greater_equal", "less", "less_equal", "between",
                                        "not_between"]).to.include.members(item.filters.query);
                                } else if (item.type === "date") {
                                    expect(["equal", "not_equal", "before", "before_equal", "after", "after_equal", "between",
                                        "not_between", "last", "current", "next"]).to.include.members(item.filters.query);
                                }
                            }
                        }
                        if (item.filters.valueQuery !== undefined) {
                            expect(item.filters.valueQuery).to.satisfy(isBooleanOrArray);
                            if (Array.isArray(item.filters.valueQuery)) {
                                expect(["top", "bottom", "equal", "not_equal", "greater", "greater_equal", "less",
                                    "less_equal", "between", "not_between"]).to.include.members(item.filters.valueQuery);
                            }
                        }
                    }
                }
            });

            if (response.data.aggregations) {
                expect(typeof response.data.aggregations).to.equal("object");
                if (Array.isArray(response.data.aggregations)) {
                    expect(["sum", "count", "distinctcount", "average", "median", "product",
                        "min", "max", "stdevp", "stdevs", "none"]).to.include.members(response.data.aggregations);
                } else if (typeof response.data.aggregations === "object") {
                    if (response.data.aggregations.any) {
                        expect(["sum", "count", "distinctcount", "average", "median", "product",
                            "min", "max", "stdevp", "stdevs", "none"]).to.include.members(response.data.aggregations.any);
                    }
                    if (response.data.aggregations.date) {
                        expect(["sum", "count", "distinctcount", "average", "median", "product",
                            "min", "max", "stdevp", "stdevs", "none"]).to.include.members(response.data.aggregations.date);
                    }
                    if (response.data.aggregations.number) {
                        expect(["sum", "count", "distinctcount", "average", "median", "product",
                            "min", "max", "stdevp", "stdevs", "none"]).to.include.members(response.data.aggregations.number);
                    }
                    if (response.data.aggregations.string) {
                        expect(["sum", "count", "distinctcount", "average", "median", "product",
                            "min", "max", "stdevp", "stdevs", "none"]).to.include.members(response.data.aggregations.string);
                    }
                }
            }

            if (response.data.filters !== undefined) {
                expect(typeof response.data.filters).to.be.oneOf(["boolean", "object"]);
                expect(response.data.filters).not.to.be.a("array");
                if (typeof response.data.filters === "object") {
                    if (response.data.filters.any) {
                        expect(typeof response.data.filters.any).to.be.oneOf(["boolean", "object"]);
                        expect(response.data.filters.any).not.to.be.a("array");
                        if (typeof response.data.filters.any === "object") {
                            if (response.data.filters.any.members !== undefined) {
                                expect(response.data.filters.any.members).to.be.a("boolean");
                            }
                            if (response.data.filters.any.query !== undefined) {
                                expect(response.data.filters.any.query).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.any.query)) {
                                    expect(["equal", "not_equal", "between", "not_between"]).to.include.members(response.data.filters.any.query);
                                }
                            }
                            if (response.data.filters.any.valueQuery !== undefined) {
                                expect(response.data.filters.any.valueQuery).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.any.valueQuery)) {
                                    expect(["top", "bottom", "equal", "not_equal", "greater", "greater_equal", "less",
                                        "less_equal", "between", "not_between"]).to.include.members(iresponse.data.filters.any.valueQuery);
                                }
                            }
                        }
                    }
                    if (response.data.filters.date) {
                        expect(typeof response.data.filters.date).to.be.oneOf(["boolean", "object"]);
                        expect(response.data.filters.date).not.to.be.a("array");
                        if (typeof response.data.filters.date === "object") {
                            if (response.data.filters.date.members !== undefined) {
                                expect(response.data.filters.date.members).to.be.a("boolean");
                            }
                            if (response.data.filters.date.query !== undefined) {
                                expect(response.data.filters.date.query).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.date.query)) {
                                    expect(["equal", "not_equal", "before", "before_equal", "after", "after_equal", "between",
                                        "not_between", "last", "current", "next"]).to.include.members(response.data.filters.date.query);
                                }
                            }
                            if (response.data.filters.date.valueQuery !== undefined) {
                                expect(response.data.filters.date.valueQuery).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.date.valueQuery)) {
                                    expect(["top", "bottom", "equal", "not_equal", "greater", "greater_equal", "less",
                                        "less_equal", "between", "not_between"]).to.include.members(response.data.filters.date.valueQuery);
                                }
                            }
                        }
                    }
                    if (response.data.filters.number) {
                        expect(typeof response.data.filters.number).to.be.oneOf(["boolean", "object"]);
                        expect(response.data.filters.number).not.to.be.a("array");
                        if (typeof response.data.filters.number === "object") {
                            if (response.data.filters.number.members !== undefined) {
                                expect(response.data.filters.number.members).to.be.a("boolean");
                            }
                            if (response.data.filters.number.query !== undefined) {
                                expect(response.data.filters.number.query).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.number.query)) {
                                    expect(["equal", "not_equal", "greater", "greater_equal",
                                        "less", "less_equal", "between", "not_between"]).to.include.members(response.data.filters.number.query);
                                }
                            }
                            if (response.data.filters.number.valueQuery !== undefined) {
                                expect(response.data.filters.number.valueQuery).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.number.valueQuery)) {
                                    expect(["top", "bottom", "equal", "not_equal", "greater", "greater_equal", "less",
                                        "less_equal", "between", "not_between"]).to.include.members(response.data.filters.number.valueQuery);
                                }
                            }
                        }
                    }
                    if (response.data.filters.string) {
                        expect(typeof response.data.filters.string).to.be.oneOf(["boolean", "object"]);
                        expect(response.data.filters.string).not.to.be.a("array");
                        if (typeof response.data.filters.string === "object") {
                            if (response.data.filters.string.members !== undefined) {
                                expect(response.data.filters.string.members).to.be.a("boolean");
                            }
                            if (response.data.filters.string.query !== undefined) {
                                expect(response.data.filters.string.query).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.string.query)) {
                                    expect(["equal", "not_equal", "begin", "not_begin", "end", "not_end", "contain",
                                        "not_contain", "greater", "greater_equal", "less", "less_equal", "between", "not_between"]).to.include.members(response.data.filters.string.query);
                                }
                            }
                            if (response.data.filters.string.valueQuery !== undefined) {
                                expect(response.data.filters.string.valueQuery).to.satisfy(isBooleanOrArray);
                                if (Array.isArray(response.data.filters.string.valueQuery)) {
                                    expect(["top", "bottom", "equal", "not_equal", "greater", "greater_equal", "less",
                                        "less_equal", "between", "not_between"]).to.include.members(response.data.filters.string.valueQuery);
                                }
                            }
                        }
                    }
                }
            }
            if (response.data.sorted !== undefined) {
                expect(response.data).to.have.property("sorted").that.is.a('boolean');
            }
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });

    
    function isBooleanOrArray(query) {
        if (Array.isArray(query) || (typeof query === "boolean")) {
            return true;
        }
        return false;
    }

    const uniqueNames = ["Category", "Size", "Color", "Destination", "Business Type", "Country", "Order Date", "Price",
     "Quantity", "Discount"];
    it('check uniqueName property values', function (done) {
        promise.then(function (response) {
            response.data.fields.forEach((item) => {
                expect(uniqueNames).to.include(item.uniqueName);
            });
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });

    it('check uniqueName property values(if sorted)', function (done) {
        promise.then(function (response) {
            if (response.data.sorted) {
                expect(response.data.sorted).to.equal(true);
                expect(response.data.fields[0]).to.equal("Business Type");
                expect(response.data.fields[1]).to.equal("Category");
                expect(response.data.fields[2]).to.equal("Color");
                expect(response.data.fields[3]).to.equal("Country");
                expect(response.data.fields[4]).to.equal("Destination");
                expect(response.data.fields[5]).to.equal("Discount");
                expect(response.data.fields[6]).to.equal("Order Date");
                expect(response.data.fields[7]).to.equal("Price");
                expect(response.data.fields[8]).to.equal("Quantity");
                expect(response.data.fields[9]).to.equal("Size");
            }
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });


    const uniqueNameTypeMap = {
        "Category": "string",
        "Color": "string",
        "Size": "string",
        "Destination": "string",
        "Business Type": "string",
        "Country": "string",
        "Order Date": "date",
        "Price": "number",
        "Quantity": "number",
        "Discount": "number"
    }

    it('check type property values', function (done) {
        promise.then(function (response) {
            response.data.fields.forEach((item) => {
                expect(item.type).to.equal(uniqueNameTypeMap[item.uniqueName]);
            });
            done();
        })
            .catch(function (error) {
                done(error);
            });
    });

});