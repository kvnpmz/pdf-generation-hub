local record Config
   header: string
   categories: {string} 
end

local contracts = require("contracts")
local dom = require("dom")
local M = {}

function M.render(config: Config): string
   local month = dom.div("header-item", "Month:", dom.div("line", ""))
   local income = dom.div("header-item", "Total Income:", dom.div("line", ""))
   local expenses = dom.div("header-item", "Income – Expenses:", dom.div("line", ""))
   local balance = dom.div("header-item", "Balance:", dom.div("line", ""))

   local html_header = dom.div("",
        dom.h1("", config.header),
        dom.header("", month, income, expenses, balance)
    )

   local rows: {string} = {}
   for _, category in ipairs(config.categories) do
       local cell_content = dom.td("bullet-base bullet-category", category)
      
      table.insert(rows, dom.tr("", 
         cell_content, 
         dom.td("", ""), 
         dom.td("", ""), 
         dom.td("", ""), 
         dom.td("", ""), 
         dom.td("bullet-base bullet-paid", "")
      ))
   end

   table.insert(rows, dom.tr("total-row", 
      dom.td("", "Total Expenses"), 
      dom.td("", ""), 
      dom.td("", ""), 
      dom.td("", ""), 
      dom.td("", ""), 
      dom.td("", "")
   ))

   local table_html = dom.table("", 
      dom.thead("", 
         dom.tr("", 
            dom.th("col-category", "Expenses Category"),
            dom.th("col-data", "Budgeted"),
            dom.th("col-data", "Actual"),
            dom.th("col-data", "Balance"),
            dom.th("col-notes", "Notes"),
            dom.th("col-paid", "Paid")
         )
      ),
      dom.tbody("", table.unpack(rows))
   )

   return html_header .. dom.main("", table_html)
end

return M as contracts.RenderStrategy
