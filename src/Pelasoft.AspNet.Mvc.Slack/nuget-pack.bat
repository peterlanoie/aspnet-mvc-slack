@echo off
echo.
echo ######  Nuget Creation Checklist  ######
echo ##                                    ##
echo ##  * Update assembly version number  ##
echo ##  * Update nuspec revision notes    ##
echo ##  * Create tag after final commit   ##
echo ##    * git tag vX.X.X                ##
echo ##    * git push origin --tags        ##
echo ##                                    ##
echo ########################################
echo.
nuget pack Pelasoft.AspNet.Mvc.Slack.csproj -Verbosity detailed -Build -IncludeReferencedProjects -Properties Configuration=Release