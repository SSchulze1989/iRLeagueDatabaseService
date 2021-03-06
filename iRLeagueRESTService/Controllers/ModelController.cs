﻿// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
using log4net;
using System.Dynamic;
using System.Reflection;
using iRLeagueDatabase.Extensions;
using System.Net.PeerToPeer.Collaboration;
using System.Web.Http.Results;
using Newtonsoft.Json;
using iRLeagueRESTService.Models;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class ModelController : ApiController
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ModelController));

        [HttpGet]
        [ActionName("Get")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetModel(string requestId, string requestType, string leagueName)
        {
            try
            {
                logger.Info($"Get Model request || type: {requestType} - league: {leagueName} - id: [{requestId}]");
                CheckLeagueRole(User, leagueName);

                if (requestId == null || requestType == null || leagueName == null)
                {
                    return BadRequest("Parameter requestType or leagueName can not be null!");
                }

                long[] requestIdValue = GetIdFromString(requestId);

                Type requestTypeType = GetRequestType(requestType);

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                MappableDTO data;
                using (var dbContext = new LeagueDbContext(databaseName))
                using (IModelDataProvider modelDataProvider = new ModelDataProvider(dbContext))
                {
                    data = modelDataProvider.Get(requestTypeType, requestIdValue);
                }
                //GC.Collect();

                logger.Info($"Get Model request || send data: {data?.ToString()}");

                return Ok(data);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetModel", e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("GetArray")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetModels([FromUri] string[] requestIds, string requestType, string leagueName)
        {
            try
            {
                logger.Info($"Get Models request || type: {requestType} - league: {leagueName} - ids: {string.Join("/", requestIds.Select(x => $"[{string.Join(",", x)}]"))}");

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
                using (var dbContext = new LeagueDbContext(databaseName))
                using (IModelDataProvider modelDataProvider = new ModelDataProvider(dbContext))
                {
                    data = modelDataProvider.GetArray(requestTypeType, requestIdValues);
                }
                //GC.Collect();

                logger.Info($"Get Models request || send data: {string.Join("/", (object[])data)}");

                return Ok(data);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetModels", e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("Get")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetModel(string requestId, string requestType, string leagueName, string fields, bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get Model Fields request || type: {requestType} - league: {leagueName} - id: [{requestId}] - fields: {fields}");
                CheckLeagueRole(User, leagueName);

                if (requestId == null || requestType == null || leagueName == null)
                {
                    return BadRequest("Parameter requestType or leagueName can not be null!");
                }

                string[] fieldValues = new string[0];
                try
                {
                    fieldValues = fields?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?? fieldValues;
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Invalid field names", e);
                }

                long[] requestIdValue = GetIdFromString(requestId);

                Type requestTypeType = GetRequestType(requestType);

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                MappableDTO data;
                using (var dbContext = new LeagueDbContext(databaseName))
                using (IModelDataProvider modelDataProvider = new ModelDataProvider(dbContext))
                {
                    data = modelDataProvider.Get(requestTypeType, requestIdValue);
                }
                //GC.Collect();

                data.SetSerializableProperties(fieldValues, excludeFields);

                var response = SelectFieldsHelper.GetSelectedFieldObject(data);

                logger.Info($"Get Model Fields request || send data: {data} - fields: {string.Join(",", fieldValues)}");

                return Json(response);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetModel Fields", e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("GetArray")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetModelsSelectFields([FromUri] string[] requestIds, string requestType, string leagueName, string fields, bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get Models Fields request || type: {requestType} - league: {leagueName} - ids: {string.Join("/", requestIds.Select(x => $"[{string.Join(",", x)}]"))} - fields: {fields}");

                CheckLeagueRole(User, leagueName);

                if (requestType == null || leagueName == null)
                {
                    //return BadRequest("Parameter requestType or leagueName can not be null!");
                }

                string[] fieldValues = new string[0];
                try
                {
                    fieldValues = fields?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?? fieldValues;
                }
                catch(Exception e)
                {
                    throw new ArgumentException("Invalid field names", e);
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

                if (requestTypeType == null)
                {
                    throw new InvalidOperationException($"Requested type {requestType} not found");
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                MappableDTO[] data;
                using (var dbContext = new LeagueDbContext(databaseName))
                using (IModelDataProvider modelDataProvider = new ModelDataProvider(dbContext))
                {
                    data = modelDataProvider.GetArray(requestTypeType, requestIdValues);
                }
                //GC.Collect();

                data.ForEach(x => x.SetSerializableProperties(fieldValues, excludeFields));

                var response = data.Select(x => SelectFieldsHelper.GetSelectedFieldObject(x));

                logger.Info($"Get Models Fields request || send data: {string.Join("/", (object[])data)} - fields: {string.Join(",", fieldValues)}");

                return Json(response);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetModels Fields", e);
                throw;
            }
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
            try
            {
                logger.Info($"Post Models request || type: {requestType} - league: {leagueName} - data: {string.Join("/", (object[])data)}");
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

                using (var dbContext = new LeagueDbContext(databaseName))
                using (IModelDataProvider modelDataProvider = new ModelDataProvider(dbContext, User.Identity.Name, User.Identity.GetUserId()))
                {
                    data = modelDataProvider.PostArray(requestTypeType, data);
                    if (dbContext.DbChanged)
                    {
                        UpdateLeague(leagueName, User);
                    }
                }
                //GC.Collect();

                logger.Info($"Post Models request || send data: {string.Join("/", (object[])data)}");
                return Ok(data);
            }
            catch (Exception e)
            {
                logger.Error("Error in PostModels", e);
                throw;
            }
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
            try
            {
                logger.Info($"Put Models request || type: {requestType} - league: {leagueName} - data: {string.Join("/", (object[])data)}");
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
                
                using (var dbContext = new LeagueDbContext(databaseName))
                using (IModelDataProvider modelDataProvider = new ModelDataProvider(dbContext, User.Identity.Name, User.Identity.GetUserId()))
                {
                    data = modelDataProvider.PutArray(requestTypeType, data);
                    if (dbContext.DbChanged)
                    {
                        UpdateLeague(leagueName, User);
                    }
                }
                //GC.Collect();
                logger.Info($"Put Models request || send data: {string.Join("/", (object[])data)}");
                return Ok(data);
            }
            catch (Exception e)
            {
                logger.Error("Error in PutModels", e);
                throw;
            }
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
            try
            {
                logger.Info($"Delete Models request || type: {requestType} - league: {leagueName} - ids: {string.Join("/", requestIds.Select(x => $"[{x}]"))}");
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
                using (var dbContext = new LeagueDbContext(databaseName))
                using (IModelDataProvider modelDataProvider = new ModelDataProvider(dbContext))
                {
                    data = modelDataProvider.DeleteArray(requestTypeType, requestIdValues);
                    if (dbContext.DbChanged)
                    {
                        UpdateLeague(leagueName, User);
                    }
                }
                //GC.Collect();
                logger.Info($"Delete Models request || send answer: {data}");
                return Ok(data);
            }
            catch (Exception e)
            {
                logger.Error("Error in PostModels", e);
                throw;
            }
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
                "iRLeagueDatabase.DataTransfer.Filters.",
                "iRLeagueDatabase.DataTransfer.Statistics."
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

        private void UpdateLeague(string leagueName, IPrincipal principal)
        {
            var register = LeagueRegister.Get();

            LeagueEntry leagueEntry = null;
            if (register.Leagues.Any(x => x.Name == leagueName) == false)
            {
                leagueEntry = new LeagueEntry()
                {
                    Name = leagueName,
                    CreatorName = principal.Identity.Name,
                    CreatorId = Guid.Parse(principal.Identity.GetUserId()),
                    CreatedOn = DateTime.Now,
                    PrettyName = leagueName
                };
                register.Leagues.Add(leagueEntry);
            }
            else
            {
                leagueEntry = register.Leagues.SingleOrDefault(x => x.Name == leagueName);
            }
            leagueEntry.LastUpdate = DateTime.Now;

            register.Save();
        }
    }
}