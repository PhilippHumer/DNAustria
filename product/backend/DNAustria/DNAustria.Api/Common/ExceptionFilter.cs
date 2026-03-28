using DNAustria.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Api.Common;


/// <summary>
/// Centralized exception filter to handle exceptions and convert them into appropriate HTTP responses
/// </summary>
public sealed class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var problem = context.Exception switch
        {
            NotFoundException ex => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Detail = ex.Message
            },
            ArgumentNullException ex => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Missing required argument",
                Detail = ex.Message
            },
            ArgumentOutOfRangeException ex => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Argument value out of range",
                Detail = ex.Message
            },
            ArgumentException ex => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid request",
                Detail = ex.Message
            },
            InvalidOperationException ex => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Invalid operation",
                Detail = ex.Message
            },
            DbUpdateException dbEx when dbEx.InnerException is PostgresException pgEx =>
                MapPostgresException(pgEx),

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
                Detail = "An unexpected error occurred."
            }
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = problem.Status
        };

        context.ExceptionHandled = true;
    }

    private static ProblemDetails MapPostgresException(PostgresException pgEx)
    {
        var detail = pgEx.SqlState switch
        {
            "23514" => pgEx.ConstraintName switch // check_violation
            {
                "email_check" => "Invalid email address format.",
                _ => $"A validation constraint was violated: {pgEx.ConstraintName}."
            },
            "23505" => $"A record with this value already exists ({pgEx.ConstraintName}).", // unique_violation
            "23503" => "This record is referenced by other data and cannot be modified.", // foreign_key_violation
            _ => "A database constraint was violated."
        };

        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = detail
        };
    }
}