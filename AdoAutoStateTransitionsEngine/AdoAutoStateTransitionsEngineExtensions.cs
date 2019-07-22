﻿using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AdoAutoStateTransitionsEngine
{
    public enum WorkItemState
    {
        Unknown,
        New,
        Active,
        Resolved,
        Closed,
        Removed,
    }

    public static class AdoAutoStateTransitionsEngineExtensions
    {
        public static string GetState(this WorkItem workItem)
        {
            return (workItem?.Fields["System.State"] as string);
        }

        public static bool IsStateNew(this WorkItem workItem)
        {
            return GetState(workItem) == WorkItemState.New.ToString();
        }

        public static bool IsWorkItemUpdate(this AdoWebHookMessage message)
        {
            return message?.eventType == "workitem.updated";
        }

        public static bool IsStateChange(this AdoWebHookMessage message)
        {
            return
                message.IsWorkItemUpdate() &&
                message?.resource?.fields?.SystemState != null &&
                message?.resource?.fields?.SystemState.oldValue != message?.resource?.fields?.SystemState.newValue;
        }

        public static bool IsChangeToActive(this AdoWebHookMessage message)
        {
            return
                message.IsStateChange() &&
                message?.resource?.fields?.SystemState.newValue == WorkItemState.Active.ToString();
        }

        public static bool IsChangeToClosed(this AdoWebHookMessage message)
        {
            return
                message.IsStateChange() &&
                (message?.resource?.fields?.SystemState.newValue == WorkItemState.Closed.ToString() ||
                 message?.resource?.fields?.SystemState.newValue == WorkItemState.Removed.ToString());
        }

        public static int WorkItemId(this AdoWebHookMessage message)
        {
            return (message?.resource?.workItemId).GetValueOrDefault();
        }
    }
}
