# Error Response Examples API

This document demonstrates how different HTTP error codes are returned using the standardized ProblemDetails format (RFC 7807 and RFC 9457).

## Overview

All error responses in this API follow RFC standards:
- **4xx Client Errors**: Use `ProblemDetails` or `ValidationProblemDetails`
- **5xx Server Errors**: Use `ProblemDetails`

The error response format provides consistent, machine-readable error information that includes:
- `type`: URI reference identifying the problem type
- `title`: Short, human-readable summary
- `status`: HTTP status code
- `detail`: Human-readable explanation specific to this occurrence
- `errors`: (ValidationProblemDetails only) Dictionary of validation errors by field

## Error Endpoints

### 400 Bad Request - Validation Errors

**Endpoint:** `POST /api/errors/validation-error`

Returns `ValidationProblemDetails` when request data fails validation.

**Example Request:**
```json
{
  "name": "AB",
  "email": "invalid-email",
  "age": 150
}
```

**Response:** `400 Bad Request`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field must be at least 3 characters long."],
    "Email": ["The Email field must be a valid email address."],
    "Age": ["The Age field must be between 0 and 120."]
  }
}
```

### 400 Bad Request - General

**Endpoint:** `POST /api/errors/bad-request`

Returns `ProblemDetails` for malformed requests that aren't validation errors.

**Response:** `400 Bad Request`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "detail": "The request contains malformed JSON or unsupported content type. This is a general bad request error, not a validation error.",
  "status": 400
}
```

### 401 Unauthorized

**Endpoint:** `GET /api/errors/unauthorized`

Returns when authentication is required but not provided or invalid.

**Response:** `401 Unauthorized`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401
}
```

### 403 Forbidden

**Endpoint:** `GET /api/errors/forbidden`

Returns when the authenticated user lacks permission to access the resource.

**Response:** `403 Forbidden`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.4",
  "title": "Forbidden",
  "detail": "You do not have permission to access this resource. This operation requires administrator privileges.",
  "status": 403
}
```

### 404 Not Found

**Endpoint:** `GET /api/errors/not-found/{id}`

Returns when a requested resource doesn't exist.

**Response:** `404 Not Found`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Resource not found",
  "detail": "The requested resource with ID 123 was not found in the system.",
  "status": 404
}
```

### 409 Conflict

**Endpoint:** `POST /api/errors/conflict`

Returns when there's a conflict with the current state (e.g., duplicate resource).

**Example Request:**
```json
{
  "identifier": "user-123"
}
```

**Response:** `409 Conflict`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflict",
  "detail": "A resource with identifier 'user-123' already exists. Please use a different identifier or update the existing resource.",
  "status": 409
}
```

### 422 Unprocessable Entity

**Endpoint:** `POST /api/errors/unprocessable`

Returns when the request is well-formed but semantically incorrect.

**Example Request:**
```json
{
  "startDate": "2024-12-31",
  "endDate": "2024-01-01"
}
```

**Response:** `422 Unprocessable Entity`
```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Unprocessable Entity",
  "detail": "The request was well-formed but contains semantic errors. For example, the start date cannot be after the end date.",
  "status": 422
}
```

### 429 Too Many Requests

**Endpoint:** `GET /api/errors/rate-limit`

Returns when rate limiting is enforced and the limit has been exceeded.

**Response:** `429 Too Many Requests`
```json
{
  "type": "https://tools.ietf.org/html/rfc6585#section-4",
  "title": "Too Many Requests",
  "detail": "Rate limit exceeded. You have made too many requests in a short period. Please wait before making additional requests.",
  "status": 429
}
```

**Headers:**
```
Retry-After: 60
```

### 500 Internal Server Error

**Endpoint:** `GET /api/errors/server-error`

Demonstrates how unhandled exceptions are caught and converted to ProblemDetails.

