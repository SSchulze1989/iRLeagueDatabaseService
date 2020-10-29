using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using System.Runtime.Serialization;
using Microsoft.AspNet.Identity;

using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueRESTService.Data;
using iRLeagueRESTService.Filters;
using System.Security.Principal;
using System.Net;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class ModelController : ApiController
    {
        [HttpGet]
        [ActionName("Get")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetModel(string requestId, string requestType, string leagueName)
        {
            CheckLeagueRole(User, leagueName);

            if (requestId == null || requestType == null || leagueName == null)
            {
                return BadRequest("Parameter requestType or leagueName can not be null!");
            }

            long[] requestIdValue = GetIdFromString(requestId);

            Type requestTypeType = GetRequestType(requestType);

            var databaseName = GetDatabaseNameFromLeagueName(leagueName);

            MappableDTO data;
            using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName)))
            {
                data = modelDataProvider.Get(requestTypeType, requestIdValue);
            }
            //GC.Collect();
            return Ok(data);
        }

        [HttpGet]
        [ActionName("GetArray")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetModels([FromUri] string[] requestIds, string requestType, string leagueName)
        {
            CheckLeagueRole(User, leagueName);

            if (requestType == null || leagueName == null)
            {
                return BadRequest("Parameter requestType or leagueName can not be null!");
            }

            long[][] requestIdValues;
            if (requestIds != null && requestIds.Count() > 0)
            {
                requestIdValues = requestIds.Select(x => GetIdFromString(x)).ToArray();
            }
            else
            {
                requestIdValues = null;
            }

            Type requestTypeType = GetRequestType(requestType);

            var databaseName = GetDatabaseNameFromLeagueName(leagueName);

            MappableDTO[] data;
            using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName)))
            {
                data = modelDataProvider.GetArray(requestTypeType, requestIdValues);
            }
            //GC.Collect();
            return Ok(data);
        }

        //[HttpPost]
        ////[Authorize(Roles = LeagueRoles.UserOrAdmin)]
        //[ActionName("Post")]
        //public IHttpActionResult PostModel([FromBody] MappableDTO data, string requestType, string leagueName)
        //{
        //    if (leagueName == null)
        //    {
        //        return BadRequest("Parameter leagueName can not be null");
        //    }

        //    if (data == null)
        //    {
        //        return null;
        //    }

        //    Type requestTypeType;
        //    if (requestType == null)
        //    {
        //        requestTypeType = data.GetType();
        //    }
        //    else
        //    {
        //        requestTypeType = GetRequestType(requestType);
        //    }

        //    if (requestTypeType == null)
        //    {
        //        return BadRequest("Could not identify request type");
        //    }

        //    var databaseName = GetDatabaseNameFromLeagueName(leagueName);

        //    using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName)))
        //    {
        //        data = modelDataProvider.Post(requestTypeType, data);
        //        return Ok(data);
        //    }
        //}

        [HttpPost]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        [ActionName("PostArray")]
        public IHttpActionResult PostModels([FromBody] MappableDTO[] data, string requestType, string leagueName)
        {
            CheckLeagueRole(User, leagueName);

            if (leagueName == null)
            {
                return BadRequest("Parameter leagueName can not be null");
            }

            if (data == null)
            {
                return null;
            }

            if (data.Count() == 0)
                return Ok(new MappableDTO[0]);

            Type requestTypeType;
            if (requestType == null)
            {
                requestTypeType = data.GetType();
            }
            else
            {
                requestTypeType = GetRequestType(requestType);
            }

            if (requestTypeType == null)
            {
                return BadRequest("Could not identify request type");
            }

            var databaseName = GetDatabaseNameFromLeagueName(leagueName);

            using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName), User.Identity.Name, User.Identity.GetUserId()))
            {
                data = modelDataProvider.PostArray(requestTypeType, data);
            }
            //GC.Collect();
            return Ok(data);
        }

        //[HttpPut]
        ////[Authorize(Roles = LeagueRoles.UserOrAdmin)]
        //[ActionName("Put")]
        //public IHttpActionResult PutModel([FromBody] MappableDTO data, string requestType, string leagueName)
        //{
        //    if (leagueName == null)
        //    {
        //        return BadRequest("Parameter leagueName can not be null");
        //    }

        //    if (data == null)
        //    {
        //        return null;
        //    }

        //    Type requestTypeType;
        //    if (requestType == null)
        //    {
        //        requestTypeType = data.GetType();
        //    }
        //    else
        //    {
        //        requestTypeType = GetRequestType(requestType);
        //    }

        //    if (requestTypeType == null)
        //    {
        //        return BadRequest("Could not identify request type");
        //    }

        //    var databaseName = GetDatabaseNameFromLeagueName(leagueName);

        //    using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName)))
        //    {
        //        data = modelDataProvider.Put(requestTypeType, data);
        //        return Ok(data);
        //    }
        //}

        [HttpPut]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        [ActionName("PutArray")]
        public IHttpActionResult PutModels([FromBody] MappableDTO[] data, [FromUri] string requestType, [FromUri] string leagueName)
        {
            CheckLeagueRole(User, leagueName);

            if (leagueName == null)
            {
                return BadRequest("Parameter leagueName can not be null");
            }

            if (data == null)
            {
                return Ok(data);
            }

            //return Ok();

            if (data.Count() == 0)
                return Ok(new MappableDTO[0]);

            Type requestTypeType;
            if (requestType == null)
            {
                requestTypeType = data.GetType();
            }
            else
            {
                requestTypeType = GetRequestType(requestType);
            }

            if (requestTypeType == null)
            {
                return BadRequest("Could not identify request type");
            }

            var databaseName = GetDatabaseNameFromLeagueName(leagueName);

            using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName), User.Identity.Name, User.Identity.GetUserId()))
            {
                data = modelDataProvider.PutArray(requestTypeType, data);
            }
            //GC.Collect();
            return Ok(data);
        }

        //[HttpDelete]
        ////[Authorize(Roles = LeagueRoles.UserOrAdmin)]
        //[ActionName("Delete")]
        //public IHttpActionResult DeleteModel(string requestId, string requestType, string leagueName)
        //{
        //    if (requestId == null || requestType == null || leagueName == null)
        //    {
        //        return BadRequest("Parameters can not be null!");
        //    }

        //    long[] requestIdValue = GetIdFromString(requestId);

        //    Type requestTypeType = GetRequestType(requestType);

        //    var databaseName = GetDatabaseNameFromLeagueName(leagueName);


        //    using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName)))
        //    {
        //        var data = modelDataProvider.Delete(requestTypeType, requestIdValue);
        //        return Ok(data);
        //    }
        //}

        [HttpDelete]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        [ActionName("DeleteArray")]
        public IHttpActionResult DeleteModels([FromUri] string[] requestIds, string requestType, string leagueName)
        {
            CheckLeagueRole(User, leagueName);

            if (requestIds == null || requestType == null || leagueName == null)
            {
                return BadRequest("Parameters can not be null!");
            }

            if (requestIds.Count() == 0)
            {
                return BadRequest("Request ids can not be empty");
            }

            long[][] requestIdValues;
            if (requestIds != null && requestIds.Count() > 0)
            {
                requestIdValues = requestIds.Select(x => GetIdFromString(x)).ToArray();
            }
            else
            {
                requestIdValues = null;
            }

            Type requestTypeType = GetRequestType(requestType);

            var databaseName = GetDatabaseNameFromLeagueName(leagueName);

            bool data;
            using (IModelDataProvider modelDataProvider = new ModelDataProvider(new LeagueDbContext(databaseName)))
            {
                data = modelDataProvider.DeleteArray(requestTypeType, requestIdValues);
            }
            //GC.Collect();
            return Ok(data);
        }

        private LeagueDbContext CreateDbContext(string datbaseName)
        {
            return new LeagueDbContext(datbaseName);
        }

        private string GetDatabaseNameFromLeagueName(string leagueName)
        {
            return $"{leagueName}_leagueDb";
        }

        private Type GetRequestType(string requestTypeString)
        {
            var searchNames = new string[]
            {
                "iRLeagueDatabase.DataTransfer.",
                "iRLeagueDatabase.DataTransfer.Members.",
                "iRLeagueDatabase.DataTransfer.Results.",
                "iRLeagueDatabase.DataTransfer.Reviews.",
                "iRLeagueDatabase.DataTransfer.Sessions.",
                "iRLeagueDatabase.DataTransfer.Filters"
            };

            var assemblyName = typeof(iRLeagueDatabase.DataTransfer.MappableDTO).Assembly.FullName;

            Type rqType = null;
            foreach (var name in searchNames)
            {
                rqType = Type.GetType(name + requestTypeString + "," + assemblyName);

                if (rqType != null)
                    break;
            }

            return rqType;
        }

        private long[] GetIdFromString(string idString)
        {
            if (idString == null)
                return null;

            var stringArray = idString.Split(',');
            var idList = new List<long>();

            if (stringArray.Count() == 0)
                return null;

            foreach(var idComponent in stringArray)
            {
                var result = long.TryParse(idComponent, out long idResult);
                if (result == false)
                    throw new ArgumentException("Invalid id format in \"" + idString + "\n Id must be of the format: \"#,#,#\" e.g => \"1,2,3\"");

                idList.Add(idResult);
            }
            
            return idList.ToArray();
        }

        private void CheckLeagueRole(IPrincipal principal, string leagueName)
        {
            if (principal.IsInRole($"{leagueName}_User") || principal.IsInRole("Administrator"))
                return;
            
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}