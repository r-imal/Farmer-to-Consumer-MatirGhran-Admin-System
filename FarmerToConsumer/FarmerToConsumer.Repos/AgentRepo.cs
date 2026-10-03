using FarmerToConsumer.Data;
using FarmerToConsumer.Entities;
using FarmerToConsumer.Shared;

namespace FarmerToConsumer.Repos
{
    public class AgentRepo(FtcDbContext context)
    {
        public Result<List<Agent>> GetAll()
        {
            var result = new Result<List<Agent>>()
            {
                Data = new List<Agent>()
            };

            try
            {
                result.Data = context.Agents.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Agent> GetById(int id)
        {
            var result = new Result<Agent>();

            try
            {
                result.Data = context.Agents.Find(id);

                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Agent Id.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Agent> GetByUserInfoId(int userInfoId)
        {
            var result = new Result<Agent>();

            try
            {
                result.Data = context.Agents.FirstOrDefault(e => e.UserInfoID == userInfoId);

                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Agent not found for this user.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Agent> Save(Agent model)
        {
            var result = new Result<Agent>();

            try
            {
                var objToSave = context.Agents.Find(model.ID);

                if (objToSave == null)
                {
                    objToSave = new Agent();
                    context.Agents.Add(objToSave);
                }

                objToSave.UserInfoID = model.UserInfoID;
                objToSave.Name = model.Name;
                objToSave.Phone = model.Phone;
                objToSave.Area = model.Area;
                objToSave.CommissionRate = model.CommissionRate;

                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Agent> UpdateNameArea(Agent model)
        {
            var result = new Result<Agent>();

            try
            {
                var objToUpdate = context.Agents.Find(model.ID);

                if (objToUpdate == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Agent Id.";
                    return result;
                }

                objToUpdate.Name = model.Name;
                objToUpdate.Area = model.Area;

                context.SaveChanges();

                result.Data = objToUpdate;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<bool> Delete(int id)
        {
            var result = new Result<bool>();

            try
            {
                var objToDelete = context.Agents.Find(id);

                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Agent Id.";
                    return result;
                }

                context.Agents.Remove(objToDelete);
                context.SaveChanges();

                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }
    }
}