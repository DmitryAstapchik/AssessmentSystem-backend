var chakram = require('chakram'),
	expect = chakram.expect;

var urlBase = "http://localhost:1960/api/";

describe("Scenario: get all items from test collection.", function() {
	it("Getting an items list should return 200 status code", function() {
		var response = chakram.get(urlBase + "swaggertest/");
		expect(response).to.have.status(200);
		return chakram.wait();
	});

	it("Getting an item list should return three elements", function() {
		var response = chakram.get(urlBase + "swaggertest/");
		expect(response).to.have.json(
		[
			{
				"Name": "Sally",
				"Value": 354
			},
			{
				"Name": "Molly",
				"Value": 985
			},
			{
				"Name": "Dolly",
				"Value": 642
			}
		]);
		return chakram.wait();
	});
});

describe("Scenario: add a new item to test collection.", function() {
	it("Adding new item should return 200 status code and added item", function() {
		var response = chakram.post(urlBase + "swaggertest?name=one&value=111");
		expect(response).to.have.status(200);
		expect(response).to.have.json(
		{
			"Name": "one",
			"Value": 111
		});
		return chakram.wait();
	});
});

describe("Scenario: remove an existed item from the test collection.", function() {
	it ("Deleting an item that is not added to test collection should return 204 status ", function() {
		var response = chakram.delete(urlBase + "swaggertest?name=one");
		expect(response).to.have.status(404);
		return chakram.wait();
	});

	it ("Deleting an existed item should return 200 and deleted item.", function() {
		var response = chakram.delete(urlBase + "swaggertest?name=Sally");
		expect(response).to.have.status(200);
		expect(response).to.have.json(
		{
			"Name": "Sally",
			"Value": 354
		});
		return chakram.wait();
	});
});