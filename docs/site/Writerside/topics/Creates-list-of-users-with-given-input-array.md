# Create list of users with given input array

<!--If an operation has several responses, you can add samples for each of them separately.-->

<api-endpoint openapi-path="./../openapi.yaml" endpoint="/user/createWithList" method="post">
    <response type="200">
        <sample>
        {
          "id": 10,
          "username": "theUser",
          "firstName": "John",
          "lastName": "James",
          "email": "john@email.com",
          "password": "12345",
          "phone": "12345",
          "userStatus": 1
        }
        </sample>
    </response>
<response type="400">
    <sample>
        {
          "code": 0,
          "details": [
            {
              "typeUrl": "string",
              "value": "string"
            }
          ],
          "message": "string"
        }
    </sample>
</response>
</api-endpoint>
