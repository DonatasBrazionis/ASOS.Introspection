# ResourceServer

## Get started

```
$  dotnet restore
$  dotnet build
$  dotnet run
```

URL: http://127.0.0.1:6000

## Check authorized endpoint

### Request

URL: `/api/values/get`  
Method: `GET`  
Header: `Authorization:Bearer CfDJ8DQjgHVp7AZJu0zT5ANdbWJZ...`

### Response

Body:

```
{
    "userId": "6cbdccce-f97f-4306-a12d-5358adaf909a",
    "email": "admin@mail.com"
}
```
