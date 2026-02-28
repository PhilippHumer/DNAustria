using DNAustria.Logic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Common;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


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
}