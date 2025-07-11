﻿namespace DataLayer
{
    public static class Constants
    {
        public const decimal MaxWorkingHours = 8.0m;

        public enum EmploymentType
        {
            FullTime,
            PartTime,
        }

        public enum DayOffReason
        {
            Vacation,
            SickLeave,
            Absence
        }

        public enum DayOffApprovalStatus
        {
            Pending,
            Approved,
            Declined
        }

        public enum SortingOrder
        {
            Ascending,
            Descending
        }

        public enum CalendarRuleSetupType
        {
            NonWorkingDay,
            ShortDay,
            Holiday,
        }

        public enum CalendarRuleRecurringPeriod
        {
            Day,
            Week,
            Month,
            Year,
        }
    }
}