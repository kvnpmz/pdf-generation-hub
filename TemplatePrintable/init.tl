--
--
local tl = require('tl')
tl.loader() 
require("templates.base.render")
local builder = require("builder")
local style_provider = require("style_provider")

global SetupDocument = builder.Compose
global ApplyStyling = style_provider.Apply
