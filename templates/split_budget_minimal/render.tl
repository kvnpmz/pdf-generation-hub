local dom = require("dom")
local contracts = require("contracts")

local record Bucket
    name: string
    pct: integer
    categories: {string}
end

local record Config
    header_line: string
    subject: string
    inputs: {string}
    buckets: {Bucket}
    check_fields: {string}
    check_result: string
end

local M = {}

local function render_inputs(inputs: {string}): string
    local items = {}
    for _, label in ipairs(inputs) do
        table.insert(items, dom.div("field",
            dom.tag("span", "field-label", label),
            dom.div("line", "")
        ))
    end
    return dom.div("fields", table.unpack(items))
end

local function render_rows(categories: {string}): string
    local rows = {}
    for _, name in ipairs(categories) do
        table.insert(rows, dom.tr("row",
            dom.td("name", name),
            dom.td("money", "")
        ))
    end
    return dom.table("list", dom.tbody("", table.unpack(rows)))
end

local function render_bucket(bucket: Bucket): string
    return dom.section("bucket",
        dom.div("bucket-top",
            dom.div("pct", tostring(bucket.pct) .. "%"),
            dom.div("name", bucket.name)
        ),
        dom.div("target",
            dom.tag("span", "target-label", "Target"),
            dom.div("line", "")
        ),
        render_rows(bucket.categories),
        dom.div("total",
            dom.tag("span", "total-label", "Total"),
            dom.div("line", "")
        )
    )
end

local function render_buckets(config: Config): string
    local items = {}
    for _, bucket in ipairs(config.buckets) do
        table.insert(items, render_bucket(bucket))
    end
    return dom.div("buckets", table.unpack(items))
end

local function render_check(config: Config): string
    local fields = {}
    for _, label in ipairs(config.check_fields) do
        table.insert(fields, dom.div("check-item",
            dom.tag("span", "check-label", label),
            dom.div("line", "")
        ))
    end

    return dom.section("check",
        dom.div("check-fields", table.unpack(fields)),
        dom.div("check-result", config.check_result)
    )
end

function M.render(config: Config): string
    local header = dom.header("masthead",
        dom.h1("", config.header_line),
        dom.div("subject", config.subject)
    )

    local main = dom.main("",
        render_inputs(config.inputs),
        render_buckets(config),
        render_check(config)
    )

    return header .. main
end

return M as contracts.TemplateRender
