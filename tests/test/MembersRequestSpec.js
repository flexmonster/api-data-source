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

    if (config.hierarchy) {
        describe('Hierarchy member', function () {

            it('Hierarchy member - 1', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Business Type" }, "filter": [{ "field": { "uniqueName": "Category" }, "include": [{ "member": "Accessories" }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(3);
                    expect(response.data.members).to.deep.include({ "value": "Specialty Bike Shop" });
                    expect(response.data.members).to.deep.include({ "value": "Value Added Reseller" });
                    expect(response.data.members).to.deep.include({ "value": "Warehouse" });
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

            it('Hierarchy member - 2', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Color" }, "filter": [{ "field": { "uniqueName": "Category" }, "include": [{ "member": "Accessories", "filter": { "field": { "uniqueName": "Business Type" }, "include": [{ "member": "Specialty Bike Shop" }] } }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(3);
                    expect(response.data.members).to.deep.include({ "value": "red" });
                    expect(response.data.members).to.deep.include({ "value": "yellow" });
                    expect(response.data.members).to.deep.include({ "value": "white" });
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

            it('Hierarchy member(number) - 1', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Quantity" }, "filter": [{ "field": { "uniqueName": "Discount" }, "include": [{ "member": 10 }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(8);
                    expect(response.data.members).to.deep.include({ "value": 9333 });
                    expect(response.data.members).to.deep.include({ "value": 36 });
                    expect(response.data.members).to.deep.include({ "value": 1187 });
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

            it('Hierarchy member(number) - 2', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Color" }, "filter": [{ "field": { "uniqueName": "Discount" }, "include": [{ "member": 50, "filter": { "field": { "uniqueName": "Quantity" }, "include": [{ "member": 851 }] } }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(1);
                    expect(response.data.members).to.deep.include({ "value": "blue" });
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

            it('Hierarchy member(number) with empty - 3', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Color" }, "filter": [{ "field": { "uniqueName": "Discount" }, "include": [{ "member": config.emptyValue, "filter": { "field": { "uniqueName": "Quantity" }, "include": [{ "member": 121 }] } }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(2);
                    expect(response.data.members).to.deep.include({ "value": "white" });
                    expect(response.data.members).to.deep.include({ "value": "yellow" });
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

            it('Hierarchy member(number) with empty - 4', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Quantity" }, "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "yellow", "filter": { "field": { "uniqueName": "Discount" }, "include": [{ "member": config.emptyValue }] } }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(19);
                    expect(response.data.members).to.deep.include({ "value": 20 });
                    expect(response.data.members).to.deep.include({ "value": 21 });
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

            it('Hierarchy member(date) - 1', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Color" }, "filter": [{ "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1554076800000 }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(1);
                    expect(response.data.members).to.deep.include({ "value": "white" });
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

            it('Hierarchy member(date) - 2', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Quantity" }, "filter": [{ "field": { "uniqueName": "Order Date" }, "include": [{ "member": 1554076800000, "filter": { "field": { "uniqueName": "Color" }, "include": [{ "member": "white" }] } }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(1);
                    expect(response.data.members).to.deep.include({ "value": 8906 });
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

            it('Hierarchy member(date) - 3', function (done) {
                const requestBody = { "index": "data", "type": "members", "field": { "uniqueName": "Order Date" }, "filter": [{ "field": { "uniqueName": "Color" }, "include": [{ "member": "green", "filter": { "field": { "uniqueName": "Category" }, "include": [{ "member": "Cars" }] } }] }], "page": 0 }

                axios.post(url, requestBody).then(function (response) {
                    expect(response.data.members).to.be.lengthOf(30);
                    expect(response.data.members).to.deep.include({ "value": 1585008000000 });
                    expect(response.data.members).to.deep.include({ "value": 1528329600000 });
                    expect(response.data.members).to.deep.include({ "value": 1532908800000 });
                    expect(response.data.members).to.deep.include({ "value": 1522108800000 });
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
    }
});