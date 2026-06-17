

local tl = require('tl')
tl.loader()
require("templates.base.render")
local builder = require("builder")
local style_provider = require("style_provider")

SetupDocument = builder.Compose
ApplyStyling = style_provider.Apply
