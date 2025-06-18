using DataLayer.Providers;
using DataLayer.Models;
using GraphQL;
using GraphQL.Types;
using TimeTracker.GraphQL.Users.Types;
using TimeTracker.GraphQL.Worktime.Types;
using BusinessLayer.Helpers;

namespace TimeTracker.GraphQL.Worktime;

public class WorktimeQuery : ObjectGraphType
{
    private readonly IWorktimeProvider worktimeProvider;
    private readonly IUserProvider userProvider;
    private readonly ICalendarProvider calendarProvider;
    private readonly IDaysOffProvider daysOffProvider;

    public WorktimeQuery(IWorktimeProvider worktimeProvider, IUserProvider userProvider, ICalendarProvider calendarProvider, IDaysOffProvider daysOffProvider, IHttpContextAccessor accessor)
    {
        this.worktimeProvider = worktimeProvider;
        this.userProvider = userProvider;
        this.calendarProvider = calendarProvider;
        this.daysOffProvider = daysOffProvider;

        Field<ListGraphType<WorktimeType>>("Records")
            .Description("Get list of worktime records")
            .Argument<SortInputType>("sorting")
            .Argument<WorktimeFilterInputType>("filter")
            .Argument<PaginationInputType>("paging")
            .Resolve(context =>
            {
                Sorting? sorting = context.GetArgument<Sorting?>("sorting");
                WorktimeFilter? filter = context.GetArgument<WorktimeFilter?>("filter");
                Paging? paging = context.GetArgument<Paging?>("paging");

                return worktimeProvider.GetWorktimeRecords(sorting, filter, paging).ToList();
            });

        Field<WorktimeType>("UnfinishedWorktimeRecord")
            .Description("Get unfinished worktime records by User Id")
            .Argument<StringGraphType>("userId", "User Id")
            .Resolve(context =>
            {
                string? userId = context.GetArgument<string?>("userId");

                if (userId == null || !Guid.TryParse(userId, out _))
                {
                    return null;
                }

                return worktimeProvider.GetUnfinishedWorktimeRecordByUserId(userId);
            });

        Field<IntGraphType>("RecordCount")
            .Description("Get record count")
            .Argument<WorktimeFilterInputType>("filter")
            .Resolve(context =>
            {
                WorktimeFilter? filter = context.GetArgument<WorktimeFilter?>("filter");
                return worktimeProvider.GetRecordCount(filter);
            });

        Field<WorktimeStatsType>("WorktimeStats")
            .Description("Get worktime statistics")
            .Argument<WorktimeFilterInputType>("filter")
            .Resolve(context =>
            {
                WorktimeFilter? filter = context.GetArgument<WorktimeFilter?>("filter");
                return GetWorktimeStats(filter);
            });
    }

    private WorktimeStats GetWorktimeStats(WorktimeFilter filter)
    {
        var worktimeRecords = worktimeProvider.GetWorktimeRecords(null, filter, null).ToList();
        var user = userProvider.GetById(filter.UserId.ToString());

        TimeSpan totalWorkTime = TimeSpan.Zero;

        foreach (var worktime in worktimeRecords)
        {
            totalWorkTime += (worktime.FinishDate - worktime.StartDate) ?? TimeSpan.Zero;
        }

        var calendarRules = calendarProvider.GetCalendarRules();
        var dayOffFilter = new DayOffRequestFilter { UserId = filter.UserId };
        var userRequests = daysOffProvider.GetRequests(dayOffFilter);
        var approvals = daysOffProvider.GetApprovals(userRequests.Select(r => r.Id).ToList());

        int WorkingDaysCount = DaysOffHelper.GetWorkingDaysCount(filter.Year, filter.Month, calendarRules, userRequests, approvals);
        decimal plannedWorktime = WorktimeHelper.GetPlannedWorktime(filter.Year, filter.Month, calendarRules, user.WorkingHoursCount, WorkingDaysCount);

        var worktimeStats = new WorktimeStats()
        {
            TotalWorkTimeMonthly = totalWorkTime.Days * 24 + totalWorkTime.Hours + (decimal)totalWorkTime.Minutes / 100,
            PlannedWorkTimeMonthly = plannedWorktime
        };

        return worktimeStats;
    }
}