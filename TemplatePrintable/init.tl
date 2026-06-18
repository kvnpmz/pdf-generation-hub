local tl = require('tl')
tl.loader() 

require("templates.base.render")
local context = require("context")
local style_provider = require("style_provider")

global SetupDocument = context.Execute
global ApplyStyling = style_provider.Apply
