package.path = package.path .. ";../../CSharp.lua/Coresystem.lua/?.lua"

require("All")()          -- coresystem.lua/All.lua
require("out.manifest")("out")    

Test.Program.Main()    -- run main method

--local methodInfo = System.Reflection.Assembly.GetEntryAssembly().getEntryPoint()
--methodInfo:Invoke()


