using HotelListing.API.Exceptions;
using HotelListing.API.Models.Users;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Net;

namespace HotelListing.API.Middleware
{
    public class ExceptionMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong while processing {context.Request.Path}");
                await HandleExceptionAsync(context, ex);
            }
        
        
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statuscode = HttpStatusCode.InternalServerError;

            var errorDetails = new ErrorDetails
            {
                ErrorType = "Failure",
                ErrorMessage = ex.Message,


            };

            switch (ex)
            {
                case NotFoundException notFoundException:
                    statuscode = HttpStatusCode.NotFound;
                    errorDetails.ErrorType = "Not Found";
                    break;

                default:
                    break;
            
            }

            string response = JsonConvert.SerializeObject(errorDetails);
            context.Response.StatusCode = (int)statuscode;

            return context.Response.WriteAsync(response);   
        }
    }

    public class ErrorDetails
    { 
     public string ErrorType { get; set; }
     public string ErrorMessage { get; set; }
    
    
    }
}
