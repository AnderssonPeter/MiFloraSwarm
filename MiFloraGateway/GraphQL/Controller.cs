using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EntityGraphQL;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;

namespace MiFloraGateway.GraphQL
{
    [Route("GraphQL")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly DatabaseContext dbContext;
        private readonly ILogger<Controller> logger;
        private readonly SchemaProvider<DatabaseContext> schemaProvider;

        public Controller(DatabaseContext dbContext, ILogger<Controller> logger, SchemaProvider<DatabaseContext> schemaProvider)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.schemaProvider = schemaProvider;
        }

        [HttpPost]
        [Authorize]
        public async Task<object> Post([FromBody]QueryRequest query)
        {
            try
            {
                var results = await schemaProvider.ExecuteQueryAsync(query, dbContext, HttpContext.RequestServices, User.Identities.FirstOrDefault());
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
