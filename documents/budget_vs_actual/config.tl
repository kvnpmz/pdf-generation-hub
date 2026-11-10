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
    formula_note = "Difference = budget − actual   ·   % used = actual ÷ budget   ·   a positive difference means you came in under budget",
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
    takeaways_note = "use the biggest gaps above",
    takeaways_hint = "These three lines become next month's starting point.",
    takeaways = {
        { label = "Biggest overspend", fields = { "category", "over by" } },
        { label = "Next month's fix", fields = { "what", "how much" } },
        { label = "Leftover to roll over", fields = { "move to", "amount" } }
    }
}
