// This file is automatically generated when you do `Build.ps1 prepare`.

import jetbrains.buildServer.configs.kotlin.v2019_2.*

// Both Swabra and swabra need to be imported
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.sshAgent
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.Swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.failureConditions.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.*

version = "2021.2"

project {

   buildType(DebugBuild)
   buildType(PublicBuild)
   buildType(PublicDeployment)
   buildType(VersionBump)
   buildTypesOrder = arrayListOf(DebugBuild,PublicBuild,PublicDeployment,VersionBump)
}

object DebugBuild : BuildType({

    name = "Build [Debug]"

    artifactRules = "+:artifacts/publish/public/**/*=>artifacts/publish/public\n+:artifacts/publish/private/**/*=>artifacts/publish/private\n+:artifacts/testResults/**/*=>artifacts/testResults\n+:artifacts/logs/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/AssemblyLocator/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/CompileTime/**/.completed=>logs\n+:%system.teamcity.build.tempDir%/Metalama/CompileTimeTroubleshooting/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/CrashReports/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/Extract/**/.completed=>logs\n+:%system.teamcity.build.tempDir%/Metalama/ExtractExceptions/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/Logs/**/*=>logs"

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        // Step to kill all dotnet or VBCSCompiler processes that might be locking files we delete in during cleanup.
        powerShell {
            name = "Kill background processes before cleanup"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
        }
        powerShell {
            name = "Build [Debug]"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "test --configuration Debug --buildNumber %build.number% --buildType %system.teamcity.buildType.id%")
        }

        // Step to kill all dotnet or VBCSCompiler processes that might be locking files that Swabra deletes in the beginning of another build.
        powerShell {
            name = "Kill background processes after build"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
            executionMode = BuildStep.ExecutionMode.ALWAYS
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            threshold = 300
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
    }

    triggers {

        vcs {
            watchChangesInDependencies = true
            branchFilter = "+:<default>"
            // Build will not trigger automatically if the commit message contains comment value.
            triggerRules = "-:comment=<<VERSION_BUMP>>|<<DEPENDENCIES_UPDATED>>:**"
        }        

    }

    dependencies {

        dependency(AbsoluteId("Metalama_Metalama20232_Metalama_DebugBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaBackstage_ReleaseBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Backstage"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaCompiler_ReleaseBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/packages/Release/Shipping/**/*=>dependencies/Metalama.Compiler"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaExtensions_DebugBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Extensions"
            }

        }

     }

})

object PublicBuild : BuildType({

    name = "Build [Public]"

    artifactRules = "+:artifacts/publish/public/**/*=>artifacts/publish/public\n+:artifacts/publish/private/**/*=>artifacts/publish/private\n+:artifacts/testResults/**/*=>artifacts/testResults\n+:artifacts/logs/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/AssemblyLocator/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/CompileTime/**/.completed=>logs\n+:%system.teamcity.build.tempDir%/Metalama/CompileTimeTroubleshooting/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/CrashReports/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/Extract/**/.completed=>logs\n+:%system.teamcity.build.tempDir%/Metalama/ExtractExceptions/**/*=>logs\n+:%system.teamcity.build.tempDir%/Metalama/Logs/**/*=>logs"

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        // Step to kill all dotnet or VBCSCompiler processes that might be locking files we delete in during cleanup.
        powerShell {
            name = "Kill background processes before cleanup"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
        }
        powerShell {
            name = "Build [Public]"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "test --configuration Public --buildNumber %build.number% --buildType %system.teamcity.buildType.id%")
        }

        // Step to kill all dotnet or VBCSCompiler processes that might be locking files that Swabra deletes in the beginning of another build.
        powerShell {
            name = "Kill background processes after build"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
            executionMode = BuildStep.ExecutionMode.ALWAYS
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            threshold = 300
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
    }

    dependencies {

        dependency(AbsoluteId("Metalama_Metalama20232_Metalama_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaBackstage_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Backstage"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaCompiler_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/packages/Release/Shipping/**/*=>dependencies/Metalama.Compiler"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaExtensions_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Extensions"
            }

        }

     }

})

object PublicDeployment : BuildType({

    name = "Deploy [Public]"

    type = Type.DEPLOYMENT

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Deploy [Public]"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "publish --configuration Public")
        }

        // Step to kill all dotnet or VBCSCompiler processes that might be locking files that Swabra deletes in the beginning of another build.
        powerShell {
            name = "Kill background processes after build"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
            executionMode = BuildStep.ExecutionMode.ALWAYS
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            threshold = 300
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
        sshAgent {
            // By convention, the SSH key name is always PostSharp.Engineering for all repositories using SSH to connect.
            teamcitySshKey = "PostSharp.Engineering"
        }
    }

    dependencies {

        dependency(AbsoluteId("Metalama_Metalama20232_Metalama_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaBackstage_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Backstage"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaCompiler_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/packages/Release/Shipping/**/*=>dependencies/Metalama.Compiler"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaExtensions_PublicBuild")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/private/**/*=>dependencies/Metalama.Extensions"
            }

        }

        dependency(AbsoluteId("Metalama_Metalama20232_MetalamaExtensions_PublicDeployment")) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


        }

        dependency(PublicBuild) {
            snapshot {
                     onDependencyFailure = FailureAction.FAIL_TO_START
            }


            artifacts {
                cleanDestination = true
                artifactRules = "+:artifacts/publish/public/**/*=>artifacts/publish/public\n+:artifacts/publish/private/**/*=>artifacts/publish/private\n+:artifacts/testResults/**/*=>artifacts/testResults"
            }

        }

     }

})

object VersionBump : BuildType({

    name = "Version Bump"

    type = Type.DEPLOYMENT

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        powerShell {
            name = "Version Bump"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "bump")
        }

        // Step to kill all dotnet or VBCSCompiler processes that might be locking files that Swabra deletes in the beginning of another build.
        powerShell {
            name = "Kill background processes after build"
            scriptMode = file {
                path = "Build.ps1"
            }
            noProfile = false
            param("jetbrains_powershell_scriptArguments", "tools kill")
            executionMode = BuildStep.ExecutionMode.ALWAYS
        }
    }

    failureConditions {
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.BUILD_DURATION
            threshold = 300
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = build {
                buildRule = lastSuccessful()
            }
            stopBuildOnFailure = true
        }
    }

    requirements {
        equals("env.BuildAgentType", "caravela04cloud")
    }

    features {
        swabra {
            lockingProcesses = Swabra.LockingProcessPolicy.KILL
            verbose = true
        }
        sshAgent {
            // By convention, the SSH key name is always PostSharp.Engineering for all repositories using SSH to connect.
            teamcitySshKey = "PostSharp.Engineering"
        }
    }

})

