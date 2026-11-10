return {
    id = "budget_vs_actual",
    template = "budget_vs_actual",
    output_name = "budget_vs_actual",
    header = "Budget vs Actual",
    subtitle = "Compare planned and actual spending",
    completed_date = "2026-11-10",
    header_fields = { "Month", "Income received", "Total budgeted", "Total spent" },
    table_title = "Line-by-line review",
    subtotal_label = "subtotal",
    total_label = "Month total",
    formula_note = "Difference = budget − actual   ·   % used = actual ÷ budget   ·   plus = under budget   ·   minus = over",
    columns = { "Category", "Budget", "Actual", "Difference", "% Used", "Roll over" },
    groups = {
        {
            name = "Needs",
            note = "must pay",
            items = {
                "Rent or mortgage",
                "Utilities",
                "Groceries",
                "Insurance",
                "Transportation",
                "Minimum debt payments",
                "Childcare",
                "Medical"
            }
        },
        {
            name = "Wants",
            note = "flexible",
            items = {
                "Dining out",
                "Subscriptions",
                "Entertainment",
                "Clothing",
                "Personal care",
                "Hobbies",
                "Gifts and giving"
            }
        },
        {
            name = "Savings and debt",
            note = "pay yourself first",
            items = {
                "Emergency fund",
                "Retirement",
                "Investments",
                "Extra debt payment"
            }
        }
    },
    takeaways_title = "Month-end takeaways",
    takeaways_note = "copy the three biggest differences from the table",
    takeaways_hint = "Start next month's budget by changing only these three numbers.",
    takeaways = { "Biggest overspend", "Next month's fix", "Leftover to roll over" }
}
