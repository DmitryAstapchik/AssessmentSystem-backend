var chakram = require('chakram'),
	expect = chakram.expect;

var urlBase = "http://localhost:1960/api/v1/accounts";

describe("Scenario: send data with wrong validation fields.", function() {
    it("Request should return 400 status code, due to short first name", function () {
        var response = chakram.post(urlBase, {
            "FirstName": "d",
            "LastName": "Band",
            "Email": "danmail@mail.ru",
            "Password": "123456KL1",
            "ConfirmPassword": "123456KL1"
    });
        expect(response).to.have.status(400);
        expect(response).to.have.json(
            {
                "message": "The request is invalid.",
                "modelState": {
                    "model.FirstName": [
                        "Minimum length is 2."
                    ]
                }
            });
		return chakram.wait();
    });

    it("Request should return 400 status code, due to empty first name", function () {
        var response = chakram.post(urlBase, {
            "LastName": "Band",
            "Email": "danmail@mail.ru",
            "Password": "123456KL1",
            "ConfirmPassword": "123456KL1"
        });
        expect(response).to.have.status(400);
        expect(response).to.have.json(
            {
                "message": "The request is invalid.",
                "modelState": {
                    "model.FirstName": [
                        "First name is required."
                    ]
                }
            });
        return chakram.wait();
    });

    it("Request should return 400 status code, due to short first name", function () {
        var response = chakram.post(urlBase, {
            "FirstName": "Danium",
            "LastName": "B",
            "Email": "danmail@mail.ru",
            "Password": "123456KL1",
            "ConfirmPassword": "123456KL1"
        });
        expect(response).to.have.status(400);
        expect(response).to.have.json(
            {
                "message": "The request is invalid.",
                "modelState": {
                    "model.LastName": [
                        "Minimum length is 2."
                    ]
                }
            });
        return chakram.wait();
    });

    it("Request should return 400 status code, due to empty first name", function () {
        var response = chakram.post(urlBase, {
            "FirstName": "Danium",
            "Email": "danmail@mail.ru",
            "Password": "123456KL1",
            "ConfirmPassword": "123456KL1"
        });
        expect(response).to.have.status(400);
        expect(response).to.have.json(
            {
                "message": "The request is invalid.",
                "modelState": {
                    "model.LastName": [
                        "Last name is required."
                    ]
                }
            });
        return chakram.wait();
    });

    it("Request should return 400 status code, due to wrong email", function () {
        var response = chakram.post(urlBase, {
            "FirstName": "Danium",
            "LastName": "Band",
            "Email": "danmailmail.ru",
            "Password": "123456KL1",
            "ConfirmPassword": "123456KL1"
        });
        expect(response).to.have.status(400);
        expect(response).to.have.json(
            {
                "message": "The request is invalid.",
                "modelState": {
                    "model.Email": [
                        "Invalid email format."
                    ]
                }
            });
        return chakram.wait();
    });

    it("Request should return 400 status code, due to weak password", function () {
        var response = chakram.post(urlBase, {
            "FirstName": "Danium",
            "LastName": "Band",
            "Email": "danmail@mail.ru",
            "Password": "1234",
            "ConfirmPassword": "1234"
        });
        expect(response).to.have.status(400);
        expect(response).to.have.json(
            {
                "message": "The request is invalid.",
                "modelState": {
                    "model.Password": [
                        "Password must contain minimum 8 characters, at least one letter and one number."
                    ]
                }
            });
        return chakram.wait();
    });

    it("Request should return 400 status code, due to do not match confirm password", function () {
        var response = chakram.post(urlBase, {
            "FirstName": "Danium",
            "LastName": "Band",
            "Email": "danmail@mail.ru",
            "Password": "1234ADSD",
            "ConfirmPassword": "1234ADSDD"
        });
        expect(response).to.have.status(400);
        expect(response).to.have.json(
            {
                "message": "The request is invalid.",
                "modelState": {
                    "model.ConfirmPassword": [
                        "Passwords do not match"
                    ]
                }
            });
        return chakram.wait();
    });
});

describe("Scenario: getting a list of accounts.", function() {
    var url = urlBase + "account";
    it("Should return 200 status code when getting a list of accounts without parameters.", function() {
        var response = chakram.get(url);
        expect(response).to.have.status(200);
        return chakram.wait();
    });

    it("Should return 400 status code when getting a list of accounts with amount parameter less than zero.", function() {
        var response = chakram.get(url + "?amount=-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });

    it("Should return 400 status code when getting a list of accounts with start parameter less than zero.", function() {
        var response = chakram.get(url + "?start=-1");
        expect(response).to.have.status(400);
        return chakram.wait();
    });
});

describe("Scenario: getting a account by Id.", function() {
    var url = urlBase + "account";

    it("Should return 404 status code when getting an account that is not exist.", function() {
        var response = chakram.get(url + "/not_existed_account");
        expect(response).to.have.status(404);
        return chakram.wait();
    });

    it("Should return 200 status code when getting an existing account.", function() {
        return chakram.get(url)
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
                    email: {
                        type: "string"
                    },
                    role: {
                        type: "string"
                    },
                    passwordHash: {
                        type: "string"
                    },
                    isActive: {
                        type: "bool"
                    },
                    isSoftDeleted: {
                        type: "bool"
                    }
                }
            });
        });
    });
});
