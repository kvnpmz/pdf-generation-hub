return {
    id = "budget_vs_actual",
    template = "budget_vs_actual",
    output_name = "budget_vs_actual",
    header = "Budget vs Actual",
    subtitle = "Month-end review — compare what you planned with what you really spent",
    completed_date = "2026-11-10",
    header_fields = { "Month", "Income received", "Total budgeted", "Total spent" },
    table_title = "Line-by-line review",
    table_note = "fill in from your bank statement",
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
    prompts = {
        { label = "Biggest overspend", hint = "category and amount" },
        { label = "One fix for next month", hint = "change one number" },
        { label = "Leftover to roll over", hint = "move it to savings" }
    }
}
