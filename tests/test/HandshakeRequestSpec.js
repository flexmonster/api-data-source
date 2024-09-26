const axios = require('axios');
const expect = require('chai').expect;
var config = require('../config.json');

const url = config.url + "/handshake";

const requestBody = { "type": "handshake", "version": "2.9.0" }

describe('Handshake request', function () {
    let promise;
    
    before(function () {
        promise = axios.post(url, requestBody);
      });

    it('should return 200 status code', function (done) {
        promise.then(function (response) {
                expect(response.status).to.equal(200);
                done();
            })
            .catch(function(error){
                done(error);
            });
    });

    it('should return version in x.x.x format', function (done) {
       promise.then(function (response) {
                expect(response.data.version).to.match(/^\d+\.\d+\.\d+$/);
                done();
            })
            .catch(function(error){
                done(error);
            });
    });
});