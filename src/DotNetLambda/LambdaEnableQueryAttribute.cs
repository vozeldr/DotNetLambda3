using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetLambda
{
    /// <summary>
    /// Fixes an issue with queries to controller actions using <see cref="EnableQueryAttribute"/> not working in
    /// unit tests or when deployed in Lambda mode.
    /// </summary>
    public class LambdaEnableQueryAttribute : EnableQueryAttribute
    {
        /// <summary>
        /// Performs the query composition after action is executed. It first tries to retrieve the IQueryable from the
        /// returning response message. It then validates the query from uri based on the validation settings on
        /// <see cref="T:Microsoft.AspNet.OData.EnableQueryAttribute" />. It finally applies the query appropriately, and reset it back on
        /// the response message.
        /// </summary>
        /// <remarks>This override forces a status code of 200 to be set on the response.</remarks>
        /// <param name="actionExecutedContext">The context related to this action, including the response message,
        /// request message and HttpConfiguration etc.</param>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.HttpContext.Response.StatusCode = 200;
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}