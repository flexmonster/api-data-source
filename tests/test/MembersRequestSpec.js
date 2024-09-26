const axios = require('axios');
const expect = require('chai').expect;
var config = require('../config.json');

const url = config.url + "/members";

//note - disable interval property if set for date testing

describe('Members request', function () {

    function typeChecking(response, memberType) {
        expect(response.data.members).to.be.a("array");
        response.data.members.forEach((item) => {
            if (item.value && item.value !== 0) {
                expect(item.value).to.be.a(memberType);
            }
            if (item.id !== undefined) {
                expect(item.id).to.be.a("string");
            }
        });
        if (response.data.sorted !== undefined) {
            expect(response.data.sorted).to.be.a("boolean");
        }
        if (response.data.page !== undefined) {
            expect(response.data.page).to.be.a("number");
        }
        if (response.data.pageTotal !== undefined) {
            expect(response.data.pageTotal).to.be.a("number");
        }
    }

    describe('String member', function () {
        const requestBody = { "type": "members", "index": "data", "field": { "uniqueName": "Category" } }
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
                typeChecking(response, "string");
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });


        const category = ["Accessories", "Bikes", "Clothing", "Components", "Cars"];

        it('should return all members', function (done) {
            promise.then(function (response) {
                expect(response.data.members).to.be.lengthOf(5);
                response.data.members.forEach((item) => {
                    expect(category).to.include(item.value);
                });
                if (response.data.page !== undefined) {
                    expect(response.data.page).to.be.equal(0);
                }
                if (response.data.pageTotal !== undefined) {
                    expect(response.data.pageTotal).to.be.equal(1);
                }
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });

        it('should return all members(sorted)', function (done) {
            promise.then(function (response) {
                if (response.data.sorted) {
                    expect(response.data.members).to.be.lengthOf(5);
                    expect(response.data.members[0]).to.equal("Accessories");
                    expect(response.data.members[1]).to.equal("Bikes");
                    expect(response.data.members[2]).to.equal("Cars");
                    expect(response.data.members[3]).to.equal("Clothing");
                    expect(response.data.members[4]).to.equal("Components");
                }
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
    });

    describe('Number member + nulls', function () {
        const requestBody = { "type": "members", "index": "data", "field": { "uniqueName": "Discount" } }
        let promise;

        before(function () {
            promise = axios.post(url, requestBody);
        });

        it('response type check', function (done) {
            promise.then(function (response) {

                typeChecking(response, "number");
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });


        it('should return all members', function (done) {
            promise.then(function (response) {
                expect(response.data.members).to.be.lengthOf(102);
                expect(response.data.members).to.satisfy((members) => {
                    return members.some(
                        (item) => {
                            return (item.value === config.emptyValue)  ? true : false;
                        });
                });
                expect(response.data.members).to.deep.include({ "value": 26 });
                expect(response.data.members).to.deep.include({ "value": 0 });
                expect(response.data.members).to.deep.include({ "value": 100 });
                if (response.data.page !== undefined) {
                    expect(response.data.page).to.be.equal(0);
                }
                if (response.data.pageTotal !== undefined) {
                    expect(response.data.pageTotal).to.be.equal(1);
                }
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
    });

    describe('Data member', function () {
        const requestBody = { "type": "members", "index": "data", "field": { "uniqueName": "Order Date" } }
        let promise;

        before(function () {
            promise = axios.post(url, requestBody);
        });

        it('response type check', function (done) {
            promise.then(function (response) {

                typeChecking(response, "number");
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });


        it('should return all members', function (done) {
            promise.then(function (response) {
                expect(response.data.members).to.be.lengthOf(623);
                expect(response.data.members).to.deep.include({ "value": 1526947200000 });
                expect(response.data.members).to.deep.include({ "value": 1560902400000 });
                expect(response.data.members).to.deep.include({ "value": 1565568000000 });
                if (response.data.page !== undefined) {
                    expect(response.data.page).to.be.equal(0);
                }
                if (response.data.pageTotal !== undefined) {
                    expect(response.data.pageTotal).to.be.equal(1);
                }
                done();
            })
                .catch(function (error) {
                    done(error);
                });
        });
    });
});