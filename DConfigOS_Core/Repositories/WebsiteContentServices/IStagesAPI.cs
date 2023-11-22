using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IStagesAPI
    {
        List<Stage> GetStages();
        List<string> GetStageRoles(int stageId);
        List<Stage> GetStagesbyRole(List<string> Roles);
        Stage GetStage(int stageId);
        List<Stage> GetNextStages(int stageId);
        List<Stage> GetNextStages(List<int> stagesIds);
        int UpdateNextStages(int stageId, List<int> nextStagesIds, string creatorId = null);
        int AddStageRoles(int stageId, string roleId, string creatorId = null);
        int DeleteStageRoles(int stageId, string roleName, string creatorId = null);
        int CreateStage(Stage stage);
        int UpdateStage(Stage stage);
        int DeleteStage(int id);

    }
}
