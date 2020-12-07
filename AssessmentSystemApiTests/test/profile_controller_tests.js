var chakram = require('chakram'),
	expect = chakram.expect;

var urlBase = "http://localhost:1960/api/v1/";

describe("Scenario: getting a list of profiles.", function() {
    var url = urlBase + "profiles";

    it("Should return 200 status code when getting a list of profiles without parameters.", function() {
        var response = chakram.get(url);
        expect(response).to.have.status(200);
        return chakram.wait();
    });

    it("Should return 400 status code when getting a list of profiles with amount parameter less than zero.", function() {
        var response = chakram.get(url + "?amount=-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });

    it("Should return 400 status code when getting a list of profiles with start parameter less than zero.", function() {
        var response = chakram.get(url + "?start=-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });
});

describe("Scenario: getting a profile by Id.", function() {
    var url = urlBase + "profiles";

    it("Should return 404 status code when getting an profile that is not exist.", function() {
        var response = chakram.get(url + "/not_existed_profile");
        expect(response).to.have.status(404);
        return chakram.wait();
    });

    it("Should return 200 status code when getting an existing profile.", function() {
        return chakram.get(url);
        .then(function (get_response) {
            return chakram.get(url + get_response.body.pop().id);
        })
        .then(function (get_reponse) {
            expect(get_reponse).to.have.status(200);
            expect(get_reponse).to.have.schema({
                "type": "object",
                properties: {
                    id: {
                        type: "string"
                    },
                    firstName: {
                        type: "string"
                    },
                    lastName: {
                        type: "string"
                    }
                }
            });
        });
    });
});

