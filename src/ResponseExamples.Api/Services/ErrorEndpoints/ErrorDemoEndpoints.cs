using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ResponseExamples.Api.Services.ErrorEndpoints;

public static class ErrorDemoEndpoints
{
    public static IEndpointRouteBuilder MapErrorDemoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/errors")
            .WithTags("Error Examples")
            .WithOpenApi();

        // 400 Bad Request with Validation Problem Details
        group.MapPost("/validation-error", ValidationError)
            .WithName("ValidationError")
            .WithSummary("Demonstrates a 400 Bad Request with validation errors")
            .WithDescription("Returns a ValidationProblemDetails response showing multiple validation failures")
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 400 Bad Request with validation errors";
                operation.Description = "Returns a ValidationProblemDetails response showing multiple validation failures. " +
                                      "Pass invalid data to trigger validation errors.";
                return operation;
            });

        // 400 Bad Request with Problem Details (non-validation)
        group.MapPost("/bad-request", BadRequestError)
            .WithName("BadRequestError")
            .WithSummary("Demonstrates a 400 Bad Request with ProblemDetails")
            .WithDescription("Returns a ProblemDetails response for a malformed request that isn't a validation error")
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 400 Bad Request with ProblemDetails";
                operation.Description = "Returns a ProblemDetails response for a malformed request that isn't a validation error";
                return operation;
            });

        // 401 Unauthorized
        group.MapGet("/unauthorized", UnauthorizedError)
            .WithName("UnauthorizedError")
            .WithSummary("Demonstrates a 401 Unauthorized error")
            .WithDescription("Returns a ProblemDetails response indicating authentication is required")
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 401 Unauthorized error";
                operation.Description = "Returns a ProblemDetails response indicating authentication is required";
                return operation;
            });

        // 403 Forbidden
        group.MapGet("/forbidden", ForbiddenError)
            .WithName("ForbiddenError")
            .WithSummary("Demonstrates a 403 Forbidden error")
            .WithDescription("Returns a ProblemDetails response indicating the user lacks permission")
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 403 Forbidden error";
                operation.Description = "Returns a ProblemDetails response indicating the user lacks permission to access this resource";
                return operation;
            });

        // 404 Not Found
        group.MapGet("/not-found/{id:int}", NotFoundError)
            .WithName("NotFoundError")
            .WithSummary("Demonstrates a 404 Not Found error")
            .WithDescription("Returns a ProblemDetails response when a resource is not found")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 404 Not Found error";
                operation.Description = "Returns a ProblemDetails response when a resource is not found";
                operation.Parameters[0].Description = "Any ID value will trigger a 404 response";
                return operation;
            });

        // 409 Conflict
        group.MapPost("/conflict", ConflictError)
            .WithName("ConflictError")
            .WithSummary("Demonstrates a 409 Conflict error")
            .WithDescription("Returns a ProblemDetails response when there's a conflict with the current state")
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 409 Conflict error";
                operation.Description = "Returns a ProblemDetails response when there's a conflict with the current state, " +
                                      "such as attempting to create a duplicate resource";
                return operation;
            });

        // 422 Unprocessable Entity
        group.MapPost("/unprocessable", UnprocessableEntityError)
            .WithName("UnprocessableEntityError")
            .WithSummary("Demonstrates a 422 Unprocessable Entity error")
            .WithDescription("Returns a ProblemDetails response for semantically incorrect data")
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 422 Unprocessable Entity error";
                operation.Description = "Returns a ProblemDetails response when the request is well-formed but semantically incorrect";
                return operation;
            });

        // 429 Too Many Requests
        group.MapGet("/rate-limit", RateLimitError)
            .WithName("RateLimitError")
            .WithSummary("Demonstrates a 429 Too Many Requests error")
            .WithDescription("Returns a ProblemDetails response when rate limit is exceeded")
            .Produces<ProblemDetails>(StatusCodes.Status429TooManyRequests)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 429 Too Many Requests error";
                operation.Description = "Returns a ProblemDetails response when rate limit is exceeded";
                return operation;
            });

        // 500 Internal Server Error (from thrown exception)
        group.MapGet("/server-error", ServerError)
            .WithName("ServerError")
            .WithSummary("Demonstrates a 500 Internal Server Error from an exception")
            .WithDescription("Throws an exception that gets caught by the ProblemDetails middleware, returning a 500 error")
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 500 Internal Server Error from an exception";
                operation.Description = "Throws an exception that gets caught by the ProblemDetails middleware and converted to a standardized error response";
                return operation;
            });

        // 503 Service Unavailable
        group.MapGet("/service-unavailable", ServiceUnavailableError)
            .WithName("ServiceUnavailableError")
            .WithSummary("Demonstrates a 503 Service Unavailable error")
            .WithDescription("Returns a ProblemDetails response when a service dependency is unavailable")
            .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Demonstrates a 503 Service Unavailable error";
                operation.Description = "Returns a ProblemDetails response when a service dependency is unavailable or the service is temporarily down";
                return operation;
            });

        return app;
    }

    /// <summary>
    /// Demonstrates a 400 Bad Request with ValidationProblemDetails
    /// </summary>
    private static BadRequest<ValidationProblemDetails> ValidationError(ValidationRequest? request)
    {
        // Collect validation errors
        var errors = new Dictionary<string, string[]>();

        if (request is null)
        {
            errors["request"] = new[] { "Request body is required." };
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                errors["Name"] = new[] { "The Name field is required." };
            }
            else if (request.Name.Length < 3)
            {
                errors["Name"] = new[] { "The Name field must be at least 3 characters long." };
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors["Email"] = new[] { "The Email field is required." };
            }
            else if (!request.Email.Contains("@"))
            {
                errors["Email"] = new[] { "The Email field must be a valid email address." };
            }

            if (request.Age < 0 || request.Age > 120)
            {
                errors["Age"] = new[] { "The Age field must be between 0 and 120." };
            }
        }

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };

        return TypedResults.BadRequest(problemDetails);
    }

    /// <summary>
    /// Demonstrates a 400 Bad Request with ProblemDetails (non-validation error)
    /// </summary>
    private static BadRequest<ProblemDetails> BadRequestError()
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Bad Request",
            Detail = "The request contains malformed JSON or unsupported content type. " +
                    "This is a general bad request error, not a validation error.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };

        return TypedResults.BadRequest(problemDetails);
    }

    /// <summary>
    /// Demonstrates a 401 Unauthorized error
    /// </summary>
    private static UnauthorizedHttpResult UnauthorizedError()
    {
        // Note: TypedResults.Unauthorized() doesn't accept ProblemDetails directly
        // The ProblemDetails middleware will handle converting it
        return TypedResults.Unauthorized();
    }

    /// <summary>
    /// Demonstrates a 403 Forbidden error
    /// </summary>
    private static Results<ForbidHttpResult, ProblemHttpResult> ForbiddenError()
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Forbidden",
            Detail = "You do not have permission to access this resource. " +
                    "This operation requires administrator privileges.",
            Status = StatusCodes.Status403Forbidden,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
        };

        return TypedResults.Problem(problemDetails);
    }

    /// <summary>
    /// Demonstrates a 404 Not Found error
    /// </summary>
    private static NotFound<ProblemDetails> NotFoundError(int id)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Resource not found",
            Detail = $"The requested resource with ID {id} was not found in the system.",
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
        };

        return TypedResults.NotFound(problemDetails);
    }

    /// <summary>
    /// Demonstrates a 409 Conflict error
    /// </summary>
    private static Conflict<ProblemDetails> ConflictError(ConflictRequest? request)
    {
        var identifier = request?.Identifier ?? "unknown";
        
        var problemDetails = new ProblemDetails
        {
            Title = "Conflict",
            Detail = $"A resource with identifier '{identifier}' already exists. " +
                    "Please use a different identifier or update the existing resource.",
            Status = StatusCodes.Status409Conflict,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10"
        };

        return TypedResults.Conflict(problemDetails);
    }

    /// <summary>
    /// Demonstrates a 422 Unprocessable Entity error
    /// </summary>
    private static UnprocessableEntity<ProblemDetails> UnprocessableEntityError(UnprocessableRequest? request)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Unprocessable Entity",
            Detail = "The request was well-formed but contains semantic errors. " +
                    "For example, the start date cannot be after the end date.",
            Status = StatusCodes.Status422UnprocessableEntity,
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
        };

        return TypedResults.UnprocessableEntity(problemDetails);
    }

    /// <summary>
    /// Demonstrates a 429 Too Many Requests error
    /// </summary>
    private static ProblemHttpResult RateLimitError()
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Too Many Requests",
            Detail = "Rate limit exceeded. You have made too many requests in a short period. " +
                    "Please wait before making additional requests.",
            Status = StatusCodes.Status429TooManyRequests,
            Type = "https://tools.ietf.org/html/rfc6585#section-4"
        };

        return TypedResults.Problem(problemDetails);
    }

    /// <summary>
    /// Demonstrates a 500 Internal Server Error by throwing an exception
    /// </summary>
    private static Ok<string> ServerError()
    {
        // This will be caught by the exception handling middleware
        // and converted to a ProblemDetails response
        throw new InvalidOperationException(
            "An unexpected error occurred while processing your request. " +
            "This exception demonstrates how the ProblemDetails middleware handles unhandled exceptions.");
    }

    /// <summary>
    /// Demonstrates a 503 Service Unavailable error
    /// </summary>
    private static ProblemHttpResult ServiceUnavailableError()
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Service Unavailable",
            Detail = "The service is temporarily unavailable due to maintenance or high load. " +
                    "Please try again later.",
            Status = StatusCodes.Status503ServiceUnavailable,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.4"
        };

        return TypedResults.Problem(problemDetails);
    }
}

/// <summary>
/// Request model for validation error demonstration
/// </summary>
public record ValidationRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public int Age { get; init; }
}

/// <summary>
/// Request model for conflict error demonstration
/// </summary>
public record ConflictRequest
{
    public string? Identifier { get; init; }
}

/// <summary>
/// Request model for unprocessable entity demonstration
/// </summary>
public record UnprocessableRequest
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
