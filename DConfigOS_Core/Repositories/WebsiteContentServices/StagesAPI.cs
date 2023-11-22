using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using DConfigOS_Core.Models.Utilities;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class StagesAPI : IStagesAPI
    {
        public virtual List<Stage> GetStages()
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var stages = context.Stages.OrderBy(m => m.Name).ToList();
            return stages;
        }

        public virtual List<string> GetStageRoles(int stageId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var stage = context.Stages.Where(x => x.Id == stageId).FirstOrDefault();
            if (stage != null)
            {
                return stage.Roles.Select(x => x.Id).ToList();
            }
            return null;
        }


        public virtual List<Stage> GetStagesbyRole(List<string> Roles)
        {
            if (Roles != null)
            {
                DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
                var stages = context.Stages.Where(x => x.Roles.Any(r => Roles.Contains(r.Id))).OrderBy(m => m.Name).ToList();
                return stages;
            }
            else
            {
                return new List<Stage>();
            }
        }

        public virtual Stage GetStage(int stageId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Id == stageId).FirstOrDefault();
            return dbStage;
        }

        public virtual List<Stage> GetNextStages(int stageId)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Id == stageId).FirstOrDefault();
            if (dbStage != null)
            {
                return dbStage.NextStages.ToList();
            }
            else
            {
                return null;
            }
        }


        public virtual List<Stage> GetNextStages(List<int> stagesIds)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            Dictionary<int, Stage> stages = new Dictionary<int, Stage>();
            foreach(int id in stagesIds)
            {
                var dbStage = context.Stages.Where(m => m.Id == id).FirstOrDefault();
                if (dbStage != null)
                {
                    foreach(Stage s in dbStage.NextStages)
                    {
                        if(!stages.ContainsKey(s.Id))
                        {
                            stages.Add(s.Id,s);
                        }
                    }
                }
            }
            return stages.Values.ToList();
        }

        public virtual int UpdateNextStages(int stageId, List<int> nextStagesIds, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Id == stageId).FirstOrDefault();
            if (dbStage != null)
            {
                dbStage.NextStages.Clear();
                if (nextStagesIds != null && nextStagesIds.Count > 0)
                {
                    var nextStages = context.Stages.Where(m => nextStagesIds.Contains(m.Id));
                    if (nextStages != null)
                    {
                        foreach (var stage in nextStages)
                        {
                            dbStage.NextStages.Add(stage);
                        }
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int AddStageRoles(int stageId, string roleId, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Id == stageId).FirstOrDefault();
            var dbRole = context.Roles.Where(m => m.Id == roleId).FirstOrDefault();
            if (dbStage != null && dbRole != null)
            {
                var existRole = dbStage.Roles.Where(x => x.Id == dbRole.Id).FirstOrDefault();
                if (existRole == null)
                {
                    dbStage.Roles.Add(dbRole);
                    context.SaveChanges();
                }

                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteStageRoles(int stageId, string roleName, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Id == stageId).FirstOrDefault();
            var dbRole = context.Roles.Where(m => m.Name == roleName).FirstOrDefault();
            if (dbStage != null && dbRole != null)
            {
                var existRole = dbStage.Roles.Where(x => x.Id == dbRole.Id).FirstOrDefault();
                if (existRole != null)
                {
                    dbStage.Roles.Remove(existRole);
                    context.SaveChanges();
                }

                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int CreateStage(Stage stage)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Name == stage.Name).FirstOrDefault();
            if (dbStage == null)
            {
                context.Stages.Add(stage);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateStage(Stage stage)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Id == stage.Id).FirstOrDefault();

            if (dbStage != null)
            {
                dbStage.Name = stage.Name;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteStage(int id)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStage = context.Stages.Where(m => m.Id == id).FirstOrDefault();
            if (dbStage != null)
            {

                dbStage.NextStages.Clear();
                context.Stages.Remove(dbStage);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }
    }
}
