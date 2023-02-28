## Overview
Forked from https://github.com/microsoft/PowerApps-TestEngine.  
See the ReadMe there for full installation instructions. 

This project has been created to demonstrate running the tests via an Azure Pipeline. 

This has required changes to 2 files only. 

1) The **azure-pipelines.yml** file has been added to run tests against the included Calculator sample canvas app. 

__Note: This canvas app has been uploaded to my developer tennant as per instructions in the main Test Engine repo__


2) The **config.dev.json** file has been added with details of the tennant and which test plan file to run. For the example, it is pointing to the testPlan.fx.yaml file in the samples/Calculator directory.

 
## Use
For future use in a pipeline you need only to add the relevant tests to a yaml file.

This can be done by creating a test using the Test Studio interface within make.powerapps.com.  Created tests can be turned into .yaml files here

> Advanced Tools > Open Tests > [Create the tests] > Download Suite

The config.json file should then be updated to point to the new .yaml file and to the location of the canvas app in your tennant.

The pipeline currently uploads the logs and screenshots from the samples/calculator/ subfolder.  This should be updated to a more appropriate location based on the new tests added.


## Credentials
The credentials for accessing the canvas app are obtained by the pipeline from Azure DevOps.  The need to be added in the Library > Variable group section of DevOps. 
The pipeline expects the variable group to be called 'credentials' with the username stored as the 'user1Email' variable & the password as 'user1Password'