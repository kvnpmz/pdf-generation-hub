#!/bin/bash
export PATH=$PWD/lua_modules/bin:$PATH
eval $(luarocks path --tree=lua_modules)
echo "Lua environment activated for this session."