**Response:** `500 Internal Server Error`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500
}
```

**Note:** In production, detailed error messages are hidden. In development mode, you may see additional details like stack traces.

### 503 Service Unavailable

**Endpoint:** `GET /api/errors/service-unavailable`

Returns when the service is temporarily unavailable (maintenance, overload, etc.).

**Response:** `503 Service Unavailable`
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.4",
  "title": "Service Unavailable",
  "detail": "The service is temporarily unavailable due to maintenance or high load. Please try again later.",
  "status": 503
}
```

**Headers:**
```
Retry-After: 120
```

## Error Response Format Comparison

### ProblemDetails (4xx and 5xx)
Used for general errors:
```json
{
  "type": "URI reference",
  "title": "Short summary",
  "status": 400,
  "detail": "Detailed explanation"
}
```

### ValidationProblemDetails (400)
Used for validation errors only:
```json
{
  "type": "URI reference",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "FieldName": ["Error message 1", "Error message 2"],
    "AnotherField": ["Error message"]
  }
}
```

## Testing the Error Endpoints

### Using curl

```bash
# Validation error
curl -X POST https://localhost:5001/api/errors/validation-error \
  -H "Content-Type: application/json" \
  -d '{"name":"AB","email":"invalid","age":150}'

# Bad request
curl -X POST https://localhost:5001/api/errors/bad-request

# Unauthorized
curl https://localhost:5001/api/errors/unauthorized

# Forbidden
curl https://localhost:5001/api/errors/forbidden

# Not found
curl https://localhost:5001/api/errors/not-found/123

# Conflict
curl -X POST https://localhost:5001/api/errors/conflict \
  -H "Content-Type: application/json" \
  -d '{"identifier":"user-123"}'

# Unprocessable entity
curl -X POST https://localhost:5001/api/errors/unprocessable \
  -H "Content-Type: application/json" \
  -d '{"startDate":"2024-12-31","endDate":"2024-01-01"}'

# Rate limit
curl https://localhost:5001/api/errors/rate-limit

# Server error
curl https://localhost:5001/api/errors/server-error

# Service unavailable
curl https://localhost:5001/api/errors/service-unavailable
```

### Using PowerShell

```powershell
# Validation error
$body = @{
    name = "AB"
    email = "invalid"
    age = 150
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/errors/validation-error" `
    -Method Post -Body $body -ContentType "application/json"

# Forbidden
Invoke-RestMethod -Uri "https://localhost:5001/api/errors/forbidden"

# Server error (will show exception details in development)
Invoke-RestMethod -Uri "https://localhost:5001/api/errors/server-error"
```

## HTTP Status Code Categories

### 4xx Client Errors
| Code | Name | When to Use |
|------|------|-------------|
| 400 | Bad Request | Malformed request or validation failures |
| 401 | Unauthorized | Authentication required or failed |
| 403 | Forbidden | User authenticated but lacks permission |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Request conflicts with current state |
| 422 | Unprocessable Entity | Syntactically correct but semantically wrong |
| 429 | Too Many Requests | Rate limit exceeded |

### 5xx Server Errors
| Code | Name | When to Use |
|------|------|-------------|
| 500 | Internal Server Error | Unexpected server error/exception |
| 503 | Service Unavailable | Service temporarily down |

## Best Practices

1. **Use ProblemDetails for all errors** - Provides consistency and follows RFC standards
2. **Distinguish between 400 and 422**:
   - 400: Malformed syntax or validation errors
   - 422: Well-formed but semantically incorrect
3. **Use ValidationProblemDetails for field-level errors** - Makes it easy for clients to map errors to form fields
4. **Include helpful detail messages** - Guide users on how to fix the problem
5. **Use standard RFC URIs in the type field** - Provides additional documentation
6. **Handle exceptions globally** - The ProblemDetails middleware catches unhandled exceptions
7. **Don't expose sensitive information in error messages** - Especially in production

## Additional Resources

- [RFC 9457 - Problem Details for HTTP APIs](https://www.rfc-editor.org/rfc/rfc9457.html)
- [RFC 7807 - Problem Details (Previous Version)](https://tools.ietf.org/html/rfc7807)
- [Microsoft Docs - Handle errors in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
