using System;
using System.Linq;
using System.Net;
using EntityGraphQL;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;

namespace MiFloraGateway.Controllers
{
    [Route("api/[controller]")]
    public class QueryController : Controller
    {
        private readonly DatabaseContext dbContext;
        private readonly ILogger<QueryController> logger;
        private readonly SchemaProvider<DatabaseContext> schemaProvider;

        public QueryController(DatabaseContext dbContext, ILogger<QueryController> logger, SchemaProvider<DatabaseContext> schemaProvider)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.schemaProvider = schemaProvider;
        }

        [HttpPost]
        [Authorize]
        public object Post([FromBody]QueryRequest query)
        {
            try
            {
                var results = schemaProvider.ExecuteQuery(query, dbContext, HttpContext.RequestServices, User.Identities.FirstOrDefault());
                // gql compile errors show up in results.Errors
                if (results.Errors.Any())
                {
                    var message = "Query compilation errors occurred:" + string.Join(", ", results.Errors.Select(x => x.Message));
                    logger.LogWarning(message);
                }
                return results;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error occurred while executing query");
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
