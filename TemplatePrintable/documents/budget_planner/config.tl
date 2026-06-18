return {
    template = "planner",
    header = "Budget Planner",
    style = "budget_planner_style",
    columns = {
        { -- Column 1
            { title = "Income", max_rows = 5, items = {} },
            { title = "Bills", max_rows = 16, items = {} },
            { title = "Savings", max_rows = 6, items = {} }
        },
        { -- Column 2
            { title = "Expenses", max_rows = 15, items = {} },
            { title = "Debt", max_rows = 6, items = {} },
            { title = "Summary", max_rows = 6, is_summary = true, items = {"Income", "Bills", "Savings", "Expenses", "Debt"} }
        }
    }
}
