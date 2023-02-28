## Overview
Forked from https://github.com/microsoft/PowerApps-TestEngine.  See the ReadMe there for full installation instructions. 

This project shows an example of running created tests (potentially from Test Studio for Canvas apps) in Azure Pipeline. 

This has required changes to 2 files only. 

1) The **azure-pipelines.yml** file will run tests for the included Calculator sample canvas app. 

This canvas app has been uploaded to my developer tennant as per instructions in the main Test Engine repo.  


2) The **config.dev.json** file has been added with relevant details populated. This file points to the tests to be run.  This means pointing to the testPlan.fx.yaml file in the samples/Calculator directory.

 
## Use
For future use in a pipeline you need only to add the relevant tests to testplan.fx.yaml file. 
Update the config file to point to that test plan and to the location of the canvas app in your tennant.

The pipeline currently uploads the logs and screenshots from the samples/calculator/ subfolder.  This should be updated to a more appropriate location based on the new tests added.


## Credentials
The credentials for accessing the canvas app are obtained by the pipeline from Azure DevOps.  The need to be added in the Library > Variable group section of DevOps. 
The pipeline expects the variable group to be called 'credentials' with the username stored as the 'user1Email' variable & the password as 'user1Password'