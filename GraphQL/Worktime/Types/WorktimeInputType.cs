﻿
using GraphQL.Types;

namespace TimeTracker.GraphQL.Worktime.Types;

public class WorktimeInputType : InputObjectGraphType<WorktimeInput>
{
    public WorktimeInputType()
    {
        Name = "WorkInput";

        Field<GuidGraphType>("Id");
        Field<NonNullGraphType<StringGraphType>>("UserId");
        Field<NonNullGraphType<StringGraphType>>("StartDate");
        Field<StringGraphType>("FinishDate");
        Field<NonNullGraphType<StringGraphType>>("LastEditorId");
    }
}

public class WorktimeInput
{
    public Guid Id { get; set; }

    public string UserId { get; set; }

    public string StartDate { get; set; } = null!;

    public string? FinishDate { get; set; } = null!;

    public string LastEditorId { get; set; }
}
