using System;
using System.Collections.Generic;
using NLua;

public class HalfHourWeeklySchedule : IRenderer
{
    public RenderResult Render(LuaTable config)
    {
        var htmlParts = new List<string>();

        string headerHtml = Dom.Header(
                "",
                Dom.H1("title", "Half Hour Weekly Schedule"),
                Dom.Div("week-label",
                    "Week Of: ",
                    Dom.Div("week-line")
                    )
                );

        htmlParts.Add(headerHtml);

        var days = new[]
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
        };

        var gridItems = new List<string>
        {
            Dom.Div("time-header", "")
        };

        foreach (var day in days)
        {
            gridItems.Add(
                    Dom.Div("day-header", day)
                    );
        }

        var start = new TimeSpan(6, 0, 0);
        var end = new TimeSpan(23, 30, 0);

        for (var time = start; time <= end; time += TimeSpan.FromMinutes(30))
        {
            int timeSlotIndex = (int)((time - start).TotalMinutes / 30);
            bool isLastTimeSlot = time == end;
            bool isThickBorder = timeSlotIndex % 2 == 0;

            string borderClass = isLastTimeSlot
                ? "time-slot-last"
                : isThickBorder
                ? "time-slot-grey-border"
                : "time-slot-black-border";

            // Add row-last class to the time slot if it's the final row
            string timeSlotRowClass = isLastTimeSlot ? "schedule-row-last" : "";

            gridItems.Add(
                    Dom.Div($"time-slot {borderClass} {timeSlotRowClass}", FormatTime(time))
                    );

            for (int i = 0; i < days.Length; i++)
            {
                string dayClass = days[i].ToLower();
                bool isLastColumn = i == days.Length - 1;

                string borderStyleClass = isThickBorder ? "border-thin" : "border-thick";

                // Build explicit classes for last column and last row
                string colClass = isLastColumn ? "schedule-col-last" : "";
                string rowClass = isLastTimeSlot ? "schedule-row-last" : "";

                string extraClass = $"schedule-cell {dayClass} {borderStyleClass} {colClass} {rowClass}";

                gridItems.Add(
                        Dom.Div(
                            extraClass,
                            Dom.Tag(
                                "textarea",
                                "schedule-input",
                                ""
                                )
                            )
                        );
            }
        }

        string mainHtml = Dom.MainTag(
                "schedule",
                gridItems.ToArray()
                );

        htmlParts.Add(mainHtml);

        return new RenderResult
        {
            Html = string.Concat(htmlParts),
            OutputName = config["output_name"]?.ToString()
                ?? config["id"]?.ToString()
                ?? "schedule"
        };
    }

    private string FormatTime(TimeSpan time)
    {
        int hour = time.Hours % 12;
        if (hour == 0) hour = 12;

        string amPm = time.Hours < 12 ? "AM" : "PM";

        return $"{hour}:{time.Minutes:D2} {amPm}";
    }
}

