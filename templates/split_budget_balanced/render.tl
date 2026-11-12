local dom = require("dom")
local contracts = require("contracts")

local record Bucket
    name: string
    pct: integer
    tag: string
    categories: {string}
end

local record Config
    header_line: string
    subject: string
    inputs: {string}
    bar_aside: string
    column_label: string
    buckets: {Bucket}
    check_fields: {string}
    check_result: string
    note: string
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

local function render_bar(buckets: {Bucket}): string
    local segments = {}
    for index, bucket in ipairs(buckets) do
        table.insert(segments, dom.div(
            "seg seg-" .. tostring(bucket.pct) .. " seg-tone-" .. tostring(index),
            dom.tag("span", "seg-pct", tostring(bucket.pct) .. "%"),
            dom.tag("span", "seg-name", bucket.name)
        ))
    end
    return dom.div("split-bar", table.unpack(segments))
end

local function render_rows(categories: {string}, column_label: string): string
    local head = dom.tr("",
        dom.th("col-name", "Category"),
        dom.th("col-money", column_label)
    )

    local rows = {}
    for _, name in ipairs(categories) do
        table.insert(rows, dom.tr("row",
            dom.td("col-name", name),
            dom.td("col-money", "")
        ))
    end

    return dom.table("list",
        dom.thead("", head),
        dom.tbody("", table.unpack(rows))
    )
end

local function render_bucket(bucket: Bucket, column_label: string): string
    return dom.section("bucket",
        dom.div("bucket-head",
            dom.div("bucket-name", bucket.name),
            dom.tag("span", "bucket-pct", tostring(bucket.pct) .. "%")
        ),
        dom.div("bucket-tag", bucket.tag),
        render_rows(bucket.categories, column_label),
        dom.div("bucket-total",
            dom.tag("span", "total-label", "Total"),
            dom.div("line", "")
        ),
        dom.div("bucket-check",
            dom.tag("span", "check-label", "Left to assign"),
            dom.div("line", "")
        )
    )
end

local function render_buckets(config: Config): string
    local items = {}
    for _, bucket in ipairs(config.buckets) do
        table.insert(items, render_bucket(bucket, config.column_label))
    end
    return dom.div("buckets", table.unpack(items))
end

local function render_check(config: Config): string
    local items = {}
    for _, label in ipairs(config.check_fields) do
        table.insert(items, dom.div("check-item",
            dom.tag("span", "check-label", label),
            dom.div("line", "")
        ))
    end

    return dom.section("check",
        dom.div("check-fields", table.unpack(items)),
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
        dom.div("section-head",
            dom.h2("", "Where the money goes"),
            dom.tag("span", "section-aside", config.bar_aside)
        ),
        render_bar(config.buckets),
        render_buckets(config),
        render_check(config),
        dom.div("note", config.note)
    )

    return header .. main
end

return M as contracts.TemplateRender
