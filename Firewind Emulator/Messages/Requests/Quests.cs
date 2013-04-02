
using System;
namespace Firewind.Messages
{
    partial class GameClientMessageHandler
    {
        public void OpenQuests()
        {
            FirewindEnvironment.GetGame().GetQuestManager().GetList(Session, Request);
        }

        public void StartQuest()
        {
            FirewindEnvironment.GetGame().GetQuestManager().ActivateQuest(Session, Request);
        }

        public void StopQuest()
        {
            FirewindEnvironment.GetGame().GetQuestManager().CancelQuest(Session, Request);
        }

        public void GetCurrentQuest()
        {
            FirewindEnvironment.GetGame().GetQuestManager().GetCurrentQuest(Session, Request);
        }

        //public void RegisterQuests()
        //{
        //    RequestHandlers.Add(3101, new RequestHandler(OpenQuests));
        //    RequestHandlers.Add(3102, new RequestHandler(StartQuest));
        //    RequestHandlers.Add(3106, new RequestHandler(StopQuest));
        //    RequestHandlers.Add(3107, new RequestHandler(GetCurrentQuest));
        //}
    }
}
