# IdentityServer

## Get started

```
$  dotnet restore
$  dotnet build
$  dotnet run
```

URL: http://127.0.0.1:5000

## Login

### Request

URL: `/connect/token`  
Method: `POST`  
Header: `Content-Type:application/x-www-form-urlencoded`  
Body:

```
grant_type:password
username:admin@mail.com
password:admin
```

### Response

Body:

```
{
    "scope": "openid offline_access",
    "token_type": "Bearer",
    "access_token": "CfDJ8DQjgHVp7AZJu0zT...",
    "expires_in": 28800,
    "refresh_token": "CfDJ8DQjgHVp7AZJu0zT5...",
    "id_token": "e30.eyJzdWIiOiI4MWEy..."
}
```

## Refresh token

### Request

URL: `/connect/token`  
Method: `POST`  
Header: `Content-Type:application/x-www-form-urlencoded`  
Body:

```
grant_type:refresh_token
refresh_token:CfDJ8DQjgHVp7AZJu...
```

### Response

Body:

```
{
    "scope": "openid offline_access",
    "token_type": "Bearer",
    "access_token": "CfDJ8DQjgHVp7AZJu0zT5...",
    "expires_in": 28800,
    "refresh_token": "CfDJ8DQjgHVp7AZJu0zT5AN...",
    "id_token": "e30.eyJzdWIiOiI2Y2JkY2..."
}
```

## Logout

### Request

URL: `/connect/logout`  
Method: `POST`  
Header: `Content-Type:application/x-www-form-urlencoded`  
Body:

```
grant_type:refresh_token
refresh_token:CfDJ8DQjgHVp7AZJu...
```

### Response

Status: `200`

## Check introspection

### Request

URL: `/connect/introspect`  
Method: `POST`  
Header: `Content-Type:application/x-www-form-urlencoded`  
Body:

```
token:CfDJ8DQjgHVp7AZJu0zT5ANdb...
client_id:5035f951-f7bb-459d-b196-bb212292bb4d
client_secret:89e43125-d963-4694-b770-096795a6e1e1
```

### Response

Body:

```
{
    "active": true,
    "iss": "http://127.0.0.1:5000/",
    "username": "6cbdccce-f97f-4306-a12d-5358adaf909a",
    "sub": "6cbdccce-f97f-4306-a12d-5358adaf909a",
    "scope": "openid offline_access",
    "jti": "96686ff8-d8eb-4bce-b4aa-ec968c011741",
    "token_type": "Bearer",
    "token_usage": "access_token",
    "iat": 1561291144,
    "nbf": 1561291144,
    "exp": 1561319944,
    "name": "6cbdccce-f97f-4306-a12d-5358adaf909a",
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "6cbdccce-f97f-4306-a12d-5358adaf909a",
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": "admin@mail.com"
}
```

## Check authorized endpoint

### Request

URL: `/api/values/get`  
Method: `GET`  
Header: `Authorization:Bearer CfDJ8DQjgHVp7AZJu0zT5ANdbWJZ...`

### Response

Body:

```
{
    "userId": "6cbdccce-f97f-4306-a12d-5358adaf909a"
}
```
