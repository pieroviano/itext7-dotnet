IF NOT DEFINED Configuration SET Configuration=Release
MSBuild.exe iTextCore.sln -t:clean
MSBuild.exe iTextCore.sln -t:restore -p:RestorePackagesConfig=true
MSBuild.exe iTextCore.sln -m /property:Configuration=%Configuration%
git add -A
git commit -a --allow-empty-message -m ''
git push