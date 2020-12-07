var chakram = require('chakram'),
    expect = chakram.expect;

var urlBase = "http://localhost:1960/api/v1/invites";

describe("Scenario: getting a list of invites.", function() {
    it("Should return 200 status code when getting a list of invites without parameters.", function() {
        var response = chakram.get(urlBase);
        expect(response).to.have.status(200);
        return chakram.wait();
    });

    it("Should return 400 status code when getting a list of invites with start parameter less than zero.", function() {
        var response = chakram.get(urlBase + "?start=-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });

    it("Should return 400 status code when getting a list of invites with amount parameter less than zero.", function() {
        var response = chakram.get(urlBase + "?amount=-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });
});

describe("Scenario: adding a new invite.", function() {
    it("Should return 201 status code when adding a new invite with correct data.", function() {
        var response = chakram.post(urlBase, {
            "email": "alex@mail.ru",
            "roleName": "student"
        });
        expect(response).to.have.status(201);
        expect(response).to.have.schema({
            "type": "object",
            properties: {
                id: {
                    type: "number"
                },
                email: {
                    type: "string"
                },
                roleName: {
                    type: "string"
                }
            }
        });
        return chakram.wait();
    });

    it("Should return 400 status code when adding a new invite without email.", function() {
        var response = chakram.post(urlBase, {
            "roleName": "student"
        });
        expect(response).to.have.status(400);
        return chakram.wait();
    });

    it("Should return 400 status code when adding a new invite with roleName.", function() {
        var response = chakram.post(urlBase, {
            "email": "alex@mail.ru"
        });
        expect(response).to.have.status(400);
        return chakram.wait();
    });
});

describe("Scenario: getting a recently added invite.", function() {
    it("Should return 400 status code when getting an invite with inviteId less than zero.", function() {
        var response = chakram.get(urlBase + "/-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });

    it("Should return 404 status code when getting an invite that is not exist.", function() {
        var response = chakram.get(urlBase + "/9999999");
        expect(response).to.have.status(404);
        return chakram.wait();
    });

    it("Should return 200 status code when getting a recently added invite with correct inviteId.", function() {
        return chakram.post(urlBase, {
            "email": "alex@mail.ru",
            "roleName": "student"
        })
        .then(function (post_response) {
            return chakram.get(urlBase + "/" + post_response.body.id);
        })
        .then(function (get_reponse) {
            expect(get_reponse).to.have.status(200);
            expect(get_reponse).to.have.schema({
                "type": "object",
                properties: {
                    id: {
                        type: "number"
                    },
                    email: {
                        type: "string"
                    },
                    roleName: {
                        type: "string"
                    }
                }
            });
        });
    });
});

describe("Scenario: deleting an invite.", function() {
    it("Should return 400 status code when deleting an invite with inviteId less than zero.", function() {
        var response = chakram.delete(urlBase + "/-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });

    it("Should return 404 status code when deleting an invite that is not exist.", function() {
        var response = chakram.delete(urlBase + "/9999999");
        expect(response).to.have.status(404);
        return chakram.wait();
    });

    it("Should return 200 status code when deleting an invite with correct inviteId.", function() {
        return chakram.post(urlBase, {
            "email": "alex@mail.ru",
            "roleName": "student"
        })
        .then(function (post_response) {
            return chakram.delete(urlBase + "/" + post_response.body.id);
        })
        .then(function (delete_reponse) {
            expect(delete_reponse).to.have.status(200);
            expect(delete_reponse).to.have.schema({
                "type": "object",
                properties: {
                    id: {
                        type: "number"
                    },
                    email: {
                        type: "string"
                    },
                    roleName: {
                        type: "string"
                    }
                }
            });
        });
    });
});