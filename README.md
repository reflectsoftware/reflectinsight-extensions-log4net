# ReflectInsight-Extensions-Log4Net

[![Build status](https://ci.appveyor.com/api/projects/status/github/reflectsoftware/reflectinsight-extensions-log4net?svg=true)](https://ci.appveyor.com/project/reflectsoftware/reflectinsight-extensions-log4net)
[![Release](https://img.shields.io/github/release/reflectsoftware/reflectinsight-extensions-log4net.svg)](https://github.com/reflectsoftware/reflectinsight-extensions-log4net/releases/latest)
[![NuGet Version](http://img.shields.io/nuget/v/reflectsoftware.insight.extensions.log4net.svg?style=flat)](http://www.nuget.org/packages/ReflectSoftware.Insight.Extensions.Log4Net/)
[![NuGet](https://img.shields.io/nuget/dt/reflectsoftware.insight.extensions.log4net.svg)](http://www.nuget.org/packages/ReflectSoftware.Insight.Extensions.Log4net/)
[![Stars](https://img.shields.io/github/stars/reflectsoftware/reflectinsight-extensions-log4net.svg)](https://github.com/reflectsoftware/reflectinsight-extensions-log4net/stargazers)

## Overview ##

We've added support for the Log4net appender. This allows you to leverage your current investment in log4net, but leverage the power and flexibility that comes with the ReflectInsight viewer. You can view your log4net messages in realtime, in a rich viewer that allows you to filter out and search for what really matters to you.

 The log4net extension supports Log4net v1.2.11.0. However if you need to support an older version, then you will need to download the ReflectInsight Logging Extensions Library from GitHub. You can then reference and rebuild the extension against a specific release of the log4net dll.

## Benefits of ReflectInsight Extensions ##

The benefits to using the Insight Extensions is that you can easily and quickly add them to your applicable with little effort and then use the ReflectInsight Viewer to view your logging in real-time, allowing you to filter, search, navigate and see the details of your logged messages.

## Getting Started

```powershell
Install-Package ReflectSoftware.Insight.Extensions.Log4Net
```

Then in your app.config or web.config file, add the following configuration sections:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="insightSettings" type="ReflectSoftware.Insight.ConfigurationHandler,ReflectSoftware.Insight" />
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
	</configSections>

  <!-- Logging Configuration 
       Please make sure you update the Insight.config file property 'Copy to Output Directory' to 'Copy always'.
       For more information on ReflectInsight and configuration help, visit http://reflectsoftware.com. -->
  <insightSettings externalConfigSource="ReflectInsight.config" />

  <log4net debug="false">
    <appender name="MyLogAppender1" type="ReflectSoftware.Insight.Extensions.Log4net.LogAppender, ReflectSoftware.Insight.Extensions.Log4net">
      <param name="InstanceName" value="log4netInstance1" />
      <param name="DisplayLevel" value="true" />
      <param name="DisplayLocation" value="true" />
    </appender>

    <root>
      <appender-ref ref="MyLogAppender1" />
    </root>
  </log4net>
    
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0" />
  </startup>
</configuration>
```

Additional configuration details for the ReflectSoftware.Insight.Extensions.Log4Net logging extension can be found [here](https://reflectsoftware.atlassian.net/wiki/display/RI5/Log4net+Extension).

## Additional Resources

[Documentation](https://reflectsoftware.atlassian.net/wiki/display/RI5/ReflectInsight+5+documentation)

[Knowledge Base](http://reflectsoftware.uservoice.com/knowledgebase)

[Submit User Feedback](http://reflectsoftware.uservoice.com/forums/158277-reflectinsight-feedback)

[Contact Support](support@reflectsoftware.com)
