IF NOT DEFINED Configuration SET Configuration=Release
MSBuild.exe iTextCore.sln -t:restore -p:RestorePackagesConfig=true
MSBuild.exe iTextCore.sln -m /property:Configuration=%Configuration%
cd NuSpecs
nuget pack -OutputDirectory ..\Packages Net4x.itext7-commons.nuspec
nuget pack -OutputDirectory ..\Packages Net4x.itext7.nuspec
cd ..
git add -A
git commit -a --allow-empty-message -m ''
git push