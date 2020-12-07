var chakram = require('chakram'),
    expect = chakram.expect;

var urlBase = "http://localhost:1960/api/v1/exercises";

describe("Scenario: getting a list of exercises.", function() {
    it("Should return 200 status code when getting a list of exercises without parameters.", function() {
        var response = chakram.get(urlBase);
        expect(response).to.have.status(200);
        return chakram.wait();
    });
});
