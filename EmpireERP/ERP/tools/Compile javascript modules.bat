cd tools\

echo Compiling JS... >&2
cd "..\ERP.Wholesale.UI.Web\Scripts\"
mkdir tmp
cd "Modules\"
for /R %%f in (*.js) do copy /y %%f ".\..\tmp\"
cd ../../../
copy /b /y "ERP.Wholesale.UI.Web\Scripts\tmp\*.js" "ERP.Wholesale.UI.Web\Scripts\modules.debug.js"
type "ERP.Wholesale.UI.Web\Scripts\modules.debug.js" | "tools\jsmin\jsmin" > "ERP.Wholesale.UI.Web\Scripts\modules.min.js"
rmdir /S /Q "ERP.Wholesale.UI.Web\Scripts\tmp"
exit